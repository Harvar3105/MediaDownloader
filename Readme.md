# Project Structure

## Requirenments

1. yt-dlp binary
2. Installed ffmpeg, ffprobe
`WinGet install FFmpeg`
3. Installed node.js

### Publishing command

1. Run build

```language bash
cd frontend
npm run build
cd ..
```

2. Publsh exe

```language bash
npm --prefix frontend run build

dotnet publish Desktop/MediaDownloader.Desktop.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true -p:DebugType=None -p:DebugSymbols=false -o release
```

3. Find release exe in `release` folder
