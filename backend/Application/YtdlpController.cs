using System.Diagnostics;

namespace MediaDownloader.Application;

public sealed class YtdlpController
{
  private readonly string _executablePath;

  public YtdlpController(string? executablePath = null)
  {
    _executablePath = executablePath ?? FindExecutablePath();
  }

  public async Task<YtdlpResult> RunAsync(CancellationToken cancellationToken = default, params string[] arguments)
  {
    using var process = CreateProcess(arguments);

    try
    {
      StartProcess(process);

      var standardOutputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
      var standardErrorTask = process.StandardError.ReadToEndAsync(cancellationToken);

      await process.WaitForExitAsync(cancellationToken);
      return new YtdlpResult(
        process.ExitCode,
        await standardOutputTask,
        await standardErrorTask);
    }
    catch (OperationCanceledException)
    {
      KillProcess(process);
      throw;
    }
  }

  public async Task<byte[]> RunBytesAsync(CancellationToken cancellationToken = default, params string[] arguments)
  {
    using var process = CreateProcess(arguments);

    try
    {
      StartProcess(process);

      using var output = new MemoryStream();
      var standardOutputTask = process.StandardOutput.BaseStream.CopyToAsync(
        output,
        cancellationToken);
      var standardErrorTask = process.StandardError.ReadToEndAsync(cancellationToken);

      await process.WaitForExitAsync(cancellationToken);
      await standardOutputTask;
      var error = await standardErrorTask;

      if (process.ExitCode != 0)
      {
        throw new InvalidOperationException(
          $"yt-dlp exited with code {process.ExitCode}: {error.Trim()}");
      }

      return output.ToArray();
    }
    catch (OperationCanceledException)
    {
      KillProcess(process);
      throw;
    }
  }

  private Process CreateProcess(IEnumerable<string> arguments)
  {
    ArgumentNullException.ThrowIfNull(arguments);

    var startInfo = new ProcessStartInfo
    {
      FileName = _executablePath,
      WorkingDirectory = Path.GetDirectoryName(_executablePath)!,
      UseShellExecute = false,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      CreateNoWindow = true,
    };

    foreach (var argument in arguments)
    {
      startInfo.ArgumentList.Add(argument ?? throw new ArgumentException(
        "Arguments cannot contain null values.", nameof(arguments)));
    }

    return new Process { StartInfo = startInfo, EnableRaisingEvents = true };
  }

  private static void StartProcess(Process process)
  {
    if (!process.Start())
    {
      throw new InvalidOperationException("Could not start yt-dlp.");
    }
  }

  private static void KillProcess(Process process)
  {
    if (process.HasExited)
    {
      return;
    }

    process.Kill(entireProcessTree: true);
  }

  private static string FindExecutablePath()
  {
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (directory != null)
    {
      var executablePath = Path.Combine(directory.FullName, "yt-dlp");
      if (File.Exists(executablePath))
      {
        return executablePath;
      }

      var windowsExecutablePath = Path.Combine(directory.FullName, "yt-dlp.exe");
      if (File.Exists(windowsExecutablePath))
      {
        return windowsExecutablePath;
      }

      directory = directory.Parent;
    }

    throw new FileNotFoundException(
      "yt-dlp was not found in the project root or its parent directories.",
      "yt-dlp");
  }
}

public sealed record YtdlpResult(int ExitCode, string StandardOutput, string StandardError)
{
  public bool IsSuccess => ExitCode == 0;
}
