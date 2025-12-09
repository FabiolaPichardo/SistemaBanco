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
        private ComboBox cmbRol;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkMostrarPassword;
        private ComboBox cmbPregunta1;
        private TextBox txtRespuesta1;
        private ComboBox cmbPregunta2;
        private TextBox txtRespuesta2;
        private ComboBox cmbPregunta3;
        private TextBox txtRespuesta3;
        private Label lblUsuarioValidacion;
        private Label lblEmailValidacion;
        private Label lblPasswordValidacion;

        private readonly string[] preguntasSeguridad = new string[]
        {
            "Cual es el nombre de tu primera mascota",
            "En que ciudad naciste",
            "Cual es tu color favorito",
            "Cual es el nombre de tu mejor amigo de la infancia",
            "Cual es tu comida favorita",
            "En que escuela estudiaste la primaria",
            "Cual fue tu primera escuela"
        };

        public FormRegistro()
        {
            InitializeComponent();
            IconHelper.SetFormIcon(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Registro de Usuario";
            this.ClientSize = new System.Drawing.Size(700, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.AutoScroll = true;

            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(700, 100),
                BackColor = BankTheme.PrimaryBlue
            };

            Button btnRegresar = new Button
            {
                Text = "←",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(50, 50),
                Font = new System.Drawing.Font("Segoe UI", 24F),
                BackColor = BankTheme.PrimaryBlue,
                ForeColor = BankTheme.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegresar.FlatAppearance.BorderSize = 0;
            btnRegresar.Click += (s, e) => this.Close();

            Label lblLogo = new Label
            {
                Text = "🏦",
                Location = new System.Drawing.Point(310, 10),
                Size = new System.Drawing.Size(80, 40),
                Font = new System.Drawing.Font("Segoe UI", 32F),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Label lblTitulo = new Label
            {
                Text = "MÓDULO BANCO",
                Location = new System.Drawing.Point(200, 55),
                Size = new System.Drawing.Size(300, 20),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Label lblSubtitulo = new Label
            {
                Text = "Registro de Nueva Cuenta",
                Location = new System.Drawing.Point(200, 75),
                Size = new System.Drawing.Size(300, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.AccentGold,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.AddRange(new Control[] { btnRegresar, lblLogo, lblTitulo, lblSubtitulo });

            Panel mainCard = BankTheme.CreateCard(50, 120, 600, 600);
            mainCard.AutoScroll = true;

            int yPos = 20;

            Label lblFormTitle = new Label
            {
                Text = "Crear Nueva Cuenta",
                Location = new System.Drawing.Point(200, yPos),
                Size = new System.Drawing.Size(200, 25),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.PrimaryBlue
            };
            yPos += 40;

            Label lblEmail = new Label
            {
                Text = "Correo Electronico",
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            txtEmail = new TextBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F)
            };
            BankTheme.StyleTextBox(txtEmail);
            txtEmail.Leave += TxtEmail_Leave;
            yPos += 35;

            lblEmailValidacion = new Label
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 15),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            Label lblRol = new Label
            {
                Text = "Rol en la Empresa",
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            cmbRol = new ComboBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRol.Items.AddRange(new string[] { "Cliente", "Cajero", "Ejecutivo", "Gerente", "Administrador" });
            cmbRol.SelectedIndex = 0;
            yPos += 45;

            Label lblNombre = new Label
            {
                Text = "Nombre Completo",
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            txtNombreCompleto = new TextBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F)
            };
            BankTheme.StyleTextBox(txtNombreCompleto);
            yPos += 45;

            Label lblUsuario = new Label
            {
                Text = "Nombre de Usuario (8-20 caracteres, letras, numeros y simbolos)",
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            txtUsuario = new TextBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                MaxLength = 20
            };
            BankTheme.StyleTextBox(txtUsuario);
            txtUsuario.TextChanged += TxtUsuario_TextChanged;
            yPos += 35;

            lblUsuarioValidacion = new Label
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 15),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            Label lblPassword = new Label
            {
                Text = "Contraseña (8-20 caracteres, mayusculas, minusculas, numeros y simbolos)",
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            txtPassword = new TextBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                UseSystemPasswordChar = true,
                MaxLength = 20
            };
            BankTheme.StyleTextBox(txtPassword);
            txtPassword.TextChanged += TxtPassword_TextChanged;
            yPos += 35;

            lblPasswordValidacion = new Label
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 15),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            Label lblConfirmPassword = new Label
            {
                Text = "Confirmar Contraseña",
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            txtConfirmPassword = new TextBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                UseSystemPasswordChar = true,
                MaxLength = 20
            };
            BankTheme.StyleTextBox(txtConfirmPassword);
            yPos += 40;

            chkMostrarPassword = new CheckBox
            {
                Text = "Mostrar contraseñas",
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(200, 20),
                Font = BankTheme.SmallFont
            };
            chkMostrarPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !chkMostrarPassword.Checked;
                txtConfirmPassword.UseSystemPasswordChar = !chkMostrarPassword.Checked;
            };
            yPos += 40;

            Label lblSeguridadTitle = new Label
            {
                Text = "Preguntas de Seguridad",
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 25),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.PrimaryBlue
            };
            yPos += 35;

            Label lblPregunta1 = new Label
            {
                Text = "Pregunta de Seguridad 1",
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            cmbPregunta1 = new ComboBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPregunta1.Items.AddRange(preguntasSeguridad);
            cmbPregunta1.SelectedIndex = 0;
            yPos += 35;

            txtRespuesta1 = new TextBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F)
            };
            BankTheme.StyleTextBox(txtRespuesta1);
            yPos += 45;

            Label lblPregunta2 = new Label
            {
                Text = "Pregunta de Seguridad 2",
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            cmbPregunta2 = new ComboBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPregunta2.Items.AddRange(preguntasSeguridad);
            cmbPregunta2.SelectedIndex = 1;
            yPos += 35;

            txtRespuesta2 = new TextBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F)
            };
            BankTheme.StyleTextBox(txtRespuesta2);
            yPos += 45;

            Label lblPregunta3 = new Label
            {
                Text = "Pregunta de Seguridad 3",
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };
            yPos += 25;

            cmbPregunta3 = new ComboBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPregunta3.Items.AddRange(preguntasSeguridad);
            cmbPregunta3.SelectedIndex = 2;
            yPos += 35;

            txtRespuesta3 = new TextBox
            {
                Location = new System.Drawing.Point(40, yPos),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F)
            };
            BankTheme.StyleTextBox(txtRespuesta3);

            mainCard.Controls.AddRange(new Control[] {
                lblFormTitle, lblEmail, txtEmail, lblEmailValidacion,
                lblRol, cmbRol, lblNombre, txtNombreCompleto,
                lblUsuario, txtUsuario, lblUsuarioValidacion,
                lblPassword, txtPassword, lblPasswordValidacion,
                lblConfirmPassword, txtConfirmPassword, chkMostrarPassword,
                lblSeguridadTitle,
                lblPregunta1, cmbPregunta1, txtRespuesta1,
                lblPregunta2, cmbPregunta2, txtRespuesta2,
                lblPregunta3, cmbPregunta3, txtRespuesta3
            });

            Button btnRegistrar = new Button
            {
                Text = "CONTINUAR",
                Location = new System.Drawing.Point(225, 730),
                Size = new System.Drawing.Size(250, 45)
            };
            BankTheme.StyleButton(btnRegistrar, true);
            btnRegistrar.Click += BtnRegistrar_Click;

            LinkLabel linkLogin = new LinkLabel
            {
                Text = "¿Ya tienes usuario? Iniciar sesión",
                Location = new System.Drawing.Point(225, 780),
                Size = new System.Drawing.Size(250, 20),
                Font = BankTheme.SmallFont,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                LinkColor = BankTheme.PrimaryBlue
            };
            linkLogin.LinkClicked += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, mainCard, btnRegistrar, linkLogin });
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
                lblEmailValidacion.Text = "X Email invalido";
                lblEmailValidacion.ForeColor = BankTheme.Danger;
                return;
            }

            lblEmailValidacion.Text = "OK Email valido";
            lblEmailValidacion.ForeColor = BankTheme.Success;
        }

        private void TxtUsuario_TextChanged(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            if (string.IsNullOrEmpty(usuario))
            {
                lblUsuarioValidacion.Text = "";
                return;
            }

            bool longitudValida = usuario.Length >= 8 && usuario.Length <= 20;
            bool tieneLetras = Regex.IsMatch(usuario, @"[a-zA-Z]");
            bool tieneNumeros = Regex.IsMatch(usuario, @"[0-9]");
            bool tieneSimbolo = Regex.IsMatch(usuario, @"[!@#$%^&*(),.?""':{}|<>_\-]");

            if (!longitudValida)
            {
                lblUsuarioValidacion.Text = $"X Debe tener 8-20 caracteres ({usuario.Length}/20)";
                lblUsuarioValidacion.ForeColor = BankTheme.Danger;
                return;
            }

            if (!tieneLetras || !tieneNumeros || !tieneSimbolo)
            {
                lblUsuarioValidacion.Text = "X Debe incluir letras, numeros y simbolos";
                lblUsuarioValidacion.ForeColor = BankTheme.Warning;
                return;
            }

            lblUsuarioValidacion.Text = $"OK Usuario valido ({usuario.Length}/20)";
            lblUsuarioValidacion.ForeColor = BankTheme.Success;
        }

        private void TxtPassword_TextChanged(object sender, EventArgs e)
        {
            string password = txtPassword.Text;
            if (string.IsNullOrEmpty(password))
            {
                lblPasswordValidacion.Text = "";
                return;
            }

            bool longitudValida = password.Length >= 8 && password.Length <= 20;
            bool tieneMayuscula = Regex.IsMatch(password, @"[A-Z]");
            bool tieneMinuscula = Regex.IsMatch(password, @"[a-z]");
            bool tieneNumero = Regex.IsMatch(password, @"[0-9]");
            bool tieneSimbolo = Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>_\-]");

            if (!longitudValida)
            {
                lblPasswordValidacion.Text = $"X Debe tener 8-20 caracteres ({password.Length}/20)";
                lblPasswordValidacion.ForeColor = BankTheme.Danger;
                return;
            }

            if (!tieneMayuscula || !tieneMinuscula || !tieneNumero || !tieneSimbolo)
            {
                lblPasswordValidacion.Text = "X Debe incluir mayusculas, minusculas, numeros y simbolos";
                lblPasswordValidacion.ForeColor = BankTheme.Warning;
                return;
            }

            lblPasswordValidacion.Text = $"OK Contraseña valida ({password.Length}/20)";
            lblPasswordValidacion.ForeColor = BankTheme.Success;
        }

        private void BtnRegistrar_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                CustomMessageBox.Show("Campo Requerido",
                    "Por favor ingrese su correo electronico.",
                    MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                CustomMessageBox.Show("E-mail no valido",
                    "Correo electronico con formato no valido.\n\nEl formato debe ser: nombreUsuario@dominio",
                    MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombreCompleto.Text))
            {
                CustomMessageBox.Show("Campo Requerido",
                    "Por favor ingrese su nombre completo.",
                    MessageBoxIcon.Warning);
                txtNombreCompleto.Focus();
                return;
            }

            string usuario = txtUsuario.Text.Trim();
            if (string.IsNullOrWhiteSpace(usuario))
            {
                CustomMessageBox.Show("Campo Requerido",
                    "Por favor ingrese un nombre de usuario.",
                    MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            bool longitudValida = usuario.Length >= 8 && usuario.Length <= 20;
            bool tieneLetras = Regex.IsMatch(usuario, @"[a-zA-Z]");
            bool tieneNumeros = Regex.IsMatch(usuario, @"[0-9]");
            bool tieneSimbolo = Regex.IsMatch(usuario, @"[!@#$%^&*(),.?""':{}|<>_\-]");

            if (!longitudValida || !tieneLetras || !tieneNumeros || !tieneSimbolo)
            {
                CustomMessageBox.Show("Nombre de usuario no valido",
                    "El nombre de usuario no cumple los requisitos:\n\n- Longitud: 8-20 caracteres\n- Debe incluir letras\n- Debe incluir numeros\n- Debe incluir simbolos",
                    MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            string password = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(password))
            {
                CustomMessageBox.Show("Campo Requerido",
                    "Por favor ingrese una contraseña.",
                    MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            bool passLongitudValida = password.Length >= 8 && password.Length <= 20;
            bool passTieneMayuscula = Regex.IsMatch(password, @"[A-Z]");
            bool passTieneMinuscula = Regex.IsMatch(password, @"[a-z]");
            bool passTieneNumero = Regex.IsMatch(password, @"[0-9]");
            bool passTieneSimbolo = Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>_\-]");

            if (!passLongitudValida || !passTieneMayuscula || !passTieneMinuscula || !passTieneNumero || !passTieneSimbolo)
            {
                CustomMessageBox.Show("Contraseña no valida",
                    "La contraseña no cumple los requisitos:\n\n- Longitud: 8-20 caracteres\n- Debe incluir mayusculas\n- Debe incluir minusculas\n- Debe incluir numeros\n- Debe incluir simbolos",
                    MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                CustomMessageBox.Show("Contraseñas no coinciden",
                    "Las contraseñas no coinciden.\n\nPor favor verifique que ambas contraseñas sean identicas.",
                    MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRespuesta1.Text))
            {
                CustomMessageBox.Show("Pregunta de Seguridad Requerida",
                    "Por favor responda la primera pregunta de seguridad.",
                    MessageBoxIcon.Warning);
                txtRespuesta1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRespuesta2.Text))
            {
                CustomMessageBox.Show("Pregunta de Seguridad Requerida",
                    "Por favor responda la segunda pregunta de seguridad.",
                    MessageBoxIcon.Warning);
                txtRespuesta2.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRespuesta3.Text))
            {
                CustomMessageBox.Show("Pregunta de Seguridad Requerida",
                    "Por favor responda la tercera pregunta de seguridad.",
                    MessageBoxIcon.Warning);
                txtRespuesta3.Focus();
                return;
            }

            try
            {

                string queryCheck = "SELECT COUNT(*) FROM usuarios WHERE usuario = @user OR email = @email";
                DataTable dtCheck = Database.ExecuteQuery(queryCheck,
                    new NpgsqlParameter("@user", usuario),
                    new NpgsqlParameter("@email", txtEmail.Text.Trim()));

                if (Convert.ToInt32(dtCheck.Rows[0][0]) > 0)
                {
                    CustomMessageBox.Show("Usuario o Email Ya Existe",
                        "El nombre de usuario o correo electronico ya esta registrado en el sistema.\n\nPor favor use un nombre de usuario o correo diferente.",
                        MessageBoxIcon.Warning);
                    return;
                }

                string queryInsert = @"INSERT INTO usuarios (usuario, contraseña, nombre_completo, email,
                                      pregunta_seguridad_1, respuesta_seguridad_1,
                                      pregunta_seguridad_2, respuesta_seguridad_2,
                                      pregunta_seguridad_3, respuesta_seguridad_3,
                                      rol, estatus, intentos_fallidos) 
                                      VALUES (@user, @pass, @nombre, @email,
                                      @preg1, @resp1, @preg2, @resp2, @preg3, @resp3,
                                      @rol, TRUE, 0) 
                                      RETURNING id_usuario";

                DataTable dtResult = Database.ExecuteQuery(queryInsert,
                    new NpgsqlParameter("@user", usuario),
                    new NpgsqlParameter("@pass", password),
                    new NpgsqlParameter("@nombre", txtNombreCompleto.Text.Trim()),
                    new NpgsqlParameter("@email", txtEmail.Text.Trim()),
                    new NpgsqlParameter("@preg1", cmbPregunta1.SelectedItem.ToString()),
                    new NpgsqlParameter("@resp1", txtRespuesta1.Text.Trim().ToLower()),
                    new NpgsqlParameter("@preg2", cmbPregunta2.SelectedItem.ToString()),
                    new NpgsqlParameter("@resp2", txtRespuesta2.Text.Trim().ToLower()),
                    new NpgsqlParameter("@preg3", cmbPregunta3.SelectedItem.ToString()),
                    new NpgsqlParameter("@resp3", txtRespuesta3.Text.Trim().ToLower()),
                    new NpgsqlParameter("@rol", cmbRol.SelectedItem.ToString()));

                int idUsuario = Convert.ToInt32(dtResult.Rows[0][0]);

                string numeroCuenta = "100" + DateTime.Now.ToString("yyyyMMddHHmmss");
                string queryAccount = @"INSERT INTO cuentas (id_usuario, numero_cuenta, tipo_cuenta, saldo, estatus) 
                                       VALUES (@id, @numero, 'AHORRO', 0.00, TRUE)";
                Database.ExecuteNonQuery(queryAccount,
                    new NpgsqlParameter("@id", idUsuario),
                    new NpgsqlParameter("@numero", numeroCuenta));

                CustomMessageBox.Show("Registrado correctamente",
                    $"Su cuenta ha sido creada exitosamente.\n\nUsuario: {usuario}\nID Usuario: {idUsuario}\nNumero de cuenta: {numeroCuenta}\n\nYa puede iniciar sesion con sus credenciales.",
                    MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Crear Cuenta",
                    $"No se pudo crear la cuenta debido a un error del sistema.\n\nDetalle tecnico: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }
    }
}
