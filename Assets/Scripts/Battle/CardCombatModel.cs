using System;
using System.Collections.Generic;
using System.Linq;

namespace FailingQuest.Cards
{
    public class CardEnemy
    {
        public string Name;
        public int Health, MaxHealth, Block, Vulnerable, Pattern, Power;
        public bool Alive => Health > 0;
        public bool Guards => Pattern % 3 == 1;
        public int IntentDamage => Guards ? 0 : Power + (Pattern % 3 == 2 ? 3 : 0);
        public string Intent => Guards ? "방어 8 · 힘 +1" : $"공격 {IntentDamage}";
    }

    // Pure C# rules; UI and scene transitions live in the prefab controller.
    public class CardCombatModel
    {
        public readonly List<Card> DrawPile = new(), Hand = new(), Discard = new(), Exhaust = new();
        public readonly List<CardEnemy> Enemies = new();
        public const int MaxMana = 100;
        public const int DefaultStartingMana = MaxMana;
        public int Health, MaxHealth, Block, Strength, Turn;
        private int mana;
        public int Mana { get => mana; set => mana = Math.Clamp(value, 0, MaxMana); }
        public bool PlayerTurn { get; private set; }
        public bool Victory => Enemies.All(e => !e.Alive);
        public bool Finished => Health <= 0 || Victory;
        private readonly Random random;
        public string Message = "";

        public CardCombatModel(IEnumerable<Card> deck, int health, int maxHealth, int seed, int startingMana = DefaultStartingMana)
        {
            random = new Random(seed);
            Health = health; MaxHealth = maxHealth;
            Mana = startingMana;
            DrawPile.AddRange(deck.Select(c => c.Copy()));
            Shuffle();
        }

        private void Shuffle()
        {
            for (int i = DrawPile.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (DrawPile[i], DrawPile[j]) = (DrawPile[j], DrawPile[i]);
            }
        }

        public void BeginTurn()
        {
            if (Finished) return;
            Turn++; Block = 0; PlayerTurn = true;
            Draw(5);
        }

        public void Draw(int count)
        {
            for (int i = 0; i < count && Hand.Count < 10; i++)
            {
                if (DrawPile.Count == 0) { DrawPile.AddRange(Discard); Discard.Clear(); Shuffle(); }
                if (DrawPile.Count == 0) break;
                Hand.Add(DrawPile[DrawPile.Count - 1]);
                DrawPile.RemoveAt(DrawPile.Count - 1);
            }
        }

        public bool Play(int index, int target)
        {
            if (!PlayerTurn || Finished || index < 0 || index >= Hand.Count) return false;
            var card = Hand[index];
            if (card.Cost > Mana) return false;
            if (card.RequiresTarget && (target < 0 || target >= Enemies.Count || !Enemies[target].Alive)) return false;
            Mana -= card.Cost;
            Hand.RemoveAt(index);
            foreach (var phrase in card.GetPhrases())
            {
                switch (phrase.Effect)
                {
                    case PhraseEffect.Attack:
                    case PhraseEffect.Vulnerable:
                    case PhraseEffect.Weaken:
                        foreach (var enemy in phrase.AllEnemies ? Enemies.Where(e => e.Alive) : new[] { Enemies[target] }.AsEnumerable())
                        {
                            if (phrase.Effect == PhraseEffect.Vulnerable) { enemy.Vulnerable += phrase.Amount; continue; }
                            if (phrase.Effect == PhraseEffect.Weaken) { enemy.Power = Math.Max(0, enemy.Power - phrase.Amount); continue; }
                            int damage = Math.Max(0, (int)((phrase.Amount + Strength) * (enemy.Vulnerable > 0 ? 1.5f : 1f)));
                            enemy.Health = Math.Max(0, enemy.Health - Math.Max(0, damage - enemy.Block));
                            enemy.Block = Math.Max(0, enemy.Block - damage);
                        }
                        break;
                    case PhraseEffect.Block: Block += phrase.Amount; break;
                    case PhraseEffect.Heal: Health = Math.Min(MaxHealth, Health + phrase.Amount); break;
                    case PhraseEffect.SelfDamage: Health = Math.Max(0, Health - phrase.Amount); break;
                    case PhraseEffect.Draw: Draw(phrase.Amount); break;
                    case PhraseEffect.Strength: Strength += phrase.Amount; break;
                    case PhraseEffect.Mana: Mana += phrase.Amount; break;
                }
            }
            (card.Exhausts ? Exhaust : Discard).Add(card);
            Message = card.Name + " 사용";
            return true;
        }

        public void EndTurn()
        {
            if (!PlayerTurn || Finished) return;
            PlayerTurn = false;
            Discard.AddRange(Hand); Hand.Clear();
        }

        public void EnemyAction(int index)
        {
            if (PlayerTurn || Finished) return;
            var enemy = Enemies[index];
            if (!enemy.Alive) return;
            enemy.Block = 0;
            if (enemy.Guards) { enemy.Block = 8; enemy.Power++; Message = enemy.Name + " · 방어 8, 힘 +1"; }
            else
            {
                int damage = Math.Max(0, enemy.IntentDamage - Block);
                Block = Math.Max(0, Block - enemy.IntentDamage);
                Health = Math.Max(0, Health - damage);
                Message = $"{enemy.Name} · {damage} 피해";
            }
            enemy.Vulnerable = Math.Max(0, enemy.Vulnerable - 1);
            enemy.Pattern++;
        }
    }
}
