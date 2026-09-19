<p align="center">
  <img src="icon.png" width="96" alt="">
</p>

<h1 align="center">Steam Desktop Authenticator 2</h1>

<p align="center">
  Steam Guard codes and trade confirmations on your desktop.<br>
  A maintained continuation of <a href="https://github.com/Jessecar96/SteamDesktopAuthenticator">Jessecar96/SteamDesktopAuthenticator</a>, which stopped in 2024.
</p>

<p align="center">
  <a href="../../releases/latest"><img src="https://img.shields.io/github/v/release/12Echo/SDA-2?label=download&color=4c9be8" alt="Latest release"></a>
  <img src="https://img.shields.io/badge/windows-10%20%7C%2011-4c9be8" alt="Windows 10 and 11">
  <img src="https://img.shields.io/badge/.NET-8%20Desktop%20Runtime-4c9be8" alt=".NET 8">
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-MIT-lightgrey" alt="MIT"></a>
</p>

---

## Before you use this

Read this part even if you skip the rest.

* **A maFile is your account.** Whoever has your `maFiles` folder can generate your login codes and
  accept your trades. Turn on encryption in the app, keep the folder out of cloud sync, and never send a
  maFile to anyone, no matter what they promise to fix.
* **Only download from this page.** Fake copies of the original SDA have emptied a lot of inventories.
  The only place to get this program is the [releases page](../../releases) of this repository.
  Anything else with this name is not us.
* **Write down your revocation code.** The app shows it when an account is added and offers to save a
  recovery kit. If you lose the maFile and the code, the only way back in is Steam Support.
* **Auto accept is a loaded gun.** Accepting confirmations without looking is exactly what API scams
  rely on. Leave it off unless the account is a bot, and if you do turn it on for trades, use the rules
  described below.
* **Removing the authenticator triggers Steam's trade hold.** Importing a maFile does not.
  Moving between this app, the old SDA and steamguard-cli is free. Moving to or from the Steam mobile
  app is not.
* SmartScreen will warn about the exe. The releases are not code signed, a certificate costs more than
  this project makes, which is nothing. Check the download came from here and carry on.

## Download

