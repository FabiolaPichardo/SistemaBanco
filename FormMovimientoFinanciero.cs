using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;
using System.Drawing;

namespace SistemaBanco
{
    public partial class FormMovimientoFinanciero : Form
    {
        private ComboBox cmbTipoOperacion;
        private TextBox txtFolio;
        private DateTimePicker dtpFecha;
        private ComboBox cmbCuentaOrdenante;
        private TextBox txtCuentaBeneficiaria;
        private TextBox txtBeneficiario;
        private TextBox txtImporte;
        private ComboBox cmbMoneda;
        private TextBox txtConcepto;
        private TextBox txtReferencia;
        private ComboBox cmbCuentaContable;
        private Label lblEstado;
        private Label lblUsuario;
        private Label lblFechaHoraSistema;

        public FormMovimientoFinanciero()
        {
            InitializeComponent();
            IconHelper.SetFormIcon(this);
            GenerarFolio();
            CargarDatosIniciales();
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Registro de Movimientos Financieros";
            this.ClientSize = new System.Drawing.Size(900, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.AutoScroll = true;

            // Header
            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(900, 90),
                BackColor = BankTheme.PrimaryBlue
            };

            HomeButton.AddToForm(this, headerPanel);

            Label lblLogo = new Label
            {
                Text = "🏦",
                Location = new System.Drawing.Point(350, 10),
                Size = new System.Drawing.Size(60, 40),
                Font = new Font("Segoe UI", 32F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblTitulo = new Label
            {
                Text = "REGISTRO DE MOVIMIENTOS FINANCIEROS",
                Location = new System.Drawing.Point(200, 55),
                Size = new System.Drawing.Size(500, 25),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.AddRange(new Control[] { lblLogo, lblTitulo });

            // SECCIÓN 1: DATOS GENERALES
            Panel seccionGenerales = BankTheme.CreateCard(50, 120, 800, 180);
            
            Label lblSeccion1 = new Label
            {
                Text = "📋 DATOS GENERALES",
                Location = new System.Drawing.Point(20, 15),
                Size = new System.Drawing.Size(760, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = BankTheme.PrimaryBlue
            };

            // Tipo de Operación (BAN-25)
            Label lblTipoOp = new Label
            {
                Text = "Tipo de Operación *",
                Location = new System.Drawing.Point(30, 50),
                Size = new System.Drawing.Size(350, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            cmbTipoOperacion = new ComboBox
            {
                Location = new System.Drawing.Point(30, 75),
                Size = new System.Drawing.Size(350, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = BankTheme.BodyFont
            };
            cmbTipoOperacion.Items.AddRange(new string[] { 
                "CARGO - Egreso (Pagos, gastos)", 
                "ABONO - Ingreso (Depósitos de clientes)" 
            });

            // Folio (BAN-23, BAN-26)
            Label lblFolio = new Label
            {
                Text = "Folio Único (Automático)",
                Location = new System.Drawing.Point(420, 50),
                Size = new System.Drawing.Size(350, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtFolio = new TextBox
            {
                Location = new System.Drawing.Point(420, 75),
                Size = new System.Drawing.Size(350, 30),
                Font = BankTheme.BodyFont,
                ReadOnly = true,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            // Fecha/Hora (BAN-26)
            Label lblFecha = new Label
            {
                Text = "Fecha y Hora (Automático)",
                Location = new System.Drawing.Point(30, 115),
                Size = new System.Drawing.Size(350, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            dtpFecha = new DateTimePicker
            {
                Location = new System.Drawing.Point(30, 140),
                Size = new System.Drawing.Size(350, 30),
                Font = BankTheme.BodyFont,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm:ss",
                Enabled = false
            };

            seccionGenerales.Controls.AddRange(new Control[] { 
                lblSeccion1, lblTipoOp, cmbTipoOperacion, lblFolio, txtFolio, lblFecha, dtpFecha 
            });

            // SECCIÓN 2: DATOS DE LA TRANSACCIÓN
            Panel seccionTransaccion = BankTheme.CreateCard(50, 320, 800, 320);
            
            Label lblSeccion2 = new Label
            {
                Text = "💰 DATOS DE LA TRANSACCIÓN",
                Location = new System.Drawing.Point(20, 15),
                Size = new System.Drawing.Size(760, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = BankTheme.PrimaryBlue
            };

            // Cuenta Ordenante (BAN-26)
            Label lblCuentaOrd = new Label
            {
                Text = "Cuenta Ordenante *",
                Location = new System.Drawing.Point(30, 50),
                Size = new System.Drawing.Size(350, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            cmbCuentaOrdenante = new ComboBox
            {
                Location = new System.Drawing.Point(30, 75),
                Size = new System.Drawing.Size(350, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = BankTheme.BodyFont
            };

            // Cuenta Beneficiaria (BAN-26)
            Label lblCuentaBen = new Label
            {
                Text = "Cuenta Beneficiaria *",
                Location = new System.Drawing.Point(420, 50),
                Size = new System.Drawing.Size(350, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtCuentaBeneficiaria = new TextBox
            {
                Location = new System.Drawing.Point(420, 75),
                Size = new System.Drawing.Size(350, 30),
                Font = BankTheme.BodyFont,
                MaxLength = 20,
                PlaceholderText = "Ingrese número de cuenta"
            };
            BankTheme.StyleTextBox(txtCuentaBeneficiaria);
            txtCuentaBeneficiaria.TextChanged += TxtCuentaBeneficiaria_TextChanged;

            // Beneficiario (BAN-26)
            Label lblBeneficiario = new Label
            {
                Text = "Beneficiario *",
                Location = new System.Drawing.Point(30, 115),
                Size = new System.Drawing.Size(740, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtBeneficiario = new TextBox
            {
                Location = new System.Drawing.Point(30, 140),
                Size = new System.Drawing.Size(740, 30),
                Font = BankTheme.BodyFont,
                MaxLength = 100,
                PlaceholderText = "Nombre del beneficiario (se autocompletará al ingresar cuenta)"
            };
            BankTheme.StyleTextBox(txtBeneficiario);

            // Importe (BAN-26)
            Label lblImporte = new Label
            {
                Text = "Importe *",
                Location = new System.Drawing.Point(30, 180),
                Size = new System.Drawing.Size(200, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtImporte = new TextBox
            {
                Location = new System.Drawing.Point(30, 205),
                Size = new System.Drawing.Size(200, 30),
                Font = BankTheme.BodyFont,
                PlaceholderText = "0.00"
            };
            BankTheme.StyleTextBox(txtImporte);
            txtImporte.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                    e.Handled = true;
                if (e.KeyChar == '.' && txtImporte.Text.Contains("."))
                    e.Handled = true;
            };

            // Moneda (BAN-26)
            Label lblMoneda = new Label
            {
                Text = "Moneda *",
                Location = new System.Drawing.Point(260, 180),
                Size = new System.Drawing.Size(150, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            cmbMoneda = new ComboBox
            {
                Location = new System.Drawing.Point(260, 205),
                Size = new System.Drawing.Size(150, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = BankTheme.BodyFont
            };
            cmbMoneda.Items.AddRange(new string[] { "MXN", "USD", "EUR" });
            cmbMoneda.SelectedIndex = 0;

            // Concepto (BAN-26)
            Label lblConcepto = new Label
            {
                Text = "Concepto *",
                Location = new System.Drawing.Point(440, 180),
                Size = new System.Drawing.Size(330, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtConcepto = new TextBox
            {
                Location = new System.Drawing.Point(440, 205),
                Size = new System.Drawing.Size(330, 30),
                Font = BankTheme.BodyFont,
                MaxLength = 200,
                PlaceholderText = "Descripción del movimiento"
            };
            BankTheme.StyleTextBox(txtConcepto);

            // Referencia/PO/Factura (BAN-26)
            Label lblReferencia = new Label
            {
                Text = "Referencia / PO / Factura",
                Location = new System.Drawing.Point(30, 245),
                Size = new System.Drawing.Size(740, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtReferencia = new TextBox
            {
                Location = new System.Drawing.Point(30, 270),
                Size = new System.Drawing.Size(740, 30),
                Font = BankTheme.BodyFont,
                MaxLength = 50,
                PlaceholderText = "Opcional - Se generará automáticamente si se deja vacío"
            };
            BankTheme.StyleTextBox(txtReferencia);

            seccionTransaccion.Controls.AddRange(new Control[] { 
                lblSeccion2, lblCuentaOrd, cmbCuentaOrdenante, lblCuentaBen, txtCuentaBeneficiaria,
                lblBeneficiario, txtBeneficiario, lblImporte, txtImporte, lblMoneda, cmbMoneda,
                lblConcepto, txtConcepto, lblReferencia, txtReferencia
            });

            // SECCIÓN 3: CONTABILIDAD Y CONTROL
            Panel seccionControl = BankTheme.CreateCard(50, 660, 800, 140);
            
            Label lblSeccion3 = new Label
            {
                Text = "📊 CONTABILIDAD Y CONTROL",
                Location = new System.Drawing.Point(20, 15),
                Size = new System.Drawing.Size(760, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = BankTheme.PrimaryBlue
            };

            // Cuenta Contable (BAN-26)
            Label lblCuentaCont = new Label
            {
                Text = "Cuenta Contable (ERP) *",
                Location = new System.Drawing.Point(30, 50),
                Size = new System.Drawing.Size(350, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            cmbCuentaContable = new ComboBox
            {
                Location = new System.Drawing.Point(30, 75),
                Size = new System.Drawing.Size(350, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = BankTheme.BodyFont
            };

            // Estado (BAN-23)
            Label lblEstadoLabel = new Label
            {
                Text = "Estado Inicial",
                Location = new System.Drawing.Point(420, 50),
                Size = new System.Drawing.Size(150, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            lblEstado = new Label
            {
                Text = "PENDIENTE",
                Location = new System.Drawing.Point(420, 75),
                Size = new System.Drawing.Size(150, 30),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = BankTheme.Warning,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Usuario (BAN-26)
            Label lblUsuarioLabel = new Label
            {
                Text = "Usuario Registrado",
                Location = new System.Drawing.Point(600, 50),
                Size = new System.Drawing.Size(170, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            lblUsuario = new Label
            {
                Location = new System.Drawing.Point(600, 75),
                Size = new System.Drawing.Size(170, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = BankTheme.PrimaryBlue,
                TextAlign = ContentAlignment.MiddleLeft
            };

            seccionControl.Controls.AddRange(new Control[] { 
                lblSeccion3, lblCuentaCont, cmbCuentaContable, lblEstadoLabel, lblEstado,
                lblUsuarioLabel, lblUsuario
            });

            // BOTONES (BAN-24)
            Button btnGuardar = new Button
            {
                Text = "✓ GUARDAR",
                Location = new System.Drawing.Point(300, 680),
                Size = new System.Drawing.Size(150, 45),
                BackColor = BankTheme.Success,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;

            Button btnCancelar = new Button
            {
                Text = "✗ CANCELAR",
                Location = new System.Drawing.Point(470, 680),
                Size = new System.Drawing.Size(150, 45),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();

            lblFechaHoraSistema = new Label
            {
                Location = new System.Drawing.Point(20, 10),
                Size = new System.Drawing.Size(860, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.Controls.AddRange(new Control[] { 
                headerPanel, seccionGenerales, seccionTransaccion, seccionControl,
                btnGuardar, btnCancelar
            });
        }

        private void TxtCuentaBeneficiaria_TextChanged(object? sender, EventArgs e)
        {
            string cuentaBuscar = txtCuentaBeneficiaria.Text.Trim();
            if (string.IsNullOrEmpty(cuentaBuscar) || cuentaBuscar.Length < 3)
            {
                txtBeneficiario.Text = "";
                return;
            }

            try
            {
                string query = @"SELECT u.nombre_completo, c.numero_cuenta 
                                FROM cuentas c 
                                INNER JOIN usuarios u ON c.id_usuario = u.id_usuario 
                                WHERE c.numero_cuenta LIKE @cuenta 
                                LIMIT 1";
                DataTable dt = Database.ExecuteQuery(query, 
                    new NpgsqlParameter("@cuenta", $"{cuentaBuscar}%"));

                if (dt.Rows.Count > 0)
                {
                    txtBeneficiario.Text = dt.Rows[0]["nombre_completo"].ToString();
                    txtBeneficiario.ForeColor = BankTheme.Success;
                }
                else
                {
                    txtBeneficiario.ForeColor = BankTheme.TextSecondary;
                }
            }
            catch
            {
                // Si hay error, no hacer nada
            }
        }

        private void GenerarFolio()
        {
            // Generar folio único (BAN-23, BAN-26)
            string folio = "MOV-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            if (txtFolio != null)
                txtFolio.Text = folio;
        }

        private void CargarDatosIniciales()
        {
            // Usuario actual (BAN-26) - Mostrar nombre completo
            if (lblUsuario != null)
            {
                try
                {
                    string query = "SELECT nombre_completo FROM usuarios WHERE id_usuario = @id";
                    DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@id", FormLogin.IdUsuarioActual));
                    if (dt.Rows.Count > 0)
                        lblUsuario.Text = dt.Rows[0]["nombre_completo"].ToString();
                    else
                        lblUsuario.Text = FormLogin.NombreUsuario;
                }
                catch
                {
                    lblUsuario.Text = FormLogin.NombreUsuario;
                }
            }



            // Cargar cuentas ordenantes (BAN-26)
            if (cmbCuentaOrdenante != null)
            {
                cmbCuentaOrdenante.Items.AddRange(new string[] {
                    "BBVA - 012345678901234567",
                    "Santander - 014012345678901234",
                    "Banamex - 002123456789012345",
                    "Caja Chica - CAJA001"
                });
                cmbCuentaOrdenante.SelectedIndex = 0;
            }

            // Cargar cuentas contables (BAN-26)
            if (cmbCuentaContable != null)
            {
                cmbCuentaContable.Items.AddRange(new string[] {
                    "1101 - Bancos",
                    "1102 - Caja",
                    "2101 - Proveedores",
                    "4101 - Ventas",
                    "5101 - Gastos Operativos",
                    "5102 - Gastos Administrativos"
                });
                cmbCuentaContable.SelectedIndex = 0;
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validación obligatoria tipo de operación (BAN-25)
            if (cmbTipoOperacion.SelectedIndex == -1)
            {
                CustomMessageBox.Show("Campo Requerido",
                    "Debe seleccionar el tipo de operación (Cargo o Abono).",
                    MessageBoxIcon.Warning);
                cmbTipoOperacion.Focus();
                return;
            }

            // Validaciones de campos obligatorios (BAN-26)
            if (cmbCuentaOrdenante.SelectedIndex == -1)
            {
                CustomMessageBox.Show("Campo Requerido", "Seleccione la cuenta ordenante.", MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCuentaBeneficiaria.Text))
            {
                CustomMessageBox.Show("Campo Requerido", "Ingrese la cuenta beneficiaria.", MessageBoxIcon.Warning);
                txtCuentaBeneficiaria.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtBeneficiario.Text))
            {
                CustomMessageBox.Show("Campo Requerido", "Ingrese el nombre del beneficiario.", MessageBoxIcon.Warning);
                txtBeneficiario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtImporte.Text))
            {
                CustomMessageBox.Show("Campo Requerido", "Ingrese el importe.", MessageBoxIcon.Warning);
                txtImporte.Focus();
                return;
            }

            if (!decimal.TryParse(txtImporte.Text, out decimal importe) || importe <= 0)
            {
                CustomMessageBox.Show("Importe Inválido", "El importe debe ser un número mayor a 0.", MessageBoxIcon.Warning);
                txtImporte.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtConcepto.Text))
            {
                CustomMessageBox.Show("Campo Requerido", "Ingrese el concepto del movimiento.", MessageBoxIcon.Warning);
                txtConcepto.Focus();
                return;
            }

            if (cmbCuentaContable.SelectedIndex == -1)
            {
                CustomMessageBox.Show("Campo Requerido", "Seleccione la cuenta contable.", MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string tipoOperacion = cmbTipoOperacion.SelectedItem.ToString().StartsWith("CARGO") ? "CARGO" : "ABONO";
                
                // Generar referencia automática si está vacía
                string referencia = string.IsNullOrWhiteSpace(txtReferencia.Text) 
                    ? $"REF-{DateTime.Now:yyyyMMddHHmmss}" 
                    : txtReferencia.Text.Trim();
                
                // Insertar movimiento financiero (BAN-23, BAN-26)
                string query = @"INSERT INTO movimientos_financieros 
                                (folio, fecha_hora, tipo_operacion, cuenta_ordenante, cuenta_beneficiaria, 
                                 beneficiario, importe, moneda, concepto, referencia, cuenta_contable, 
                                 estado, id_usuario) 
                                VALUES (@folio, @fecha, @tipo, @cuentaOrd, @cuentaBen, @beneficiario, 
                                        @importe, @moneda, @concepto, @referencia, @cuentaCont, 
                                        @estado, @usuario)";

                Database.ExecuteNonQuery(query,
                    new NpgsqlParameter("@folio", txtFolio.Text),
                    new NpgsqlParameter("@fecha", DateTime.Now),
                    new NpgsqlParameter("@tipo", tipoOperacion),
                    new NpgsqlParameter("@cuentaOrd", cmbCuentaOrdenante.SelectedItem.ToString()),
                    new NpgsqlParameter("@cuentaBen", txtCuentaBeneficiaria.Text.Trim()),
                    new NpgsqlParameter("@beneficiario", txtBeneficiario.Text.Trim()),
                    new NpgsqlParameter("@importe", importe),
                    new NpgsqlParameter("@moneda", cmbMoneda.SelectedItem.ToString()),
                    new NpgsqlParameter("@concepto", txtConcepto.Text.Trim()),
                    new NpgsqlParameter("@referencia", referencia),
                    new NpgsqlParameter("@cuentaCont", cmbCuentaContable.SelectedItem.ToString()),
                    new NpgsqlParameter("@estado", "PENDIENTE"),
                    new NpgsqlParameter("@usuario", FormLogin.IdUsuarioActual));

                // Confirmación de registro (BAN-24)
                CustomMessageBox.Show("Movimiento Registrado",
                    $"El movimiento financiero ha sido registrado exitosamente.\n\n" +
                    $"Folio: {txtFolio.Text}\n" +
                    $"Tipo: {tipoOperacion}\n" +
                    $"Importe: {importe:C} {cmbMoneda.SelectedItem}\n" +
                    $"Estado: PENDIENTE\n\n" +
                    $"El movimiento quedará registrado para auditoría y seguimiento.",
                    MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                // Advertencia si no cumple validaciones (BAN-24)
                CustomMessageBox.Show("Error al Registrar",
                    $"No se pudo registrar el movimiento financiero.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }
    }
}
