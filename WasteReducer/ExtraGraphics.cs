using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WasteReducer
{
    public static class ExtraGraphics
    {
        public static int BOXWIDTH = 250;
        public static int BOXHEIGHT = 200;

        /// <summary>
        /// Calculate the dominant color of the image
        /// </summary>
        /// <param name="image">Image to parse</param>
        /// <param name="useDominant">True: uses most dominant R/G/B; False: uses average</param>
        /// <returns></returns>
        public enum SearchPattern
        {
            Dominant,
            Average,
            BlackAndWhite
        }
        public static Color GetDominantColor(Bitmap image, SearchPattern mode)
        {
            int R = 0, G = 0, B = 0, S = 0;
            for (int i = image.Height / 4; i < image.Height * 3 / 4; i += 10)
                for (int j = image.Width / 4; j < image.Width * 3 / 4; j += 10)
                {
                    Color c = image.GetPixel(j, i);
                    if (mode == SearchPattern.Dominant)
                    {
                        R += (c.R >= c.G && c.R >= c.B) ? 1 : 0;
                        G += (c.G >= c.R && c.G >= c.B) ? 1 : 0;
                        B += (c.B >= c.G && c.B >= c.R) ? 1 : 0;
                    }
                    else
                    {
                        R += c.R;
                        G += c.G;
                        B += c.B;
                        S++;
                    }
                }
            if (mode==SearchPattern.Dominant)
            {
                int Rn = (R >= G && R >= B) ? 255 : 0;
                int Gn = (G >= R && R >= B) ? 255 : 0;
                int Bn = (B >= G && B >= R) ? 255 : 0;
                R = Rn; B = Bn; G = Gn;
            }
            else
            {
                R /= S; G /= S; B /= S;
                if (mode == SearchPattern.BlackAndWhite)
                {
                    if (R + G + B > 128*3)
                    {
                        R = 255;G = 255;B = 255;
                    }
                    else
                    {
                        R = 0;G = 0;B = 0;
                    }
                }
            }

            return Color.FromArgb(R, G, B);
        }

        /// <summary>
        /// Overlays text onto the original. The original file is modified. The same is returned
        /// </summary>
        /// <param name="original">the original image. Is not modified</param>
        /// <param name="text">The text ot be overlayed</param>
        /// <param name="font">the font familiy and sizeis the only relevant attribute</param>
        /// <returns> pointer to original</returns>
        public static Bitmap AddTextToPicture(Bitmap original, string text, Font font,
                StringAlignment horizontal = StringAlignment.Center, StringAlignment vertical = StringAlignment.Center)
        {
            Bitmap newIm = (Bitmap)original.Clone();

            float scalingFactor = (float)((float)newIm.Height / 100.0);
            Font resizedFont = new Font(font.FontFamily, font.Size * scalingFactor);

            Color colShadow = GetDominantColor(newIm, SearchPattern.BlackAndWhite);
            Color col = Color.FromArgb(255 - colShadow.R, 255 - colShadow.G, 255 - colShadow.B);

            StringFormat format = new StringFormat();
            format.Alignment = horizontal;
            format.LineAlignment = vertical;
            //format.Trimming = StringTrimming.EllipsisCharacter;
            //Bitmap img = new Bitmap(width, height);
            Graphics Graph = Graphics.FromImage(newIm);

            //G.Clear(background);
            int offset = (int)Math.Ceiling(scalingFactor * 1);
            var rect_shadow = new Rectangle(offset, offset, newIm.Width, newIm.Height);

            SolidBrush brush_shadow = new SolidBrush(colShadow);
            Graph.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            Graph.DrawString(text, resizedFont, brush_shadow, rect_shadow, format);
            brush_shadow.Dispose();

            var rect = new Rectangle(0, 0, newIm.Width, newIm.Height);
            SolidBrush brush_text = new SolidBrush(col);
            Graph.DrawString(text, resizedFont, brush_text, rect, format);
            brush_text.Dispose();

            return newIm;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Loads the bitmap from file, copies it, than releases the file resource
        /// </summary>
        /// <param name="path">Path to the image file</param>
        /// <returns></returns>
        public static Bitmap LoadImage(string path)
        {
            Bitmap img = null;
            try
            {
                using (Bitmap b = new Bitmap(path))
                {
                    img = new Bitmap(b.Width, b.Height, b.PixelFormat);
                    img.SetResolution(b.HorizontalResolution, b.VerticalResolution);
                    //img.Palette = b.Palette; -- clears the image. Maybe not loaded properly? Oh, well
                    img.Tag = b.Tag;

                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.DrawImage(b, 0, 0);
                        g.Flush();
                    }
                }
            }
            catch
            {
                img = WasteReducer.Properties.Resources.placeholder;
            }

            return ResizeImage(img, img.Width* BOXHEIGHT / img.Height, BOXHEIGHT);
        }

        //TODO: Escape activates cancel
        /// <summary>
        /// Creates a window with a single input field that returns the value entered into that field
        /// </summary>
        public static class Prompt
        {
            public static string ShowDialog(string text, string caption)
            {
                Form prompt = new Form()
                {
                    Width = 210,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };
                Label textLabel = new Label() { Left = 5, Top = 15, Text = text, Width = 190, TextAlign = System.Drawing.ContentAlignment.MiddleCenter };
                TextBox textBox = new TextBox() { Left = 50, Top = 40, Width = 100 };
                Button confirmation = new Button() { Text = "Ok", Left = 40, Width = 50, Top = 70, DialogResult = DialogResult.OK };
                Button cancel = new Button() { Text = "Cancel", Left = 110, Width = 50, Top = 70, DialogResult = DialogResult.Cancel };

                ///TODO(optional): Add a different icon for this
                prompt.Icon = global::WasteReducer.Properties.Resources.icon;
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(cancel);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
            }
        }


        public static PictureBox GeneratePictureBox(Image image)
        {
            PictureBox pictureBox = new PictureBox();
            ///Eliminates the possibility of not having a selection zone around the image
            if ((double)image.Size.Width / image.Size.Height != (double)BOXWIDTH / BOXHEIGHT)
                pictureBox.Size = new System.Drawing.Size(BOXWIDTH, BOXHEIGHT);
            else
                pictureBox.Size = new System.Drawing.Size(BOXWIDTH-15, BOXHEIGHT);

            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            pictureBox.Padding = new System.Windows.Forms.Padding(5);

            pictureBox.Image = image;

            return pictureBox;
        }
    }
}
