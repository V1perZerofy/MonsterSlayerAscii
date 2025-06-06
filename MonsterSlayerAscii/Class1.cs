using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterSlayerAscii
{
    internal class Monster
    {
        private static readonly Random random = new Random();
        private int health;
        private int maxHealth;

        public Monster(int pHealth)
        {
            health = pHealth;
            maxHealth = pHealth;
        }

        public int Health { get => health; set => health = value; }
        public int MaxHealth { get => maxHealth; set => maxHealth = value; }
        public int Attack(int x, int y) => random.Next(x, y);
    }

    internal class Player
    {
        private static readonly Random random = new Random();
        private int health;
        private int maxHealth;
        private int extendedAttack = 0;

        public Player(int pHealth)
        {
            health = pHealth;
            maxHealth = pHealth;
        }

        public int Health { get => health; set => health = value; }
        public int MaxHealth { get => maxHealth; set => maxHealth = value; }
        public int ExtendedAttackValue { get => extendedAttack; set => extendedAttack = value; }
        public int Attack(int x, int y) => random.Next(x, y);
        public int Heal(int x, int y) => random.Next(x, y);
    }

    internal class GameLogic
    {
        private Player player;
        private Monster monster;

        public GameLogic(Player player, Monster monster)
        {
            this.player = player;
            this.monster = monster;
        }

        public (int playerDmg, int monsterDmg) HandleAttack()
        {
            int playerDmg = player.Attack(5, 12);
            monster.Health -= playerDmg;
            int monsterDmg = monster.Attack(8, 15);
            player.Health -= monsterDmg;
            if (player.ExtendedAttackValue > 0) player.ExtendedAttackValue--;
            return (playerDmg, monsterDmg);
        }

        public (int playerDmg, int monsterDmg, bool success) HandleExtendedAttack()
        {
            if (player.ExtendedAttackValue == 0)
            {
                player.ExtendedAttackValue = 3;
                int playerDmg = player.Attack(10, 25);
                monster.Health -= playerDmg;
                int monsterDmg = monster.Attack(8, 15);
                player.Health -= monsterDmg;
                return (playerDmg, monsterDmg, true);
            }
            return (0, 0, false);
        }

        public (int healAmount, int monsterDmg, bool success) HandleHeal()
        {
            if (player.Health < player.MaxHealth)
            {
                int heal = player.Heal(8, 20);
                player.Health = Math.Min(player.Health + heal, player.MaxHealth);
                int monsterDmg = monster.Attack(8, 15);
                player.Health -= monsterDmg;
                if (player.ExtendedAttackValue > 0) player.ExtendedAttackValue--;
                return (heal, monsterDmg, true);
            }
            return (0, 0, false);
        }

        public bool IsVictory() => monster.Health <= 0;
        public bool IsDefeat() => player.Health <= 0;
    }

}
