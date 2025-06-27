using System;
using System.Linq;

namespace EnemyEditor
{
    public class GameManager
    {
        private readonly CEnemyTemplateList _enemyTemplates;
        private readonly CIconList _iconList;
        private readonly Random _random = new Random();

        public Player Player { get; }
        public ActiveEnemy CurrentEnemy { get; private set; }
        public event Action OnGameStateChanged;

        public GameManager(CEnemyTemplateList templates, CIconList iconList)
        {
            _enemyTemplates = templates;
            _iconList = iconList;
            Player = new Player();
        }

        public void LoadEnemies(string path)
        {
            try
            {
                _enemyTemplates.LoadFromJson(path);
                if (_enemyTemplates.GetEnemyNames().Any())
                {
                    SpawnNewEnemy();
                }
            }
            catch (Exception)
            {
                CurrentEnemy = null;
            }
            GameStateChanged();
        }

        private void SpawnNewEnemy()
        {
            var enemyNames = _enemyTemplates.GetEnemyNames();
            if (!enemyNames.Any()) return;

            var randomIndex = _random.Next(enemyNames.Count);
            var randomTemplate = _enemyTemplates.GetEnemyByIndex(randomIndex);

            CurrentEnemy = new ActiveEnemy(randomTemplate, Player.Level);
        }

        public void AttackEnemy()
        {
            if (CurrentEnemy == null) return;

            bool isDefeated = CurrentEnemy.TakeDamage(Player.Damage);

            if (isDefeated)
            {
                Player.Gold += CurrentEnemy.GoldValue;
                Player.Level++;
                SpawnNewEnemy();
            }
            GameStateChanged();
        }

        public CIcon GetCurrentEnemyIcon()
        {
            if (CurrentEnemy == null) return null;
            return _iconList.FindByName(CurrentEnemy.IconName);
        }

        public void UpgradePlayerDamage()
        {
            Player.UpgradeDamage();
            GameStateChanged();
        }

        private void GameStateChanged()
        {
            OnGameStateChanged?.Invoke();
        }
    }
}