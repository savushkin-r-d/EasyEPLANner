using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace GenerateDevicesTreeIcons
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var outDir = args.Length > 0
                ? args[0]
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "src", "Resources");
            outDir = Path.GetFullPath(outDir);

            var icons = new[]
            {
                "devicesTreeObject",
                "devicesTreeType",
                "devicesTreeDevice",
                "devicesTreeData",
                "devicesTreeParameters",
                "devicesTreeRuntimeParameters",
                "devicesTreeProperties",
                "devicesTreeChannels",
            };

            foreach (var name in icons)
            {
                using (var bmp = TreeIconDrawer.Draw(name))
                {
                    var path = Path.Combine(outDir, name + ".png");
                    bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine(path);
                }
            }
        }
    }

    internal static class TreeIconDrawer
    {
        private static readonly Color ObjectFill = Color.FromArgb(184, 228, 240);
        private static readonly Color TypeFill = Color.FromArgb(200, 230, 201);
        private static readonly Color DarkGray = Color.FromArgb(97, 97, 97);
        private static readonly Color Stroke = Color.FromArgb(30, 58, 95);
        private static readonly Color FillLight = Color.FromArgb(232, 238, 244);
        private static readonly Color Blue = Color.FromArgb(74, 144, 217);
        private static readonly Color Metal = Color.FromArgb(144, 164, 174);
        private static readonly Color Panel = Color.FromArgb(84, 110, 122);

        public static Bitmap Draw(string name)
        {
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                g.Clear(Color.Transparent);
                switch (name)
                {
                    case "devicesTreeObject": DrawLocationIcon(g, ObjectFill, true); break;
                    case "devicesTreeType": return CreateTypeFromObjectTemplate();
                    case "devicesTreeDevice": DrawDevice(g); break;
                    case "devicesTreeData": DrawData(g); break;
                    case "devicesTreeParameters": DrawGear(g, false); break;
                    case "devicesTreeRuntimeParameters": DrawGear(g, true); break;
                    case "devicesTreeProperties": DrawWrench(g); break;
                    case "devicesTreeChannels": DrawChannelsGroup(g); break;
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Иконка объекта (локация): квадрат 12×12, штриховая рамка, «+».
        /// </summary>
        private static void DrawLocationIcon(Graphics g, Color fill, bool plusSymbol)
        {
            g.SmoothingMode = SmoothingMode.None;
            g.FillRectangle(new SolidBrush(fill), 2, 2, 12, 12);

            using (var dashedPen = new Pen(Color.Black, 1f)
            {
                DashStyle = DashStyle.Dash,
                DashPattern = new[] { 2f, 1.5f },
            })
            {
                g.DrawRectangle(dashedPen, 2, 2, 11, 11);
            }

            if (plusSymbol)
            {
                g.FillRectangle(Brushes.Black, 7, 5, 2, 6);
                g.FillRectangle(Brushes.Black, 5, 7, 6, 2);
            }
        }

        /// <summary>
        /// Тип = та же картинка, что объект; только светло-зелёный фон и «T» вместо «+».
        /// </summary>
        private static Bitmap CreateTypeFromObjectTemplate()
        {
            using (var objectBitmap = new Bitmap(16, 16))
            {
                using (var g = Graphics.FromImage(objectBitmap))
                {
                    g.Clear(Color.Transparent);
                    DrawLocationIcon(g, ObjectFill, true);
                }

                var typeBitmap = new Bitmap(16, 16);
                for (var y = 0; y < 16; y++)
                {
                    for (var x = 0; x < 16; x++)
                    {
                        var pixel = objectBitmap.GetPixel(x, y);
                        if (IsObjectFillPixel(pixel) || IsPlusPixel(x, y))
                            typeBitmap.SetPixel(x, y, TypeFill);
                        else
                            typeBitmap.SetPixel(x, y, pixel);
                    }
                }

                using (var g = Graphics.FromImage(typeBitmap))
                {
                    g.SmoothingMode = SmoothingMode.None;
                    // «T» в той же области, что «+» (5–10 по X, 5–10 по Y).
                    g.FillRectangle(Brushes.Black, 5, 5, 6, 2);
                    g.FillRectangle(Brushes.Black, 7, 5, 2, 6);
                }

                return typeBitmap;
            }
        }

        private static bool IsObjectFillPixel(Color c)
        {
            return c.A > 200 && c.R > 160 && c.G > 200 && c.B > 220;
        }

        private static bool IsPlusPixel(int x, int y)
        {
            return (x >= 7 && x <= 8 && y >= 5 && y <= 10) ||
                (y >= 7 && y <= 8 && x >= 5 && x <= 10);
        }

        private static void DrawDevice(Graphics g)
        {
            // Пиксельная плата: зелёный PCB + дорожки + пэды.
            g.SmoothingMode = SmoothingMode.None;

            var border = Color.FromArgb(31, 84, 50);
            var pcb = Color.FromArgb(56, 142, 60);
            var pcbLight = Color.FromArgb(102, 187, 106);
            var trace = Color.FromArgb(182, 220, 160);
            var pad = Color.FromArgb(244, 196, 48);
            var hole = Color.FromArgb(56, 56, 56);

            // Контур платы 12x10
            FillStrokeRect(g, 2, 3, 12, 10, pcb);

            // Лёгкий блик сверху
            g.FillRectangle(new SolidBrush(pcbLight), 3, 4, 10, 2);

            // Пэды
            g.FillRectangle(new SolidBrush(pad), 3, 4, 2, 2);
            g.FillRectangle(new SolidBrush(pad), 11, 4, 2, 2);
            g.FillRectangle(new SolidBrush(pad), 3, 10, 2, 2);
            g.FillRectangle(new SolidBrush(pad), 11, 10, 2, 2);
            g.FillRectangle(new SolidBrush(pad), 7, 7, 2, 2);

            // Отверстия в пэдах
            g.FillRectangle(new SolidBrush(hole), 4, 5, 1, 1);
            g.FillRectangle(new SolidBrush(hole), 12, 5, 1, 1);
            g.FillRectangle(new SolidBrush(hole), 4, 11, 1, 1);
            g.FillRectangle(new SolidBrush(hole), 12, 11, 1, 1);
            g.FillRectangle(new SolidBrush(hole), 8, 8, 1, 1);

            // Дорожки (пиксельные проводки)
            g.FillRectangle(new SolidBrush(trace), 5, 5, 6, 1);  // верхняя
            g.FillRectangle(new SolidBrush(trace), 4, 8, 4, 1);  // средняя левая
            g.FillRectangle(new SolidBrush(trace), 8, 9, 4, 1);  // средняя правая
            g.FillRectangle(new SolidBrush(trace), 7, 6, 1, 4);  // вертикаль в центр
            g.FillRectangle(new SolidBrush(trace), 10, 6, 1, 4); // вертикаль справа

            // Небольшие выводы/контакты сбоку
            g.FillRectangle(new SolidBrush(border), 1, 5, 1, 1);
            g.FillRectangle(new SolidBrush(border), 1, 9, 1, 1);
            g.FillRectangle(new SolidBrush(border), 14, 6, 1, 1);
            g.FillRectangle(new SolidBrush(border), 14, 10, 1, 1);
        }

        private static void DrawData(Graphics g)
        {
            FillStrokeRect(g, 3, 2, 10, 12, Color.White);
            using (var pen = new Pen(Stroke, 0.7f))
            {
                g.DrawLine(pen, 5, 5, 11, 5);
                g.DrawLine(pen, 5, 8, 11, 8);
                g.DrawLine(pen, 5, 11, 9, 11);
            }
            FillStrokeRect(g, 2, 2, 3, 3, Blue);
        }

        private static void DrawGear(Graphics g, bool withPencil)
        {
            DrawGearShape(g, 8, 8);
            if (withPencil)
            {
                DrawPencil(g);
            }
        }

        private static void DrawGearShape(Graphics g, float cx, float cy)
        {
            // Минималистичная шестерня: простое кольцо + 8 прямоугольных зубьев.
            const float rOuter = 4.4f;
            const float rInner = 2.1f;

            g.FillEllipse(new SolidBrush(Metal), cx - rOuter, cy - rOuter, rOuter * 2, rOuter * 2);
            using (var pen = StrokePen(0.8f))
                g.DrawEllipse(pen, cx - rOuter, cy - rOuter, rOuter * 2, rOuter * 2);

            // Зубья (сверху/снизу/слева/справа + диагональные)
            FillStrokeRect(g, cx - 0.9f, cy - 6.1f, 1.8f, 1.6f, Metal);
            FillStrokeRect(g, cx - 0.9f, cy + 4.5f, 1.8f, 1.6f, Metal);
            FillStrokeRect(g, cx - 6.1f, cy - 0.9f, 1.6f, 1.8f, Metal);
            FillStrokeRect(g, cx + 4.5f, cy - 0.9f, 1.6f, 1.8f, Metal);

            FillStrokeRect(g, cx - 4.9f, cy - 4.9f, 1.5f, 1.5f, Metal);
            FillStrokeRect(g, cx + 3.4f, cy - 4.9f, 1.5f, 1.5f, Metal);
            FillStrokeRect(g, cx - 4.9f, cy + 3.4f, 1.5f, 1.5f, Metal);
            FillStrokeRect(g, cx + 3.4f, cy + 3.4f, 1.5f, 1.5f, Metal);

            g.FillEllipse(new SolidBrush(FillLight), cx - rInner, cy - rInner, rInner * 2, rInner * 2);
            using (var pen = StrokePen(0.8f))
                g.DrawEllipse(pen, cx - rInner, cy - rInner, rInner * 2, rInner * 2);
        }

        private static void DrawPencil(Graphics g)
        {
            // Более заметный карандаш справа снизу.
            using (var body = new GraphicsPath())
            {
                body.AddPolygon(new[]
                {
                    new PointF(9.2f, 10.2f),
                    new PointF(12.8f, 6.6f),
                    new PointF(14.1f, 7.9f),
                    new PointF(10.5f, 11.5f),
                });
                g.FillPath(new SolidBrush(Color.FromArgb(255, 140, 0)), body);
                using (var pen = new Pen(Color.FromArgb(230, 120, 0), 0.7f))
                    g.DrawPath(pen, body);
            }

            // Наконечник
            using (var tip = new GraphicsPath())
            {
                tip.AddPolygon(new[]
                {
                    new PointF(14.1f, 7.9f),
                    new PointF(15.2f, 7.1f),
                    new PointF(14.8f, 8.9f),
                });
                g.FillPath(new SolidBrush(Color.FromArgb(224, 200, 150)), tip);
                g.FillEllipse(Brushes.DimGray, 14.8f, 7.7f, 0.8f, 0.8f);
            }

            // Ластик
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 200, 160)), 8.5f, 10.5f, 1.1f, 1.6f);
        }

        private static void DrawWrench(Graphics g)
        {
            // Пиксельный ключ в стиле референса: тёмно-синий контур + бирюзовая заливка.
            g.SmoothingMode = SmoothingMode.None;

            var outline = Color.FromArgb(44, 60, 102);
            var darkFill = Color.FromArgb(92, 168, 184);
            var midFill = Color.FromArgb(120, 198, 210);
            var lightFill = Color.FromArgb(160, 222, 228);

            // 16x16 mask: O=outline, D=dark fill, M=mid fill, L=light fill
            var rows = new[]
            {
                "................",
                "........OOO.....",
                ".......ODDMO....",
                "......ODMLMOO...",
                ".....ODML..MO...",
                "....ODMM...MO...",
                "...ODML...OMO...",
                "..ODML...OMO....",
                ".ODML...OMO.....",
                "ODML...OMO......",
                "OML...OMO.......",
                ".OM..ODMO.......",
                "..OMOODMO.......",
                "...OOOMOO.......",
                ".....OO.........",
                "................",
                "................",
            };

            for (var y = 0; y < rows.Length; y++)
            {
                var row = rows[y];
                for (var x = 0; x < row.Length; x++)
                {
                    var ch = row[x];
                    if (ch == '.')
                        continue;

                    Color c;
                    if (ch == 'O')
                        c = outline;
                    else if (ch == 'D')
                        c = darkFill;
                    else if (ch == 'M')
                        c = midFill;
                    else if (ch == 'L')
                        c = lightFill;
                    else
                        c = Color.Transparent;
                    g.FillRectangle(new SolidBrush(c), x, y, 1, 1);
                }
            }
        }

        private static void DrawChannelsGroup(Graphics g)
        {
            using (var wirePen = new Pen(Color.FromArgb(96, 125, 139), 1.1f))
            {
                g.DrawLine(wirePen, 1, 5, 10, 5);
                g.DrawLine(wirePen, 1, 8, 10, 8);
                g.DrawLine(wirePen, 1, 11, 10, 11);
            }

            DrawChannelPlug(g, 10, 5);
            DrawChannelPlug(g, 10, 8);
            DrawChannelPlug(g, 10, 11);
        }

        private static void DrawChannelPlug(Graphics g, int x, int y)
        {
            using (var plug = new GraphicsPath())
            {
                plug.AddPolygon(new[]
                {
                    new PointF(x, y - 1.5f),
                    new PointF(x + 4, y),
                    new PointF(x, y + 1.5f),
                });
                FillStrokePath(g, plug, Blue);
            }
        }

        private static Pen StrokePen(float w)
        {
            return new Pen(Stroke, w) { LineJoin = LineJoin.Round };
        }

        private static void FillStrokePath(Graphics g, GraphicsPath path, Color fill)
        {
            g.FillPath(new SolidBrush(fill), path);
            using (var pen = StrokePen(0.9f))
                g.DrawPath(pen, path);
        }

        private static void FillStrokeRect(Graphics g, float x, float y, float w, float h, Color fill)
        {
            g.FillRectangle(new SolidBrush(fill), x, y, w, h);
            using (var pen = StrokePen(0.9f))
                g.DrawRectangle(pen, x, y, w, h);
        }
    }
}
