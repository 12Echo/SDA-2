# Steam Desktop Authenticator

A desktop implementation of Steam's mobile authenticator. This is a maintained fork of
[Jessecar96/SteamDesktopAuthenticator](https://github.com/Jessecar96/SteamDesktopAuthenticator),
which was deprecated upstream in October 2024.

The maFile format, `manifest.json`, the `maFiles` folder next to the executable, and the
`SteamAuth` library keep the same layout as upstream so existing files and tools keep working.

## Building

Requires Windows and the .NET 8 SDK or newer.

```bash
dotnet build SteamDesktopAuthenticator.sln -c Release
```

Output: `Steam Desktop Authenticator/bin/Release/net8.0-windows/`. Running the build needs the
.NET 8 Desktop Runtime.

## Layout

| Path | What |
|---|---|
| `Steam Desktop Authenticator/` | WinForms application |
| `lib/SteamAuth/` | Steam authenticator library, vendored from `geel9/SteamAuth` |
| `docs/` | Upstream README and notes |
