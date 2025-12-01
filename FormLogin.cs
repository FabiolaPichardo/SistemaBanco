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

        private CheckBox chkMostrarPassword;
        private Button btnLogin;

        private void InitializeComponent()
        {
            this.Text = "Módulo de Banco - Banco Premier";
            this.ClientSize = new System.Drawing.Size(500, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = BankTheme.LightGray;

            // Panel superior con logo y título
            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(500, 150),
                BackColor = BankTheme.PrimaryBlue
            };

            Label lblLogo = new Label
            {
                Text = "🏦",
                Location = new System.Drawing.Point(210, 20),
                Size = new System.Drawing.Size(80, 60),
                Font = new System.Drawing.Font("Segoe UI", 48F),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Label lblTitulo = new Label
            {
                Text = "BANCO PREMIER",
                Location = new System.Drawing.Point(100, 85),
                Size = new System.Drawing.Size(300, 35),
                Font = BankTheme.TitleFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Label lblSubtitulo = new Label
            {
                Text = "Banca Digital Segura",
                Location = new System.Drawing.Point(100, 115),
                Size = new System.Drawing.Size(300, 25),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.AccentGold,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.AddRange(new Control[] { lblLogo, lblTitulo, lblSubtitulo });

            // Panel de login (card)
            Panel loginCard = BankTheme.CreateCard(50, 180, 400, 430);

            Label lblLoginTitle = new Label
            {
                Text = "Inicio de Sesión",
                Location = new System.Drawing.Point(120, 20),
                Size = new System.Drawing.Size(160, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            Label lblLoginSubtitle = new Label
            {
                Text = "Accede a tu cuenta de Banco",
                Location = new System.Drawing.Point(100, 50),
                Size = new System.Drawing.Size(200, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Label lblUsuario = new Label
            {
                Text = "Nombre de usuario (máx. 20 caracteres)",
                Location = new System.Drawing.Point(40, 90),
                Size = new System.Drawing.Size(320, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            TextBox txtUsuario = new TextBox
            {
                Name = "txtUsuario",
                Location = new System.Drawing.Point(40, 115),
                Size = new System.Drawing.Size(320, 35),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                MaxLength = 20
            };
            BankTheme.StyleTextBox(txtUsuario);

            Label lblPassword = new Label
            {
                Text = "Contraseña",
                Location = new System.Drawing.Point(40, 165),
                Size = new System.Drawing.Size(280, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            // Crear TextBox de contraseña
            TextBox txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(280, 35),
                UseSystemPasswordChar = true,
                Font = new System.Drawing.Font("Segoe UI", 11F),
                BorderStyle = BorderStyle.None
            };

            Panel passwordPanel = new Panel
            {
                Location = new System.Drawing.Point(40, 190),
                Size = new System.Drawing.Size(320, 35),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Asignar eventos después de declarar ambos controles
            txtUsuario.TextChanged += (s, e) => ValidarCampos(txtUsuario, txtPassword, btnLogin);
            txtPassword.TextChanged += (s, e) => ValidarCampos(txtUsuario, txtPassword, btnLogin);

            Button btnTogglePassword = new Button
            {
                Text = "👁",
                Location = new System.Drawing.Point(282, 0),
                Size = new System.Drawing.Size(38, 33),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new System.Drawing.Font("Segoe UI", 12F)
            };
            btnTogglePassword.FlatAppearance.BorderSize = 0;
            btnTogglePassword.Click += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
                btnTogglePassword.Text = txtPassword.UseSystemPasswordChar ? "👁" : "🙈";
            };

            passwordPanel.Controls.AddRange(new Control[] { txtPassword, btnTogglePassword });

            // Checkbox mostrar contraseña (alternativa)
            chkMostrarPassword = new CheckBox
            {
                Text = "Mostrar contraseña",
                Location = new System.Drawing.Point(40, 230),
                Size = new System.Drawing.Size(150, 20),
                Font = BankTheme.SmallFont,
                Visible = false // Usar el botón de ojo en su lugar
            };

            // Link recuperar contraseña
            LinkLabel linkRecuperar = new LinkLabel
            {
                Text = "¿Olvidaste tu contraseña?",
                Location = new System.Drawing.Point(220, 230),
                Size = new System.Drawing.Size(140, 20),
                Font = BankTheme.SmallFont,
                TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                LinkColor = BankTheme.PrimaryBlue
            };
            linkRecuperar.LinkClicked += (s, e) =>
            {
                FormRecuperacion formRecup = new FormRecuperacion();
                formRecup.ShowDialog();
            };

            btnLogin = new Button
            {
                Text = "CONTINUAR",
                Location = new System.Drawing.Point(40, 270),
                Size = new System.Drawing.Size(320, 45),
                Enabled = false
            };
            BankTheme.StyleButton(btnLogin, true);

            Button btnRegistrar = new Button
            {
                Text = "REGISTRARSE",
                Location = new System.Drawing.Point(40, 325),
                Size = new System.Drawing.Size(320, 40)
            };
            BankTheme.StyleButton(btnRegistrar, false);
            btnRegistrar.Click += (s, e) =>
            {
                FormRegistro formReg = new FormRegistro();
                formReg.ShowDialog();
            };

            Button btnSalir = new Button
            {
                Text = "Salir",
                Location = new System.Drawing.Point(40, 375),
                Size = new System.Drawing.Size(320, 35),
                BackColor = System.Drawing.Color.FromArgb(100, 100, 100),
                FlatStyle = FlatStyle.Flat
            };
            btnSalir.FlatAppearance.BorderSize = 0;
            btnSalir.Font = BankTheme.BodyFont;
            btnSalir.ForeColor = BankTheme.White;
            btnSalir.Cursor = Cursors.Hand;

            loginCard.Controls.AddRange(new Control[] {
                lblLoginTitle, lblLoginSubtitle, lblUsuario, txtUsuario,
                lblPassword, passwordPanel, linkRecuperar,
                btnLogin, btnRegistrar, btnSalir
            });

            // Footer
            Label lblFooter = new Label
            {
                Text = "© 2025 Banco Premier. Todos los derechos reservados.",
                Location = new System.Drawing.Point(50, 550),
                Size = new System.Drawing.Size(400, 30),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Eventos
            txtPassword.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    btnLogin.PerformClick();
                    e.Handled = true;
                }
            };

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
                    // Verificar si el usuario existe
                    string queryUsuario = "SELECT id_usuario, contraseña, nombre_completo, estatus, bloqueado_hasta, intentos_fallidos FROM usuarios WHERE usuario = @user";
                    DataTable dtUsuario = Database.ExecuteQuery(queryUsuario, new NpgsqlParameter("@user", usuario));

                    if (dtUsuario.Rows.Count == 0)
                    {
                        MessageBox.Show("Usuario no encontrado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int idUsuario = Convert.ToInt32(dtUsuario.Rows[0]["id_usuario"]);
                    string passwordDB = dtUsuario.Rows[0]["contraseña"].ToString();
                    string nombreCompleto = dtUsuario.Rows[0]["nombre_completo"].ToString();
                    bool estatus = Convert.ToBoolean(dtUsuario.Rows[0]["estatus"]);
                    int intentosFallidos = Convert.ToInt32(dtUsuario.Rows[0]["intentos_fallidos"]);
                    object bloqueadoHasta = dtUsuario.Rows[0]["bloqueado_hasta"];

                    // Verificar si está bloqueado
                    if (bloqueadoHasta != DBNull.Value)
                    {
                        DateTime fechaBloqueo = Convert.ToDateTime(bloqueadoHasta);
                        if (DateTime.Now < fechaBloqueo)
                        {
                            TimeSpan tiempoRestante = fechaBloqueo - DateTime.Now;
                            MessageBox.Show($"Cuenta bloqueada temporalmente.\nIntente nuevamente en {tiempoRestante.Minutes} minutos.",
                                "Cuenta Bloqueada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else
                        {
                            // Desbloquear cuenta
                            Database.ExecuteNonQuery("UPDATE usuarios SET bloqueado_hasta = NULL, intentos_fallidos = 0 WHERE id_usuario = @id",
                                new NpgsqlParameter("@id", idUsuario));
                            intentosFallidos = 0;
                        }
                    }

                    // Verificar si está activo
                    if (!estatus)
                    {
                        MessageBox.Show("Cuenta desactivada. Contacte al administrador.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Verificar contraseña
                    if (password != passwordDB)
                    {
                        intentosFallidos++;
                        
                        if (intentosFallidos >= 3)
                        {
                            // Bloquear cuenta por 15 minutos
                            DateTime bloqueadoHastaFecha = DateTime.Now.AddMinutes(15);
                            Database.ExecuteNonQuery(@"UPDATE usuarios SET intentos_fallidos = @intentos, bloqueado_hasta = @hasta 
                                                      WHERE id_usuario = @id",
                                new NpgsqlParameter("@intentos", intentosFallidos),
                                new NpgsqlParameter("@hasta", bloqueadoHastaFecha),
                                new NpgsqlParameter("@id", idUsuario));

                            MessageBox.Show("Demasiados intentos fallidos.\nSu cuenta ha sido bloqueada por 15 minutos.",
                                "Cuenta Bloqueada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            Database.ExecuteNonQuery("UPDATE usuarios SET intentos_fallidos = @intentos WHERE id_usuario = @id",
                                new NpgsqlParameter("@intentos", intentosFallidos),
                                new NpgsqlParameter("@id", idUsuario));

                            MessageBox.Show($"Contraseña incorrecta\n\nIntentos restantes: {3 - intentosFallidos}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }

                    // Login exitoso - Resetear intentos fallidos
                    Database.ExecuteNonQuery(@"UPDATE usuarios SET intentos_fallidos = 0, ultima_sesion = @ahora 
                                              WHERE id_usuario = @id",
                        new NpgsqlParameter("@ahora", DateTime.Now),
                        new NpgsqlParameter("@id", idUsuario));

                    IdUsuarioActual = idUsuario;
                    NombreUsuario = nombreCompleto;

                    // Obtener cuenta del usuario
                    string queryCuenta = "SELECT id_cuenta FROM cuentas WHERE id_usuario = @id";
                    DataTable dtCuenta = Database.ExecuteQuery(queryCuenta, new NpgsqlParameter("@id", IdUsuarioActual));
                    if (dtCuenta.Rows.Count > 0)
                    {
                        IdCuentaActual = Convert.ToInt32(dtCuenta.Rows[0]["id_cuenta"]);
                    }

                    this.Hide();
                    FormMenu menuForm = new FormMenu();
                    menuForm.ShowDialog();
                    
                    // Limpiar campos al volver del menú
                    txtUsuario.Text = "";
                    txtPassword.Text = "";
                    btnLogin.Enabled = false;
                    this.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnSalir.Click += (s, e) => Application.Exit();

            this.Controls.AddRange(new Control[] { headerPanel, loginCard, lblFooter });
        }

        private void ValidarCampos(TextBox txtUsuario, TextBox txtPassword, Button btnLogin)
        {
            // Habilitar botón solo si ambos campos tienen contenido
            btnLogin.Enabled = !string.IsNullOrWhiteSpace(txtUsuario.Text) &&
                              !string.IsNullOrWhiteSpace(txtPassword.Text);
        }
    }
}
