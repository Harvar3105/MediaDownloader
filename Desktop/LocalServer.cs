using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace MediaDownloader.Desktop;

public sealed class LocalServer
{
  private readonly HttpClient _httpClient = new();
  private Process? _process;

  public async Task<Uri> StartAsync()
  {
    var port = GetPort();
    var address = new Uri($"http://127.0.0.1:{port}");
    _process = StartProcess(address);

    try
    {
      await WaitUntilReadyAsync(address);
      return address;
    }
    catch
    {
      Stop();
      throw;
    }
  }

  public void Stop()
  {
    if (_process is null)
    {
      return;
    }

    try
    {
      if (!_process.HasExited)
      {
        _process.Kill(true);
        _process.WaitForExit(5000);
      }
    }
    finally
    {
      _process.Dispose();
      _process = null;
    }
  }

  private static int GetPort()
  {
  #if DEBUG
    return 5003;
  #else
    return 5000;
  #endif
  }

  private static Process StartProcess(Uri address)
  {
    var applicationDirectory = GetApplicationDirectory();
    var publishedServerPath = Path.Combine(applicationDirectory, "Server", "MediaDownloader.Api.exe");
    var startInfo = new ProcessStartInfo
    {
      UseShellExecute = false,
      CreateNoWindow = true,
      WindowStyle = ProcessWindowStyle.Hidden,
      WorkingDirectory = Path.GetDirectoryName(publishedServerPath) ?? AppContext.BaseDirectory
    };

    if (File.Exists(publishedServerPath))
    {
      startInfo.FileName = publishedServerPath;
      startInfo.ArgumentList.Add("--urls");
      startInfo.ArgumentList.Add(address.ToString());
    }
    else
    {
      var projectPath = FindApiProjectPath();
      startInfo.FileName = "dotnet";
      startInfo.WorkingDirectory = Path.GetDirectoryName(projectPath)!;
      startInfo.ArgumentList.Add("run");
      startInfo.ArgumentList.Add("--project");
      startInfo.ArgumentList.Add(projectPath);
      startInfo.ArgumentList.Add("--no-launch-profile");
      startInfo.ArgumentList.Add("--");
      startInfo.ArgumentList.Add("--urls");
      startInfo.ArgumentList.Add(address.ToString());
    }

    return Process.Start(startInfo) ?? throw new InvalidOperationException("Could not create local server process.");
  }

  private async Task WaitUntilReadyAsync(Uri address)
  {
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

    while (!timeout.IsCancellationRequested)
    {
      if (_process?.HasExited == true)
      {
        throw new InvalidOperationException("Local server has exited during launch.");
      }

      try
      {
        using var response = await _httpClient.GetAsync(address, timeout.Token);
        if (response.IsSuccessStatusCode)
        {
          return;
        }
      }
      catch (HttpRequestException)
      {
      }

      await Task.Delay(250, timeout.Token);
    }

    throw new TimeoutException("Local server did not respond in 30 sec.");
  }

  private static string FindApiProjectPath()
  {
    var directory = new DirectoryInfo(GetApplicationDirectory());

    while (directory is not null)
    {
      var projectPath = Path.Combine(directory.FullName, "backend", "Api", "MediaDownloader.Api.csproj");
      if (File.Exists(projectPath))
      {
        return projectPath;
      }

      directory = directory.Parent;
    }

    throw new FileNotFoundException("Could not find local project file.");
  }

  private static string GetApplicationDirectory()
  {
    return Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
  }
}
