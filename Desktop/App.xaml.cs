using System.Windows;

namespace MediaDownloader.Desktop;

public partial class App : Application
{
  private readonly LocalServer _localServer = new();

  protected override async void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    try
    {
      var serverAddress = await _localServer.StartAsync();
      var mainWindow = new MainWindow(serverAddress);
      MainWindow = mainWindow;
      mainWindow.Show();
    }
    catch (Exception exception)
    {
      MessageBox.Show($"Could not start local server.\n\n{exception.Message}", "Media Downloader", MessageBoxButton.OK, MessageBoxImage.Error);
      Shutdown();
    }
  }

  protected override void OnExit(ExitEventArgs e)
  {
    _localServer.Stop();
    base.OnExit(e);
  }
}
