using System;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Png2Braille
{
    internal class Program
    {
        public const int Width = 274;
        public const int Height = 72;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCP(uint wCodePageId);

        private static void Main(string[] args)
        {
            SetConsoleOutputCP(65001);
            SetConsoleCP(65001);
            Console.SetWindowSize(Width, Height+1);

            const string alphabet = "⠀⠁⠂⠃⠄⠅⠆⠇⠈⠉⠊⠋⠌⠍⠎⠏⠐⠑⠒⠓⠔⠕⠖⠗⠘⠙⠚⠛⠜⠝⠞⠟⠠⠡⠢⠣⠤⠥⠦⠧⠨⠩⠪⠫⠬⠭⠮⠯⠰⠱⠲⠳⠴⠵⠶⠷⠸⠹⠺⠻⠼⠽⠾⠿⡀⡁⡂⡃⡄⡅⡆⡇⡈⡉⡊⡋⡌⡍⡎⡏⡐⡑⡒⡓⡔⡕⡖⡗⡘⡙⡚⡛⡜⡝⡞⡟⡠⡡⡢⡣⡤⡥⡦⡧⡨⡩⡪⡫⡬⡭⡮⡯⡰⡱⡲⡳⡴⡵⡶⡷⡸⡹⡺⡻⡼⡽⡾⡿⢀⢁⢂⢃⢄⢅⢆⢇⢈⢉⢊⢋⢌⢍⢎⢏⢐⢑⢒⢓⢔⢕⢖⢗⢘⢙⢚⢛⢜⢝⢞⢟⢠⢡⢢⢣⢤⢥⢦⢧⢨⢩⢪⢫⢬⢭⢮⢯⢰⢱⢲⢳⢴⢵⢶⢷⢸⢹⢺⢻⢼⢽⢾⢿⣀⣁⣂⣃⣄⣅⣆⣇⣈⣉⣊⣋⣌⣍⣎⣏⣐⣑⣒⣓⣔⣕⣖⣗⣘⣙⣚⣛⣜⣝⣞⣟⣠⣡⣢⣣⣤⣥⣦⣧⣨⣩⣪⣫⣬⣭⣮⣯⣰⣱⣲⣳⣴⣵⣶⣷⣸⣹⣺⣻⣼⣽⣾⣿";

            using var image = Image.Load<Rgb24>("test.jpg");
            image.Mutate(x => x.Resize(Width * 2, Height * 4).Grayscale());

            for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
                Console.Write(alphabet[GetCharId(x, y, image)]);
        }

        public static int GetCharId(int x, int y, Image<Rgb24> image)
        {
            var min = int.MaxValue;
            var minInd = -1;
            for (var i = 0; i < 256; i++)
            {
                var s =
                    Math.Abs(255 * ((i >> 0) & 1) - image[x*2+0, y*4+0].R) +
                    Math.Abs(255 * ((i >> 1) & 1) - image[x*2+0, y*4+2].R) +
                    Math.Abs(255 * ((i >> 2) & 1) - image[x*2+1, y*4+0].R) +
                    Math.Abs(255 * ((i >> 3) & 1) - image[x*2+0, y*4+1].R) +
                    Math.Abs(255 * ((i >> 4) & 1) - image[x*2+0, y*4+3].R) +
                    Math.Abs(255 * ((i >> 5) & 1) - image[x*2+1, y*4+1].R) +
                    Math.Abs(255 * ((i >> 6) & 1) - image[x*2+1, y*4+2].R) +
                    Math.Abs(255 * ((i >> 7) & 1) - image[x*2+1, y*4+3].R);
                if (s >= min) continue;
                min = s;
                minInd = i;
            }

            return minInd;
        }
    }
}