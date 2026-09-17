# Steam Desktop Authenticator — Upstream Source Analysis

Analysis of `Jessecar96/SteamDesktopAuthenticator` at commit `3443a52` (v1.0.15, 2024-10-20)
and its submodule `geel9/SteamAuth` at commit `01c0c9b` (2023-07-06).

The vendored copy lives in `upstream-src/`. Total non-designer C#: ~4,300 lines
(SteamAuth lib ~1,100, WinForms app ~3,000, TestBed ~150).

---

## 1. What it is

SDA is a Windows desktop replacement for the Steam mobile app's authenticator. It:

1. Logs into Steam as if it were the Android mobile app (SteamKit2, platform type `MobileApp`).
2. Enrolls itself as the account's mobile authenticator via `ITwoFactorService/AddAuthenticator`.
3. Stores the returned secrets (`shared_secret`, `identity_secret`, `revocation_code`, ...) in a
   JSON file called a **maFile**, optionally AES-encrypted.
4. Generates the 5-character Steam Guard TOTP codes every 30 seconds.
5. Lists, accepts, and denies mobile confirmations (trades, market listings) through the
   `steamcommunity.com/mobileconf/*` endpoints.

It was abandoned in October 2024 with a "no longer supported" warning baked into startup.

---

## 2. Repository layout

```
SteamDesktopAuthenticator.sln
Steam Desktop Authenticator/          WinForms app (net8.0-windows)
  Program.cs                          entry point, single-instance check, CLI args
  MainForm.cs / .Designer.cs          account list, code display, timers, tray icon, update check
  LoginForm.cs                        SteamKit2 login + full authenticator linking flow
  UserFormAuthenticator.cs            SteamKit2 IAuthenticator (device code / email code prompts)
  ConfirmationFormWeb.cs              confirmations list UI (hand-built panels, no browser)
  TradePopupForm.cs                   bottom-right popup for periodic confirmation checks
  ImportAccountForm.cs                import a maFile (encrypted or not) from elsewhere
  WelcomeForm.cs                      first-run screen; migrate old install dir
  SettingsForm.cs                     periodic checking + auto-confirm toggles
  Manifest.cs                         manifest.json + maFile persistence, encryption orchestration
  FileEncryptor.cs                    PBKDF2 + AES-CBC for maFiles
  InputForm / PhoneInputForm / ListInputForm / CaptchaForm   small dialogs
  ConfirmationButton.cs               Button subclass carrying a Confirmation
  CommandLineOptions.cs               -k <key>, -s (silent)
  App.config                          .NET Framework leftovers (ignored on .NET 8)
lib/SteamAuth/                        git submodule -> geel9/SteamAuth
  SteamAuth/                          netstandard2.0 class library
    SteamGuardAccount.cs              secrets, TOTP, confirmation hash, confirmation ops, deactivate
    AuthenticatorLinker.cs            AddAuthenticator / FinalizeAddAuthenticator flow
    SessionData.cs                    steamid + JWT access/refresh tokens, cookies, token refresh
    TimeAligner.cs                    one-shot server time offset
    SteamWeb.cs / CookieAwareWebClient.cs   thin WebClient wrappers
    Confirmation.cs                   DTOs for /mobileconf/getlist
    APIEndpoints.cs, Util.cs
  TestBed/                            console app demonstrating link flow (not in the SDA .sln)
```

Dependencies: `SteamKit2 3.0.0-Beta.4` (login only), `Newtonsoft.Json 13.0.3`,
`CommandLineParser 2.9.1`. No test projects. No CI.

---

## 3. How to build

Verified on this machine with .NET SDK 9.0.200 targeting net8.0-windows: **0 errors, ~1,000 warnings**.

Requirements:

- Windows (WinForms). Target framework is `net8.0-windows`.
- .NET 8 SDK or newer (a newer SDK builds net8 fine once the targeting pack restores).
- Git with submodules. `Download ZIP` on GitHub does not include `lib/SteamAuth`.

```bash
git clone --recurse-submodules https://github.com/Jessecar96/SteamDesktopAuthenticator.git
cd SteamDesktopAuthenticator
dotnet build SteamDesktopAuthenticator.sln -c Release
```

