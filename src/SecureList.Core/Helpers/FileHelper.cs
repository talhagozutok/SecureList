using SecureList.Core.Entities;
using System.Diagnostics;

namespace SecureList.Core.Helpers;

public static class FileHelper
{
    public static string[] ReadLinesFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"{filePath} could not found.");
        }

        return File.ReadAllLines(filePath);
    }

    public static bool IsFileContentsEqual(string filePath1, string filePath2)
    {
        try
        {
            if (!File.Exists(filePath1) || !File.Exists(filePath2))
            {
                return false;
            }

            string content1 = File.ReadAllText(filePath1);
            string content2 = File.ReadAllText(filePath2);

            return content1.Equals(content2);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error while comparing file contents: " + ex.Message);
            return false;
        }
    }

    public static void CopyProcessedFile(PasswordFile processedFile)
    {
        var processedDirectory = StaticDetails.ProcessedDirectory;

        if (!Directory.Exists(processedDirectory))
        {
            Directory.CreateDirectory(processedDirectory);
        }

        File.Copy(
            sourceFileName: processedFile.FullPath,
            destFileName: Path.Combine(processedDirectory, processedFile.Name),
            overwrite: true);
    }

    public static bool IsPathValid(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        var invalidWildcards = new HashSet<char> { '\\', '/', ':', '?', '<', '>', '|', '*', '.' };
        var invalidPathChars = Path.GetInvalidPathChars();
        var invalidChars = invalidWildcards.Union(invalidPathChars);

        return !path.Any(invalidChars.Contains);
    }

    public static DirectoryInfo CreateSubdirectoryInIndexDirectoryWithChar(char ch)
    {
        var indexDirectory = StaticDetails.IndexDirectory;
        var newDirectory = Path.Combine(indexDirectory, ch.ToString());

        if (!IsPathValid(ch.ToString()))
        {
            newDirectory = Path.Combine(indexDirectory, "undefined");
        }

        var createdDirectory = Directory.CreateDirectory(newDirectory);

        return createdDirectory;
    }
}
