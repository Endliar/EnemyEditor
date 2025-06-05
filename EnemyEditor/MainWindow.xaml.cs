using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnemyEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CIconList iconList;
        private CEnemyTemplateList enemyList = new CEnemyTemplateList();
        private CIcon selectedIcon = null;

        public MainWindow()
        {
            InitializeComponent();

            iconList = new CIconList(64, 64, 600, 400);
            iconList.Load("/Images");

            RenderIcons();
        }

        private void RenderIcons()
        {
            IconsCanvas.Children.Clear();
            foreach (var icon in iconList.GetIcons())
            {
                IconsCanvas.Children.Add(icon.CloneIcon());
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIcon == null)
            {
                MessageBox.Show("Please select an icon first!");
                return;
            }

            var dialog = new AddEnemyDialog(selectedIcon.Name());
            if (dialog.ShowDialog() == true)
            {
                enemyList.AddEnemy(
                    dialog.EnemyName,
                    dialog.IconName,
                    dialog.BaseLife,
                    dialog.LifeModifier,
                    dialog.BaseGold,
                    dialog.GoldModifier,
                    dialog.SpawnChance
                );

                // Обновление списка
                UpdateEnemyList();
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (EnemyList.SelectedItem != null)
            {
                string selectedName = EnemyList.SelectedItem.ToString();
                enemyList.DeleteEnemyByName(selectedName);
                UpdateEnemyList();
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}");
                }
            }
        }

        private void UpdateEnemyList()
        {
            EnemyList.ItemsSource = null;
            EnemyList.ItemsSource = enemyList.GetEnemyNames();
            EnemyList.Items.Refresh();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            UpdateEnemyList(); // Инициализация списка при запуске
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(IconsCanvas);
            selectedIcon = iconList.IsMouseOver(mousePos);

            if (selectedIcon != null)
            {
                // Обновление UI для показа выбранной иконки
                SelectionBorder.Visibility = Visibility.Visible;
                SelectionBorder.Width = selectedIcon.IconWidth() + 10; // +10 для рамки
                SelectionBorder.Height = selectedIcon.IconHeight() + 10;
                Canvas.SetLeft(SelectionBorder, selectedIcon.X() - 5); // -5 для центрирования рамки
                Canvas.SetTop(SelectionBorder, selectedIcon.Y() - 5);
                SelectionBorder.BorderBrush = Brushes.Yellow;
                SelectionBorder.BorderThickness = new Thickness(3);
            }
            else
            {
                SelectionBorder.Visibility = Visibility.Collapsed;
            }
        }
    }
}