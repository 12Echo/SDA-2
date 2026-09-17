# Steam Desktop Authenticator 2

A desktop implementation of Steam's mobile authenticator, continued from
[Jessecar96/SteamDesktopAuthenticator](https://github.com/Jessecar96/SteamDesktopAuthenticator),
which was deprecated upstream in October 2024.

The maFile format, `manifest.json`, the `maFiles` folder next to the executable, and the
`SteamAuth` library keep the same layout as upstream so existing files and tools keep working.
New settings are added to the same files as extra fields, nothing is renamed or moved.

## What it does

* Login codes with a countdown, copy from the window or the tray menu.
* Trade and market confirmations in a scrollable list, one card per confirmation, with the
  type (trade, market listing, phone change, account recovery) on each card.
* Confirmations arrive live over a Steam client connection, or by polling on an interval you
  pick, per account. A Windows notification or a small popup with accept and deny.
* Auto accept for market listings and for trades, with rules for trades: only when you give
  nothing, only with partners you list, or both. Anything that fails a rule stays for you.
* Approve a Steam login QR code that is on your screen, the way the mobile app scans it.
* Per account display names, drag to reorder, right click for the account menu.
* Recovery kit: a folder with the maFile and revocation code, offered when an account is added.
* Encryption of the maFiles folder with a passkey. The change is backed up and verified, and an
  interrupted change is restored on the next start.
* One click updates from the GitHub releases of this repository.
* Translations from json files in the `languages` folder, see [docs/TRANSLATING.md](docs/TRANSLATING.md).
* Dark, DPI aware interface.

## Running

Download the latest release zip, unpack it anywhere, run `Steam Desktop Authenticator 2.exe`.
It needs the .NET 8 Desktop Runtime, Windows offers to install it on first start if it is missing.
Windows 7 is not supported.

To move from an old Steam Desktop Authenticator, pick "Import accounts from an existing SDA
install" on the welcome screen, or copy the old `maFiles` folder next to the new exe.

## Building

Requires Windows and the .NET 8 SDK or newer.

```bash
dotnet build SteamDesktopAuthenticator.sln -c Release
```

Output: `Steam Desktop Authenticator/bin/Release/net8.0-windows/`.

A release build for distribution:

```bash
dotnet publish "Steam Desktop Authenticator/Steam Desktop Authenticator.csproj" -c Release -r win-x64 --self-contained false -o releases/x.y.z
```

## Layout

| Path | What |
|---|---|
| `Steam Desktop Authenticator/` | WinForms application |
| `lib/SteamAuth/` | Steam authenticator library, vendored from `geel9/SteamAuth` |
| `languages/` | Translation files and the template for new ones |
| `docs/` | Upstream README, notes, translating guide |

## Settings stored in manifest.json

Global: `start_with_windows`, `start_minimized`, `notification_style` (`Windows` or `Popup`),
`check_updates`, `language`.

Per entry: `confirmations` (`Off`, `Periodic`, `Live`), `check_interval`, `auto_confirm_trades`,
`auto_confirm_market`, `auto_confirm_trades_receive_only`, `auto_confirm_trades_partners_only`,
`auto_confirm_trade_partners`, `display_name`, `persona_name`, `avatar_url`, `profile_updated`.

Each maFile's `Session` may also carry `ClientRefreshToken`, the Steam client session used for
live confirmations. Tools that read maFiles can ignore it.
