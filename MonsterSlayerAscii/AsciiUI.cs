using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterSlayerAscii
{
    internal class AsciiUI
    {
        private Player player;
        private Monster monster;
        private GameLogic logic;
        private string currentInfo = "Welcome to Monster Slayer!";

        public AsciiUI(Player player, Monster monster)
        {
            this.player = player;
            this.monster = monster;
            this.logic = new GameLogic(player, monster);
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                DisplayLogo();
                DisplayCurrentInfo();
                DisplayHealthBar("Player", player.Health, player.MaxHealth, 105, "\u001b[38;5;46m");
                DisplayHealthBar("Monster", monster.Health, monster.MaxHealth, 105, "\u001b[38;5;196m");
                Console.WriteLine("\n\n");
                BattleMenu();

                char input = Console.ReadKey().KeyChar;
                switch (char.ToLower(input))
                {
                    case 'a':
                        var (pDmgA, mDmgA) = logic.HandleAttack();
                        UpdateInfo("attack", pDmgA, mDmgA);
                        break;
                    case 'e':
                        var (pDmgE, mDmgE, successE) = logic.HandleExtendedAttack();
                        UpdateInfo(successE ? "extended" : "cooldown", pDmgE, mDmgE, cooldown: player.ExtendedAttackValue);
                        break;
                    case 'h':
                        var (heal, mDmgH, successH) = logic.HandleHeal();
                        UpdateInfo(successH ? "heal" : "cantheal", healAmount: heal, monsterDmg: mDmgH);
                        break;
                    case 's':
                        UpdateInfo("surrender");
                        Console.Clear();
                        DisplayLogo();
                        DisplayCurrentInfo();
                        DisplayHealthBar("Player", player.Health, player.MaxHealth, 105, "\u001b[38;5;46m");
                        DisplayHealthBar("Monster", monster.Health, monster.MaxHealth, 105, "\u001b[38;5;196m");
                        Console.WriteLine("\n\n");
                        BattleMenu();
                        return; // Exit the game after surrendering
                    default:
                        UpdateInfo("invalid");
                        continue;
                }

                if (logic.IsVictory() || logic.IsDefeat())
                {
                    UpdateInfo(logic.IsVictory() ? "victory" : "defeat");
                    Console.Clear();
                    DisplayLogo();
                    DisplayCurrentInfo();
                    DisplayHealthBar("Player", player.Health, player.MaxHealth, 105, "\u001b[38;5;46m");
                    DisplayHealthBar("Monster", monster.Health, monster.MaxHealth, 105, "\u001b[38;5;196m");
                    Console.WriteLine("\n\n");
                    BattleMenu();
                    return;
                }
            }
        }

        private void DisplayLogo()
        {
            string logo = @"
               _____                          __                   _________.__                             
              /     \   ____   ____   _______/  |_  ___________   /   _____/|  | _____  ___.__. ___________ 
             /  \ /  \ /  _ \ /    \ /  ___/\   __\/ __ \_  __ \  \_____  \ |  | \__  \<   |  |/ __ \_  __ \
            /    Y    (  <_> )   |  \\___ \  |  | \  ___/|  | \/  /        \|  |__/ __ \\___  \  ___/|  | \/
            \____|__  /\____/|___|  /____  > |__|  \___  >__|    /_______  /|____(____  / ____|\___  >__|   
                    \/            \/     \/            \/                \/           \/\/         \/       
            ";
            foreach (var line in logo.Split('\n'))
            {
                int padding = (Console.WindowWidth - line.Length) / 2;
                Console.WriteLine("\u001b[38;5;46m" + new string(' ', Math.Max(0, padding)) + line.TrimEnd('\r') + "\u001b[0m");
            }
        }

        private void DisplayHealthBar(string label, int current, int max, int width, string color)
        {
            if (current < 0) current = 0;
            double ratio = (double)current / max;
            int filled = (int)(ratio * width);
            string bar = new string('█', filled).PadRight(width);
            string barLine = $"|{bar}|";
            string topLabel = $" {label} ".PadLeft((width / 2) + (label.Length / 2) + 2, '=').PadRight(width + 2, '=');

            int leftPadding = (Console.WindowWidth - (width + 2)) / 2;
            string pad = new string(' ', Math.Max(0, leftPadding));

            Console.WriteLine(pad + topLabel);
            Console.WriteLine(pad + color + barLine + "\u001b[0m");
        }

        private void DisplayCurrentInfo()
        {
            Console.WriteLine(currentInfo.PadLeft((Console.WindowWidth / 2) + (currentInfo.Length / 2)) + "\n");
        }

        private void BattleMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string[,] options = {
            { "[A]TTACK", "[E]XTENDED ATTACK" },
            { "[H]EAL", "[S]URRENDER" }
        };
            ConsoleColor bg = ConsoleColor.Magenta;
            ConsoleColor fg = ConsoleColor.Yellow;

            int boxWidth = 25, boxHeight = 3, margin = 4;
            int startX = (Console.WindowWidth - (2 * boxWidth + margin)) / 2;
            int startY = Console.CursorTop;

            for (int row = 0; row < 2; row++)
                for (int col = 0; col < 2; col++)
                    DrawBox(startX + col * (boxWidth + margin), startY + row * (boxHeight + 1), boxWidth, boxHeight, options[row, col], fg, bg);
        }

        private void DrawBox(int x, int y, int width, int height, string text, ConsoleColor fg, ConsoleColor bg)
        {
            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.BackgroundColor = bg;
                Console.ForegroundColor = fg;

                if (i == height / 2)
                {
                    int pad = (width - text.Length) / 2;
                    Console.Write(new string(' ', pad) + text + new string(' ', width - pad - text.Length));
                }
                else
                    Console.Write(new string(' ', width));
            }
            Console.ResetColor();
        }

        private void UpdateInfo(string action, int playerDmg = 0, int monsterDmg = 0, int healAmount = 0, int cooldown = 0)
        {
            currentInfo = action switch
            {
                "attack" => $"You hit the monster for {playerDmg} damage. The monster hits you for {monsterDmg} damage.",
                "extended" => $"You unleash an extended attack for {playerDmg} damage! The monster counters with {monsterDmg} damage.",
                "cooldown" => $"Extended Attack is on cooldown: {cooldown} turn(s) left.",
                "heal" => $"You healed for {healAmount} HP. The monster hits you for {monsterDmg} damage.",
                "cantheal" => "You cannot heal right now.",
                "surrender" => "You gave up. The monster cackles in victory.",
                "invalid" => "Invalid choice. Please try again.",
                "victory" => "Congratulations! You have defeated the monster!",
                "defeat" => "You have been defeated by the monster. Game over.",
                _ => currentInfo
            };
        }
    }

}
