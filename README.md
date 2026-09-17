# SDA-2

Private fork / rewrite of [Steam Desktop Authenticator](https://github.com/Jessecar96/SteamDesktopAuthenticator),
which was deprecated upstream in October 2024.

## Layout

| Path | What |
|---|---|
| `upstream-src/` | Vendored copy of upstream at `3443a52` (v1.0.15) with the `geel9/SteamAuth` submodule inlined at `01c0c9b`. Unmodified reference. |
| `docs/UPSTREAM-ANALYSIS.md` | Full read-through: architecture, data formats, protocol details, build steps, code-quality review, bug list, recommendations. |

## Building the upstream reference

Requires Windows and the .NET 8 SDK or newer.

```bash
dotnet build upstream-src/SteamDesktopAuthenticator.sln -c Release
```

Output: `upstream-src/Steam Desktop Authenticator/bin/Release/net8.0-windows/`.
