using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WasteReducer
{
    public partial class WasteBagForm : Form
    {
        CategorizerLogic logic;
        List<Product> shelf;
        FlowLayoutPanel shelfLane;
        List<Product> discounted;
        FlowLayoutPanel discountedLane;
        List<List<Product>> wasteBags;
        FlowLayoutPanel wasteBagLanePanel = null;
        List<Product> trash;
        FlowLayoutPanel trashLane;
        WasteBagConfiguration WBConfig = null;

        public WasteBagForm(List<Product> productList, Dictionary<Product,Bitmap> productIcons)
        {
            InitializeComponent();

            var products = new List<Product>();
            foreach (var prod in productList)
            {
                for (int i = 0; i < prod.Count; i++)
                {
                    products.Add(prod);
                }
            }

            WBConfig = new WasteBagConfiguration();
            logic = new CategorizerLogic(products, WBConfig);
            

            shelf = logic.GetShelfItems();
            shelfLane = AddLane(Lanetype.SHELF);
            if (shelf.Count > 0) 
                shelfLane.Controls.Add(new RotatedLabel("Shelf"));
            foreach (Product prod in shelf)
            {
                shelfLane.Controls.Add(CreatePictureBox(productIcons[prod]));
            }

            discounted = logic.GetDiscountedProducts();
            discountedLane = AddLane(Lanetype.DISCOUNT);
            if (discounted.Count>0)
                discountedLane.Controls.Add(new RotatedLabel("Discount"));
            foreach (Product prod in discounted)
            {
                discountedLane.Controls.Add(CreatePictureBox(productIcons[prod]));
            }

            trash = logic.GetExpiredItems();
            trashLane = AddLane(Lanetype.TRASH);
            if (trash.Count > 0)
                trashLane.Controls.Add(new RotatedLabel("Trash"));
            foreach (Product prod in trash)
            {
                trashLane.Controls.Add(CreatePictureBox(productIcons[prod]));
            }
            
            wasteBags = logic.GetWasteBags();
            if (wasteBags.Count > 0)
            {
                foreach (List<Product> bag in wasteBags)
                {
                    if (bag.Count == 0)
                        continue;
                    FlowLayoutPanel wasteBagLane = AddLane(Lanetype.ZWB);
                    Label zwblabel = new Label();
                    zwblabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    zwblabel.Font = new Font("Calibri", 32);
                    zwblabel.Size = new Size(210,96);
                    zwblabel.Text = "ZWB " + (wasteBags.IndexOf(bag)+1) + '\n' + bag.Sum(p=>p.Price) + '€';
                    wasteBagLane.Controls.Add(zwblabel);
                    wasteBagLane.Size = new Size(wasteBagLane.Size.Width, wasteBagLane.Height + zwblabel.Height);
                    foreach (Product prod in bag)
                    {
                        var pb = CreatePictureBox(productIcons[prod]);
                        wasteBagLane.Controls.Add(pb);
                        wasteBagLane.Size = new Size(wasteBagLane.Size.Width, wasteBagLane.Height + pb.Height+1);
                    }
                }
            }

        }

        #region Helper Classes And Functions


        private Dictionary<Product,Image> GenerateReverseMap(Dictionary<PictureBox, Product> original)
        {
            var reverse = new Dictionary<Product, Image>();
            foreach (PictureBox pb in original.Keys)
            {
                if (!reverse.ContainsKey(original[pb]))
                {
                    reverse.Add(original[pb], pb.Image);
                }
                
            }
            return reverse;
        }


        enum Lanetype { SHELF, DISCOUNT, ZWB, TRASH}

        private FlowLayoutPanel AddLane(Lanetype type)
        {
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();

            if (type == Lanetype.ZWB)
            {
                return addWasteBagLane();
            }
            else
            {
                this.flowLayoutPanelMain.Controls.Add(flowLayoutPanel);
            }
                
            switch (type)
            {
                case Lanetype.SHELF:
                    flowLayoutPanel.BackColor = System.Drawing.Color.LightCoral;
                    break;
                case Lanetype.DISCOUNT:
                    flowLayoutPanel.BackColor = System.Drawing.Color.PapayaWhip;
                    break;
                case Lanetype.TRASH:
                    flowLayoutPanel.BackColor = System.Drawing.Color.DarkRed;
                    break;
                default:
                    break;
            }

            flowLayoutPanel.AutoSize = true;
            flowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            flowLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            flowLayoutPanel.Location = new System.Drawing.Point(3, 3);
            flowLayoutPanel.Name = "flowLayoutPanel" + type.ToString();
            flowLayoutPanel.Size = new System.Drawing.Size(2, 2);
            flowLayoutPanel.TabIndex = 0;

            return flowLayoutPanel;
        }

        private FlowLayoutPanel addWasteBagLane()
        {
            if (wasteBagLanePanel == null)
            {
                wasteBagLanePanel = new FlowLayoutPanel();

                this.flowLayoutPanelMain.Controls.Add(wasteBagLanePanel);

                wasteBagLanePanel.AutoSize = false;
                wasteBagLanePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
                wasteBagLanePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                wasteBagLanePanel.Location = new System.Drawing.Point(3, 3);
                wasteBagLanePanel.Name = "flowLayoutPanelZWB";
                ///TODO: fix the height
                wasteBagLanePanel.Size = new System.Drawing.Size(this.flowLayoutPanelMain.Size.Width, 2000);
                wasteBagLanePanel.TabIndex = 0;
            }

            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();

            this.wasteBagLanePanel.Controls.Add(flowLayoutPanel);

            flowLayoutPanel.AutoSize = false;
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            flowLayoutPanel.Location = new System.Drawing.Point(3, 3);
            flowLayoutPanel.Name = "flowLayoutPanelZWB";
            flowLayoutPanel.Size = new System.Drawing.Size(210, 42);
            flowLayoutPanel.TabIndex = 0;

            return flowLayoutPanel;
        }

        private PictureBox CreatePictureBox(Image image)
        {
            PictureBox pictureBox = new PictureBox();
            ///Eliminates the possibility of not having a selection zone around the image
            if (image.Size.Height / image.Size.Width == 5 / 4)
                pictureBox.Size = new System.Drawing.Size(210, 160);
            else
                pictureBox.Size = new System.Drawing.Size(200, 160);

            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            pictureBox.Padding = new System.Windows.Forms.Padding(5);
            //pictureBox.Click += new System.EventHandler(this.PictureBox_Click);

            pictureBox.Image = image;

            return pictureBox;
        }

        #endregion

        #region Buttons

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

        #region Keyboard Events

        private void WasteBagFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case ((char)27):
                    this.Dispose();
                    this.Close();
                    break;
            }
        }

        private void WasteBagFrom_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case (Keys.ControlKey):
                    //isControlPressed = true;
                    break;
            }
        }

        private void WasteBagFrom_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (Keys.ControlKey):
                    //isControlPressed = false;
                    break;
                default: break;
            }
        }

        #endregion

        #region Other events
        private void WasteBagFrom_Resize(object sender, EventArgs e)
        {
            flowLayoutPanelMain.Size = new System.Drawing.Size(this.ClientSize.Width, this.ClientSize.Height - this.menuStrip1.Height);
        }
        #endregion
    }
}