Output: `Steam Desktop Authenticator/bin/Release/net8.0-windows/Steam Desktop Authenticator.exe`
plus `SteamAuth.dll`, `SteamKit2.dll`, `protobuf-net*.dll`, `Newtonsoft.Json.dll`, `CommandLine.dll`.

The build is **framework-dependent**: users need the .NET 8 Desktop Runtime installed. For a
self-contained single exe:

```bash
dotnet publish "Steam Desktop Authenticator/Steam Desktop Authenticator.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Note: `Manifest.GetExecutableDir()` uses `Assembly.GetEntryAssembly().Location`, which is an
**empty string** under single-file publish. The maFiles path would break. Switch to
`AppContext.BaseDirectory` before shipping single-file.

Visual Studio: 2022 17.8+ with the ".NET desktop development" workload opens the `.sln` directly.

Warning breakdown from a clean Release build:

| Code | Count | Meaning |
|---|---|---|
| CA1416 | 982 | WinForms API called from code not marked `[SupportedOSPlatform("windows")]` (noise) |
| SYSLIB0022 | 8 | `RijndaelManaged` obsolete, use `Aes` |
| SYSLIB0023 | 8 | `RNGCryptoServiceProvider` obsolete, use `RandomNumberGenerator` |
| SYSLIB0041 | 2 | `Rfc2898DeriveBytes(string, byte[], int)` obsolete: default SHA1 + no explicit hash |
| SYSLIB0014 | 2 | `WebClient` obsolete, use `HttpClient` |
| CS0649 | 2 | JSON field never assigned (Newtonsoft assigns via reflection, harmless) |

---

## 4. Data formats

### 4.1 maFile (`maFiles/<steamid64>.maFile`)

The maFile is literally the raw JSON response of `ITwoFactorService/AddAuthenticator` deserialized
into `SteamGuardAccount`, plus three fields SDA adds (`device_id`, `fully_enrolled`, `Session`).
That is why the property names are snake_case Steam names.

```json
{
  "shared_secret":   "base64 20 bytes  -> TOTP HMAC key",
  "serial_number":   "string",
  "revocation_code": "RXXXXX  -> needed to remove the authenticator without the app",
  "uri":             "otpauth://totp/Steam:<name>?secret=<base32>&issuer=Steam",
  "server_time":     1234567890,
  "account_name":    "steam login name",
  "token_gid":       "string",
  "identity_secret": "base64 20 bytes  -> confirmation HMAC key",
  "secret_1":        "base64",
  "status":          1,
  "device_id":       "android:<guid>  -> must be sent with every /mobileconf request",
  "fully_enrolled":  true,
  "Session": {
    "SteamID":      76561198000000000,
    "AccessToken":  "<JWT, ~24h>",
    "RefreshToken": "<JWT, long-lived>",
    "SessionID":    "32 hex chars (random, used as CSRF cookie)"
  }
}
```

`Session` has no `[JsonProperty]` attributes so its keys are PascalCase, unlike everything
else. Older (pre-2023) maFiles carried `OAuthToken`, `WebCookie`, `SteamLogin`,
`SteamLoginSecure` inside `Session`; Newtonsoft ignores unknown keys so those files still load,
but `AccessToken` is empty so the app forces a re-login on import.

When encrypted, the maFile's entire content is replaced by a base64 string of the AES ciphertext.
The salt and IV are **not** in the maFile; they live in `manifest.json`. An encrypted maFile
without its manifest cannot be decrypted, which is why `ImportAccountForm` hunts for a
`manifest.json` next to the imported file.

### 4.2 manifest.json (`maFiles/manifest.json`)

```json
{
  "encrypted": false,
  "first_run": false,
  "entries": [
    { "encryption_iv": null, "encryption_salt": null, "filename": "7656....maFile", "steamid": 7656... }
  ],
  "periodic_checking": false,
  "periodic_checking_interval": 5,
  "periodic_checking_checkall": false,
  "auto_confirm_market_transactions": false,
  "auto_confirm_trades": false
}
```

Encryption is all-or-nothing across every account. `encrypted` is forced back to `false`
whenever `entries` is empty.

---

## 5. Core algorithms (the part worth keeping)

### 5.1 Steam Guard code (`SteamGuardAccount.GenerateSteamGuardCodeForTime`)

Standard RFC 6238 TOTP with a Steam-specific output alphabet:

1. `counter = steamTime / 30`, encoded as 8-byte big-endian.
2. `hash = HMAC-SHA1(base64decode(shared_secret), counter)`.
3. Dynamic truncation: `offset = hash[19] & 0x0F`, `code = (hash[offset] & 0x7F) << 24 | hash[offset+1] << 16 | hash[offset+2] << 8 | hash[offset+3]`.
4. Five characters, least significant digit first, base 26 over the alphabet
   `23456789BCDFGHJKMNPQRTVWXY` (digits/letters that are hard to confuse).

The `Regex.Unescape(SharedSecret)` call before base64 decoding is a legacy hack for maFiles
where the secret was stored with escaped slashes (`\/`).

### 5.2 Time alignment (`TimeAligner`)

`POST https://api.steampowered.com/ITwoFactorService/QueryTime/v0001` with body `steamid=0`
returns `{ "response": { "server_time": N } }`. The offset is computed **once** per process
and cached in static fields. If the request fails the class silently stays unaligned and every
later call retries.

