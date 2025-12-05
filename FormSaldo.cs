using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;
using System.Drawing;

namespace SistemaBanco
{
    public partial class FormSaldo : Form
    {
        private System.Windows.Forms.Timer timerActualizacion;
        private bool actualizacionAutomatica = true;

        public FormSaldo()
        {
            InitializeComponent();
            IconHelper.SetFormIcon(this);
            CargarSaldo();
            IniciarActualizacionAutomatica();
        }

        private Label lblSaldo;
        private Label lblCuenta;
        private Label lblActualizacion;
        private Label lblTipoCuenta;
        private Label lblIndicadorEstado;

        private void InitializeComponent()
        {
            bool puedeVerHistorico = RoleManager.PuedeVerHistorico(FormLogin.RolUsuario);

            this.Text = "Módulo Banco - Revisión de Saldos";
            this.ClientSize = new System.Drawing.Size(900, puedeVerHistorico ? 750 : 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(900, 90),
                BackColor = BankTheme.PrimaryBlue
            };

            HomeButton.AddToForm(this, headerPanel);

            string titulo = "REVISIÓN DE SALDOS";

            Label lblLogo = new Label
            {
                Text = "🏦",
                Location = new System.Drawing.Point(250, 20),
                Size = new System.Drawing.Size(50, 50),
                Font = new Font("Segoe UI", 32F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblTitulo = new Label
            {
                Text = titulo,
                Location = new System.Drawing.Point(310, 30),
                Size = new System.Drawing.Size(500, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.AddRange(new Control[] { lblLogo, lblTitulo });

            Panel mainCard = BankTheme.CreateCard(50, 110, 800, 300);

            Label lblCuentaLabel = new Label
            {
                Text = "Número de Cuenta",
                Location = new System.Drawing.Point(40, 25),
                Size = new System.Drawing.Size(300, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            lblCuenta = new Label
            {
                Location = new System.Drawing.Point(40, 50),
                Size = new System.Drawing.Size(300, 25),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = BankTheme.PrimaryBlue
            };

            Label lblTipoCuentaLabel = new Label
            {
                Text = "Tipo de Cuenta",
                Location = new System.Drawing.Point(420, 25),
                Size = new System.Drawing.Size(300, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            lblTipoCuenta = new Label
            {
                Location = new System.Drawing.Point(420, 50),
                Size = new System.Drawing.Size(300, 25),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = BankTheme.PrimaryBlue
            };

            Panel separador = new Panel
            {
                Location = new System.Drawing.Point(40, 95),
                Size = new System.Drawing.Size(720, 2),
                BackColor = BankTheme.LightGray
            };

            Label lblSaldoLabel = new Label
            {
                Text = "Saldo Disponible",
                Location = new System.Drawing.Point(40, 115),
                Size = new System.Drawing.Size(720, 25),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblIndicadorEstado = new Label
            {
                Location = new System.Drawing.Point(40, 145),
                Size = new System.Drawing.Size(720, 30),
                Font = new Font("Segoe UI", 16F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblSaldo = new Label
            {
                Location = new System.Drawing.Point(40, 180),
                Size = new System.Drawing.Size(720, 50),
                Font = BankTheme.MoneyFont,
                ForeColor = BankTheme.Success,
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblActualizacion = new Label
            {
                Text = $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm:ss}",
                Location = new System.Drawing.Point(40, 245),
                Size = new System.Drawing.Size(720, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleCenter
            };

            mainCard.Controls.AddRange(new Control[] { 
                lblCuentaLabel, lblCuenta, lblTipoCuentaLabel, lblTipoCuenta,
                separador, lblSaldoLabel, lblIndicadorEstado, lblSaldo, lblActualizacion 
            });

            Panel filtrosPanel = null;
            if (puedeVerHistorico)
            {
                filtrosPanel = CrearPanelFiltros();
                this.Controls.Add(filtrosPanel);
            }

            int controlPanelY = puedeVerHistorico ? 500 : 430;
            Panel controlPanel = new Panel
            {
                Location = new Point(50, controlPanelY),
                Size = new Size(800, 60),
                BackColor = Color.Transparent
            };

            CheckBox chkAutoActualizar = new CheckBox
            {
                Text = "Actualización automática (cada 30s)",
                Location = new Point(0, 15),
                Size = new Size(280, 30),
                Font = BankTheme.BodyFont,
                Checked = true
            };
            chkAutoActualizar.CheckedChanged += (s, e) =>
            {
                actualizacionAutomatica = chkAutoActualizar.Checked;
                if (actualizacionAutomatica)
                    timerActualizacion?.Start();
                else
                    timerActualizacion?.Stop();
            };

            Button btnActualizar = new Button
            {
                Text = "🔄 Refrescar",
                Location = new Point(300, 10),
                Size = new Size(140, 40),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnActualizar, false);
            btnActualizar.Click += (s, e) => CargarSaldo();

            bool puedeExportarCompleto = RoleManager.PuedeExportarCompleto(FormLogin.RolUsuario);

            Button btnExportarPDF = new Button
            {
                Text = "📄 PDF",
                Location = new Point(460, 10),
                Size = new Size(140, 40),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnExportarPDF, false);
            btnExportarPDF.Click += (s, e) => ExportarPDF();

            controlPanel.Controls.AddRange(new Control[] { chkAutoActualizar, btnActualizar, btnExportarPDF });

            if (puedeExportarCompleto)
            {
                Button btnExportarWord = new Button
                {
                    Text = "📝 Word",
                    Location = new Point(620, 10),
                    Size = new Size(90, 40),
                    Font = BankTheme.BodyFont
                };
                BankTheme.StyleButton(btnExportarWord, false);
                btnExportarWord.Click += (s, e) => ExportarWord();
                controlPanel.Controls.Add(btnExportarWord);

                Button btnExportarExcel = new Button
                {
                    Text = "📊 Excel",
                    Location = new Point(720, 10),
                    Size = new Size(90, 40),
                    Font = BankTheme.BodyFont
                };
                BankTheme.StyleButton(btnExportarExcel, false);
                btnExportarExcel.Click += (s, e) => ExportarExcel();
                controlPanel.Controls.Add(btnExportarExcel);
            }

            int btnCerrarY = puedeVerHistorico ? 680 : 580;
            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new Point(350, btnCerrarY),
                Size = new Size(200, 45)
            };
            BankTheme.StyleButton(btnCerrar, false);
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, mainCard, controlPanel, btnCerrar });
        }

        private void CargarSaldo()
        {
            try
            {
                DateTime inicioConsulta = DateTime.Now;
                string query = "SELECT numero_cuenta, saldo, tipo_cuenta FROM cuentas WHERE id_cuenta = @id";
                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));

                if (dt.Rows.Count > 0)
                {
                    string numeroCuenta = dt.Rows[0]["numero_cuenta"].ToString();
                    decimal saldo = Convert.ToDecimal(dt.Rows[0]["saldo"]);
                    string tipoCuenta = dt.Rows[0]["tipo_cuenta"].ToString();

                    lblCuenta.Text = numeroCuenta;
                    lblTipoCuenta.Text = tipoCuenta;
                    lblSaldo.Text = "$" + saldo.ToString("N2");

                    TimeSpan tiempoRespuesta = DateTime.Now - inicioConsulta;
                    string textoTiempo = tiempoRespuesta.TotalMilliseconds < 1000 
                        ? $"({tiempoRespuesta.TotalMilliseconds:F0}ms)" 
                        : $"({tiempoRespuesta.TotalSeconds:F1}s)";

                    lblActualizacion.Text = $"Última actualización: {DateTime.Now:dd/MM/yyyy} - {DateTime.Now:HH:mm} hrs {textoTiempo}";
                    lblActualizacion.ForeColor = BankTheme.Success;

                    ActualizarIndicadorEstado(saldo);

                    if (!actualizacionAutomatica || tiempoRespuesta.TotalSeconds > 1)
                    {
                        MostrarNotificacionTemporal("✓ Actualizado correctamente", BankTheme.Success);
                    }
                }
            }
            catch (Exception ex)
            {

                lblActualizacion.Text = "⚠ Datos desactualizados - intente refrescar manualmente";
                lblActualizacion.ForeColor = BankTheme.Danger;

                CustomMessageBox.Show("Error al Actualizar Saldo",
                    "No se pudo actualizar el saldo. Verifique su conexión e intente nuevamente.\n\nDetalle: " + ex.Message,
                    MessageBoxIcon.Error);
            }
        }

        private void ActualizarIndicadorEstado(decimal saldo)
        {
            if (saldo > 10000)
            {
                lblIndicadorEstado.Text = "✓ Estado Saludable";
                lblIndicadorEstado.ForeColor = BankTheme.Success;
                lblSaldo.ForeColor = BankTheme.Success;
            }
            else if (saldo >= 1000)
            {
                lblIndicadorEstado.Text = "● Estado Normal";
                lblIndicadorEstado.ForeColor = Color.FromArgb(59, 130, 246); // Azul
                lblSaldo.ForeColor = Color.FromArgb(59, 130, 246);
            }
            else if (saldo > 0)
            {
                lblIndicadorEstado.Text = "⚠ Saldo Bajo";
                lblIndicadorEstado.ForeColor = BankTheme.Warning;
                lblSaldo.ForeColor = BankTheme.Warning;
            }
            else
            {
                lblIndicadorEstado.Text = "⚠ Alerta - Saldo Insuficiente";
                lblIndicadorEstado.ForeColor = BankTheme.Danger;
                lblSaldo.ForeColor = BankTheme.Danger;
            }
        }

        private void IniciarActualizacionAutomatica()
        {
            timerActualizacion = new System.Windows.Forms.Timer();
            timerActualizacion.Interval = 30000; // 30 segundos
            timerActualizacion.Tick += (s, e) =>
            {
                if (actualizacionAutomatica)
                {
                    CargarSaldo();
                }
            };
            timerActualizacion.Start();
        }

        private void MostrarNotificacionTemporal(string mensaje, Color color)
        {

            Label lblNotificacion = new Label
            {
                Text = mensaje,
                Location = new Point(this.ClientSize.Width / 2 - 150, 90),
                Size = new Size(300, 30),
                Font = BankTheme.BodyFont,
                ForeColor = color,
                BackColor = Color.FromArgb(240, 253, 244), // Verde muy claro
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };

            this.Controls.Add(lblNotificacion);
            lblNotificacion.BringToFront();

            System.Windows.Forms.Timer timerNotificacion = new System.Windows.Forms.Timer();
            timerNotificacion.Interval = 3000;
            timerNotificacion.Tick += (s, e) =>
            {
                this.Controls.Remove(lblNotificacion);
                lblNotificacion.Dispose();
                timerNotificacion.Stop();
                timerNotificacion.Dispose();
            };
            timerNotificacion.Start();
        }

        private void ExportarPDF()
        {
            try
            {
                string query = "SELECT numero_cuenta, saldo, tipo_cuenta FROM cuentas WHERE id_cuenta = @id";
                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));

                if (dt.Rows.Count > 0)
                {
                    string numeroCuenta = dt.Rows[0]["numero_cuenta"].ToString();
                    decimal saldo = Convert.ToDecimal(dt.Rows[0]["saldo"]);
                    string tipoCuenta = dt.Rows[0]["tipo_cuenta"].ToString();

                    string contenido = $@"
╔═══════════════════════════════════════════════════════════╗
║           🏦 MÓDULO BANCO - REPORTE DE SALDO             ║
╚═══════════════════════════════════════════════════════════╝

INFORMACIÓN DEL USUARIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  Usuario:          {FormLogin.NombreUsuario}
  Rol:              {FormLogin.RolUsuario}
  Número de Cuenta: {numeroCuenta}
  Tipo de Cuenta:   {tipoCuenta}

SALDO DISPONIBLE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  💰 ${saldo:N2}

INFORMACIÓN DEL REPORTE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  Fecha de consulta:    {DateTime.Now:dd/MM/yyyy}
  Hora de consulta:     {DateTime.Now:HH:mm:ss} hrs
  Generado por:         {FormLogin.NombreUsuario}
  Última actualización: {lblActualizacion.Text}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
© 2025 Módulo Banco - Documento Confidencial
Este reporte es válido únicamente para la fecha indicada
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
";

                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "Archivo PDF (*.pdf)|*.pdf|Archivo de texto (*.txt)|*.txt",
                        FileName = $"Saldo_PDF_{numeroCuenta}_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                        Title = "Exportar Reporte de Saldo (PDF)"
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.WriteAllText(saveDialog.FileName, contenido);
                        MostrarNotificacionTemporal("✓ Archivo generado", BankTheme.Success);
                        CustomMessageBox.Show("Exportación Exitosa",
                            $"El reporte PDF se ha guardado correctamente en:\n{saveDialog.FileName}",
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Exportar PDF",
                    $"No se pudo exportar el reporte.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        private Panel CrearPanelFiltros()
        {
            Panel panelFiltros = BankTheme.CreateCard(50, 400, 800, 80);

            Label lblFiltros = new Label
            {
                Text = "Filtrar",
                Location = new Point(20, 15),
                Size = new Size(100, 25),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            Label lblPeriodo = new Label
            {
                Text = "Período:",
                Location = new Point(20, 45),
                Size = new Size(80, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            ComboBox cmbPeriodo = new ComboBox
            {
                Location = new Point(100, 43),
                Size = new Size(150, 25),
                Font = BankTheme.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPeriodo.Items.AddRange(new string[] { "Hoy", "Esta semana", "Este mes", "Este año", "Todo", "Personalizado" });
            cmbPeriodo.SelectedIndex = 0;

            DateTimePicker dtpInicio = new DateTimePicker
            {
                Location = new Point(270, 43),
                Size = new Size(150, 25),
                Font = BankTheme.BodyFont,
                Format = DateTimePickerFormat.Short,
                Visible = false
            };

            DateTimePicker dtpFin = new DateTimePicker
            {
                Location = new Point(440, 43),
                Size = new Size(150, 25),
                Font = BankTheme.BodyFont,
                Format = DateTimePickerFormat.Short,
                Visible = false
            };

            Label lblFiltroAplicado = new Label
            {
                Text = "",
                Location = new Point(610, 45),
                Size = new Size(170, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.Success
            };

            cmbPeriodo.SelectedIndexChanged += (s, e) =>
            {
                bool esPersonalizado = cmbPeriodo.SelectedItem.ToString() == "Personalizado";
                dtpInicio.Visible = esPersonalizado;
                dtpFin.Visible = esPersonalizado;

                if (!esPersonalizado)
                {
                    AplicarFiltroPeriodo(cmbPeriodo.SelectedItem.ToString(), lblFiltroAplicado);
                }
            };

            Button btnAplicarFiltro = new Button
            {
                Text = "Aplicar",
                Location = new Point(610, 40),
                Size = new Size(80, 30),
                Font = BankTheme.BodyFont,
                Visible = false
            };
            BankTheme.StyleButton(btnAplicarFiltro, true);
            btnAplicarFiltro.Click += (s, e) =>
            {
                if (cmbPeriodo.SelectedItem.ToString() == "Personalizado")
                {
                    AplicarFiltroPersonalizado(dtpInicio.Value, dtpFin.Value, lblFiltroAplicado);
                }
            };

            cmbPeriodo.SelectedIndexChanged += (s, e) =>
            {
                btnAplicarFiltro.Visible = cmbPeriodo.SelectedItem.ToString() == "Personalizado";
            };

            panelFiltros.Controls.AddRange(new Control[] { 
                lblFiltros, lblPeriodo, cmbPeriodo, dtpInicio, dtpFin, lblFiltroAplicado, btnAplicarFiltro 
            });

            return panelFiltros;
        }

        private void AplicarFiltroPeriodo(string periodo, Label lblFiltroAplicado)
        {
            DateTime fechaInicio = DateTime.Now;
            DateTime fechaFin = DateTime.Now;

            switch (periodo)
            {
                case "Hoy":
                    fechaInicio = DateTime.Today;
                    fechaFin = DateTime.Today.AddDays(1).AddSeconds(-1);
                    break;
                case "Esta semana":
                    fechaInicio = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    fechaFin = fechaInicio.AddDays(7).AddSeconds(-1);
                    break;
                case "Este mes":
                    fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    fechaFin = fechaInicio.AddMonths(1).AddSeconds(-1);
                    break;
                case "Este año":
                    fechaInicio = new DateTime(DateTime.Now.Year, 1, 1);
                    fechaFin = fechaInicio.AddYears(1).AddSeconds(-1);
                    break;
                case "Todo":
                    fechaInicio = DateTime.MinValue;
                    fechaFin = DateTime.MaxValue;
                    lblFiltroAplicado.Text = "Mostrando: Todo";
                    CargarSaldo();
                    return;
            }

            lblFiltroAplicado.Text = $"{fechaInicio:dd MMM} - {fechaFin:dd MMM}";
            CargarSaldo();
        }

        private void AplicarFiltroPersonalizado(DateTime inicio, DateTime fin, Label lblFiltroAplicado)
        {
            if (inicio > fin)
            {
                CustomMessageBox.Show("Fechas Inválidas",
                    "La fecha de inicio no puede ser mayor que la fecha de fin.",
                    MessageBoxIcon.Warning);
                return;
            }

            lblFiltroAplicado.Text = $"{inicio:dd MMM} - {fin:dd MMM}";
            CargarSaldo();
        }

        private void MostrarMensajeAccesoRestringido()
        {

            RegistrarIntentoAccesoNoAutorizado();

            Panel bannerRestriccion = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(this.ClientSize.Width, 60),
                BackColor = Color.FromArgb(254, 243, 199), // Amarillo claro
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblIcono = new Label
            {
                Text = "⚠",
                Location = new Point(15, 15),
                Size = new Size(30, 30),
                Font = new Font("Segoe UI", 20F),
                ForeColor = Color.FromArgb(146, 64, 14), // Amarillo oscuro
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblMensaje = new Label
            {
                Text = "Acceso restringido: solo ejecutivos de cuenta y gerentes pueden consultar saldos históricos.",
                Location = new Point(55, 10),
                Size = new Size(this.ClientSize.Width - 150, 20),
                Font = BankTheme.BodyFont,
                ForeColor = Color.FromArgb(146, 64, 14),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblRol = new Label
            {
                Text = $"Su rol actual: {FormLogin.RolUsuario}",
                Location = new Point(55, 32),
                Size = new Size(this.ClientSize.Width - 150, 18),
                Font = BankTheme.SmallFont,
                ForeColor = Color.FromArgb(146, 64, 14),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Button btnCerrarBanner = new Button
            {
                Text = "Aceptar",
                Location = new Point(this.ClientSize.Width - 100, 15),
                Size = new Size(80, 30),
                Font = BankTheme.BodyFont,
                BackColor = Color.FromArgb(217, 119, 6),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCerrarBanner.FlatAppearance.BorderSize = 0;
            btnCerrarBanner.Click += (s, e) => bannerRestriccion.Visible = false;

            bannerRestriccion.Controls.AddRange(new Control[] { lblIcono, lblMensaje, lblRol, btnCerrarBanner });
            this.Controls.Add(bannerRestriccion);
            bannerRestriccion.BringToFront();

            CustomMessageBox.Show("Acceso Restringido",
                $"Acceso restringido: consulte con un ejecutivo de cuenta o gerente para acceder a saldos históricos.\n\nSu rol actual: {FormLogin.RolUsuario}\n\nRoles con acceso:\n• Ejecutivo\n• Gerente\n• Administrador",
                MessageBoxIcon.Warning);
        }

        private void RegistrarIntentoAccesoNoAutorizado()
        {
            try
            {

                string logQuery = @"INSERT INTO logs_auditoria (id_usuario, accion, detalle, fecha) 
                                   VALUES (@id, @accion, @detalle, @fecha)";

                Database.ExecuteNonQuery(logQuery,
                    new NpgsqlParameter("@id", FormLogin.IdUsuarioActual),
                    new NpgsqlParameter("@accion", "ACCESO_DENEGADO_HISTORICO"),
                    new NpgsqlParameter("@detalle", $"Usuario {FormLogin.NombreUsuario} (Rol: {FormLogin.RolUsuario}) intentó acceder a saldos históricos"),
                    new NpgsqlParameter("@fecha", DateTime.Now));
            }
            catch
            {

            }
        }

        private void ExportarWord()
        {
            if (!RoleManager.PuedeExportarCompleto(FormLogin.RolUsuario))
            {
                MostrarMensajeAccesoRestringido();
                return;
            }

            try
            {
                string query = "SELECT numero_cuenta, saldo, tipo_cuenta FROM cuentas WHERE id_cuenta = @id";
                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));

                if (dt.Rows.Count > 0)
                {
                    string numeroCuenta = dt.Rows[0]["numero_cuenta"].ToString();
                    decimal saldo = Convert.ToDecimal(dt.Rows[0]["saldo"]);
                    string tipoCuenta = dt.Rows[0]["tipo_cuenta"].ToString();

                    string contenido = $@"
MÓDULO BANCO - REPORTE DETALLADO DE SALDO
===========================================

INFORMACIÓN DE LA CUENTA
-------------------------
Usuario:          {FormLogin.NombreUsuario}
Rol:              {FormLogin.RolUsuario}
Número de Cuenta: {numeroCuenta}
Tipo de Cuenta:   {tipoCuenta}

SALDO DISPONIBLE
-----------------
${saldo:N2}

INFORMACIÓN DEL REPORTE
-----------------------
Fecha de consulta:    {DateTime.Now:dd/MM/yyyy}
Hora de consulta:     {DateTime.Now:HH:mm:ss} hrs
Reporte generado por: {FormLogin.NombreUsuario}
Última actualización: {lblActualizacion.Text}

FILTROS APLICADOS
-----------------
(Ninguno - Saldo actual)

-------------------------------------------
© 2025 Módulo Banco
Este documento es confidencial y de uso exclusivo
del titular de la cuenta.
===========================================
";

                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "Documento de Word (*.doc)|*.doc|Archivo de texto (*.txt)|*.txt",
                        FileName = $"Saldo_Word_{numeroCuenta}_{DateTime.Now:yyyyMMdd_HHmmss}.doc",
                        Title = "Exportar Reporte Word"
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.WriteAllText(saveDialog.FileName, contenido);
                        MostrarNotificacionTemporal("✓ Archivo generado", BankTheme.Success);
                        CustomMessageBox.Show("Exportación Exitosa",
                            $"El reporte Word se ha guardado correctamente en:\n{saveDialog.FileName}",
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Exportar Word",
                    $"No se pudo exportar el reporte.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        private void ExportarExcel()
        {
            if (!RoleManager.PuedeExportarCompleto(FormLogin.RolUsuario))
            {
                MostrarMensajeAccesoRestringido();
                return;
            }

            try
            {
                string query = "SELECT numero_cuenta, saldo, tipo_cuenta FROM cuentas WHERE id_cuenta = @id";
                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));

                if (dt.Rows.Count > 0)
                {
                    string numeroCuenta = dt.Rows[0]["numero_cuenta"].ToString();
                    decimal saldo = Convert.ToDecimal(dt.Rows[0]["saldo"]);
                    string tipoCuenta = dt.Rows[0]["tipo_cuenta"].ToString();

                    string contenido = $@"MÓDULO BANCO - REPORTE DE SALDO
Fecha de generación,{DateTime.Now:dd/MM/yyyy HH:mm:ss}

INFORMACIÓN DE LA CUENTA
Campo,Valor
Usuario,{FormLogin.NombreUsuario}
Rol,{FormLogin.RolUsuario}
Número de Cuenta,{numeroCuenta}
Tipo de Cuenta,{tipoCuenta}

SALDO
Concepto,Monto
Saldo Disponible,${saldo:N2}

INFORMACIÓN DEL REPORTE
Campo,Valor
Fecha de consulta,{DateTime.Now:dd/MM/yyyy}
Hora de consulta,{DateTime.Now:HH:mm:ss}
Generado por,{FormLogin.NombreUsuario}
Última actualización,{lblActualizacion.Text}

© 2025 Módulo Banco - Documento Confidencial
";

                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "Archivo Excel (*.csv)|*.csv|Todos los archivos (*.*)|*.*",
                        FileName = $"Saldo_Excel_{numeroCuenta}_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                        Title = "Exportar Reporte Excel"
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.WriteAllText(saveDialog.FileName, contenido);
                        MostrarNotificacionTemporal("✓ Archivo generado", BankTheme.Success);
                        CustomMessageBox.Show("Exportación Exitosa",
                            $"El reporte Excel se ha guardado correctamente en:\n{saveDialog.FileName}\n\nPuede abrirlo con Microsoft Excel o cualquier hoja de cálculo.",
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Exportar Excel",
                    $"No se pudo exportar el reporte.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timerActualizacion?.Stop();
            timerActualizacion?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
