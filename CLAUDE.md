# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ooceBot is a Twitch chat bot for the channel `obtooce`, built as a .NET 7.0 console application. It handles chat commands, audio/video playback via OBS, channel point management, user attendance tracking, and timed announcements.

## Build & Run

```bash
# Build
dotnet build

# Run (Debug)
dotnet run

# Build Release (also auto-runs the executable via post-build hook in .csproj)
dotnet build -c Release
```

The project targets .NET 7.0. Visual Studio is the primary IDE (ooceBot.sln).

## Key Dependencies

- **TwitchLib** — Twitch IRC client and API wrapper
- **NAudio** — Audio playback with fade effects
- **obs-websocket-dotnet** — OBS WebSocket control for video/audio sources
- **Microsoft.Data.Sqlite** — SQLite database (persistent user/stats storage)
- **Zalgo** — Text corruption for timer messages

Configuration/credentials are stored in `App.config` (not committed — contains OAuth tokens, API keys, OBS password, Nightbot credentials).

## Architecture

### Command Routing (Program.cs + BotVariables.cs)

Commands are routed via four static `Dictionary<string, Action<CommandArgs>>` in `BotVariables.cs`:

| Dictionary | Access Level | Examples |
|---|---|---|
| `AdminCommands` | Broadcaster/mod only | `!back`, `!brb`, `!game`, `!title` |
| `CommandsList` | All chatters | `!here`, `!play`, `!quote`, `!stats` |
| `VideoCommands` | Subscribers/VIPs | `!lobster`, `!who`, `!wtf` |
| `WordCommands` | All chatters | `f`, `lol`, `nice`, `w`, `wow` |

`Program.cs` receives each chat message, checks access level, then looks up and invokes the delegate. All command methods receive a single `CommandArgs` object (defined in `Commands/CommandArgs.cs`) containing the TwitchClient, ChatMessage, SqliteConnection, HttpClient, TwitchAPI, parsed command text/quantifier, and a Random instance.

### Command Implementation Files

- `Commands/CommandMethods.cs` — General user commands (28 methods)
- `Commands/AdminCommandMethods.cs` — Broadcaster/mod commands
- `Commands/VideoCommandMethods.cs` — Sub/VIP video commands; each connects to OBS, reduces Nightbot volume, plays video, restores volume
- `Commands/WordCommandMethods.cs` — Simple one-word trigger responses

### Database (SQLite)

Four tables initialized at startup via `SQL/TableSQLMethods.cs`:
- **Chatters** — Id, DisplayName, HasTheme, HasChattedThisStream
- **Attendance** — AttendanceCount, TotalAttendance, IsPresent, PointsForRedemption
- **ArcadeRecords** — wagering history, token balance, streaks
- **CommandUsage** — per-command usage counts

`SQL/DBQueryMethods.cs` contains helper methods called on each message (`UpdateChatterDataPlusMaybeTheme`, `VerifyExistenceInChattersTable`).

### External Integrations

- **OBS** (`AudioVideo/PlayVideos.cs`, `Authorization/OBSManager.cs`) — Connects via WebSocket to play/hide media sources
- **Nightbot** (`AudioVideo/VolumeControl.cs`, `Authorization/NighbotOAuthManager.cs`) — HTTP API calls to control song request volume during video playback
- **Twitch API** (`Authorization/TwitchOAuthManager.cs`, `Functionality/StreamCommandMethods.cs`) — Updates stream title/game, manages OAuth refresh
- **Chess.com** (`Functionality/ChessCommandMethods.cs`) — REST API for player stats/audit (no auth required)
- **Twitch EventSub WebSocket** (`Authorization/WebSocketMethods.cs`) — Subscription event handling (partially implemented)

### Arcade/Token System

`Miscellaneous/ArcadeMethods.cs` implements a wagering mini-game (`!play` command): 50/50 RNG with a slight house edge (midpoint at 45/100). Tracks per-user stats in the ArcadeRecords table.

### Attendance System

`!here` command in `CommandMethods.cs` tracks daily check-ins. Every 10th attendance awards 2000 channel points (`BotVariables.ATTENDANCE_REDEMPTION_POINTS`).

### Timer Messages

`Timers/TimerMethods.cs` posts one of 15 rotating messages to chat every 20 minutes using .NET 6+ `PeriodicTimer`.

## Adding New Commands

1. Add the method to the appropriate `*CommandMethods.cs` file with signature `public static void MethodName(CommandArgs args)`
2. Register it in the appropriate dictionary in `BotVariables.cs`
3. For general commands, also add it to `CommandDictionary` (used by `!help`)
4. If it needs a DB entry in CommandUsage, add it to `PopulateCommandUsageTable()` in `SQL/DBQueryMethods.cs`