### 5.3 Confirmation hash (`_generateConfirmationHashForTime`)

```
msg  = bigEndian8(time) || UTF8(tag)[0..32]
hash = base64(HMAC-SHA1(base64decode(identity_secret), msg))
k    = urlencode(hash)
```

The loop body is clearly decompiled from the Android app (`n2`, `n3`, `n4` variable names).

### 5.4 Confirmation endpoints

All requests carry the mobile User-Agent `okhttp/3.12.12` and these cookies on
`steamcommunity.com`:

| Cookie | Value |
|---|---|
| `steamLoginSecure` | `<steamid64>%7C%7C<access_token>` |
| `sessionid` | random 32 hex |
| `mobileClient` | `android` |
| `mobileClientVersion` | `777777 3.6.1` |

Common query params for every `/mobileconf/*` call:

| Param | Value |
|---|---|
| `p` | `device_id` |
| `a` | steamid64 |
| `k` | confirmation hash for `tag` |
| `t` | steam time |
| `m` | `react` |
| `tag` | `conf` (list), `accept` (allow), `reject` (cancel) |

- List: `GET /mobileconf/getlist?...&tag=conf` -> `{ success, needauth, conf: [ { id, nonce, creator_id, headline, summary[], accept, cancel, icon, type } ] }`
- Single op: `GET /mobileconf/ajaxop?op=allow|cancel&...&cid=<id>&ck=<nonce>` (note: `op` is `allow`/`cancel`, but `tag` is `accept`/`reject`)
- Multi op: `POST /mobileconf/multiajaxop` form body `op=allow&...&cid[]=..&ck[]=..`

`type` enum: 1 Test, 2 Trade, 3 MarketListing, 4 FeatureOptOut, 5 PhoneNumberChange, 6 AccountRecovery.

### 5.5 Login (`LoginForm` + SteamKit2)

```csharp
var client = new SteamClient(); client.Connect(); // busy-wait until IsConnected
var session = await client.Authentication.BeginAuthSessionViaCredentialsAsync(new AuthSessionDetails {
    Username, Password,
    IsPersistentSession = false,
    PlatformType = EAuthTokenPlatformType.k_EAuthTokenPlatformType_MobileApp,
    ClientOSType = EOSType.Android9,
    Authenticator = new UserFormAuthenticator(existingAccountOrNull),
});
var poll = await session.PollingWaitForResultAsync();
// poll.AccessToken, poll.RefreshToken, session.SteamID
```

`PlatformType = MobileApp` is essential; tokens minted for other platforms are rejected by
`ITwoFactorService` and `/mobileconf`. The `IAuthenticator` supplies a device code (generated
from the existing maFile when re-logging) or an email code (prompted).

### 5.6 Token refresh (`SessionData.RefreshAccessToken`)

`POST https://api.steampowered.com/IAuthenticationService/GenerateAccessTokenForApp/v1/`
with `refresh_token`, `steamid` -> `{ response: { access_token } }`. Expiry is checked by
base64url-decoding the JWT payload and reading `exp`. No signature check, which is fine for
a client-side expiry test.

### 5.7 Linking flow (`AuthenticatorLinker`)

