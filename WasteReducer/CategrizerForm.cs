﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WasteReducer
{
    public partial class CategrizerForm : Form
    {

        System.Windows.Forms.Control.ControlCollection pictureBoxes;
        PictureBox selectedPictureBox;
        private bool isControlPressed = false;
        private DatabaseHandler db = null;
        private const string PATH = "res/img/";
        private Dictionary<PictureBox, Product> addedProducts;

        public CategrizerForm()
        {
            InitializeComponent();

            pictureBoxes = this.flowLayoutPanelMain.Controls;
            addedProducts = new Dictionary<PictureBox, Product>();

            try
            {
                db = new DatabaseHandler(PATH);
            }
            catch (DatabaseException e)
            {
                MessageBox.Show(e.Message, "Could not open database");
                this.Dispose();
                this.Close();
                Application.Exit();
            }
        }

        private void Categorizer_Load(object sender, EventArgs e)
        {

        }

        #region Utility Classes and functions

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
                Label textLabel = new Label() { Left = 5, Top = 15, Text = text, Width=190,TextAlign = System.Drawing.ContentAlignment.MiddleCenter };
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

        ///TODO(optional): analyse if handling only the ID instead of the Product object is better
        private void AddItem(long id)
        {
            Product prod = db.GetProduct(id);
            if (prod == null)
            {
                MessageBox.Show("The specified product does not exist");
            }
            else
            {
                string filename = prod.ImageName;
                Image im = LoadImage(filename);
                PictureBox pic = AddPictureBox(im);
                addedProducts.Add(pic, prod);
            }
        }

        private void PromptAddItem()
        {
            string result = Prompt.ShowDialog("Type in the bar code", "Add code manually");
            if (result!=null)
            {
                long barcode = -1;
                long.TryParse(result, out barcode);
                if (barcode == -1)
                {
                    MessageBox.Show("Invalid Barcode");
                }
                else
                {
                    AddItem(barcode);
                }
            }

        }

        private void PasteItem()
        {
            long barcode=-1;
            long.TryParse(Clipboard.GetText(TextDataFormat.Text),out barcode);
            if (barcode > 0)
            {
                AddItem(barcode);
            }
        }


        ///TODO(optional): Reduce picture size after loading to reduce memory usage. Also: don't load the same image twice
        private PictureBox AddPictureBox(Image image)
        {

            if (labelHelp.Visible)
            {
                labelHelp.Visible = false;
                labelHelp.Enabled = false;
            }
            PictureBox pictureBox = new PictureBox();
            pictureBox.Name = "pictureBox" + pictureBoxes.Count;
            ///Eliminates the possibility of not having a selection zone around the image
            if (image.Size.Height / image.Size.Width == 5 / 4)
                pictureBox.Size = new System.Drawing.Size(210, 160);
            else
                pictureBox.Size = new System.Drawing.Size(200, 160);

            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            pictureBox.Padding = new System.Windows.Forms.Padding(5);
            pictureBox.Click += new System.EventHandler(this.PictureBox_Click);

            pictureBox.Image = image;

            pictureBoxes.Add(pictureBox);

            this.flowLayoutPanelMain.ResumeLayout(false);

            return pictureBox;
        }

        private void SelectPictureBox(PictureBox pb)
        {
            if (selectedPictureBox != null)
            {
                selectedPictureBox.BackColor = SystemColors.Control;
            }
            selectedPictureBox = (pb);
            selectedPictureBox.BackColor = Color.CadetBlue;
        }

        private void RemovePictureBox()
        {
            if (selectedPictureBox != null)
            {
                pictureBoxes.Remove(selectedPictureBox);
                addedProducts.Remove(selectedPictureBox);
                selectedPictureBox.Image.Dispose();
                selectedPictureBox.Dispose();
                selectedPictureBox = null;
                if (this.pictureBoxes.Count == 0)
                {
                    labelHelp.Visible = true;
                }
            }
            else
            {
                if (pictureBoxes.Count > 0)
                {
                    SelectPictureBox((PictureBox)pictureBoxes[0]);
                }
            }
        }

        private void ClearWorkspace()
        {
            selectedPictureBox = null;
            pictureBoxes.Clear();
            addedProducts = new Dictionary<PictureBox, Product>();
            System.GC.Collect();
            labelHelp.Visible = true;
        }

        private void CalcuateWasteBags()
        {
            if (pictureBoxes.Count < 1)
            {
                MessageBox.Show("Please add at least one product", "Not enough products");
            }
            else
            {
                new WasteBagForm(addedProducts).Show();
            }
        }


        /// <summary>
        /// Loads the bitmap from file, copies it, than releases the file resource
        /// </summary>
        /// <param name="path">Path to the image file</param>
        /// <returns></returns>
        static Bitmap LoadImage(string path)
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
            

            return img;
        }

        #endregion

        #region ToolStrip Events

        #region File

        private void ClearWorkspaceCtrlDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearWorkspace();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
            Application.Exit();
        }


        #endregion

        #region Add

        private void AddBarcodeStripMenuItem_Click(object sender, EventArgs e)
        {
            PromptAddItem();
        }

        #endregion

        #region Generate

        private void GenerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalcuateWasteBags();
        }

        #endregion

        #region Help
        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String text = "Scan an Item to add it to the selection.\n" +
                        "It can also be added by double clicking in an empty region" + 
                        "\n\n" +
                        "Delete or backspace removes the item\n" +
                        "\n\n" + 
                        "Press Enter or click on 'Generate' to get a suggestion on what should be discounted" +
                        "and which items should go into a Zero Waste Bag";
            String caption = "Help/About";
            MessageBox.Show(text, caption);
        }
        #endregion

        #endregion

        #region Mouse Events
        private void FlowLayoutPanelMain_DoubleClick(object sender, EventArgs e)
        {
            PromptAddItem();
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            SelectPictureBox((PictureBox)sender);
        }
        #endregion

        #region Keyboard Events

        private void Categorizer_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case ((char)27):
                    this.Dispose();
                    this.Close();
                    Application.Exit();
                    break;
            }
        }

        private void Categorizer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (Keys.B):
                    ///DeBUG
                    foreach (Product p in db.Database)
                    {
                        AddItem(p.Id);
                    }
                    break;
                case (Keys.D):
                    ClearWorkspace();
                    break;
                case (Keys.ControlKey):
                    isControlPressed = true;
                    break;
                case (Keys.V):
                    if (isControlPressed)
                    {
                        PasteItem();
                    }
                    break;
                case (Keys.Enter):
                    CalcuateWasteBags();
                    break;
                case (Keys.Delete):
                //intentional fallthrough
                case (Keys.Back):
                    RemovePictureBox();
                    break;
            }
        }

        private void Categorizer_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (Keys.ControlKey):
                    isControlPressed = false;
                    break;
                default: break;
            }
        }

        #endregion

        #region other events
        private void Categorizer_Resize(object sender, EventArgs e)
        {
            flowLayoutPanelMain.Size = new System.Drawing.Size(this.ClientSize.Width, this.ClientSize.Height - this.menuStrip1.Height);
        }
        #endregion
    }
}
