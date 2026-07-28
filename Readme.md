Project Structure:

MediaDownloader/
├── Backend/
│
│   ├── MediaDownloader.Api/
│   │   ├── Controllers/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── MediaDownloader.Api.csproj
│   │
│   ├── MediaDownloader.Domain/
│   │   ├── Entities/
│   │   ├── Interfaces/
│   │   └── MediaDownloader.Domain.csproj
│   │
│   ├── MediaDownloader.Application/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── MediaDownloader.Application.csproj
│   │
│   ├── MediaDownloader.Infrastructure/
│   │   ├── Extractors/
│   │   ├── FFmpeg/
│   │   ├── Storage/
│   │   └── MediaDownloader.Infrastructure.csproj
│   │
│   └── MediaDownloader.Workers/
│       ├── DownloadWorker.cs
│       └── MediaDownloader.Workers.csproj
│
├── Frontend/
│   ├── src/
│   └── package.json
│
└── MediaDownloader.sln