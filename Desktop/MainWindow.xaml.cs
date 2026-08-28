using System.Windows;

namespace MediaDownloader.Desktop;

public partial class MainWindow : Window
{
  private readonly Uri _serverAddress;

  public MainWindow(Uri serverAddress)
  {
    InitializeComponent();
    _serverAddress = serverAddress;
    Loaded += OnLoaded;
  }

  private async void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      await Browser.EnsureCoreWebView2Async();
      Browser.Source = _serverAddress;
    }
    catch (Exception exception)
    {
      MessageBox.Show($"Could not open app interface.\n\n{exception.Message}", "Media Downloader", MessageBoxButton.OK, MessageBoxImage.Error);
      Close();
    }
  }
}
