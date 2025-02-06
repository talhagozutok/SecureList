using SecureList.Core.Entities;
using SecureList.Core.Helpers;

namespace SecureList.Core;

public class IndexService(PasswordRepository passwordRepository)
{
    private readonly PasswordRepository _passwordRepository = passwordRepository;

    public void Index()
    {
        var charGroups = _passwordRepository.Passwords
            .Where(kvp => !string.IsNullOrEmpty(kvp.Key))
            .GroupBy(kvp => kvp.Key[0].ToString(), StringComparer.Ordinal)
            .SelectMany(grp =>
                grp.Select(kvp => new {
                    Password = kvp.Key,
                    FileName = kvp.Value,
                    Hashes = CryptographyHelper.CalculateHashes(kvp.Key)})
                .Select(a => $"{a.Password}|{a.Hashes.MD5Hash}|{a.Hashes.SHA1Hash}|{a.Hashes.SHA256Hash}|{a.FileName}")
                .Chunk(StaticDetails.MaximumLineNumberInIndexFiles)
                .Select(chunk => new { CharGroup = grp.Key, Chunk = chunk }));

        int fileNaming = 0;
        foreach (var item in charGroups)
        {
            var directory = FileHelper.CreateSubdirectoryInIndexDirectoryWithChar(item.CharGroup[0]);
            var filePath = Path.Combine(directory.ToString(), $"{fileNaming}.txt");

            File.WriteAllLines(filePath, item.Chunk);

            fileNaming++;
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