```
AddAuthenticator()
  POST ITwoFactorService/AddAuthenticator/v1?access_token=..
       steamid, authenticator_time, authenticator_type=1, device_identifier, sms_phone_id=1
  status 1  -> AwaitingFinalization (response IS the SteamGuardAccount)
  status 2  -> no phone on account:
                 if PhoneNumber null            -> MustProvidePhoneNumber
                 else IPhoneService/SetAccountPhoneNumber (phone_number, phone_country_code)
                      -> MustConfirmEmail; next call polls IsAccountWaitingForEmailConfirmation,
                         then SendPhoneVerificationCode, waits 2s, and retries AddAuthenticator
  status 29 -> AuthenticatorPresent
FinalizeAddAuthenticator(smsCode)   (loop up to 10x)
  POST ITwoFactorService/FinalizeAddAuthenticator/v1?access_token=..
       steamid, authenticator_code (TOTP from new secret), authenticator_time, activation_code=sms, validate_sms_code=1
  status 89 -> BadSMSCode
  want_more -> retry with next code (Steam wants two consecutive valid codes)
  success   -> FullyEnrolled = true
```

Country code: uses `PhoneCountryCode` if provided, else `IUserAccountService/GetUserCountry`.

Removal: `POST ITwoFactorService/RemoveAuthenticator/v1?access_token=..` with
`revocation_code`, `revocation_reason=1`, `steamguard_scheme` (1 = back to email, 2 = remove).

The UI enforces a good safety ritual: save the maFile **before** finalizing, show the revocation
code, make the user type it back, and delete the maFile if any step fails.

---

## 6. Runtime flow

```
Program.Main
  single-instance check (process name + path)
  parse -k/-s
  Manifest.GetManifest()      -> ManifestParseException? -> GenerateNewManifest(scanDir)
                                                           -> encrypted maFile found? -> "locked out" wiki + exit
  "no longer supported" MessageBox
  FirstRun && no entries -> WelcomeForm  else -> MainForm

MainForm_Shown
  manifest.FirstRun = false; Save
  timerSteamGuard_Tick once (aligns time)
  if encrypted: prompt passkey (unless -k given); cancel -> Application.Exit
  loadSettings (periodic timer); loadAccountsList (decrypts every maFile); checkForUpdates (GitHub API)

timerSteamGuard (1000 ms)   -> re-read steam time, regenerate code, update progress bar
timerTradesPopup (N*1000 ms, off by default)
  -> for current account (or all): refresh access token if expired, FetchConfirmations,
     auto-accept Trade/MarketListing if enabled, else show TradePopupForm

ConfirmationFormWeb_Shown   -> refresh token if needed, FetchConfirmations, build one Panel per confirmation
```

---

## 7. Code quality assessment

### 7.1 What is genuinely good

- **Protocol knowledge is correct and hard-won.** The endpoint list, query params, cookie set,
  tag/op mismatch, `m=react`, `android:<guid>` device id, `MobileApp` platform type, and the
  "want_more" two-code finalize dance are the real value of this repo. Reverse-engineered from
  the Android app over years; there is no official documentation.
- **Small, flat, readable.** 30 files, no frameworks, no DI container, no abstractions for their
  own sake. A new contributor can read the whole thing in an hour.
- **Library/app split.** `SteamAuth` is netstandard2.0 with no UI dependency and is reusable
  from a CLI, a service, or a different GUI toolkit.
- **Explicit result enums** (`LinkResult`, `FinalizeResult`) instead of exceptions for expected
  protocol outcomes.
- **Safety ritual around linking**: persist before finalize, force revocation-code readback,
  clean up on failure, "re-save with FullyEnrolled" afterward.
- **Explicit JSON DTOs** with `[JsonProperty]` for every Steam response, private nested classes
  keep them out of the public surface.
- **Double-press confirmation** in the tray popup, and a warning dialog before enabling auto-accept.
- **Semaphore guard** (`SemaphoreSlim.Wait(0)`) so overlapping timer ticks don't stack.
- **The 2023 rewrite** (SteamKit2 login, JWT sessions, `IsTokenExpired`) is noticeably cleaner
  than the 2015 code it replaced.
- Dead old paths (CefSharp, Squirrel, captcha login) were actually deleted rather than left
  behind `#if`s. Mostly.

### 7.2 What is bad: structural

- **God-form pattern.** `MainForm` and `LoginForm` own state, persistence calls, network calls,
  and dialogs. `LoginForm.btnSteamLogin_Click` is 240 lines mixing login, linking, encryption
  prompts, and finalize. Untestable without a UI thread.
