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
        Console.WriteLine($"Şifre bulunamadı.");
    }

    Console.Write($"\nTekrar arama yapmak için bir tuşa basın..."); Console.ReadKey();
    Console.Clear();
} while (true);
