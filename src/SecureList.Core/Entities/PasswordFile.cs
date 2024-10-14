using SecureList.Core.Helpers;
using System.Diagnostics;

namespace SecureList.Core.Entities;

public class PasswordFile(string fileName) : FileSystemInfo
{
    private readonly PasswordRepository _passwordRepository = PasswordRepository.Instance;
    private bool _isProcessed = false;

    public override string Name => fileName;
    public new string FullPath => Path.Join(StaticDetails.UnprocessedPasswordsDirectory, Name);
    public override bool Exists => File.Exists(FullPath);
    public override void Delete() => File.Delete(FullPath);

    public void Process()
    {
        if (_isProcessed) return;

        Debug.WriteLine($"Info: Processing file {FullPath}");
        var passwords = FileHelper.ReadLinesFromFile(FullPath);
        foreach (var pw in passwords)
        {
            _passwordRepository.TryAddPassword(pw, Name);
        }

        Debug.WriteLine($"Success: Processed file {FullPath}");
        _isProcessed = true;
        FileHelper.CopyProcessedFile(this);

        return;
    }

    public bool IsProcessed()
    {
        // Compare the contents of the file in Unprocessed-Passwords with the file in Processed.
        // if contents are not equal return false.
        var processedPasswordFilePath = Path.Combine(StaticDetails.ProcessedDirectory, fileName);
        if (!FileHelper.IsFileContentsEqual(FullPath, processedPasswordFilePath))
        {
            return false;
        }

        _isProcessed = true;

        return true;
    }
}
