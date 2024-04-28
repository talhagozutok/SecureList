using SecureList.Core.Entities;
using SecureList.Core.Helpers;

namespace SecureList.Core;

public class IndexService(PasswordRepository passwordRepository)
{
    private readonly PasswordRepository _passwordRepository = passwordRepository;

    public void Index(Dictionary<string, string> passwords)
    {
        // Groups passwords by the first character (case-sensitive) of their keys
        // then chunks each group into subgroups, each containing a maximum number of lines specified by StaticDetails.MaximumLineNumberInIndexFiles.
        var charGroups = passwords
            .GroupBy(kvp => kvp.Key[0].ToString(), StringComparer.Ordinal)
            .Select(charGroup => charGroup.Chunk(StaticDetails.MaximumLineNumberInIndexFiles))
            .ToList();

        int fileNaming = 0;
        foreach (var chGroup in charGroups)
        {
            foreach (var chunk in chGroup)
            {
                foreach (var line in chunk)
                {
                    var password = line.Key;
                    var filePath = line.Value;

                    var directory = FileHelper.CreateSubdirectoryInIndexDirectoryWithChar(password[0]);
                    var (md5Hash, sha1Hash, sha256Hash) = CryptographyHelper.CalculateHashes(password);

                    File.AppendAllText(
                        path: Path.Combine(directory.ToString(), $"{fileNaming}.txt"),
                        contents: $"{password}|{md5Hash}|{sha1Hash}|{sha256Hash}|{filePath}\n");
                }
                fileNaming++;
            }
        }
    }

    public void ReadIndex()
    {
        var indexDirectory = StaticDetails.IndexDirectory;

        if (!Directory.Exists(indexDirectory))
        {
            return;
        }

        var filePaths = Directory.GetFiles(indexDirectory, "*.*", SearchOption.AllDirectories);
        foreach (var fp in filePaths)
        {
            var lines = FileHelper.ReadLinesFromFile(fp);
            foreach (var line in lines)
            {
                var parts = line.Split("|");

                _passwordRepository.TryAddPassword(parts[0], parts[4]);
            }
        }
    }
}
