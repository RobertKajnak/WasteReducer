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
        private bool isGroupingEnabled;

        public CategrizerForm()
        {
            InitializeComponent();
            var screen = Screen.FromControl(this);
            this.Width = screen.WorkingArea.Width*7/10;
            this.Height = screen.WorkingArea.Height * 7 / 10;
            ExtraGraphics.BOXHEIGHT = screen.WorkingArea.Height *10/53;
            ExtraGraphics.BOXWIDTH = ExtraGraphics.BOXHEIGHT * 5 / 4;


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

            isGroupingEnabled = groupItemsToolStripMenuItem.Checked;
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
                AddItem(prod);
            }
        }

        private void AddItem(Product product)
        {
            List<Product> products = new List<Product>();
            if (isGroupingEnabled)
            {
                var toRemove = new List<PictureBox>();
                foreach (var pb in addedProducts.Keys)
                {
                    if (product.Equals(addedProducts[pb]))
                    {
                        toRemove.Add(pb);
                    }
                }
                product.Count += toRemove.Sum(x => addedProducts[x].Count);

                foreach (var pb in toRemove)
                {
                    SelectPictureBox(pb);
                    RemovePictureBox();
                }
                products.Add(product);
            }
            else
            {
                for (int i = 0; i < product.Count; i++)
                {
                    products.Add(new Product(product));
                }
            }
            foreach (var prod in products)
            {
                Image im = GetProductImage(prod);
                string overlayText = prod.Category + '\n' + prod.Price + '€' + '\n' + prod.Id + "\n To Exp: " + (prod.ExpiryDate - DateTime.Today).TotalDays;
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

        private void OrganizeDuplicates()
        {
            var allProducts = addedProducts.Values;
            ClearWorkspace();
            foreach (Product p in allProducts)
            {
                AddItem(p);
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
                new WasteBagForm(addedProducts.Values.ToList(),productIcons).Show();
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

        private void ZoomIn()
        {
            ExtraGraphics.BOXHEIGHT = (int)(ExtraGraphics.BOXHEIGHT * 1.2);
            ExtraGraphics.BOXWIDTH = (int)(ExtraGraphics.BOXWIDTH * 1.2);
            OrganizeDuplicates();
        }
        private void ZoomOut()
        {
            ExtraGraphics.BOXHEIGHT = (int)(ExtraGraphics.BOXHEIGHT / 1.2);
            ExtraGraphics.BOXWIDTH = (int)(ExtraGraphics.BOXWIDTH / 1.2);
            OrganizeDuplicates();
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

        #region Options
        private void groupItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (groupItemsToolStripMenuItem.Checked)
            {
                isGroupingEnabled = true;
            }
            else
            {
                isGroupingEnabled = false;
            }
            OrganizeDuplicates();
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
                case (Keys.Add):
                    ZoomIn();
                    break;
                case (Keys.Subtract):
                    ZoomOut();
                    break;
                case (Keys.U):
                    ///debUg
                    AddDemoItems();
                    break;
                case (Keys.D):
                    ClearWorkspace();
                    break;
                case (Keys.G):
                    groupItemsToolStripMenuItem.Checked = !groupItemsToolStripMenuItem.Checked;
                    isGroupingEnabled = groupItemsToolStripMenuItem.Checked;
                    OrganizeDuplicates();
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

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }
    }
}
