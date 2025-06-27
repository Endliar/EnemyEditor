using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EnemyEditor
{
    public partial class MainWindow : Window
    {
        private CIconList iconList;
        private CEnemyTemplateList enemyList = new CEnemyTemplateList();
        private CIcon selectedIconForCreation = null;

        public MainWindow()
        {
            InitializeComponent();

            iconList = new CIconList(64, 64, (int)IconsCanvas.Width, (int)IconsCanvas.Height);
            iconList.Load("/Images");

            RenderIcons();
            UpdateEnemyList();
        }

        private void RenderIcons()
        {
            IconsCanvas.Children.Clear();
            IconsCanvas.Children.Add(SelectionBorder);
            foreach (var icon in iconList.GetIcons())
            {
                IconsCanvas.Children.Add(icon.CloneIcon());
            }
        }

        #region Central Panel Logic

        private void PopulateCentralPanel(CEnemyTemplate enemy)
        {
            if (enemy == null)
            {
                ClearCentralPanel();
                return;
            }

            TxtName.Text = enemy.Name();
            TxtBaseLife.Text = enemy.Baselife().ToString();
            TxtLifeModifier.Text = enemy.LifeModifier().ToString(CultureInfo.InvariantCulture);
            TxtBaseGold.Text = enemy.BaseGold().ToString();
            TxtGoldModifier.Text = enemy.GoldModifier().ToString(CultureInfo.InvariantCulture);
            TxtSpawnChance.Text = enemy.SpawnChance().ToString(CultureInfo.InvariantCulture);

            CIcon icon = iconList.FindByName(enemy.IconName());
            if (icon != null)
            {
                ImgSelectedIcon.Source = (icon.GetIcon().Fill as ImageBrush)?.ImageSource;
                TxtIconName.Text = icon.Name();
            }
            else
            {
                ImgSelectedIcon.Source = null;
                TxtIconName.Text = "Icon not found";
            }
        }

        private void ClearCentralPanel()
        {
            TxtName.Clear();
            TxtBaseLife.Text = "100";
            TxtLifeModifier.Text = "1.0";
            TxtBaseGold.Text = "10";
            TxtGoldModifier.Text = "1.0";
            TxtSpawnChance.Text = "0.5";
            ImgSelectedIcon.Source = null;
            TxtIconName.Text = "No icon selected";
            selectedIconForCreation = null;
            EnemyList.SelectedItem = null;
        }

        private bool ValidateInput(out CEnemyTemplate newEnemy)
        {
            newEnemy = null;

            if (selectedIconForCreation == null && EnemyList.SelectedItem == null)
            {
                MessageBox.Show("Please select an icon from the right panel to create a new enemy.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string iconName = selectedIconForCreation?.Name() ?? (EnemyList.SelectedItem != null ? enemyList.GetEnemyByName(EnemyList.SelectedItem.ToString()).IconName() : null);

            if (string.IsNullOrEmpty(iconName))
            {
                MessageBox.Show("Icon is not selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

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
                MessageBox.Show("Life Modifier must be a positive number greater than 0! Use dot (.) as decimal separator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(TxtBaseGold.Text, out int baseGold) || baseGold < 1)
            {
                MessageBox.Show("Base Gold must be a positive integer (1 or greater)!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(TxtGoldModifier.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double goldModifier) || goldModifier <= 0)
            {
                MessageBox.Show("Gold Modifier must be a positive number greater than 0!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(TxtSpawnChance.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double spawnChance) || spawnChance <= 0 || spawnChance > 1)
            {
                MessageBox.Show("Spawn Chance must be between 0 (exclusive) and 1!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            newEnemy = new CEnemyTemplate(TxtName.Text, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance);
            return true;
        }


        #endregion

        #region Button Clicks

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput(out CEnemyTemplate newEnemy))
            {
                enemyList.AddEnemy(
                    newEnemy.Name(), newEnemy.IconName(), newEnemy.Baselife(),
                    newEnemy.LifeModifier(), newEnemy.BaseGold(),
                    newEnemy.GoldModifier(), newEnemy.SpawnChance()
                );

                UpdateEnemyList();
                EnemyList.SelectedItem = newEnemy.Name();
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (EnemyList.SelectedItem != null)
            {
                string selectedName = EnemyList.SelectedItem.ToString();
                enemyList.DeleteEnemyByName(selectedName);
                UpdateEnemyList();
                ClearCentralPanel();
            }
            else
            {
                MessageBox.Show("Select an enemy to remove.");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Enemies",
                DefaultExt = ".json",
                Filter = "JSON files (.json)|*.json"
            };

            if (dlg.ShowDialog() == true)
            {
                enemyList.SaveToJson(dlg.FileName);
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Enemies",
                DefaultExt = ".json",
                Filter = "JSON files (.json)|*.json"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    enemyList.LoadFromJson(dlg.FileName);
                    UpdateEnemyList();
                    ClearCentralPanel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}");
                }
            }
        }

        #endregion

        #region UI Updates and Events

        private void UpdateEnemyList()
        {
            var selectedItem = EnemyList.SelectedItem;
            EnemyList.ItemsSource = enemyList.GetEnemyNames();
            if (selectedItem != null && enemyList.GetEnemyNames().Contains(selectedItem.ToString()))
            {
                EnemyList.SelectedItem = selectedItem;
            }
        }

        private void EnemyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnemyList.SelectedItem != null)
            {
                string selectedName = EnemyList.SelectedItem.ToString();
                CEnemyTemplate enemy = enemyList.GetEnemyByName(selectedName);
                PopulateCentralPanel(enemy);

                selectedIconForCreation = null;
                SelectionBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void IconsCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(IconsCanvas);
            CIcon clickedIcon = iconList.IsMouseOver(mousePos);

            if (clickedIcon != null)
            {
                EnemyList.SelectedItem = null;
                ClearCentralPanel();

                selectedIconForCreation = clickedIcon;

                if (ImgSelectedIcon.Parent is Border border)
                {
                    (ImgSelectedIcon.Parent as Border).DataContext = clickedIcon;
                }
                ImgSelectedIcon.Source = (clickedIcon.GetIcon().Fill as ImageBrush)?.ImageSource;
                TxtIconName.Text = clickedIcon.Name();

                SelectionBorder.Visibility = Visibility.Visible;
                SelectionBorder.Width = clickedIcon.IconWidth() + 4;
                SelectionBorder.Height = clickedIcon.IconHeight() + 4;
                Canvas.SetLeft(SelectionBorder, clickedIcon.X() - 2);
                Canvas.SetTop(SelectionBorder, clickedIcon.Y() - 2);
            }
        }

        #endregion

        private void BtnPlayGame_Click(object sender, RoutedEventArgs e)
        {
            string enemyFilePath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName),
                "Enemies.json"
            );

            if (!System.IO.File.Exists(enemyFilePath))
            {
                var result = MessageBox.Show(
                    "The 'Enemies.json' file was not found. You should save your enemy list first.\nDo you want to save now?",
                    "File Not Found",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    BtnSave_Click(sender, e);
                    if (!System.IO.File.Exists(enemyFilePath))
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            GameWindow gameWindow = new GameWindow(this.iconList);
            gameWindow.Show();
        }
    }
}