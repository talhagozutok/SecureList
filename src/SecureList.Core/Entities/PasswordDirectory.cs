namespace SecureList.Core.Entities;

public class PasswordsDirectory : FileSystemInfo
{
    public override bool Exists => Directory.Exists(Name);
    public override string Name => StaticDetails.UnprocessedPasswordsDirectory;
    public new string FullPath => Name;
    public override void Delete() => Directory.Delete(Name, recursive: true);

    public IEnumerable<PasswordFile> Files
    {
        get
        {
            if (!Exists)
            {
                Directory.CreateDirectory(Name);
            }

            return Directory.GetFiles(Name)
                .Select(file => Path.GetFileName(file))
                .Select(fileName => new PasswordFile(fileName));
        }
    }
}
