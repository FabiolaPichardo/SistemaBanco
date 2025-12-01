using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormRecuperacion : Form
    {
        private TextBox txtEmailUsuario;
        private Panel panelStep1;
        private Panel panelStep2;
        private TextBox txtToken;
        private TextBox txtNuevaPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkMostrarPassword;
        private Label lblPasswordStrength;
        private string tokenGenerado;
        private int idUsuarioRecuperacion;

        public FormRecuperacion()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Banco Premier - Recuperación de Contraseña";
            this.ClientSize = new System.Drawing.Size(600, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Header
            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(600, 100),
                BackColor = BankTheme.PrimaryBlue
            };

            Label lblLogo = new Label
            {
                Text = "🔐",
                Location = new System.Drawing.Point(260, 10),
                Size = new System.Drawing.Size(80, 40),
                Font = new System.Drawing.Font("Segoe UI", 32F),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Label lblTitulo = new Label
            {
                Text = "Recuperación de Contraseña",
                Location = new System.Drawing.Point(150, 55),
                Size = new System.Drawing.Size(300, 20),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Label lblSubtitulo = new Label
            {
                Text = "Recupera el acceso a tu cuenta",
                Location = new System.Drawing.Point(150, 75),
                Size = new System.Drawing.Size(300, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.AccentGold,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.AddRange(new Control[] { lblLogo, lblTitulo, lblSubtitulo });

            // Panel Paso 1: Solicitar email
            panelStep1 = BankTheme.CreateCard(50, 120, 500, 300);
            panelStep1.Visible = true;

            Label lblStep1Title = new Label
            {
                Text = "Paso 1: Verificación de Identidad",
                Location = new System.Drawing.Point(100, 20),
                Size = new System.Drawing.Size(300, 25),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            Label lblInstrucciones = new Label
            {
                Text = "Ingresa tu correo electrónico o nombre de usuario.\nTe enviaremos un código de verificación.",
                Location = new System.Drawing.Point(40, 60),
                Size = new System.Drawing.Size(420, 40),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            Label lblEmail = new Label
            {
                Text = "Correo Electrónico o Usuario",
                Location = new System.Drawing.Point(40, 120),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtEmailUsuario = new TextBox
            {
                Location = new System.Drawing.Point(40, 145),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F)
            };
            BankTheme.StyleTextBox(txtEmailUsuario);

            Button btnEnviarCodigo = new Button
            {
                Text = "ENVIAR CÓDIGO",
                Location = new System.Drawing.Point(150, 200),
                Size = new System.Drawing.Size(200, 45)
            };
            BankTheme.StyleButton(btnEnviarCodigo, true);
            btnEnviarCodigo.Click += BtnEnviarCodigo_Click;

            panelStep1.Controls.AddRange(new Control[] { lblStep1Title, lblInstrucciones, lblEmail, txtEmailUsuario, btnEnviarCodigo });

            // Panel Paso 2: Ingresar código y nueva contraseña
            panelStep2 = BankTheme.CreateCard(50, 120, 500, 450);
            panelStep2.Visible = false;

            Label lblStep2Title = new Label
            {
                Text = "Paso 2: Nueva Contraseña",
                Location = new System.Drawing.Point(130, 20),
                Size = new System.Drawing.Size(240, 25),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            Label lblToken = new Label
            {
                Text = "Código de Verificación (enviado por email)",
                Location = new System.Drawing.Point(40, 60),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtToken = new TextBox
            {
                Location = new System.Drawing.Point(40, 85),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                MaxLength = 6
            };
            BankTheme.StyleTextBox(txtToken);

            Label lblNuevaPassword = new Label
            {
                Text = "Nueva Contraseña",
                Location = new System.Drawing.Point(40, 135),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtNuevaPassword = new TextBox
            {
                Location = new System.Drawing.Point(40, 160),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                UseSystemPasswordChar = true
            };
            BankTheme.StyleTextBox(txtNuevaPassword);
            txtNuevaPassword.TextChanged += TxtNuevaPassword_TextChanged;

            lblPasswordStrength = new Label
            {
                Location = new System.Drawing.Point(40, 193),
                Size = new System.Drawing.Size(420, 15),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };

            Label lblConfirmPassword = new Label
            {
                Text = "Confirmar Nueva Contraseña",
                Location = new System.Drawing.Point(40, 215),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtConfirmPassword = new TextBox
            {
                Location = new System.Drawing.Point(40, 240),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                UseSystemPasswordChar = true
            };
            BankTheme.StyleTextBox(txtConfirmPassword);

            chkMostrarPassword = new CheckBox
            {
                Text = "Mostrar contraseñas",
                Location = new System.Drawing.Point(40, 280),
                Size = new System.Drawing.Size(200, 20),
                Font = BankTheme.SmallFont
            };
            chkMostrarPassword.CheckedChanged += (s, e) =>
            {
                txtNuevaPassword.UseSystemPasswordChar = !chkMostrarPassword.Checked;
                txtConfirmPassword.UseSystemPasswordChar = !chkMostrarPassword.Checked;
            };

            Button btnCambiarPassword = new Button
            {
                Text = "CAMBIAR CONTRASEÑA",
                Location = new System.Drawing.Point(125, 330),
                Size = new System.Drawing.Size(250, 45)
            };
            BankTheme.StyleButton(btnCambiarPassword, true);
            btnCambiarPassword.Click += BtnCambiarPassword_Click;

            panelStep2.Controls.AddRange(new Control[] {
                lblStep2Title, lblToken, txtToken, lblNuevaPassword, txtNuevaPassword,
                lblPasswordStrength, lblConfirmPassword, txtConfirmPassword,
                chkMostrarPassword, btnCambiarPassword
            });

            // Botón volver
            LinkLabel linkVolver = new LinkLabel
            {
                Text = "← Volver al inicio de sesión",
                Location = new System.Drawing.Point(200, 580),
                Size = new System.Drawing.Size(200, 20),
                Font = BankTheme.SmallFont,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                LinkColor = BankTheme.PrimaryBlue
            };
            linkVolver.LinkClicked += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, panelStep1, panelStep2, linkVolver });
        }

        private void BtnEnviarCodigo_Click(object sender, EventArgs e)
        {
            string emailUsuario = txtEmailUsuario.Text.Trim();

            if (string.IsNullOrEmpty(emailUsuario))
            {
                MessageBox.Show("Ingrese su correo electrónico o nombre de usuario", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Buscar usuario por email o nombre de usuario
                string query = "SELECT id_usuario, email, nombre_completo FROM usuarios WHERE email = @input OR usuario = @input";
                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@input", emailUsuario));

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontró ninguna cuenta con ese correo o usuario", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                idUsuarioRecuperacion = Convert.ToInt32(dt.Rows[0]["id_usuario"]);
                string email = dt.Rows[0]["email"].ToString();
                string nombre = dt.Rows[0]["nombre_completo"].ToString();

                // Generar token de 6 dígitos
                Random random = new Random();
                tokenGenerado = random.Next(100000, 999999).ToString();

                // Guardar token en base de datos
                string queryToken = @"INSERT INTO tokens_recuperacion (id_usuario, token, fecha_expiracion) 
                                     VALUES (@id, @token, @expira)";
                Database.ExecuteNonQuery(queryToken,
                    new NpgsqlParameter("@id", idUsuarioRecuperacion),
                    new NpgsqlParameter("@token", tokenGenerado),
                    new NpgsqlParameter("@expira", DateTime.Now.AddMinutes(15)));

                // En producción, aquí se enviaría el email
                // Por ahora, mostrar el código en un mensaje
                MessageBox.Show($"Código de verificación generado:\n\n{tokenGenerado}\n\n(En producción se enviaría a: {email})\n\nEl código expira en 15 minutos.",
                    "Código Generado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Cambiar a paso 2
                panelStep1.Visible = false;
                panelStep2.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtNuevaPassword_TextChanged(object sender, EventArgs e)
        {
            string password = txtNuevaPassword.Text;
            if (string.IsNullOrEmpty(password))
            {
                lblPasswordStrength.Text = "";
                return;
            }

            int strength = 0;
            if (password.Length >= 8) strength++;
            if (Regex.IsMatch(password, @"[a-z]")) strength++;
            if (Regex.IsMatch(password, @"[A-Z]")) strength++;
            if (Regex.IsMatch(password, @"[0-9]")) strength++;
            if (Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]")) strength++;

            switch (strength)
            {
                case 0:
                case 1:
                    lblPasswordStrength.Text = "Seguridad: Muy débil";
                    lblPasswordStrength.ForeColor = BankTheme.Danger;
                    break;
                case 2:
                    lblPasswordStrength.Text = "Seguridad: Débil";
                    lblPasswordStrength.ForeColor = BankTheme.Warning;
                    break;
                case 3:
                    lblPasswordStrength.Text = "Seguridad: Media";
                    lblPasswordStrength.ForeColor = BankTheme.Warning;
                    break;
                case 4:
                    lblPasswordStrength.Text = "Seguridad: Fuerte";
                    lblPasswordStrength.ForeColor = BankTheme.Success;
                    break;
                case 5:
                    lblPasswordStrength.Text = "Seguridad: Muy fuerte";
                    lblPasswordStrength.ForeColor = BankTheme.Success;
                    break;
            }
        }

        private void BtnCambiarPassword_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtToken.Text))
            {
                MessageBox.Show("Ingrese el código de verificación", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNuevaPassword.Text))
            {
                MessageBox.Show("Ingrese la nueva contraseña", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNuevaPassword.Text.Length < 8)
            {
                MessageBox.Show("La contraseña debe tener al menos 8 caracteres", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNuevaPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Las contraseñas no coinciden", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Verificar token
                string queryToken = @"SELECT id_token FROM tokens_recuperacion 
                                     WHERE id_usuario = @id AND token = @token 
                                     AND fecha_expiracion > @ahora AND usado = FALSE";
                DataTable dt = Database.ExecuteQuery(queryToken,
                    new NpgsqlParameter("@id", idUsuarioRecuperacion),
                    new NpgsqlParameter("@token", txtToken.Text.Trim()),
                    new NpgsqlParameter("@ahora", DateTime.Now));

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Código de verificación inválido o expirado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idToken = Convert.ToInt32(dt.Rows[0]["id_token"]);

                // Actualizar contraseña
                string queryUpdate = "UPDATE usuarios SET contraseña = @pass WHERE id_usuario = @id";
                Database.ExecuteNonQuery(queryUpdate,
                    new NpgsqlParameter("@pass", txtNuevaPassword.Text), // En producción usar hash
                    new NpgsqlParameter("@id", idUsuarioRecuperacion));

                // Marcar token como usado
                string queryUsado = "UPDATE tokens_recuperacion SET usado = TRUE WHERE id_token = @id";
                Database.ExecuteNonQuery(queryUsado, new NpgsqlParameter("@id", idToken));

                MessageBox.Show("¡Contraseña cambiada exitosamente!\n\nYa puedes iniciar sesión con tu nueva contraseña.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
