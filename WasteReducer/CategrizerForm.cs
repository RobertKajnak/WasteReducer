using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

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
        private Dictionary<Product, Bitmap> productIcons;
        private Font overlayFont;

        public CategrizerForm()
        {
            InitializeComponent();

            pictureBoxes = this.flowLayoutPanelMain.Controls;
            addedProducts = new Dictionary<PictureBox, Product>();
            productIcons = new Dictionary<Product, Bitmap>();
            labelHelp.Size = new Size(this.ClientSize.Width - 30, this.ClientSize.Height - this.menuStrip1.Height - 30);

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

            overlayFont = new Font("calibri", 12F);

        }

        private void Categorizer_Load(object sender, EventArgs e)
        {

        }

        #region Utility Classes and functions

        

        ///TODO(optional): analyse if handling only the ID instead of the Product object is better
        private void AddItem(long id, DateTime date)
        {
            ProductBase prodB = db.GetProduct(id);
            if (prodB == null)
            {
                MessageBox.Show("The specified product does not exist");
            }
            else
            {
                Product prod = new Product(prodB, date);
                //string filename = prod.ImageName;
                //Image im = LoadImage(filename);
                Image im = GetProductImage(prod);
                string overlayText = prod.Category + '\n' + prod.Price + '€' + '\n' + prod.Id;
                im = ExtraGraphics.AddTextToPicture(new Bitmap(im), overlayText, overlayFont);
                if (prod.Count > 1)
                {
                    overlayText = 'x' + prod.Count.ToString();
                    im = ExtraGraphics.AddTextToPicture(new Bitmap(im), overlayText, overlayFont, StringAlignment.Far, StringAlignment.Far);
                }
                PictureBox pic = AddPictureBox(im);
                addedProducts.Add(pic, prod);
            }
        }

        private void PromptAddItem()
        {
            var prompt = new AddItemForm();
            if (prompt.ShowDialog() == DialogResult.OK)
            {
                long barcode = prompt.barcode;

                if (barcode == -1)
                {
                    MessageBox.Show("Invalid Barcode");
                }
                else
                {
                    AddItem(barcode, prompt.date);
                }
            }

        }

        /*private void PasteItem()
        {
            long barcode=-1;
            long.TryParse(Clipboard.GetText(TextDataFormat.Text),out barcode);
            if (barcode > 0)
            {
                AddItem(barcode);
            }
        }*/


        ///TODO(optional): Reduce picture size after loading to reduce memory usage. Also: don't load the same image twice
        private PictureBox AddPictureBox(Image image)
        {

            if (labelHelp.Visible)
            {
                labelHelp.Visible = false;
                labelHelp.Enabled = false;
            }

            var pictureBox = ExtraGraphics.GeneratePictureBox(image);
            pictureBox.Name = "pictureBox" + pictureBoxes.Count;
            pictureBox.Click += new System.EventHandler(this.PictureBox_Click);
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
        /// Checks if a product has already been loaded and adds the necessary image.
        /// If the product is loaded, it uses the existing image, otherwise it loads it from disk
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private Bitmap GetProductImage(Product product)
        {
            if (!productIcons.ContainsKey(product))
                productIcons.Add(product, ExtraGraphics.LoadImage(product.ImageName));
            return productIcons[product];
        }


        private void AddDemoItems()
        {
            foreach (int i in new[]{6395,6395,6395,7614,7614,14757,24436,
                25351,25351,25351,25351,31289,1040,1040,1040, 21585, 21585, 21585,
                21578,21583, 12756,12756,12756,1624,7205,7205,7205,7205,32457,32457,
                498,498})
                AddItem(i,DateTime.Today.AddDays(2));
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
                    ///deBug
                    Random rand = new Random();
                    foreach (ProductBase p in db.Database)
                    {
                        AddItem(p.Id,DateTime.Today.AddDays(rand.Next(0,5)));
                    }
                    break;
                case (Keys.U):
                    ///debUg
                    AddDemoItems();
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
                        //PasteItem();
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
            labelHelp.Size = new Size(this.ClientSize.Width-30, this.ClientSize.Height - this.menuStrip1.Height-30);
        }
        #endregion
    }
}
