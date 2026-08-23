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
dotnet publish .\backend\Api\MediaDownloader.Api.csproj  -c Release  -r win-x64  --self-contained true  -p:PublishSingleFile=true  -p:IncludeNativeLibrariesForSelfExtract=true -o .\release
```

3. Find release exe in `release` folder
