using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormRegistro : Form
    {
        private TextBox txtUsuario;
        private TextBox txtNombreCompleto;
        private TextBox txtEmail;
        private TextBox txtTelefono;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkMostrarPassword;
        private Label lblPasswordStrength;
        private Label lblUsuarioValidacion;
        private Label lblEmailValidacion;

        public FormRegistro()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Banco Premier - Registro de Usuario";
            this.ClientSize = new System.Drawing.Size(600, 750);
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
                Text = "🏦",
                Location = new System.Drawing.Point(260, 10),
                Size = new System.Drawing.Size(80, 40),
                Font = new System.Drawing.Font("Segoe UI", 32F),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Label lblTitulo = new Label
            {
                Text = "Inicio de Sesión",
                Location = new System.Drawing.Point(150, 55),
                Size = new System.Drawing.Size(300, 20),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Label lblSubtitulo = new Label
            {
                Text = "Accede a tu cuenta de Banco",
                Location = new System.Drawing.Point(150, 75),
                Size = new System.Drawing.Size(300, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.AccentGold,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.AddRange(new Control[] { lblLogo, lblTitulo, lblSubtitulo });

            // Card principal
            Panel mainCard = BankTheme.CreateCard(50, 120, 500, 550);

            Label lblFormTitle = new Label
            {
                Text = "Crear Nueva Cuenta",
                Location = new System.Drawing.Point(150, 20),
                Size = new System.Drawing.Size(200, 25),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            // Usuario
            Label lblUsuario = new Label
            {
                Text = "Nombre de Usuario (máx. 20 caracteres)",
                Location = new System.Drawing.Point(40, 60),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtUsuario = new TextBox
            {
                Location = new System.Drawing.Point(40, 85),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                MaxLength = 20
            };
            BankTheme.StyleTextBox(txtUsuario);
            txtUsuario.TextChanged += TxtUsuario_TextChanged;

            lblUsuarioValidacion = new Label
            {
                Location = new System.Drawing.Point(40, 118),
                Size = new System.Drawing.Size(420, 15),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };

            // Nombre completo
            Label lblNombre = new Label
            {
                Text = "Nombre Completo",
                Location = new System.Drawing.Point(40, 140),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtNombreCompleto = new TextBox
            {
                Location = new System.Drawing.Point(40, 165),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F)
            };
            BankTheme.StyleTextBox(txtNombreCompleto);

            // Email
            Label lblEmail = new Label
            {
                Text = "Correo Electrónico",
                Location = new System.Drawing.Point(40, 205),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtEmail = new TextBox
            {
                Location = new System.Drawing.Point(40, 230),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F)
            };
            BankTheme.StyleTextBox(txtEmail);
            txtEmail.Leave += TxtEmail_Leave;

            lblEmailValidacion = new Label
            {
                Location = new System.Drawing.Point(40, 263),
                Size = new System.Drawing.Size(420, 15),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };

            // Teléfono
            Label lblTelefono = new Label
            {
                Text = "Teléfono (opcional)",
                Location = new System.Drawing.Point(40, 285),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtTelefono = new TextBox
            {
                Location = new System.Drawing.Point(40, 310),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F)
            };
            BankTheme.StyleTextBox(txtTelefono);

            // Contraseña
            Label lblPassword = new Label
            {
                Text = "Contraseña",
                Location = new System.Drawing.Point(40, 350),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtPassword = new TextBox
            {
                Location = new System.Drawing.Point(40, 375),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                UseSystemPasswordChar = true
            };
            BankTheme.StyleTextBox(txtPassword);
            txtPassword.TextChanged += TxtPassword_TextChanged;

            lblPasswordStrength = new Label
            {
                Location = new System.Drawing.Point(40, 408),
                Size = new System.Drawing.Size(420, 15),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };

            // Confirmar contraseña
            Label lblConfirmPassword = new Label
            {
                Text = "Confirmar Contraseña",
                Location = new System.Drawing.Point(40, 430),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtConfirmPassword = new TextBox
            {
                Location = new System.Drawing.Point(40, 455),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                UseSystemPasswordChar = true
            };
            BankTheme.StyleTextBox(txtConfirmPassword);

            // Mostrar contraseña
            chkMostrarPassword = new CheckBox
            {
                Text = "Mostrar contraseñas",
                Location = new System.Drawing.Point(40, 495),
                Size = new System.Drawing.Size(200, 20),
                Font = BankTheme.SmallFont
            };
            chkMostrarPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !chkMostrarPassword.Checked;
                txtConfirmPassword.UseSystemPasswordChar = !chkMostrarPassword.Checked;
            };

            mainCard.Controls.AddRange(new Control[] {
                lblFormTitle, lblUsuario, txtUsuario, lblUsuarioValidacion,
                lblNombre, txtNombreCompleto, lblEmail, txtEmail, lblEmailValidacion,
                lblTelefono, txtTelefono, lblPassword, txtPassword, lblPasswordStrength,
                lblConfirmPassword, txtConfirmPassword, chkMostrarPassword
            });

            // Botones
            Button btnRegistrar = new Button
            {
                Text = "CREAR CUENTA",
                Location = new System.Drawing.Point(175, 690),
                Size = new System.Drawing.Size(250, 45)
            };
            BankTheme.StyleButton(btnRegistrar, true);
            btnRegistrar.Click += BtnRegistrar_Click;

            LinkLabel linkLogin = new LinkLabel
            {
                Text = "¿Ya tienes cuenta? Inicia sesión",
                Location = new System.Drawing.Point(175, 740),
                Size = new System.Drawing.Size(250, 20),
                Font = BankTheme.SmallFont,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                LinkColor = BankTheme.PrimaryBlue
            };
            linkLogin.LinkClicked += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, mainCard, btnRegistrar, linkLogin });
        }

        private void TxtUsuario_TextChanged(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            if (string.IsNullOrEmpty(usuario))
            {
                lblUsuarioValidacion.Text = "";
                return;
            }

            if (usuario.Length > 20)
            {
                lblUsuarioValidacion.Text = "✗ Máximo 20 caracteres";
                lblUsuarioValidacion.ForeColor = BankTheme.Danger;
                return;
            }

            // Validar caracteres permitidos
            if (!Regex.IsMatch(usuario, @"^[a-zA-Z0-9_\-\.]+$"))
            {
                lblUsuarioValidacion.Text = "✗ Solo letras, números, _, - y .";
                lblUsuarioValidacion.ForeColor = BankTheme.Danger;
                return;
            }

            lblUsuarioValidacion.Text = $"✓ {usuario.Length}/20 caracteres";
            lblUsuarioValidacion.ForeColor = BankTheme.Success;
        }

        private void TxtEmail_Leave(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                lblEmailValidacion.Text = "";
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                lblEmailValidacion.Text = "✗ Email inválido";
                lblEmailValidacion.ForeColor = BankTheme.Danger;
                return;
            }

            lblEmailValidacion.Text = "✓ Email válido";
            lblEmailValidacion.ForeColor = BankTheme.Success;
        }

        private void TxtPassword_TextChanged(object sender, EventArgs e)
        {
            string password = txtPassword.Text;
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

        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("Ingrese un nombre de usuario", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            if (txtUsuario.Text.Length > 20)
            {
                MessageBox.Show("El nombre de usuario no puede exceder 20 caracteres", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombreCompleto.Text))
            {
                MessageBox.Show("Ingrese su nombre completo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreCompleto.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Ingrese su correo electrónico", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Ingrese un correo electrónico válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Ingrese una contraseña", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text.Length < 8)
            {
                MessageBox.Show("La contraseña debe tener al menos 8 caracteres", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Las contraseñas no coinciden", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                // Verificar si el usuario ya existe
                string queryCheck = "SELECT COUNT(*) FROM usuarios WHERE usuario = @user OR email = @email";
                DataTable dtCheck = Database.ExecuteQuery(queryCheck,
                    new NpgsqlParameter("@user", txtUsuario.Text.Trim()),
                    new NpgsqlParameter("@email", txtEmail.Text.Trim()));

                if (Convert.ToInt32(dtCheck.Rows[0][0]) > 0)
                {
                    MessageBox.Show("El usuario o correo electrónico ya está registrado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Insertar nuevo usuario
                string queryInsert = @"INSERT INTO usuarios (usuario, contraseña, nombre_completo, email, telefono) 
                                      VALUES (@user, @pass, @nombre, @email, @tel) RETURNING id_usuario";
                
                DataTable dtResult = Database.ExecuteQuery(queryInsert,
                    new NpgsqlParameter("@user", txtUsuario.Text.Trim()),
                    new NpgsqlParameter("@pass", txtPassword.Text), // En producción usar hash
                    new NpgsqlParameter("@nombre", txtNombreCompleto.Text.Trim()),
                    new NpgsqlParameter("@email", txtEmail.Text.Trim()),
                    new NpgsqlParameter("@tel", string.IsNullOrWhiteSpace(txtTelefono.Text) ? DBNull.Value : (object)txtTelefono.Text.Trim()));

                int idUsuario = Convert.ToInt32(dtResult.Rows[0][0]);

                // Crear cuenta bancaria automáticamente
                string numeroCuenta = "100" + DateTime.Now.ToString("yyyyMMddHHmmss");
                string queryAccount = @"INSERT INTO cuentas (id_usuario, numero_cuenta, tipo_cuenta, saldo) 
                                       VALUES (@id, @numero, 'AHORRO', 0.00)";
                Database.ExecuteNonQuery(queryAccount,
                    new NpgsqlParameter("@id", idUsuario),
                    new NpgsqlParameter("@numero", numeroCuenta));

                MessageBox.Show($"¡Cuenta creada exitosamente!\n\nUsuario: {txtUsuario.Text}\nNúmero de cuenta: {numeroCuenta}\n\nYa puedes iniciar sesión.",
                    "Registro Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la cuenta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
