using Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorControls
{
    [ExcludeFromCodeCoverage]
    public partial class SearchIterator : UserControl
    {
        public SearchIterator()
        {
            InitializeComponent();
        }


        private void SearchIterator_Load(object sender, EventArgs e)
        {
            Dock = DockStyle.Fill;
        }



        private void UpButton_Click(object sender, EventArgs e)
        {
            if (maximum == 0)
                return;

            if (Index == 1)
                Index = maximum;
            else
                Index--;

            IndexChanged?.Invoke(this, Index);
            drawPanel.Invalidate();
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            if (maximum == 0)
                return;

            if (Index == maximum)
                Index = 1;
            else
                Index++;

            IndexChanged?.Invoke(this, Index);
            drawPanel.Invalidate();
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            if (maximum == 0)
                return;

            var font = new Font(FontFamily.GenericSansSerif, 7);
            g.DrawString($"{Index,5}", font, Brushes.Black, 0, 0);
            g.DrawLine(
                new Pen(Color.Black, 0.15f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot},
                4, 11, drawPanel.Size.Width - 4, 11);
            g.DrawString($"{maximum,5}", font, Brushes.Black, 0, 10);
        }

        public int Maximum 
        {
            get => maximum;
            set
            {
                Index = 0;
                maximum = value;
                drawPanel.Invalidate();
            }
        }

        /// <summary>
        /// Индекс найденного элемента
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// Количество найденных элементов, если 0, то их нет
        /// </summary>
        private int maximum = 0;

        public delegate void OnIndexChanged(object sender, int index);
        public event OnIndexChanged IndexChanged;



        private void UpButton_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            var width = upButton.Size.Width;
            var height = upButton.Size.Height;

            g.FillPolygon(new SolidBrush(Color.Black),
                new PointF[] 
                {
                    new PointF(width / 2.0f, height / 3.0f - 1f),
                    new PointF(width - (width / 4.0f), height - (height / 3.0f) - 1f),
                    new PointF(width / 4.0f, height - (height / 3.0f) - 1f),
                });
        }

        private void DownButton_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            // берем размер с одной кнопки, чтоб стрелки были одинаковыми
            var width = upButton.Size.Width;
            var height = upButton.Size.Height;

            g.FillPolygon(new SolidBrush(Color.Black),
                new PointF[]
                {
                    new PointF(width / 4.0f, height / 3.0f),
                    new PointF(width - (width / 4.0f), height / 3.0f),
                    new PointF(width / 2.0f, height - (height / 3.0f)),
                });
        }
    }
}
