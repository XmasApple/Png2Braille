using System;
using System.Linq;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Png2Braille
{
    internal static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCP(uint wCodePageId);

        private static void Main()
        {
            var width = Console.LargestWindowWidth;
            var height = Console.LargestWindowHeight;
            SetConsoleOutputCP(65001);
            SetConsoleCP(65001);
            Console.SetWindowSize(width, height);

            const string alphabet =
                "⠀⠁⠂⠃⠄⠅⠆⠇⠈⠉⠊⠋⠌⠍⠎⠏⠐⠑⠒⠓⠔⠕⠖⠗⠘⠙⠚⠛⠜⠝⠞⠟⠠⠡⠢⠣⠤⠥⠦⠧⠨⠩⠪⠫⠬⠭⠮⠯⠰⠱⠲⠳⠴⠵⠶⠷⠸⠹⠺⠻⠼⠽⠾⠿⡀⡁⡂⡃⡄⡅⡆⡇⡈⡉⡊⡋⡌⡍⡎⡏⡐⡑⡒⡓⡔⡕⡖⡗⡘⡙⡚⡛⡜⡝⡞⡟⡠⡡⡢⡣⡤⡥⡦⡧⡨⡩⡪⡫⡬⡭⡮⡯⡰⡱⡲⡳⡴⡵⡶⡷⡸⡹⡺⡻⡼⡽⡾⡿⢀⢁⢂⢃⢄⢅⢆⢇⢈⢉⢊⢋⢌⢍⢎⢏⢐⢑⢒⢓⢔⢕⢖⢗⢘⢙⢚⢛⢜⢝⢞⢟⢠⢡⢢⢣⢤⢥⢦⢧⢨⢩⢪⢫⢬⢭⢮⢯⢰⢱⢲⢳⢴⢵⢶⢷⢸⢹⢺⢻⢼⢽⢾⢿⣀⣁⣂⣃⣄⣅⣆⣇⣈⣉⣊⣋⣌⣍⣎⣏⣐⣑⣒⣓⣔⣕⣖⣗⣘⣙⣚⣛⣜⣝⣞⣟⣠⣡⣢⣣⣤⣥⣦⣧⣨⣩⣪⣫⣬⣭⣮⣯⣰⣱⣲⣳⣴⣵⣶⣷⣸⣹⣺⣻⣼⣽⣾⣿";

            using var image = Image.Load<Rgb24>("test.jpg");
            image.Mutate(x => x.Resize(width * 2, height * 4).Grayscale());

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                Console.Write(alphabet[GetCharId(x, y, image)]);
        }

        public static int GetCharId(int x, int y, Image<Rgb24> image)
        {
            var min = int.MaxValue;
            var minInd = -1;
            for (var i = 0; i < 256; i++)
            {
                var s = new[]
                    {
                        (0, 0),
                        (0, 2),
                        (1, 0),
                        (0, 1),
                        (0, 3),
                        (1, 1),
                        (1, 2),
                        (1, 3),
                    }
                    .Select((t, ind) => (t, ind))
                    .Sum(t => Math.Abs(255 * ((i >> t.Item2) & 1) -
                                       image[x * 2 + t.Item1.Item1, y * 4 + t.Item1.Item2].R));
                if (s < min) (min, minInd) = (s, i);
            }

            return minInd;
        }
    }
}