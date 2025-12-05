using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormRecuperacion : Form
    {
        private TextBox txtUsuario;
        private Panel panelStep1;
        private Panel panelStep2;
        private TextBox txtRespuesta1;
        private TextBox txtRespuesta2;
        private TextBox txtRespuesta3;
        private Label lblPregunta1;
        private Label lblPregunta2;
        private Label lblPregunta3;
        private TextBox txtNuevaPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkMostrarPassword;
        private int idUsuarioRecuperacion;
        private string respuestaCorrecta1 = "";
        private string respuestaCorrecta2 = "";
        private string respuestaCorrecta3 = "";
        private string emailUsuario = "";
        private string nombreUsuario = "";

        public FormRecuperacion()
        {
            InitializeComponent();
            IconHelper.SetFormIcon(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Recuperación de Contraseña";
            this.ClientSize = new System.Drawing.Size(700, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(700, 100),
                BackColor = BankTheme.PrimaryBlue
            };

            Label lblLogo = new Label
            {
                Text = "🔐",
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
                Text = "Recuperación de Contraseña",
                Location = new System.Drawing.Point(200, 75),
                Size = new System.Drawing.Size(300, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.AccentGold,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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

            headerPanel.Controls.AddRange(new Control[] { btnRegresar, lblLogo, lblTitulo, lblSubtitulo });

            panelStep1 = BankTheme.CreateCard(50, 120, 600, 500);
            panelStep1.Visible = true;

            Label lblStep1Title = new Label
            {
                Text = "Paso 1: Verificación de Identidad",
                Location = new System.Drawing.Point(150, 20),
                Size = new System.Drawing.Size(300, 25),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            Label lblUsuario = new Label
            {
                Text = "Nombre de Usuario",
                Location = new System.Drawing.Point(40, 60),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtUsuario = new TextBox
            {
                Location = new System.Drawing.Point(40, 85),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F)
            };
            BankTheme.StyleTextBox(txtUsuario);
            txtUsuario.Leave += TxtUsuario_Leave;

            Label lblPreguntasTitle = new Label
            {
                Text = "Preguntas de Seguridad",
                Location = new System.Drawing.Point(40, 130),
                Size = new System.Drawing.Size(520, 25),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.PrimaryBlue,
                Visible = false
            };

            lblPregunta1 = new Label
            {
                Location = new System.Drawing.Point(40, 170),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary,
                Visible = false
            };

            txtRespuesta1 = new TextBox
            {
                Location = new System.Drawing.Point(40, 195),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                Visible = false
            };
            BankTheme.StyleTextBox(txtRespuesta1);

            lblPregunta2 = new Label
            {
                Location = new System.Drawing.Point(40, 240),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary,
                Visible = false
            };

            txtRespuesta2 = new TextBox
            {
                Location = new System.Drawing.Point(40, 265),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                Visible = false
            };
            BankTheme.StyleTextBox(txtRespuesta2);

            lblPregunta3 = new Label
            {
                Location = new System.Drawing.Point(40, 310),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary,
                Visible = false
            };

            txtRespuesta3 = new TextBox
            {
                Location = new System.Drawing.Point(40, 335),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                Visible = false
            };
            BankTheme.StyleTextBox(txtRespuesta3);

            Button btnContinuarStep1 = new Button
            {
                Text = "CONTINUAR",
                Location = new System.Drawing.Point(200, 420),
                Size = new System.Drawing.Size(200, 45)
            };
            BankTheme.StyleButton(btnContinuarStep1, true);
            btnContinuarStep1.Click += BtnContinuarStep1_Click;

            panelStep1.Controls.AddRange(new Control[] {
                lblStep1Title, lblUsuario, txtUsuario, lblPreguntasTitle,
                lblPregunta1, txtRespuesta1, lblPregunta2, txtRespuesta2,
                lblPregunta3, txtRespuesta3, btnContinuarStep1
            });

            panelStep2 = BankTheme.CreateCard(50, 120, 600, 450);
            panelStep2.Visible = false;

            Label lblStep2Title = new Label
            {
                Text = "Paso 2: Restablecimiento de Contraseña",
                Location = new System.Drawing.Point(120, 20),
                Size = new System.Drawing.Size(360, 25),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            Label lblInstrucciones = new Label
            {
                Text = "La contraseña debe tener entre 8 y 20 caracteres,\nincluyendo letras mayúsculas, minúsculas, números y símbolos.",
                Location = new System.Drawing.Point(40, 60),
                Size = new System.Drawing.Size(520, 40),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };

            Label lblNuevaPassword = new Label
            {
                Text = "Nueva Contraseña",
                Location = new System.Drawing.Point(40, 120),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtNuevaPassword = new TextBox
            {
                Location = new System.Drawing.Point(40, 145),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                UseSystemPasswordChar = true
            };
            BankTheme.StyleTextBox(txtNuevaPassword);

            Label lblConfirmPassword = new Label
            {
                Text = "Confirmar Nueva Contraseña",
                Location = new System.Drawing.Point(40, 195),
                Size = new System.Drawing.Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtConfirmPassword = new TextBox
            {
                Location = new System.Drawing.Point(40, 220),
                Size = new System.Drawing.Size(520, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                UseSystemPasswordChar = true
            };
            BankTheme.StyleTextBox(txtConfirmPassword);

            chkMostrarPassword = new CheckBox
            {
                Text = "Mostrar contraseñas",
                Location = new System.Drawing.Point(40, 265),
                Size = new System.Drawing.Size(200, 20),
                Font = BankTheme.SmallFont
            };
            chkMostrarPassword.CheckedChanged += (s, e) =>
            {
                txtNuevaPassword.UseSystemPasswordChar = !chkMostrarPassword.Checked;
                txtConfirmPassword.UseSystemPasswordChar = !chkMostrarPassword.Checked;
            };

            Button btnContinuarStep2 = new Button
            {
                Text = "CONTINUAR",
                Location = new System.Drawing.Point(200, 330),
                Size = new System.Drawing.Size(200, 45)
            };
            BankTheme.StyleButton(btnContinuarStep2, true);
            btnContinuarStep2.Click += BtnContinuarStep2_Click;

            panelStep2.Controls.AddRange(new Control[] {
                lblStep2Title, lblInstrucciones, lblNuevaPassword, txtNuevaPassword,
                lblConfirmPassword, txtConfirmPassword, chkMostrarPassword, btnContinuarStep2
            });

            this.Controls.AddRange(new Control[] { headerPanel, panelStep1, panelStep2 });
        }

        private void TxtUsuario_Leave(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            if (string.IsNullOrEmpty(usuario)) return;

            try
            {

                string query = @"SELECT id_usuario, nombre_completo, email,
                                pregunta_seguridad_1, respuesta_seguridad_1,
                                pregunta_seguridad_2, respuesta_seguridad_2,
                                pregunta_seguridad_3, respuesta_seguridad_3
                                FROM usuarios WHERE usuario = @user AND estatus = TRUE";
                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@user", usuario));

                if (dt.Rows.Count > 0)
                {
                    idUsuarioRecuperacion = Convert.ToInt32(dt.Rows[0]["id_usuario"]);
                    nombreUsuario = dt.Rows[0]["nombre_completo"].ToString();
                    emailUsuario = dt.Rows[0]["email"].ToString();

                    lblPregunta1.Text = dt.Rows[0]["pregunta_seguridad_1"].ToString();
                    respuestaCorrecta1 = dt.Rows[0]["respuesta_seguridad_1"].ToString().ToLower().Trim();

                    lblPregunta2.Text = dt.Rows[0]["pregunta_seguridad_2"].ToString();
                    respuestaCorrecta2 = dt.Rows[0]["respuesta_seguridad_2"].ToString().ToLower().Trim();

                    lblPregunta3.Text = dt.Rows[0]["pregunta_seguridad_3"].ToString();
                    respuestaCorrecta3 = dt.Rows[0]["respuesta_seguridad_3"].ToString().ToLower().Trim();

                    var lblPreguntasTitle = panelStep1.Controls[3] as Label;
                    if (lblPreguntasTitle != null) lblPreguntasTitle.Visible = true;
                    lblPregunta1.Visible = true;
                    txtRespuesta1.Visible = true;
                    lblPregunta2.Visible = true;
                    txtRespuesta2.Visible = true;
                    lblPregunta3.Visible = true;
                    txtRespuesta3.Visible = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void BtnContinuarStep1_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();

            if (string.IsNullOrEmpty(usuario))
            {
                CustomMessageBox.Show("Campo Requerido",
                    "Por favor ingrese su nombre de usuario.",
                    MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            if (idUsuarioRecuperacion == 0)
            {
                CustomMessageBox.Show("Usuario no registrado en el sistema",
                    "El nombre de usuario ingresado no se encuentra registrado en el sistema.\n\nPor favor verifique que el usuario sea correcto o regístrese si aún no tiene una cuenta.",
                    MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            string respuesta1 = txtRespuesta1.Text.ToLower().Trim();
            string respuesta2 = txtRespuesta2.Text.ToLower().Trim();
            string respuesta3 = txtRespuesta3.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(respuesta1) || string.IsNullOrEmpty(respuesta2) || string.IsNullOrEmpty(respuesta3))
            {
                CustomMessageBox.Show("Respuestas Incompletas",
                    "Por favor responda todas las preguntas de seguridad.",
                    MessageBoxIcon.Warning);
                return;
            }

            if (respuesta1 != respuestaCorrecta1 || respuesta2 != respuestaCorrecta2 || respuesta3 != respuestaCorrecta3)
            {
                CustomMessageBox.Show("Algunas respuestas son incorrectas",
                    "Una o más respuestas de seguridad no coinciden con las registradas.\n\nPor favor verifique sus respuestas e intente nuevamente.",
                    MessageBoxIcon.Warning);
                return;
            }

            panelStep1.Visible = false;
            panelStep2.Visible = true;
        }

        private void BtnContinuarStep2_Click(object sender, EventArgs e)
        {
            string nuevaPassword = txtNuevaPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(nuevaPassword))
            {
                CustomMessageBox.Show("Campo Requerido",
                    "Por favor ingrese su nueva contraseña.",
                    MessageBoxIcon.Warning);
                txtNuevaPassword.Focus();
                return;
            }

            if (nuevaPassword.Length < 8 || nuevaPassword.Length > 20)
            {
                CustomMessageBox.Show("La contraseña no cumple con todos los requisitos",
                    "La contraseña debe tener entre 8 y 20 caracteres.\n\nPor favor ingrese una contraseña válida.",
                    MessageBoxIcon.Warning);
                txtNuevaPassword.Focus();
                return;
            }

            bool tieneMayuscula = Regex.IsMatch(nuevaPassword, @"[A-Z]");
            bool tieneMinuscula = Regex.IsMatch(nuevaPassword, @"[a-z]");
            bool tieneNumero = Regex.IsMatch(nuevaPassword, @"[0-9]");
            bool tieneSimbolo = Regex.IsMatch(nuevaPassword, @"[!@#$%^&*(),.?""':{}|<>]");

            if (!tieneMayuscula || !tieneMinuscula || !tieneNumero || !tieneSimbolo)
            {
                CustomMessageBox.Show("La contraseña no cumple con todos los requisitos",
                    "La contraseña debe incluir:\n\n• Al menos una letra mayúscula\n• Al menos una letra minúscula\n• Al menos un número\n• Al menos un símbolo (!@#$%^&*...)\n\nPor favor ingrese una contraseña que cumpla con todos los requisitos.",
                    MessageBoxIcon.Warning);
                txtNuevaPassword.Focus();
                return;
            }

            if (nuevaPassword != confirmPassword)
            {
                CustomMessageBox.Show("Las contraseñas no coinciden",
                    "La nueva contraseña y la contraseña de confirmación no son iguales.\n\nPor favor verifique que ambas contraseñas sean idénticas.",
                    MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            try
            {

                string queryUpdate = "UPDATE usuarios SET contraseña = @pass WHERE id_usuario = @id";
                Database.ExecuteNonQuery(queryUpdate,
                    new NpgsqlParameter("@pass", nuevaPassword),
                    new NpgsqlParameter("@id", idUsuarioRecuperacion));

                if (EmailService.ConfiguracionValida())
                {
                    EnviarCorreoConfirmacion(emailUsuario, nombreUsuario);
                }

                CustomMessageBox.Show("La contraseña se ha actualizado correctamente",
                    $"Su contraseña ha sido restablecida exitosamente.\n\nSe ha enviado una confirmación a su correo electrónico: {emailUsuario}\n\nYa puede iniciar sesión con su nueva contraseña.",
                    MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Actualizar Contraseña",
                    $"No se pudo actualizar la contraseña debido a un error del sistema.\n\nDetalle técnico: {ex.Message}\n\nPor favor intente nuevamente o contacte al administrador.",
                    MessageBoxIcon.Error);
            }
        }

        private void EnviarCorreoConfirmacion(string toEmail, string nombreUsuario)
        {
            try
            {
                string subject = "Contraseña Restablecida - Módulo Banco";

                string body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ background-color: #1e3a8a; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ padding: 30px; text-align: center; }}
                        .success-icon {{ font-size: 64px; color: #10b981; margin: 20px 0; }}
                        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🏦 Módulo Banco</h1>
                            <p>Confirmación de Cambio de Contraseña</p>
                        </div>
                        <div class='content'>
                            <div class='success-icon'>✓</div>
                            <h2>¡Contraseña Actualizada!</h2>
                            <p>Hola <strong>{nombreUsuario}</strong>,</p>
                            <p>Tu contraseña ha sido restablecida exitosamente.</p>
                            <p>Si no realizaste este cambio, por favor contacta inmediatamente con nuestro equipo de soporte.</p>
                            <p style='margin-top: 30px;'>Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                        </div>
                        <div class='footer'>
                            <p>© 2025 Módulo Banco. Todos los derechos reservados.</p>
                            <p>Este es un correo automático, por favor no responder.</p>
                        </div>
                    </div>
                </body>
                </html>";

                EmailService.EnviarCorreo(toEmail, subject, body);
            }
            catch
            {

            }
        }
    }
}
