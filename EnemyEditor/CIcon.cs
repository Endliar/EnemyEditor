using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EnemyEditor
{
    public class CIcon
    {
        private string name;
        private int iconWidth;
        private int iconHeight;
        private Point position;
        private Rectangle icon;
        private ImageBrush brush;

        public CIcon(int iconWidth, int iconHeight, string imagePath)
        {
            this.name = System.IO.Path.GetFileNameWithoutExtension(imagePath);
            this.iconWidth = iconWidth;
            this.iconHeight = iconHeight;
            this.position = new Point(0, 0);

            // Создание графического элемента
            icon = new Rectangle
            {
                Stroke = Brushes.Black,
                Width = iconWidth,
                Height = iconHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Загрузка изображения
            try
            {
                brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                    Stretch = Stretch.Uniform
                };
                icon.Fill = brush;
            }
            catch (Exception ex)
            {
                icon.Fill = Brushes.Red;
                Debug.WriteLine($"Error loading image: {ex.Message}");
            }

            UpdatePosition();
        }

        private void UpdatePosition() =>
            icon.RenderTransform = new TranslateTransform(position.X, position.Y);

        public string Name() => name;
        public double X() => position.X;
        public double Y() => position.Y;
        public int IconWidth() => iconWidth;
        public int IconHeight() => iconHeight;
        public Rectangle GetIcon() => icon;

        public void SetPosition(Point newPosition)
        {
            position = newPosition;
            UpdatePosition();
        }

        public bool IsMouseOver(Point mousePosition)
        {
            // Убираем весь scroll-мусор!
            return mousePosition.X >= position.X &&
                   mousePosition.X <= position.X + iconWidth &&
                   mousePosition.Y >= position.Y &&
                   mousePosition.Y <= position.Y + iconHeight;
        }

        public Rectangle CloneIcon() => new Rectangle
        {
            Width = iconWidth,
            Height = iconHeight,
            Fill = icon.Fill,
            Stroke = icon.Stroke,
            RenderTransform = new TranslateTransform(position.X, position.Y)
        };
    }
}
