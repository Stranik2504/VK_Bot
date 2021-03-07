using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace VK_Bot.Components
{
    public static class ImageOverlay
    {
        private const int WidthOverlay = 580;
        private const int HeightOverlay = 200;
        private const int Rounding = 30;
        private const int SizePrize = 60;

        private static string _wayBackground = @"Assets\Background.png";
        private static string _nullAvatar = @"Assets\Null.png";
        private static string _wayBack = @"Assets\Back.jpg";

        public static string CreateTexture(string nameTextureAvatar, string nameUser, string[] photos)
        {
            try
            {
                Bitmap[] bitmaps = new Bitmap[10];
                // Юзер фото
                if (File.Exists($@"Avatar\{nameTextureAvatar}")) { bitmaps[0] = new Bitmap($@"Avatar\{nameTextureAvatar}"); } else { bitmaps[0] = new Bitmap(_nullAvatar); }
                for (int i = 0; i < photos.Length; i++)
                {
                    bitmaps[i + 1] = new Bitmap(photos[i]);
                }

                OverlayByNumber(bitmaps, nameUser, ConfigManager.Configs.TextPhoto, nameTextureAvatar);

                for (int i = 0; i < bitmaps.Length; i++)
                {
                    bitmaps[i].Dispose();
                }

                if (File.Exists($@"Avatar\{nameTextureAvatar}")) { try { File.Delete($@"Avatar\{nameTextureAvatar}"); } catch (Exception ex) { $"[Bot][CreateTexture][DeleteAvatar]: {ex.Message}".Log(); } }

                return $"{nameTextureAvatar}";
            }
            catch (Exception ex) { $"[Bot][CreateTexture]: {ex.Message}".Log(); }

            return "";
        }

        public static void OverlayByNumber(Bitmap[] bitmaps, string userName, string text, string resultsWay)
        {
            try
            {
                if (bitmaps.Length == 10)
                {
                    Bitmap results = new Bitmap(_wayBack);
                    results = new Bitmap(results, new Size(580, 200));

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 1; j <= 3; j++)
                        {
                            Overlay(results, Fillet(Compression(bitmaps[j + i * 3], SizePrize), Rounding), new Point(190 + 100 * j, -5 + 75 * i), new Size(SizePrize, SizePrize));
                        }
                    }

                    Overlay(results, new Bitmap(_wayBackground), new Point(0, 0), new Size(WidthOverlay, HeightOverlay));

                    //User photo
                    Overlay(results, Fillet(bitmaps[0], ConfigManager.Configs.Radius), new Point(190, 40), new Size(WidthOverlay, HeightOverlay), new Size(SizePrize, SizePrize));

                    Overlay(results, userName, new Point(219 - (int)GetWidthString(results, userName) / 2, 110), new Size(WidthOverlay, HeightOverlay));
                    Overlay(results, text, new Point(219 - (int)GetWidthString(results, text) / 2, 132), new Size(WidthOverlay, HeightOverlay));

                    results.Save(resultsWay);

                    results.Dispose();
                }
                else { Bitmap background = new Bitmap(_wayBackground); background.Save(resultsWay); }
            }
            catch (Exception ex) { $"[ImageOverlay][OverlayByNumber]: {ex.Message}".Log(); }
        }

        public static void Overlay(Bitmap firstImage, Bitmap secondImage, string resultsWay, Point secondPoint, Size secondSize)
        {
            try
            {
                Bitmap TargetBitmap = firstImage;
                Bitmap OverlayBitmap = secondImage;

                Bitmap ResultBitmap = new Bitmap(TargetBitmap.Width, TargetBitmap.Height);
                Graphics graph = Graphics.FromImage(ResultBitmap);
                graph.CompositingMode = CompositingMode.SourceOver;

                graph.DrawImage(TargetBitmap, 0, 0);
                graph.DrawImage(OverlayBitmap, secondPoint.X, secondPoint.Y, secondSize.Width, secondSize.Height);

                ResultBitmap.Save(resultsWay);
                graph.Dispose();
            }
            catch (Exception ex) { $"[ImageOverlay][Overlay(with save results)]: {ex.Message}".Log(); }
        }

        public static void Overlay(Bitmap firstImage, Bitmap secondImage, Point secondPoint, Size firstSize, Size secondSize)
        {
            try
            {
                firstImage = new Bitmap(firstImage, firstSize);
                Graphics graph = Graphics.FromImage(firstImage);
                graph.CompositingMode = CompositingMode.SourceOver;

                graph.DrawImage(secondImage, secondPoint.X, secondPoint.Y, secondSize.Width, secondSize.Height);
                graph.Dispose();
            }
            catch (Exception ex) { $"[ImageOverlay][Overlay(with firstSize)]: {ex.Message}".Log(); }
        }

        public static void Overlay(Bitmap firstImage, Bitmap secondImage, Point secondPoint, Size secondSize)
        {
            try
            {
                Graphics graph = Graphics.FromImage(firstImage);
                graph.CompositingMode = CompositingMode.SourceOver;

                graph.DrawImage(secondImage, secondPoint.X, secondPoint.Y, secondSize.Width, secondSize.Height);
                graph.Dispose();
            }
            catch (Exception ex) { $"[ImageOverlay][Overlay]: {ex.Message}".Log(); }
        }

        public static void Overlay(Bitmap firstImage, string text, Point textPoint, Size firstSize)
        {
            try
            {
                firstImage = new Bitmap(firstImage, firstSize);
                Graphics graph = Graphics.FromImage(firstImage);
                graph.CompositingMode = CompositingMode.SourceOver;

                graph.DrawString(text, new Font(ConfigManager.Configs.NameFont, ConfigManager.Configs.FontSize, FontStyle.Regular), new SolidBrush(ColorTranslator.FromHtml(ConfigManager.Configs.TextColor)), textPoint.X, textPoint.Y);
                graph.Dispose();
            }
            catch (Exception ex) { $"[ImageOverlay][Overlay(Text)]: {ex.Message}".Log(); }
        }

        public static float GetWidthString(Bitmap image, string text)
        {
            try
            {
                Graphics graphics = Graphics.FromImage(image);
                return graphics.MeasureString(text, new Font(ConfigManager.Configs.NameFont, ConfigManager.Configs.FontSize, FontStyle.Regular)).Width;
            }
            catch (Exception ex) { $"[ImageOverlay][GetWidthString]: {ex.Message}".Log(); }

            return 0.0f;
        }

        public static Bitmap Fillet(Bitmap image, int radius)
        {
            try
            {
                radius *= 2;
                Bitmap roundedImage = new Bitmap(image.Width, image.Height);

                using (Graphics g = Graphics.FromImage(roundedImage))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    Brush brush = new TextureBrush(image);
                    GraphicsPath gp = new GraphicsPath();
                    gp.AddArc(0, 0, radius, radius, 180, 90);
                    gp.AddArc(0 + roundedImage.Width - radius, 0, radius, radius, 270, 90);
                    gp.AddArc(0 + roundedImage.Width - radius, 0 + roundedImage.Height - radius, radius, radius, 0, 90);
                    gp.AddArc(0, 0 + roundedImage.Height - radius, radius, radius, 90, 90);
                    g.FillPath(brush, gp);
                    image.Dispose();
                    g.Dispose();
                    gp.Dispose();
                    brush.Dispose();
                }

                return roundedImage;
            }
            catch (Exception ex) { $"[ImageOverlay][Fillet]: {ex.Message}".Log(); }

            return null;
        }

        public static Bitmap Compression(Bitmap image, int size)
        {
            Bitmap resultsImage = new Bitmap(size, size);

            using (Graphics graph = Graphics.FromImage(resultsImage))
            {
                graph.CompositingMode = CompositingMode.SourceOver;
                graph.DrawImage(image, 0, 0, size, size);
            }

            return resultsImage;
        }
    }
}
