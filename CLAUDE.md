# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ooceBot is a Twitch chat bot for the channel `obtooce`, built as a .NET 9.0 console application. It handles chat commands, audio/video playback via OBS, channel point management, user attendance tracking, and timed announcements.

## Build & Run

```bash
# Build
dotnet build

# Run (Debug)
dotnet run

# Build Release (also auto-runs the executable via post-build hook in .csproj)
dotnet build -c Release
```

The project targets .NET 9.0. Visual Studio is the primary IDE (ooceBot.sln). A separate `ooceBot.Tests` project holds tests and is excluded from the main project's compile items.

## Key Dependencies

- **TwitchLib** / **TwitchLib.EventSub.Websockets** — Twitch IRC client, Helix API wrapper, and EventSub subscriptions
- **NAudio** — Audio playback with fade effects
- **obs-websocket-dotnet** — OBS WebSocket control for video/audio sources
- **Microsoft.Data.Sqlite** — Raw ADO.NET SQLite access (table creation, some direct queries)
- **Microsoft.EntityFrameworkCore** / **Microsoft.EntityFrameworkCore.Sqlite** — EF Core access to the same SQLite database via `Models/TwitchBotContext.cs`
- **Microsoft.Extensions.Hosting** — referenced, not yet wired into a generic host (see below)
- **System.Configuration.ConfigurationManager** — `App.config` settings access

Configuration/credentials are stored in `App.config` (not committed — contains OAuth tokens, API keys, OBS password, Nightbot credentials).

## Architecture

### Command Routing (Program.cs + BotVariables.cs)

Commands are routed via static `Dictionary<string, BotVariables.Command>` fields in `BotVariables.cs` (`Command` is a `delegate void Command(CommandArgs args)`):

| Dictionary | Access Level | Examples |
|---|---|---|
| `AdminCommands` | Broadcaster/mod only | `back`, `brb`, `!game`, `!loud`, `!rngmove`, `!title` |
| `ChatterCommands` | All chatters, community-specific | `!boner`, `!jacob`, `!tarf` |
| `CommandsList` | All chatters, general | `!here`, `!play`, `!quote`, `!stats` |
| `WordCommands` | All chatters, single-word triggers | `!`, `f`, `lol`, `nice`, `w`, `wow` |

`Program.cs` receives each chat message via `Client_OnMessageReceived`, first updates chatter data, then checks bits, then looks up and invokes the delegate from `CommandsList` → `AdminCommands` → `ChatterCommands` → `WordCommands` (in that lookup order). All command methods receive a single `CommandArgs` object (defined in `Commands/CommandArgs.cs`) containing the TwitchClient, ChatMessage, SqliteConnection, HttpClient, TwitchAPI, parsed command text/quantifier, a `TwitchBotContext` (EF Core), and a `Random` instance.

### Command Implementation Files

- `Commands/CommandMethods.cs` — General user commands (~38 methods)
- `Commands/AdminCommandMethods.cs` — Broadcaster/mod commands (`Back`, `BRB`, `Game`, `Loud`, `RNGMove`, `Title`)
- `Commands/ChatterCommandMethods.cs` — Community-specific one-off commands (`Boner`, `Jacob`, `Tarf`)
- `Commands/WordCommandMethods.cs` — Simple one-word trigger responses

Video/audio playback used by admin commands lives in `AudioVideo/PlayVideos.cs` and `AudioVideo/PlaySounds.cs`, not in a dedicated command file.

### Database (SQLite)

Six tables initialized at startup via `SQL/TableSQLMethods.cs` (`InitializeAllTables`):
- **Chatters** — Id, DisplayName, HasTheme, LastChattedStreamDate (compared against `BotVariables.StreamStartTime`, not a boolean, so a bot restart mid-stream doesn't re-trigger first-message-of-stream logic like auto-shoutouts)
- **AttendanceRecords** — AttendanceCount, TotalAttendance, LastPresentDate, PointsForRedemption
- **ArcadeRecords** — wagering history, token balance, streaks
- **CommandUsage** — per-command usage counts (reset to 0 on every startup)
- **DapRecords** — DapsGiven, DapsReceived
- **GeneralStreamData** — single-row table for misc counters (e.g. BirdCounter)

Two parallel data-access paths exist against the same `TwitchStats.db`:
- Raw ADO.NET via `Microsoft.Data.Sqlite` (`CommandArgs.Connection`), used for most existing queries.
- EF Core via `Models/TwitchBotContext.cs` (`CommandArgs.Context`, backed by the static `Program.dbContext`), with `DbSet`s mirroring the tables above (`Chatters`, `AttendanceRecords`, `ArcadeRecords`, `DapRecords`, `GeneralStreamData`).

`SQL/DBQueryMethods.cs` contains `UpdateChatterDataPlusMaybeTheme`, called on every message via the EF Core context to upsert the chatter row.

### External Integrations

- **OBS** (`AudioVideo/PlayVideos.cs`, `AudioVideo/PlaySounds.cs`, `Authorization/OBSManager.cs`) — Connects via WebSocket to play/hide media and audio sources
- **Nightbot** (`AudioVideo/VolumeControl.cs`, `Authorization/NighbotOAuthManager.cs`) — HTTP API calls to control song request volume during video playback
- **Twitch API** (`Authorization/TwitchOAuthManager.cs`, `Functionality/StreamCommandMethods.cs`) — Updates stream title/game, manages OAuth refresh
- **Chess.com** (`Functionality/ChessCommandMethods.cs`) — REST API for player stats/audit (no auth required)
- **Twitch EventSub WebSocket** (`Authorization/EventSubWebsocketManager.cs`, `Functionality/WebSocketMethods.cs`) — Subscription event handling
- **GitHub Gist** (`Authorization/GistManager.cs`) — Publishes the current custom channel-point rewards list (`BotVariables.CustomRewards`) to a Gist on startup

### Arcade/Token System

`Miscellaneous/ArcadeMethods.cs` implements a wagering mini-game (`!play` command): 50/50 RNG with a slight house edge (midpoint at 45/100). Tracks per-user stats in the ArcadeRecords table.

### Attendance System

`!here` command in `CommandMethods.cs` tracks daily check-ins. Every 10th attendance awards channel points (`BotVariables.ATTENDANCE_POINT_VALUE`, currently 2000).

### Timer Messages

`Timers/TimerMethods.cs` posts one of 9 rotating messages to chat every 10 minutes (interval passed in from `Program.cs`) using .NET 6+ `PeriodicTimer`.

## Adding New Commands

1. Add the method to the appropriate `*CommandMethods.cs` file with signature `public static void MethodName(CommandArgs args)`
2. Register it in the appropriate dictionary in `BotVariables.cs`
3. For general commands, also add it to `CommandDictionary` (used by `!help`)
4. If it needs a DB entry in CommandUsage, add a corresponding row/handling for it wherever `CommandUsage` counts are incremented

## Frontend (ooceBotWithFrontend)

`ooceBotWithFrontend` is a duplicate of this project and is the active workspace for adding a React-based UI/API layer on top of the bot. Treat `ooceBot` as the stable, frontend-free baseline — do not add frontend/API code here; make those changes in `ooceBotWithFrontend` instead. Port over fixes/features made in one project to the other manually if they need to stay in sync.
