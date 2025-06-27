using System;
using System.Numerics;

namespace EnemyEditor
{
    public class ActiveEnemy
    {
        public string Name { get; }
        public string IconName { get; }
        public LargeNumber MaxHealth { get; }
        public LargeNumber CurrentHealth { get; private set; }
        public LargeNumber GoldValue { get; }

        public ActiveEnemy(CEnemyTemplate template, int playerLevel)
        {
            Name = template.Name();
            IconName = template.IconName();

            var healthMultiplier = Math.Pow(template.LifeModifier(), playerLevel - 1);
            BigInteger calculatedHealth = (BigInteger)(template.Baselife() * healthMultiplier);
            MaxHealth = new LargeNumber(calculatedHealth);
            CurrentHealth = new LargeNumber(calculatedHealth);

            // Золото = Базовое * (МодификаторЗолота ^ (УровеньИгрока - 1))
            var goldMultiplier = Math.Pow(template.GoldModifier(), playerLevel - 1);
            BigInteger calculatedGold = (BigInteger)(template.BaseGold() * goldMultiplier);
            GoldValue = new LargeNumber(calculatedGold);
        }

        /// <summary>
        /// Наносит урон противнику.
        /// </summary>
        /// <param name="damage">Количество урона.</param>
        /// <returns>True, если противник побежден.</returns>
        public bool TakeDamage(LargeNumber damage)
        {
            CurrentHealth -= damage;
            if ((BigInteger)CurrentHealth < 0)
            {
                CurrentHealth = new LargeNumber(0);
            }
            return (BigInteger)CurrentHealth <= 0;
        }
    }
}