1. Install the **.NET 8 Desktop Runtime**, x64:
   [download from Microsoft](https://dotnet.microsoft.com/download/dotnet/8.0) (pick "Desktop Runtime" under .NET 8, Windows x64).
   Windows usually offers to install it on first start if it is missing.
2. Grab `SteamDesktopAuthenticator2-x.y.z.zip` from the [latest release](../../releases/latest).
3. Unpack it anywhere you like. There is no installer. Your accounts end up in a `maFiles` folder next to the exe.
4. Run `Steam Desktop Authenticator 2.exe`.

Windows 10 and 11 only. Windows 7 and 8 cannot run .NET 8 and cannot talk to Steam's servers anymore either.

## Coming from the original SDA

Nothing about the files changed. The `maFiles` folder, the `manifest.json` inside it and the maFile format
are the same as upstream, so:

* On the welcome screen pick **Import accounts from an old SDA install or a recovery kit** and select
  your old SDA folder. Everything is copied over, including your encryption passkey setup.
* Or just copy your old `maFiles` folder next to the new exe and start it.
* Tools that read maFiles (ArchiSteamFarm, steamguard-cli, trade bots) keep working on the same files.
  New settings are extra fields in `manifest.json`, nothing is renamed.

There is no trade hold for this. You are not removing or re-adding an authenticator, only moving the file.

## What is different from SDA

| | Steam Desktop Authenticator 1.0.x | Steam Desktop Authenticator 2 |
|---|---|---|
| Runtime | .NET Framework 4.7 | .NET 8 |
| Look | Windows 7 era forms, light only | Dark, DPI aware, one style everywhere |
| Confirmations | One window, fixed size, whole page reloads after every click | Scrollable cards with the type on each, only the card you acted on disappears |
| New confirmations | Poll every N seconds, popup that steals focus | Live over a Steam client connection, or poll, per account. Windows notification or a small popup that never takes focus |
| Auto accept | All trades or nothing | Rules: only trades where you give nothing, only trades with partners you list |
| Many accounts | All checked at the same instant, Steam rate limits you | Checks are spaced out, connections open one after another |
| Accounts list | Alphabetical, cannot be changed | Your order, drag to move, rename, right click menu |
| Encryption | Rewrites files in place, a crash mid way loses them | Backup, rewrite, verify, then delete the backup. Interrupted changes are restored on next start. Optional lock after idle time |
| Forgotten passkey | Wiki page | Restore from a recovery kit or backup after logging into each account, or start over with the old files kept |
| Passkey prompts | Shown in clear text | Masked, with a Show button |
| Recovery | Revocation code shown once | Recovery kit folder with the maFile and the code, offered when an account is added, saveable later |
| Login QR codes | Not supported | Approve or deny a Steam login QR that is on your screen, like the mobile app does |
| Updates | Link to the releases page | Downloads and installs itself, accounts untouched |
| Languages | English | Any, from a json file in `languages/` |

The old `-k passkey` and `-s` command line switches still work.

## Features

**Codes.** The current code with a countdown bar, a Copy button, Ctrl+C in the window, and the code in the
tray menu with its own countdown. Time is synced with Steam at start, every hour, after the PC wakes from
sleep and whenever the Windows clock is changed, with the round trip taken into account, so codes stay
right on a machine left running or with a wrong clock. Offline, it retries every 30 seconds instead of on
every code.

**Confirmations.** Trades, market listings, phone number changes and account recoveries as cards. Click a
card to expand it. Accept or cancel one at a time, or let the popup do it when a new one arrives. If Steam
refuses a trade confirmation because you have not acknowledged its trade protection notice yet, the app
tells you where to click.

**Live confirmations.** Per account, choose between not checking, checking every N seconds, or staying
connected to Steam so confirmations arrive the moment they exist. The live mode keeps one Steam client
session open per account and falls back to polling if Steam refuses the connection. It needs one extra
login the first time.

**Auto accept, with rules.** Market listings can be accepted without asking. Trades too, but you can
restrict them to offers where you give nothing, to partners whose SteamID64 you list, or both. The app reads
the actual offer behind the confirmation to decide. If it cannot read it, the trade is left for you.

**Accounts.** Profile name and avatar from Steam, a session indicator that turns orange when a login is
about to expire and red when it has expired or Steam rejects it, a display name of your choosing, drag to
reorder, search. Right click an account for login, confirmations, recovery kit, rename and remove.
Under the account card a warning row shows what keeps the account from trading: a limited account, and a
countdown while trades are still held after the authenticator was added. The dot next to an account in
the list has the same colour. Steam only tells the account itself why it
cannot trade (for example that Steam Guard must have been on for 15 days), so SDA loads Steam's trade pages
with the account's session once a day and shows the answer in the warning row; Selected Account, Check
trade status asks right away. Until Steam has been asked, a fresh authenticator shows a 15 day countdown.
Import Account takes several maFiles at once.

**QR login approval.** Open a Steam sign in page or the Steam client so its QR code is on screen, then
Selected Account, Approve login QR on screen. The app finds the code on any of your monitors, asks Steam
which device and location is behind it, and you approve or deny. Same protocol as the mobile app.

**Tray.** Restore, switch account, view confirmations, check for confirmations now, copy the code, approve
a QR, quit. Start with Windows and start minimized are in Settings.

**Encryption.** AES with a passkey you choose, the same scheme as upstream so encrypted files stay
compatible. Changing or removing the passkey is backed up and verified. Settings can lock SDA after N
minutes without keyboard or mouse input: the passkey is dropped from memory, codes and accounts leave the
window and the tray, and the next click asks for it again. File, Lock does it at once.

**Forgot the passkey.** The passkey prompt has a "Forgot passkey?" link. It restores accounts from a
recovery kit or backup folder, after a fresh login with the account password for each one, and moves the
old encrypted files to a `maFiles.locked-<date>` folder next to the app in case the passkey turns up. With
no kit, it sets the files aside, opens Steam's help page for removing the authenticator, and starts empty.

**Backups.** File, Back up all accounts writes a folder with an unencrypted maFile per account and a
`revocation codes.txt`. The welcome screen and the passkey recovery both read that folder back. SDA asks
once a month if there has been no backup for three months.

**Groups.** Give accounts a group from the Selected Account menu, then pick a group in the dropdown next to
the search box to show only those. The choice is remembered. Groups show as a small chip in the list.

**Batch confirmations.** Accept all or Cancel all at the top of the confirmations window, or tick some
cards and the buttons act on those only. One confirmation dialog, one request to Steam.

**Log.** `sda2.log` next to the app records errors, refused confirmations, live connection failures,
updates, locks and recoveries. Never codes, tokens or secrets. It rolls over at 1 MB. If SDA crashes, the
dialog says so and the stack trace is in the log.

**Updates.** Checked at startup (can be turned off) and on demand. One click downloads the release,
swaps the files after the app closes, and starts the new version. The `maFiles` folder is never touched.

## Settings reference

All settings live in `maFiles/manifest.json`. Upstream fields are unchanged, these are added.

Global

| Field | Values | Meaning |
|---|---|---|
| `start_with_windows` | bool | Run key under HKCU |
| `start_minimized` | bool | Start in the tray |
| `notification_style` | `Windows`, `Popup` | How new confirmations are announced |
| `check_updates` | bool | Check GitHub releases at startup |
| `language` | e.g. `de` | File in `languages/` to load, empty for English |
| `lock_after_minutes` | minutes, 0 for never | Lock after this much time without input, needs encryption |
| `last_backup`, `backup_reminded` | unix time | When the last backup was saved and the reminder last shown |
| `list_group` | string | Group the account list is filtered to, empty for all |

Per account entry

| Field | Values | Meaning |
|---|---|---|
| `confirmations` | `Off`, `Periodic`, `Live` | How this account is watched |
| `check_interval` | seconds, 5 or more | Polling interval for `Periodic` |
| `auto_confirm_market` | bool | Accept market listings unattended |
| `auto_confirm_trades` | bool | Accept trades unattended, subject to the two rules below |
| `auto_confirm_trades_receive_only` | bool | Only trades where this account gives nothing |
| `auto_confirm_trades_partners_only` | bool | Only trades with the partners listed |
| `auto_confirm_trade_partners` | list of SteamID64 | The partner list |
| `display_name` | string | Name shown in the app, empty for the Steam profile name |
| `persona_name`, `avatar_url`, `profile_updated` | | Cached profile data |
| `limited_account` | bool | Cached with the profile |
| `group` | string | Group shown in the list and used by the filter |
| `trade_note`, `trade_checked` | string, unix time | What Steam's trade page said about the account and when it was asked |

A maFile's `Session` may contain `ClientRefreshToken`, the Steam client session used for live mode.
Other tools can ignore it.

## Translations

Copy `languages/template.json` to `languages/<code>.json`, fill in the values, pick the language in
Settings. Details and how to send one in as a pull request are in [docs/TRANSLATING.md](docs/TRANSLATING.md).

## Building

Windows, .NET 8 SDK or newer.

```bash
dotnet build SteamDesktopAuthenticator.sln -c Release
```

For a distributable folder like the releases:

```bash
dotnet publish "Steam Desktop Authenticator/Steam Desktop Authenticator.csproj" -c Release -r win-x64 --self-contained false -o out
```

| Path | What |
|---|---|
| `Steam Desktop Authenticator/` | The WinForms app |
| `lib/SteamAuth/` | Steam authenticator library, from `geel9/SteamAuth`, with additive changes only |
| `languages/` | Translation files and the template |
| `docs/` | Upstream README, an analysis of the upstream code, the translating guide |

## Troubleshooting

**"Session expired, login again"** on the account card. Steam sessions last a while but not forever.
Selected Account, Login again. Your codes keep working in the meantime, only confirmations need the session.

**Confirm does nothing on a trade.** Steam introduced a trade protection notice that has to be
acknowledged once in a browser. Log in on steamcommunity.com, open your inventory, Trade Offers, accept
the notice, then confirm again.

**Steam asks for a phone code but I have no phone on the account.** The code comes by email in that case.
Check spam.

**403 or 429 when checking confirmations.** Too many requests from one IP in a short time. Raise the check
interval, or use live mode, which does not poll at all.

**I forgot my encryption passkey.** There is no way around it, that is the point of encryption. Use your
recovery kit, or remove the authenticator with the revocation code and set it up again (15 day trade hold).

**The update check says nothing.** It only speaks up when there is a newer version. Settings has a switch
for the startup check, and the link at the bottom of the main window checks on demand.

## Credits

Jesse Cardone wrote the original Steam Desktop Authenticator and geel9 wrote SteamAuth, both MIT licensed.
Live confirmations and QR approval use [SteamKit2](https://github.com/SteamRE/SteamKit). QR codes are read with
[ZXing.Net](https://github.com/micjahn/ZXing.Net).

If you run Linux or prefer a terminal, [steamguard-cli](https://github.com/dyc3/steamguard-cli) reads the same
maFiles and is actively maintained.
