using System.IO;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EnemyEditor
{
    public partial class GameWindow : Window
    {
        private readonly GameManager _gameManager;
        private readonly string _enemyFilePath;

        public GameWindow(CIconList iconList)
        {
            InitializeComponent();
            _gameManager = new GameManager(new CEnemyTemplateList(), iconList);
            _gameManager.OnGameStateChanged += UpdateUI;

            _enemyFilePath = Path.Combine(
                Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                "Enemies.json"
            );

            Loaded += GameWindow_Loaded;
        }

        private void GameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(_enemyFilePath))
            {
                _gameManager.LoadEnemies(_enemyFilePath);
            }
            else
            {
                MessageBox.Show(
                    $"Enemy file not found at:\n{_enemyFilePath}\n\nPlease save an enemy list from the editor first.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                this.Close();
            }
        }

        private void UpdateUI()
        {
            TxtPlayerLevel.Text = $"Level: {_gameManager.Player.Level}";
            TxtPlayerGold.Text = $"Gold: {_gameManager.Player.Gold}";
            TxtPlayerDamage.Text = $"Damage: {_gameManager.Player.Damage}";

            BtnUpgradeDamage.IsEnabled = _gameManager.Player.CanAffordUpgrade();
            TxtUpgradeCost.Text = $"Upgrade Damage (Cost: {_gameManager.Player.UpgradeCost})";

            var enemy = _gameManager.CurrentEnemy;
            if (enemy != null)
            {
                TxtEnemyName.Text = enemy.Name;
                TxtEnemyHealth.Text = $"{enemy.CurrentHealth.ToFullString()} / {enemy.MaxHealth.ToFullString()}";

                EnemyHealthBar.Maximum = (double)(BigInteger)enemy.MaxHealth;
                EnemyHealthBar.Value = (double)(BigInteger)enemy.CurrentHealth;

                var icon = _gameManager.GetCurrentEnemyIcon();
                if (icon != null && icon.GetIcon().Fill is ImageBrush brush)
                {
                    ImgEnemyIcon.Source = brush.ImageSource;
                }
                else
                {
                    ImgEnemyIcon.Source = null;
                }
                BtnEnemy.IsEnabled = true;
            }
            else
            {
                TxtEnemyName.Text = "No enemies loaded!";
                TxtEnemyHealth.Text = "---";
                EnemyHealthBar.Value = 0;
                ImgEnemyIcon.Source = null;
                BtnEnemy.IsEnabled = false;
            }
        }

        private void BtnEnemy_Click(object sender, RoutedEventArgs e)
        {
            _gameManager.AttackEnemy();
        }

        private void BtnUpgradeDamage_Click(object sender, RoutedEventArgs e)
        {
            _gameManager.UpgradePlayerDamage();
        }
    }
}