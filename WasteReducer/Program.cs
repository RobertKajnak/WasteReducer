using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WasteReducer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            /*try
            {*/
                Application.Run(new CategrizerForm());
            /*}
            catch (Exception e)
            {
                MessageBox.Show(e.Message+ "\n\nStacktrace:\n"+e.StackTrace, "An Error has occured");
            }*/
            
        }
    }
}
