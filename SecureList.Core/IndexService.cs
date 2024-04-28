using SecureList.Core.Entities;
using SecureList.Core.Helpers;

namespace SecureList.Core;

public class IndexService(PasswordRepository passwordRepository)
{
    private readonly PasswordRepository _passwordRepository = passwordRepository;

    public void Index(Dictionary<string, string> passwords)
    {
        var charGroups = passwords
            .GroupBy(kvp => kvp.Key[0].ToString(), StringComparer.Ordinal)
            .Select(k =>
                k.Select(kvp => new
                {
                    Password = kvp.Key,
                    Filename = kvp.Value,
                    Hashes = CryptographyHelper.CalculateHashes(kvp.Key)
                })
                .Select(a => $"{a.Password}|{a.Hashes.md5Hash}|{a.Hashes.sha1Hash}|{a.Hashes.sha256Hash}|{a.Filename}")
            )
            .Select(charGroup => charGroup.Chunk(StaticDetails.MaximumLineNumberInIndexFiles));

        int fileNaming = 0;
        foreach (var chGroup in charGroups)
        {
            foreach (var chunk in chGroup)
            {
                var directory = FileHelper.CreateSubdirectoryInIndexDirectoryWithChar(chunk.First()[0]);
                var filePath = Path.Combine(directory.ToString(), $"{fileNaming}.txt");

                File.AppendAllLines(filePath, chunk);

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
