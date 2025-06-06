using MonsterSlayerAscii;

public class MonsterSlayer
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Welcome to Monster Slayer!");
        Player player = new Player(100);
        Monster monster = new Monster(100);
        new AsciiUI(player, monster).Run();
        Console.WriteLine("Thanks for playing!");
    }
}
