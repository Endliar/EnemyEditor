using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EnemyEditor
{
    /// <summary>
    /// Логика взаимодействия для AddEnemyDialog.xaml
    /// </summary>
    public partial class AddEnemyDialog : Window
    {
        public string EnemyName { get; private set; }
        public string IconName { get; private set; }
        public int BaseLife { get; private set; }
        public double LifeModifier { get; private set; }
        public int BaseGold { get; private set; }
        public double GoldModifier { get; private set; }
        public double SpawnChance { get; private set; }

        public AddEnemyDialog(string selectedIconName)
        {
            InitializeComponent();
            TxtIcon.Text = selectedIconName;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                DialogResult = true;
                Close();
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Name is required!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(TxtBaseLife.Text, out int baseLife) || baseLife < 1)
            {
                MessageBox.Show("Base Life must be a positive integer (1 or greater)!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(TxtLifeModifier.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double lifeModifier) || lifeModifier <= 0)
            {
                MessageBox.Show("Life Modifier must be a positive number greater than 0!\nUse dot (.) as decimal separator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(TxtBaseGold.Text, out int baseGold) || baseGold < 1)
            {
                MessageBox.Show("Base Gold must be a positive integer (1 or greater)!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(TxtGoldModifier.Text, out double goldModifier) || goldModifier <= 0)
            {
                MessageBox.Show("Gold Modifier must be a positive number greater than 0!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(TxtSpawnChance.Text, out double spawnChance) ||
                spawnChance <= 0 || spawnChance > 1)
            {
                MessageBox.Show("Spawn Chance must be between 0 (exclusive) and 1!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            EnemyName = TxtName.Text;
            IconName = TxtIcon.Text;
            BaseLife = baseLife;
            LifeModifier = lifeModifier;
            BaseGold = baseGold;
            GoldModifier = goldModifier;
            SpawnChance = spawnChance;

            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
