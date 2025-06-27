using System.Numerics;

namespace EnemyEditor
{
    public class Player
    {
        public int Level { get; set; }
        public LargeNumber Gold { get; set; }
        public LargeNumber Damage { get; set; }
        public LargeNumber UpgradeCost { get; set; }

        public Player()
        {
            Level = 1;
            Gold = new LargeNumber(0);
            Damage = new LargeNumber(10);
            UpgradeCost = new LargeNumber(50);
        }

        public bool CanAffordUpgrade()
        {
            return Gold >= UpgradeCost;
        }

        public void UpgradeDamage()
        {
            if (CanAffordUpgrade())
            {
                Gold -= UpgradeCost;

                BigInteger damageIncrease = (BigInteger)((double)((BigInteger)Damage) * 0.2);
                Damage += new LargeNumber(damageIncrease + 5);

                BigInteger costIncrease = (BigInteger)((double)((BigInteger)UpgradeCost) * 0.15);
                UpgradeCost += new LargeNumber(costIncrease + 20);
            }
        }
    }
}