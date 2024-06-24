using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class Program
{
  static void Main(string[] args)
  {
    string rootPath = @"W:\repo"; // Cambia esta ruta por la ruta de tu carpeta
    var repoInfoList = new List<RepoInfo>();

    TraverseDirectory(rootPath, "", repoInfoList);

    PrintDirectoryTree(rootPath, "", repoInfoList);
  }

  static void TraverseDirectory(string currentPath, string relativePath, List<RepoInfo> repoInfoList)
  {
    foreach (var directory in Directory.GetDirectories(currentPath))
    {
      var dirName = Path.GetFileName(directory);
      var newRelativePath = Path.Combine(relativePath, dirName);

      if (File.Exists(Path.Combine(directory, ".gitignore")))
      {
        var repoUrl = GetGitRepositoryUrl(directory);
        repoInfoList.Add(new RepoInfo { Path = newRelativePath, Url = repoUrl });
      }
      else
      {
        TraverseDirectory(directory, newRelativePath, repoInfoList);
      }
    }
  }

  static string GetGitRepositoryUrl(string directory)
  {
    var startInfo = new ProcessStartInfo
    {
      FileName = "git",
      Arguments = "config --get remote.origin.url",
      WorkingDirectory = directory,
      RedirectStandardOutput = true,
      UseShellExecute = false,
      CreateNoWindow = true
    };

    using (var process = Process.Start(startInfo))
    {
      using (var reader = process.StandardOutput)
      {
        string result = reader.ReadToEnd();
        return result.Trim();
      }
    }
  }

  static void PrintDirectoryTree(string currentPath, string indent, List<RepoInfo> repoInfoList)
  {
    foreach (var directory in repoInfoList)
    {
      Console.WriteLine($"{indent}{directory.Path} - {directory.Url}");
    }
  }

  class RepoInfo
  {
    public string Path { get; set; }
    public string Url { get; set; }
  }
}
