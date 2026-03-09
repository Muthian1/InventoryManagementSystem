using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagementSystem
{

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

           
            try
            {
                AuthService.EnsureUserTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing authentication: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var login = new LoginForm())
            {
                var result = login.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Application.Run(new Form1());
                }
            }
        }
    }
}
