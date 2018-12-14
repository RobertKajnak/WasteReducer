﻿using System;
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
        List<List<Product>> productLanes;
        FlowLayoutPanel shelfLane;
        FlowLayoutPanel discountedLane;
        List<List<Product>> wasteBags;
        List<FlowLayoutPanel> wasteBageLanes;
        FlowLayoutPanel wasteBagLanePanel = null;
        FlowLayoutPanel trashLane;
        WasteBagConfiguration WBConfig = null;
        Dictionary<Product, Bitmap> productIcons;
        bool isGroupingEnabled;

        public WasteBagForm(List<Product> productList, Dictionary<Product,Bitmap> productIcons)
        {
            InitializeComponent();

            var screen = Screen.FromControl(this);
            this.Width = screen.WorkingArea.Width * 7 / 10;
            this.Height = screen.WorkingArea.Height * 9 / 10;

            this.productIcons = productIcons;

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

            productLanes = new List<List<Product>>();

            productLanes.Add(logic.GetShelfItems());
            shelfLane = AddLane(Lanetype.SHELF);
            AddProductsToLane("Shelf", shelfLane, logic.GetShelfItems());
            
            discountedLane = AddLane(Lanetype.DISCOUNT);
            productLanes.Add(logic.GetDiscountedProducts());
            AddProductsToLane("Discount", discountedLane, logic.GetDiscountedProducts());
            
            trashLane = AddLane(Lanetype.TRASH);
            productLanes.Add(logic.GetExpiredItems());
            AddProductsToLane("Trash", trashLane, logic.GetExpiredItems());

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
                    zwblabel.Size = new Size(ExtraGraphics.BOXWIDTH+10,96);
                    zwblabel.Text = "ZWB " + (wasteBags.IndexOf(bag)+1) + '\n' + bag.Sum(p=>p.Price) + '€';
                    wasteBagLane.Controls.Add(zwblabel);
                    //wasteBagLane.Size = new Size(wasteBagLane.Size.Width, wasteBagLane.Height + zwblabel.Height);
                    foreach (Product prod in bag)
                    {
                        var pb = AddToLane(prod, wasteBagLane);
                        //wasteBagLane.Size = new Size(wasteBagLane.Size.Width, wasteBagLane.Height + pb.Height+1);
                    }
                    wasteBagLanePanel.Size = new Size(wasteBagLanePanel.Width, wasteBageLanes.Max(x => x.Height)+10);
                }
            }

        }

        #region Helper Classes And Functions

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

        

        private void AddProductsToLane(string label, FlowLayoutPanel lane, List<Product> products)
        {
            if (products.Count > 0)
                lane.Controls.Add(new RotatedLabel(label));
            foreach (Product prod in products)
            {
                AddToLane(prod, lane);
            }
        }

        private FlowLayoutPanel addWasteBagLane()
        {
            if (wasteBagLanePanel == null)
            {
                wasteBageLanes = new List<FlowLayoutPanel>();
                wasteBagLanePanel = new FlowLayoutPanel();

                this.flowLayoutPanelMain.Controls.Add(wasteBagLanePanel);

                wasteBagLanePanel.AutoSize = true;
                wasteBagLanePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                wasteBagLanePanel.Location = new System.Drawing.Point(3, 3);
                wasteBagLanePanel.Name = "flowLayoutPanelZWB";
                ///TODO: fix the height
                wasteBagLanePanel.Size = new System.Drawing.Size(10, 10);
                wasteBagLanePanel.TabIndex = 0;
            }

            wasteBagLanePanel.Size = new System.Drawing.Size(
                wasteBagLanePanel.Width + ExtraGraphics.BOXWIDTH+5, wasteBagLanePanel.Height);

            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            wasteBageLanes.Add(flowLayoutPanel);

            this.wasteBagLanePanel.Controls.Add(flowLayoutPanel);

            flowLayoutPanel.AutoSize = true;
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            flowLayoutPanel.Location = new System.Drawing.Point(3, 3);
            flowLayoutPanel.Name = "flowLayoutPanelZWB";
            flowLayoutPanel.Size = new System.Drawing.Size(ExtraGraphics.BOXWIDTH, 42);
            flowLayoutPanel.TabIndex = 0;

            return flowLayoutPanel;
        }

        /// <summary>
        /// Returns the added Picturebox
        /// </summary>
        /// <param name="prod"></param>
        /// <param name="lane"></param>
        /// <returns></returns>
        private PictureBox AddToLane(Product prod,FlowLayoutPanel lane)
        {
            Image im = productIcons[prod];
            im = ExtraGraphics.AddTextOverlay(im, prod);
            PictureBox pb = ExtraGraphics.GeneratePictureBox(im);
            lane.Controls.Add(pb);
            return pb;
        }

        private void OrganizeLane(FlowLayoutPanel lane, List<Product> products)
        {
            Control l = lane.Controls[0];
            lane.Controls.Clear();

            var organizedList = new List<Product>();
            if (isGroupingEnabled)
            {
                foreach (Product p in products)
                {
                    if (organizedList.Contains(p))
                    {
                        var foundP = organizedList.Find(x => p.Equals(x));
                        foundP.Count+=p.Count;
                    }
                    else
                    {
                        organizedList.Add(new Product(p));
                    }
                }
            }
            else
            {
                foreach (Product p in products)
                {
                    for (int i = 0; i < p.Count; i++)
                    {
                        organizedList.Add(new Product(p));
                    }
                }
            }
            lane.Controls.Add(l);
            foreach (var p in organizedList)
            {
                AddToLane(p, lane);
            }
        }

        private void OrganizeLanes()
        {
            var lanes = new[] { shelfLane, discountedLane, trashLane };
            for (int i = 0; i < 3; i++)
            {
                if (productLanes[i].Count > 1)
                {
                    OrganizeLane(lanes[i], productLanes[i]);
                }
            }
            for (int i=0;i<wasteBags.Count;i++)
            {
                OrganizeLane(wasteBageLanes[i], wasteBags[i]);
            }
            

        }

        #endregion

        #region Buttons

        #region Options
        private void groupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isGroupingEnabled = groupToolStripMenuItem.Checked;
            OrganizeLanes();
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
                case (Keys.G):
                    isGroupingEnabled = !isGroupingEnabled;
                    groupToolStripMenuItem.Checked = isGroupingEnabled;
                    OrganizeLanes();
                    break;
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
