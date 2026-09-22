namespace MediaDownloader.Application;

public static class ArgumentFactory
{
  private static string[] NecessaryArguments = new[] { "-q", "-o", "", "--js-runtime", "node" };

  public static string[] GetArguments(string pathWithName)
  {
    string[] copy = (string[]) NecessaryArguments.Clone();
    copy[2] = pathWithName;
    return copy;
  }
}