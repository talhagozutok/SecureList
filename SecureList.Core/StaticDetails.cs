namespace SecureList.Core;

public static class StaticDetails
{
    public static readonly string UnprocessedPasswordsDirectory =
        Path.Combine(_projectDirectory, "Unprocessed-Passwords");

    public static readonly string ProcessedDirectory =
        Path.Combine(_projectDirectory, "Processed");

    public static readonly string IndexDirectory =
        Path.Combine(_projectDirectory, "Index");

    public static readonly int MaximumLineNumberInIndexFiles = 10_000;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
    private static string _projectDirectory
        => Directory.GetParent(Environment.CurrentDirectory) // ...\Code\SecureList.Console\bin\Debug\
        .Parent // ...\Code\SecureList.Console\bin\
        .Parent // ...\Code\SecureList.Console\
        .Parent // ...\Code\
        .Parent // ...\
        .FullName;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}
