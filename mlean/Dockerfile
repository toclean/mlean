# Base image with runtime dependencies
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Install FFmpeg, Opus, and PulseAudio dependencies
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    ffmpeg libopus0 libopusfile0 libopus-dev libavcodec-extra pulseaudio wget build-essential && \
    rm -rf /var/lib/apt/lists/*

# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["mlean.csproj", "./"]
RUN dotnet restore "mlean.csproj"

# Copy the remaining files and build the application
COPY . .
RUN dotnet publish "mlean.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 2: Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Ensure PulseAudio socket permissions (for audio support)
RUN useradd -m pulseuser && \
    mkdir -p /run/user/1000/pulse && \
    chown -R pulseuser:pulseuser /run/user/1000

# Switch to non-root user for security
USER pulseuser

# Set PulseAudio server environment variable for audio compatibility
ENV PULSE_SERVER=unix:/run/user/1000/pulse/native

# Entry point for the application
CMD ["dotnet", "mlean.dll"]
