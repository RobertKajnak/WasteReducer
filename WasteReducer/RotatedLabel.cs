using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WasteReducer
{
    /// <summary>
    /// This is trimmed version of the code found at:
    /// @"https://www.codeproject.com/Articles/8383/Customized-Text-Orientated-Controls-in-C-Part-I-La>"
    /// </summary>
    class RotatedLabel : System.Windows.Forms.Label
    {
        private double rotationAngle;
        private string text;

        public RotatedLabel(string text)
        {
            //Setting the initial condition.
            rotationAngle = 270d;
            this.Size = new Size(32, 160);
            this.text = text;
            this.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Font = new Font("Calibri", 32);
        }


        
        public override string Text
        {
            get => text;
            set { text = value; this.Invalidate(); }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.Trimming = StringTrimming.None;

            Brush textBrush = new SolidBrush(this.ForeColor);

            //Getting the width and height of the text, which we are going to write
            float width = graphics.MeasureString(text, this.Font).Width;
            float height = graphics.MeasureString(text, this.Font).Height;

            double angle = (rotationAngle / 180) * Math.PI;
            graphics.TranslateTransform(
                (ClientRectangle.Width + (float)(height * Math.Sin(angle)) - (float)(width * Math.Cos(angle))) / 2,
                (ClientRectangle.Height - (float)(height * Math.Cos(angle)) - (float)(width * Math.Sin(angle))) / 2);
            graphics.RotateTransform((float)rotationAngle);
            graphics.DrawString(text, this.Font, textBrush, 0, 0);
            graphics.ResetTransform();

                
        }
        
    }
}
