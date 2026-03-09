using System;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnSignup;

        public LoginForm()
        {
            Text = "Sign in";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(320, 180);

            var lblUser = new Label { Text = "Username:", Location = new Point(12, 18), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(100, 14), Width = 200, Name = "txtUsername" };

            var lblPass = new Label { Text = "Password:", Location = new Point(12, 58), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(100, 54), Width = 200, UseSystemPasswordChar = true, Name = "txtPassword" };

            btnLogin = new Button { Text = "Login", Location = new Point(100, 100), Width = 95 };
            btnSignup = new Button { Text = "Sign up", Location = new Point(205, 100), Width = 95 };

            btnLogin.Click += BtnLogin_Click;
            btnSignup.Click += BtnSignup_Click;

            Controls.AddRange(new Control[] { lblUser, txtUsername, lblPass, txtPassword, btnLogin, btnSignup });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var user = txtUsername.Text.Trim();
            var pass = txtPassword.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Enter username and password.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (AuthService.ValidateUser(user, pass))
                {
                    DialogResult = DialogResult.OK; // signal success to Program.Main
                    Close();
                }
                else
                {
                    MessageBox.Show("Invalid credentials.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSignup_Click(object sender, EventArgs e)
        {
            using (var signup = new SignupForm())
            {
                signup.ShowDialog(this);
            }
        }
    }
}