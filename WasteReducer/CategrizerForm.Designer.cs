namespace WasteReducer
{
    partial class CategrizerForm
    {

        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearWorkspaceCtrlDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanelMain = new System.Windows.Forms.FlowLayoutPanel();
            this.labelHelp = new System.Windows.Forms.Label();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.AllowDrop = true;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.generateToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1134, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearWorkspaceCtrlDToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // clearWorkspaceCtrlDToolStripMenuItem
            // 
            this.clearWorkspaceCtrlDToolStripMenuItem.Name = "clearWorkspaceCtrlDToolStripMenuItem";
            this.clearWorkspaceCtrlDToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.clearWorkspaceCtrlDToolStripMenuItem.Text = "Clear Workspace (Ctrl+D)";
            this.clearWorkspaceCtrlDToolStripMenuItem.Click += new System.EventHandler(this.ClearWorkspaceCtrlDToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.closeToolStripMenuItem.Text = "Close (Escape)";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // generateToolStripMenuItem
            // 
            this.generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            this.generateToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.generateToolStripMenuItem.Text = "Generate!";
            this.generateToolStripMenuItem.Click += new System.EventHandler(this.GenerateToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(82, 20);
            this.helpToolStripMenuItem.Text = "Help/About";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.HelpToolStripMenuItem_Click);
            // 
            // flowLayoutPanelMain
            // 
            this.flowLayoutPanelMain.AllowDrop = true;
            this.flowLayoutPanelMain.AutoScroll = true;
            this.flowLayoutPanelMain.Location = new System.Drawing.Point(0, 27);
            this.flowLayoutPanelMain.Name = "flowLayoutPanelMain";
            this.flowLayoutPanelMain.Size = new System.Drawing.Size(1134, 584);
            this.flowLayoutPanelMain.TabIndex = 1;
            this.flowLayoutPanelMain.DoubleClick += new System.EventHandler(this.FlowLayoutPanelMain_DoubleClick);
            // 
            // labelHelp
            // 
            this.labelHelp.Enabled = false;
            this.labelHelp.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHelp.Location = new System.Drawing.Point(3, 27);
            this.labelHelp.Name = "labelHelp";
            this.labelHelp.Size = new System.Drawing.Size(1110, 536);
            this.labelHelp.TabIndex = 1;
            this.labelHelp.Text = "Scan an item or double click \r\nin the empty region to add an item\r\n\r\nPress Backsp" +
    "ace or delete to remove it\r\n\r\nPress Enter or click \r\n\'Generate\' to sort the item" +
    "s\"";
            this.labelHelp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.groupItemsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // groupItemsToolStripMenuItem
            // 
            this.groupItemsToolStripMenuItem.CheckOnClick = true;
            this.groupItemsToolStripMenuItem.Name = "groupItemsToolStripMenuItem";
            this.groupItemsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.groupItemsToolStripMenuItem.Text = "Group Items (G)";
            this.groupItemsToolStripMenuItem.Click += new System.EventHandler(this.groupItemsToolStripMenuItem_Click);
            // 
            // CategrizerForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1134, 611);
            this.Controls.Add(this.labelHelp);
            this.Controls.Add(this.flowLayoutPanelMain);
            this.Controls.Add(this.menuStrip1);
            this.Icon = global::WasteReducer.Properties.Resources.icon;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CategrizerForm";
            this.Text = "Product Category Suggester";
            this.Load += new System.EventHandler(this.Categorizer_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Categorizer_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Categorizer_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Categorizer_KeyUp);
            this.Resize += new System.EventHandler(this.Categorizer_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMain;
        private System.Windows.Forms.ToolStripMenuItem generateToolStripMenuItem;
        private System.Windows.Forms.Label labelHelp;
        private System.Windows.Forms.ToolStripMenuItem clearWorkspaceCtrlDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groupItemsToolStripMenuItem;
    }
}

