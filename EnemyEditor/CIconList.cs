using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CIconList
    {
        private List<CIcon> icons = new List<CIcon>();
        private int border = 10; // Отступ между иконками
        private int x = 0;       // Начальная позиция X
        private int y = 0;       // Начальная позиция Y
        private int x_sh = 0;    // Смещение по X
        private int y_sh = 0;    // Смещение по Y
        private int imageWidth;
        private int imageHeight;
        private int canvasW;
        private int canvasH;

        public CIconList(int iconWidth, int iconHeight, int canvasWidth, int canvasHeight)
        {
            imageWidth = iconWidth;
            imageHeight = iconHeight;
            canvasW = canvasWidth;
            canvasH = canvasHeight;
        }

        public void Load(string path)
        {
            string folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + path;
            string filter = "*.png";

            if (!Directory.Exists(folder)) return;

            string[] files = Directory.GetFiles(folder, filter);
            foreach (string file in files)
            {
                icons.Add(new CIcon(imageWidth, imageHeight, file));
            }
            ArrangeIcons();
        }

        private void ArrangeIcons()
        {
            int currentX = x + x_sh;
            int currentY = y + y_sh;

            foreach (var icon in icons)
            {
                icon.SetPosition(new Point(currentX, currentY));
                currentX += imageWidth + border;
            }
        }

        public int GetDeltaY() => y_sh;

        public void ScrollX(double delta)
        {
            x_sh += (int)delta;
            // Ограничение прокрутки
            x_sh = Math.Max(0, Math.Min(x_sh, GetMaxXScroll()));
            ArrangeIcons();
        }

        private int GetMaxXScroll()
        {
            int totalWidth = icons.Count * (imageWidth + border) - border;
            return Math.Max(0, totalWidth - canvasW);
        }

        public void Scroll(double delta)
        {
            y_sh += (int)delta;
            // Ограничение прокрутки
            y_sh = y_sh < 0 ? 0 : y_sh;
            ArrangeIcons();
        }

        public List<CIcon> GetIcons() => icons;

        public CIcon FindByName(string name) =>
            icons.Find(icon => icon.Name() == name);

        public CIcon IsMouseOver(Point mousePosition) =>
            icons.Find(icon => icon.IsMouseOver(mousePosition));
    }
}
