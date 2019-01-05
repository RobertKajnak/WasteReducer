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
    public partial class AddItemForm : Form
    {
        public long barcode = -1;
        public DateTime date;

        /// <summary>
        /// Handles adding a new item to the list of items.
        /// </summary>
        public AddItemForm()
        {
            InitializeComponent();
            labelToday.Text += DateTime.Today.ToString("dd.MM.yyyy");
            this.ActiveControl = textBox1;
            this.date = DateTime.Today.AddDays(2);
        }

       /// <summary>
       /// ON successs returns the barcode entered
       /// </summary>
        private void ReturnSuccess()
        {
            long.TryParse(this.textBox1.Text, out barcode);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ReturnFail()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            ReturnSuccess();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            ReturnFail();
        }


        

        private void radioButton0_KeyPress(object sender, KeyPressEventArgs e)
        {
            AddItemForm_KeyPress(sender, e);
        }

        private void radioButton1_KeyPress(object sender, KeyPressEventArgs e)
        {
            AddItemForm_KeyPress(sender, e);
        }


        ///Allows only digits to be entered
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                AddItemForm_KeyPress(sender, e);
            }

            
        }

        private void radioButton2_KeyPress(object sender, KeyPressEventArgs e)
        {
            AddItemForm_KeyPress(sender, e);
        }

        private void AddItemForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                ///esc
                case ((char)27):
                    ReturnFail();
                    break;
                ///enter
                case ((char)13):
                    ReturnSuccess();
                    break;
                default:
                    break;
            }
        }



        private void radioButton0_CheckedChanged(object sender, EventArgs e)
        {
            this.date = DateTime.Today;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.date = DateTime.Today.AddDays(1);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.date = DateTime.Today.AddDays(2);
        }
    }
}
