
---

# 🎵 mLean Music Bot 🎵

A powerful and customizable Discord music bot built using **Lavalink** and **Lavalink4NET**, designed to bring seamless music playback, EQ control, and audio filtering to your Discord server.

---

## 🚀 Features

- Play music from YouTube, SoundCloud, and other sources.
- Advanced Equalizer (EQ) and audio filters support.
- Commands for track management (Play, Pause, Stop, Skip, etc.).
- Automatic status updates for the currently playing song.
- Integrated OAuth support for YouTube access.
- Persistent configuration using `application.yml` and `Constants.cs`.

---

## 📂 Project Structure

```
.
├── Dockerfile             # Docker setup for containerized deployment
├── docker-compose.yml     # Compose file for multi-container orchestration
├── application.yml        # Lavalink configuration (must be created)
├── src/
│   ├── AudioModule.cs     # Core music commands and audio logic
│   ├── Constants.cs       # Bot constants (e.g., tokens, IDs)
│   └── Program.cs         # Main entry point for the bot
└── plugins/               # Lavalink plugins (optional)
```

---

## ⚙️ Prerequisites

Before running the bot, ensure you have the following:

1. **Lavalink**: A self-hosted audio service.
2. **Docker** and **Docker Compose** (for containerized deployment).
3. **application.yml**: Required for configuring Lavalink.

---

## 🔧 Configuration

### 1️⃣ `application.yml` (Lavalink Configuration)

This configuration file is essential for Lavalink. It must be placed in the root directory (same level as `docker-compose.yml`).

Example content:

```yaml
server:
  port: 2333
  address: 0.0.0.0
  plugins:
    youtube:
      enabled: true
      oauth:
        enabled: true
        clients:
          - "WEB"
      token: "your-token-here"
lavalink:
  server:
    password: "youshallnotpass"
```

### 2️⃣ `Constants.cs` (Bot Secrets and Configuration)

Create a `Constants.cs` file in the `src/` directory to store sensitive data like tokens and API keys.

```csharp
namespace mlean
{
    public static class Constants
    {
        public const string BotToken = "your-bot-token";
        public const ulong GuildId = 123456789012345678;  // Example Guild ID
    }
}
```

---

## 🐳 Docker Setup

### Step 1: Build the Docker Image

```bash
docker-compose build
```

### Step 2: Run the Bot and Lavalink

```bash
docker-compose up -d
```

This command will start both the **Lavalink** service and the **mLean bot** in detached mode.

### Step 3: Verify the Services

Use the following commands to check logs:

```bash
docker logs lavalink
docker logs mlean
```

---

## 📦 Environment Variables

The bot and Lavalink services rely on several environment variables configured in `docker-compose.yml`:

- **`BOT_PREFIX`**: Command prefix (default: `!`).
- **`LAVALINK_SERVER_PASSWORD`**: Lavalink server password (default: `youshallnotpass`).
- **`SERVER_ADDRESS`**: Address of the Lavalink server (default: `http://lavalink`).
- **`PULSE_SERVER`**: PulseAudio configuration for sound devices.

---

## 📜 Commands

Use the bot's prefix (default: `!`) followed by the command name.

### Music Commands
- **`!join`**: Joins your current voice channel.
- **`!leave`**: Leaves the voice channel.
- **`!play <query>`**: Plays a song or adds it to the queue.
- **`!pause`**: Pauses the current track.
- **`!resume`**: Resumes playback.
- **`!skip`**: Skips the current song.
- **`!stop`**: Stops the music and clears the queue.

### Filter Commands
- **`!show-filters`**: Displays the status of all audio filters.
- **`!screw-it`**: Toggles the Screw-It mode.
- **`!timescale <speed> <pitch> <rate>`**: Adjusts the playback timescale.
- **`!vibrato <frequency> <depth>`**: Applies or disables the vibrato filter.

### Equalizer Commands
- **`!set-eq <band>:<gain>`**: Adjusts the gain of a specific EQ band.
- **`!set-eq-all <gain>`**: Sets all EQ bands to the same gain.
- **`!show-eq`**: Displays the current EQ settings visually.
- **`!reset-equalizer`**: Resets the EQ to default settings.

---

## 📖 Troubleshooting

### Issue: Lavalink Won't Start
- Ensure `application.yml` is correctly mounted and accessible.
- Check that the Java options are correctly set in `docker-compose.yml`.

### Issue: Bot Not Connecting to Lavalink
- Verify that the Lavalink service is running on port 2333.
- Ensure the password matches between the bot and `application.yml`.

---

## 🛠️ Development Setup

### Clone the Repository
```bash
git clone https://github.com/yourusername/mlean-bot.git
cd mlean-bot
```

### Install Dependencies
```bash
dotnet restore
```

### Run Locally
Make sure Lavalink is running, then:
```bash
dotnet run --project src/mlean.csproj
```

---

## 🤝 Contributing

We welcome contributions! To get started:

1. Fork the repository.
2. Create a new branch: `git checkout -b feature/your-feature`.
3. Commit your changes: `git commit -m 'Add some feature'`.
4. Push to the branch: `git push origin feature/your-feature`.
5. Submit a pull request.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## ❤️ Acknowledgments

- [Lavalink](https://github.com/freyacodes/Lavalink) - Audio streaming server.
- [Discord.Net](https://github.com/discord-net/Discord.Net) - .NET wrapper for the Discord API.
- [Lavalink4NET](https://github.com/angelobreuer/Lavalink4NET) - Lavalink client for .NET.

---

Feel free to copy, modify, and share this template. Let us know if you encounter any issues! 🎶

---

This `README.md` ensures the repository is well-documented, covering all aspects from setup to usage and troubleshooting.