- **Static singleton manifest** (`Manifest._manifest`) plus `GetManifest(forceLoad: true)`
  re-reads scattered around to get "the latest". Two forms can hold different instances.
- **Zero tests.** Not even for the pure functions (TOTP, confirmation hash, encryption) that are
  trivially testable and where a bug locks users out of their accounts.
- **`async void` everywhere** (all event handlers). Any unhandled exception kills the process.
  `timerSteamGuard_Tick` runs every second and is `async void`.
- **Exceptions as control flow with catch-alls.** `catch (Exception) { }` swallows network,
  JSON, and logic errors alike; `GenerateNewManifest` converts *any* parse error into
  "your file is encrypted, you're locked out".
- **Duplicated code.** Passkey-prompt loop is copied three times (`Manifest.PromptForPassKey`,
  `LoginForm.btnSteamLogin_Click`, `LoginForm.HandleManifest`). `ImportAccountForm` redefines
  `AppManifest`, `ImportManifest`, `ImportManifestEntry`, all identical to `Manifest`'s types,
  and duplicates the whole encrypted/unencrypted import body.
- **`ImportAccountForm` is a different author's style entirely:** string flags `"0"/"1"` instead
  of `bool`, `#region` inside a method, "Encripted" misspellings, path math by `string.Replace`,
  reads `maFiles/manifest.json` relative to the **current working directory** instead of the
  exe directory, `this.Close()` called *before* doing the work.
- **Obsolete APIs throughout:** `WebClient`, `RijndaelManaged`, `RNGCryptoServiceProvider`,
  SHA1-default `Rfc2898DeriveBytes`. All flagged by the compiler and all easy to replace.
- **Dead code:** `CaptchaForm`, `ListInputForm`, `LoginForm.SetUsername/FilterPhoneNumber/PhoneNumberOkay`,
  `WGTokenInvalidException`/`WGTokenExpiredException` (never thrown), `LinkResult.MustRemovePhoneNumber`
  (never returned), `GetConfirmationTradeOfferID`, `ConfirmationDetailsResponse`,
  `MOBILEAUTH_GETWGTOKEN`, the "Loading..." text swap in `btnTradeConfirmations_Click`.
- **Project leftovers:** `App.config` with .NET Framework 4.7.2 runtime + binding redirects,
  ClickOnce properties (`ApplicationVersion 0.2.2.*`, `BootstrapperEnabled`),
  `SquirrelAwareVersion` metadata, `ComVisible(true)`, `GenerateAssemblyInfo=false`.
- **Phone-home:** every launch hits `api.github.com/repos/Jessecar96/...` for an update check.
  A fork must change or remove this.
- Paths built with `"/"` string concatenation instead of `Path.Combine`.
- No `nullable`, no analyzers, no `.editorconfig`, mixed `this.` usage, mixed brace styles,
  PascalCase and camelCase private fields side by side.

### 7.3 What is bad: security

- **AES-CBC with no authentication tag.** Ciphertext is malleable. Wrong-password detection
  relies on PKCS7 padding failure, which has roughly a 1/256 chance of *not* failing; in that
  case decrypt "succeeds" with garbage, `JsonConvert` throws, and the exception is unhandled all
  the way up (`GetAllAccounts` -> `VerifyPasskey` -> `PromptForPassKey`) and crashes the app.
  Use AES-GCM or encrypt-then-MAC.
- **PBKDF2-SHA1, 50,000 iterations, 8-byte salt.** The doc comment says 100k. OWASP 2023 says
  1.3M for SHA1 or 600k for SHA256; NIST says 16-byte salt minimum. Cheap to fix, but changing it
  breaks reading old maFiles unless versioned.
- **Salt and IV stored in `manifest.json`, separate from the ciphertext.** Lose or corrupt the
  manifest and every encrypted maFile is unrecoverable. This is the failure mode behind most
  "locked out" issues on the tracker.
- **Non-atomic writes.** `File.WriteAllText` directly onto the maFile and manifest. A crash or
  power loss mid-write truncates the file holding the only copy of the secret. No temp-and-rename,
  no backups, no versioning.
- **`ChangeEncryptionKey` re-encrypts files in place one by one and saves the manifest last.**
  A crash halfway leaves files encrypted with the new key and a manifest with the old salts.
