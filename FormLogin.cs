using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormLogin : Form
    {
        public static int IdUsuarioActual;
        public static string NombreUsuario;
        public static int IdCuentaActual;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Sistema Bancario - Login";
            this.Size = new System.Drawing.Size(400, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblTitulo = new Label { Text = "SISTEMA BANCARIO", Location = new System.Drawing.Point(100, 20), Size = new System.Drawing.Size(200, 30), Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold) };
            Label lblUsuario = new Label { Text = "Usuario:", Location = new System.Drawing.Point(50, 70), Size = new System.Drawing.Size(80, 20) };
            TextBox txtUsuario = new TextBox { Name = "txtUsuario", Location = new System.Drawing.Point(140, 68), Size = new System.Drawing.Size(200, 20) };
            Label lblPassword = new Label { Text = "Contraseña:", Location = new System.Drawing.Point(50, 110), Size = new System.Drawing.Size(80, 20) };
            TextBox txtPassword = new TextBox { Name = "txtPassword", Location = new System.Drawing.Point(140, 108), Size = new System.Drawing.Size(200, 20), UseSystemPasswordChar = true };
            Button btnLogin = new Button { Text = "Ingresar", Location = new System.Drawing.Point(140, 150), Size = new System.Drawing.Size(100, 30) };
            Button btnSalir = new Button { Text = "Salir", Location = new System.Drawing.Point(250, 150), Size = new System.Drawing.Size(90, 30) };

            btnLogin.Click += (s, e) =>
            {
                string usuario = txtUsuario.Text.Trim();
                string password = txtPassword.Text;

                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Ingrese usuario y contraseña", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    string query = "SELECT id_usuario, nombre_completo FROM usuarios WHERE usuario = @user AND contraseña = @pass AND estatus = TRUE";
                    DataTable dt = Database.ExecuteQuery(query,
                        new NpgsqlParameter("@user", usuario),
                        new NpgsqlParameter("@pass", password));

                    if (dt.Rows.Count > 0)
                    {
                        IdUsuarioActual = Convert.ToInt32(dt.Rows[0]["id_usuario"]);
                        NombreUsuario = dt.Rows[0]["nombre_completo"].ToString();

                        // Obtener cuenta del usuario
                        string queryCuenta = "SELECT id_cuenta FROM cuentas WHERE id_usuario = @id";
                        DataTable dtCuenta = Database.ExecuteQuery(queryCuenta, new NpgsqlParameter("@id", IdUsuarioActual));
                        if (dtCuenta.Rows.Count > 0)
                        {
                            IdCuentaActual = Convert.ToInt32(dtCuenta.Rows[0]["id_cuenta"]);
                        }

                        this.Hide();
                        new FormMenu().ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnSalir.Click += (s, e) => Application.Exit();

            this.Controls.AddRange(new Control[] { lblTitulo, lblUsuario, txtUsuario, lblPassword, txtPassword, btnLogin, btnSalir });
        }
    }
}
