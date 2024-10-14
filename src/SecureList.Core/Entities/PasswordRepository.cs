namespace SecureList.Core.Entities;

public sealed class PasswordRepository
{
    private static readonly Lazy<PasswordRepository> instance
        = new(() => new PasswordRepository());

    private PasswordRepository()
    {
        Passwords = new();
    }

    public static PasswordRepository Instance => instance.Value;

    // TKey: password
    // TValue: file location
    public Dictionary<string, string> Passwords { get; private set; }

    public bool TryAddPassword(string password, string filePath)
        => Passwords.TryAdd(password, filePath);
}