- **Secrets sit in `string` objects** for the process lifetime; passkey is kept in a field on
  `MainForm`. Not fixable in .NET without a lot of effort, but the passkey via `-k` on the
  command line also ends up in process listings and shell history.
- `sessionid` generated with `System.Random`. It is only a CSRF cookie, so low impact.
- `ImportAccountForm` decrypts the imported file and **saves it unencrypted** when the local
  manifest is unencrypted, with a message box saying "Your Account in now Decrypted!".

### 7.4 Concrete bugs found while reading

| # | Where | Bug |
|---|---|---|
| 1 | `MainForm.btnManageEncryption_Click` | Pressing **Cancel** on the "enter new passkey" dialog with an empty box does not abort. Both dialogs return `Canceled=true` with empty text, the guard `Canceled && !IsNullOrEmpty(text)` is false, `newPassKey` becomes `null`, and encryption is **removed**. |
| 2 | `MainForm_Shown` | On `ManifestParseException` it calls `this.Close()` then continues to `this.manifest.FirstRun = false` on a null reference. |
| 3 | `MainForm_Shown` | If the passkey prompt is cancelled it calls `Application.Exit()` but keeps executing the rest of the handler. |
| 4 | `MainForm.IsFilter` | Lowercases the search text but not the account name, so the "case-insensitive search" commit only works for all-lowercase names. |
| 5 | `MainForm.listAccounts_KeyDown` | Ctrl+Up/Down passes the **sorted ListBox index** to `Manifest.MoveEntry`, which indexes the **unsorted** `Entries` list. Moves the wrong entry, and the list is re-sorted anyway so nothing visible happens. |
| 6 | `MainForm.listAccounts_SelectedValueChanged` | Matches by `AccountName` string. Two maFiles with the same account name (re-linked account) always select the first. |
| 7 | `MainForm.timerTradesPopup_Tick` | `popupFrm.Account` is only ever set to `currentAccount`, but with "check all accounts" the popup receives confirmations from **other** accounts and accepts/denies them with the wrong account's `identity_secret`/session. |
| 8 | `MainForm.timerTradesPopup_Tick` | `confirmationsSemaphore.Release()` is not in a `finally`. `AcceptMultipleConfirmations` is outside any try; an exception there leaks the semaphore (periodic checking silently stops forever) and, being `async void`, crashes the process. |
| 9 | `TradePopupForm.btnAccept_Click` / `btnDeny_Click` | `acc.AcceptConfirmation(...)` is not awaited. Result ignored, errors lost, and `Reset()` hides the popup as if it succeeded. `lblDesc` is hard-coded to "Confirmation", so the user accepts blind. |
| 10 | `Manifest.GetAllAccounts` | `if (limit != -1 && limit >= accounts.Count) break;` is inverted. Works only for `limit == 1` by coincidence. |
| 11 | `Manifest.GetAllAccounts` | `DecryptData` throws `ArgumentException` on an empty-string passkey; only `null` is guarded. |
| 12 | `TimeAligner.AlignTime*` | Only `WebException` is caught. A non-JSON response (Cloudflare page, HTML error) throws `JsonReaderException` inside the 1-second `async void` timer and kills the app. |
| 13 | `TimeAligner` | Aligns once per process. Clock drift on a machine left running for weeks makes codes stale with no re-sync. |
| 14 | `SessionData.IsTokenExpired` | No guard for a non-JWT token string; `tokenComponents[1]` throws `IndexOutOfRange`. Old maFiles with a legacy `OAuthToken` are only saved by `AccessToken` being null. |
| 15 | `SessionData.RefreshAccessToken` | `response.Response` is null when Steam returns an error envelope; NRE instead of a useful message. |
| 16 | `SteamGuardAccount._sendConfirmationAjax` | When the session is dead Steam returns HTML; `JsonConvert` throws instead of returning `false`. |
| 17 | `LoginForm.btnSteamLogin_Click` | `while (!steamClient.IsConnected) await Task.Delay(500);` never times out; a failed connect hangs the form forever with the button disabled. `SteamClient` is never disconnected or disposed. |
| 18 | `UserFormAuthenticator` | SteamKit invokes the authenticator on a thread-pool thread; `MessageBox.Show` / `ShowDialog` run off the UI thread. Works by accident. On a wrong device code it sleeps 30 s with no feedback. |
| 19 | `ImportAccountForm` | `else if (IV_Found == null)` is checked twice; second should be `Salt_Found`. Also reads `maFiles/manifest.json` relative to CWD. |
| 20 | `Program.Main`, `WelcomeForm`, `MainForm.compareVersions` | `Process.Start("https://...")` on .NET Core has `UseShellExecute=false` by default and throws `Win32Exception` for URLs. The "locked out" help link and "download update" both fail on .NET 8. |
| 21 | `Manifest.GetExecutableDir` | `Assembly.GetEntryAssembly().Location` is empty under single-file publish. |
| 22 | `FileEncryptor` | Doc comment says 100k PBKDF2 rounds; constant is 50k. |
| 23 | `AuthenticatorLinker.FinalizeAddAuthenticator` | Uses a raw `WebClient` instead of `SteamWeb.POSTRequest`, the only place that does. Status 88 (`want_more`) handling is tangled: falls through to `!Success` before reaching the `WantMore` retry. |
| 24 | `WelcomeForm.btnImportConfig_Click` | Copies with `newPath.Replace(pathToCopy, ...)`, which corrupts paths if the source dir string appears twice in the path. |

