using System;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public class SignupForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirm;
        private Button btnCreate;

        public SignupForm()
        {
            Text = "Sign up";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(360, 220);

            var lblUser = new Label { Text = "Username:", Location = new Point(12, 18), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(120, 14), Width = 220 };

            var lblPass = new Label { Text = "Password:", Location = new Point(12, 58), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(120, 54), Width = 220, UseSystemPasswordChar = true };

            var lblConfirm = new Label { Text = "Confirm:", Location = new Point(12, 98), AutoSize = true };
            txtConfirm = new TextBox { Location = new Point(120, 94), Width = 220, UseSystemPasswordChar = true };

            btnCreate = new Button { Text = "Create account", Location = new Point(120, 140), Width = 220 };
            btnCreate.Click += BtnCreate_Click;

            Controls.AddRange(new Control[] { lblUser, txtUsername, lblPass, txtPassword, lblConfirm, txtConfirm, btnCreate });
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            var user = txtUsername.Text.Trim();
            var p1 = txtPassword.Text;
            var p2 = txtConfirm.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(p1))
            {
                MessageBox.Show("Enter username and password.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (p1 != p2)
            {
                MessageBox.Show("Passwords do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (AuthService.CreateUser(user, p1, out var error))
                {
                    MessageBox.Show("Account created. You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("Sign up failed: " + error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sign up error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SignupForm
            // 
            this.ClientSize = new System.Drawing.Size(996, 657);
            this.Name = "SignupForm";
            this.ResumeLayout(false);

        }
    }
}