using System.Security.Cryptography.X509Certificates;
using System.Transactions;
namespace Stage0;

partial class Program
{
     static void Main(string[] args)
    {
        Wwlcome7621();
        Welcom1590();
        Console.ReadKey();
    }
    static partial void Welcom1590();
    private static void Wwlcome7621()
    {
        Console.WriteLine("Enter your name: ");
        string userName = Console.ReadLine();
        Console.WriteLine($"{userName}, welcome to my first console application");
    }
}