### 7.5 Style fingerprint (how Jesse codes)

- Pragmatic, ship-it, single-developer style. Comments are sparse but honest ("catch-alls are
  bad!", "we're fucked", "Really basic way to wait until Steam is connected").
- Writes the happy path first and adds `if (x == null) return` guards where crashes were reported.
- Prefers `MessageBox.Show` as both logging and error handling.
- Keeps Steam DTOs private and nested inside the class that uses them, which is tidy.
- Tends to leave old fields/enums in place when a protocol changes (`MustRemovePhoneNumber`,
  `WGTokenInvalidException`) rather than removing them.
- Accepts community PRs of very different quality without normalizing style (`ImportAccountForm`).
- Versions by hand in `AssemblyInfo.cs`; tags releases; no changelog file.

---

## 8. Recommendations for the fork

Priority order, roughly by risk to the user's Steam account:

1. **Fix the encryption-removal-on-cancel bug (#1)** and the non-atomic writes. Write to
   `<file>.tmp`, flush, then `File.Replace` with a `.bak`. Keep the last N backups.
2. **Store salt + IV + a format version inside the maFile** (e.g. a JSON envelope
   `{ "v": 2, "kdf": "pbkdf2-sha256", "iter": 600000, "salt": ..., "iv": ..., "data": ... }`).
   Read v1 (manifest-side salt/IV) for migration, always write v2. Then an encrypted maFile is
   self-contained and importable alone.
3. **Switch to AES-GCM** (`System.Security.Cryptography.AesGcm`) so a wrong password is a
   clean authentication failure, not a padding roll of the dice.
4. **Replace `WebClient` with a single `HttpClient`** in `SteamWeb`, with a timeout, and return
   typed results instead of throwing on HTML responses.
5. **Extract the flows out of the forms** into plain classes (`AccountStore`, `LinkingFlow`,
   `ConfirmationService`) that take an `IUserPrompts` interface. Then unit-test TOTP, hash,
   encryption round-trip, manifest migration, and the link state machine.
6. **Fix the periodic checker** (#7, #8, #9): one popup per account, `try/finally` around the
   semaphore, await the accept/deny, and show what is being accepted.
7. **Remove the update phone-home** or point it at the fork with an opt-out.
8. **Modernize the project file:** `net8.0-windows` is fine, but drop `App.config`, ClickOnce
   and Squirrel leftovers, enable `<Nullable>enable</Nullable>`, add `[SupportedOSPlatform("windows")]`
   at the assembly level to silence CA1416, add `.editorconfig`, and add a GitHub Actions build.
9. **Vendor `SteamAuth`** instead of the submodule. Upstream geel9/SteamAuth is also dead, and
   you will need to change it.
10. Consider `AppContext.BaseDirectory` for the maFiles path, and an option to put maFiles in
    `%APPDATA%` for installed (non-portable) use.
11. Re-align server time periodically (every hour, or on any "invalid code" from Steam).

Things **not** worth changing: the protocol constants, the TOTP/hash implementations (once
tested), the maFile field names (compatibility with every other SDA-format tool: ASF, steamguard-cli,
SDA-Android, etc.), and the linking safety ritual.
