using SecureList.Core;
using SecureList.Core.Entities;

var passwordRepository = PasswordRepository.Instance;
var passwordsDirectory = new PasswordsDirectory();
var passwordFiles = passwordsDirectory.Files;
var indexService = new IndexService(passwordRepository);

foreach (var passwordFile in passwordFiles)
{
    if (!passwordFile.IsProcessed())
    {
        passwordFile.Process();
    }
}

indexService.Index(passwordRepository.Passwords);
indexService.ReadIndex();

string? userInput;
do
{
    Console.WriteLine("Programdan çıkış için [\x1b[31mCTRL + C\x1b[0m]");
    Console.Write("Dosyalarda arama için şifre girin: ");
    userInput = Console.ReadLine();

    if (string.IsNullOrEmpty(userInput))
    {
        Console.Clear();
        continue;
    }

    var isPasswordFound = passwordRepository.Passwords
        .TryGetValue(userInput, out string? fileLocation);

    if (isPasswordFound)
    {
        Console.WriteLine($"Şifre [{userInput}] \"{fileLocation}\" konumunda bulundu.");
    }
    else
    {
        Console.WriteLine("Şifre bulunamadı.");
        Console.Write("Ekleniyor...");

        var fileName = "userDefined.txt";
        var processedDir = StaticDetails.ProcessedDirectory;
        var unprocessedDir = StaticDetails.UnprocessedPasswordsDirectory;
        var processedFile = Path.Join(processedDir, fileName);
        var unprocessedFile = Path.Join(unprocessedDir, fileName);

        if (!Directory.Exists(processedDir))
        {
            Directory.CreateDirectory(processedDir);
        }

        if (!Directory.Exists(unprocessedDir))
        {
            Directory.CreateDirectory(unprocessedDir);
        }

        // Append the new password to the unprocessed passwords file.
        File.AppendAllText(unprocessedFile, $"{userInput}\n");
        // Append the new password to the processed passwords file.
        File.AppendAllText(processedFile, $"{userInput}\n");

        // Add password to repository
        // then reindex the password repository.
        passwordRepository.Passwords.TryAdd(userInput, fileName);
        indexService.Index(passwordRepository.Passwords);

        // Set the output encoding to UTF-8 for proper display of checkmark symbol.
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        // Display checkmark symbol to indicate successful addition of the password.
        Console.Write("\u2714");
    }

    Console.Write($"\nTekrar arama yapmak için bir tuşa basın..."); Console.ReadKey();
    Console.Clear();
} while (true);
