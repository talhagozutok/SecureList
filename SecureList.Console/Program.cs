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
        var processedFile = Path.Join(StaticDetails.ProcessedDirectory, fileName);
        var unprocessedFile = Path.Join(StaticDetails.UnprocessedPasswordsDirectory, fileName);

        // append new password to `Unprocessed-Password` and `Processed` directories
        // then add it to passwordRepository.Passwords
        File.AppendAllText(unprocessedFile, $"{userInput}\n");
        File.AppendAllText(processedFile, $"{userInput}\n");
        passwordRepository.Passwords.TryAdd(userInput, fileName);

        // reindex
        indexService.Index(passwordRepository.Passwords);

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Write("\u2714");
    }

    Console.Write($"\nTekrar arama yapmak için bir tuşa basın..."); Console.ReadKey();
    Console.Clear();
} while (true);
