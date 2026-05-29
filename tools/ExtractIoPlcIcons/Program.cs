using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Forms;

namespace ExtractIoPlcIcons
{
    internal static class Program
    {
        private const int ClampIndex = 12;
        private const int CableIndex = 13;

        private static void Main(string[] args)
        {
            var resxPath = args.Length > 0
                ? args[0]
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "src", "IO", "View", "IOViewControl.resx");
            var outDir = args.Length > 1
                ? args[1]
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "src", "Resources");

            resxPath = Path.GetFullPath(resxPath);
            outDir = Path.GetFullPath(outDir);

            using (var reader = new ResXResourceReader(resxPath))
            {
                foreach (DictionaryEntry entry in reader)
                {
                    if (entry.Key as string != "ViewItemImageList.ImageStream")
                        continue;

                    var imageList = new ImageList
                    {
                        ImageStream = (ImageListStreamer)entry.Value,
                    };

                    Save(imageList.Images[ClampIndex], Path.Combine(outDir, "io_plc_clamp.png"));
                    Save(imageList.Images[CableIndex], Path.Combine(outDir, "io_plc_cable.png"));
                    return;
                }
            }

            System.Console.Error.WriteLine("ImageStream not found in " + resxPath);
            Environment.Exit(1);
        }

        private static void Save(Image image, string path)
        {
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            System.Console.WriteLine(path);
        }
    }
}
