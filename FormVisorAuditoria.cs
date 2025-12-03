using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace SistemaBanco
{
    /// <summary>
    /// Visor de Auditoría del Sistema (BAN-56)
    /// </summary>
    public partial class FormVisorAuditoria : Form
    {
        private DataGridView dgvAuditoria;
        private DateTimePicker dtpFechaInicio;
        private DateTimePicker dtpFechaFin;
        private ComboBox cmbUsuario;
        private ComboBox cmbAccion;
        private TextBox txtBusqueda;
        private Button btnBuscar;
        private Button btnLimpiar;
        private Button btnExportarPDF;
        private Button btnExportarWord;
        private Button btnExportarExcel;
        private Label lblTotalRegistros;
        private Button btnAnterior;
        private Button btnSiguiente;
        private Label lblPaginacion;
        
        private int paginaActual = 1;
        private int registrosPorPagina = 50;
        private int totalPaginas = 0;

        public FormVisorAuditoria()
        {
            InitializeComponent();
            CargarFiltros();
            CargarAuditoria();
            
            // Registrar acceso al visor
            AuditLogger.Log(AuditLogger.AuditAction.ConsultaHistorial, 
                "Acceso al visor de auditoría");
        }

        private void InitializeComponent()
        {
            this.Text = "Visor de Auditoría del Sistema";
            this.ClientSize = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Header
            Panel headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1400, 80),
                BackColor = BankTheme.PrimaryBlue
            };

            Button btnVolver = HomeButton.Create(this);
            btnVolver.Location = new Point(20, 20);

            Label lblTitulo = new Label
            {
                Text = "🔍 Visor de Auditoría del Sistema",
                Location = new Point(120, 20),
                Size = new Size(600, 40),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = BankTheme.White
            };

            headerPanel.Controls.AddRange(new Control[] { btnVolver, lblTitulo });

            // Panel de filtros
            Panel panelFiltros = BankTheme.CreateCard(20, 100, 1360, 120);

            Label lblFechaInicio = new Label
            {
                Text = "Fecha Inicio:",
                Location = new Point(20, 20),
                Size = new Size(100, 25),
                Font = BankTheme.BodyFont
            };

            dtpFechaInicio = new DateTimePicker
            {
                Location = new Point(120, 18),
                Size = new Size(150, 25),
                Font = BankTheme.BodyFont,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(-30)
            };

            Label lblFechaFin = new Label
            {
                Text = "Fecha Fin:",
                Location = new Point(290, 20),
                Size = new Size(80, 25),
                Font = BankTheme.BodyFont
            };

            dtpFechaFin = new DateTimePicker
            {
                Location = new Point(370, 18),
                Size = new Size(150, 25),
                Font = BankTheme.BodyFont,
                Format = DateTimePickerFormat.Short
            };

            Label lblUsuario = new Label
            {
                Text = "Usuario:",
                Location = new Point(540, 20),
                Size = new Size(70, 25),
                Font = BankTheme.BodyFont
            };

            cmbUsuario = new ComboBox
            {
                Location = new Point(610, 18),
                Size = new Size(150, 25),
                Font = BankTheme.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblAccion = new Label
            {
                Text = "Acción:",
                Location = new Point(780, 20),
                Size = new Size(60, 25),
                Font = BankTheme.BodyFont
            };

            cmbAccion = new ComboBox
            {
                Location = new Point(840, 18),
                Size = new Size(180, 25),
                Font = BankTheme.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblBusqueda = new Label
            {
                Text = "Búsqueda:",
                Location = new Point(20, 60),
                Size = new Size(100, 25),
                Font = BankTheme.BodyFont
            };

            txtBusqueda = new TextBox
            {
                Location = new Point(120, 58),
                Size = new Size(300, 25),
                Font = BankTheme.BodyFont,
                PlaceholderText = "Buscar en detalles..."
            };

            btnBuscar = new Button
            {
                Text = "🔍 Buscar",
                Location = new Point(440, 55),
                Size = new Size(120, 30)
            };
            BankTheme.StyleButton(btnBuscar, true);
            btnBuscar.Click += (s, e) => CargarAuditoria();

            btnLimpiar = new Button
            {
                Text = "🔄 Limpiar",
                Location = new Point(580, 55),
                Size = new Size(120, 30)
            };
            BankTheme.StyleButton(btnLimpiar, false);
            btnLimpiar.Click += BtnLimpiar_Click;

            lblTotalRegistros = new Label
            {
                Text = "Total: 0 registros",
                Location = new Point(1050, 20),
                Size = new Size(280, 25),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleRight
            };

            // Botones de exportación
            btnExportarPDF = new Button
            {
                Text = "📄 PDF",
                Location = new Point(1050, 55),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(220, 53, 69)
            };
            BankTheme.StyleButton(btnExportarPDF, true);
            btnExportarPDF.BackColor = Color.FromArgb(220, 53, 69);
            btnExportarPDF.Click += (s, e) => ExportarDatos("PDF");

            btnExportarWord = new Button
            {
                Text = "📝 Word",
                Location = new Point(1150, 55),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(0, 112, 192)
            };
            BankTheme.StyleButton(btnExportarWord, true);
            btnExportarWord.BackColor = Color.FromArgb(0, 112, 192);
            btnExportarWord.Click += (s, e) => ExportarDatos("Word");

            btnExportarExcel = new Button
            {
                Text = "📊 Excel",
                Location = new Point(1250, 55),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(33, 115, 70)
            };
            BankTheme.StyleButton(btnExportarExcel, true);
            btnExportarExcel.BackColor = Color.FromArgb(33, 115, 70);
            btnExportarExcel.Click += (s, e) => ExportarDatos("Excel");

            panelFiltros.Controls.AddRange(new Control[] {
                lblFechaInicio, dtpFechaInicio, lblFechaFin, dtpFechaFin,
                lblUsuario, cmbUsuario, lblAccion, cmbAccion,
                lblBusqueda, txtBusqueda, btnBuscar, btnLimpiar,
                lblTotalRegistros, btnExportarPDF, btnExportarWord, btnExportarExcel
            });

            // DataGridView
            dgvAuditoria = new DataGridView
            {
                Location = new Point(20, 240),
                Size = new Size(1360, 480),
                Font = BankTheme.BodyFont,
                BackgroundColor = BankTheme.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 30 }
            };

            dgvAuditoria.ColumnHeadersDefaultCellStyle.BackColor = BankTheme.PrimaryBlue;
            dgvAuditoria.ColumnHeadersDefaultCellStyle.ForeColor = BankTheme.White;
            dgvAuditoria.ColumnHeadersDefaultCellStyle.Font = BankTheme.HeaderFont;
            dgvAuditoria.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAuditoria.EnableHeadersVisualStyles = false;
            dgvAuditoria.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            // Panel de paginación
            Panel panelPaginacion = new Panel
            {
                Location = new Point(20, 730),
                Size = new Size(1360, 50),
                BackColor = BankTheme.White
            };

            btnAnterior = new Button
            {
                Text = "◀ Anterior",
                Location = new Point(20, 10),
                Size = new Size(120, 30)
            };
            BankTheme.StyleButton(btnAnterior, false);
            btnAnterior.Click += (s, e) => CambiarPagina(-1);

            lblPaginacion = new Label
            {
                Text = "Página 1 de 1",
                Location = new Point(580, 10),
                Size = new Size(200, 30),
                Font = BankTheme.BodyFont,
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnSiguiente = new Button
            {
                Text = "Siguiente ▶",
                Location = new Point(1220, 10),
                Size = new Size(120, 30)
            };
            BankTheme.StyleButton(btnSiguiente, false);
            btnSiguiente.Click += (s, e) => CambiarPagina(1);

            panelPaginacion.Controls.AddRange(new Control[] { btnAnterior, lblPaginacion, btnSiguiente });

            this.Controls.AddRange(new Control[] {
                headerPanel, panelFiltros, dgvAuditoria, panelPaginacion
            });
        }

        private void CargarFiltros()
        {
            try
            {
                // Cargar usuarios
                cmbUsuario.Items.Add("Todos");
                string queryUsuarios = "SELECT DISTINCT usuario FROM auditoria_sistema ORDER BY usuario";
                DataTable dtUsuarios = Database.ExecuteQuery(queryUsuarios);
                if (dtUsuarios != null)
                {
                    foreach (DataRow row in dtUsuarios.Rows)
                    {
                        cmbUsuario.Items.Add(row["usuario"].ToString());
                    }
                }
                cmbUsuario.SelectedIndex = 0;

                // Cargar acciones
                cmbAccion.Items.Add("Todas");
                foreach (var action in Enum.GetValues(typeof(AuditLogger.AuditAction)))
                {
                    cmbAccion.Items.Add(action.ToString());
                }
                cmbAccion.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar filtros: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarAuditoria()
        {
            try
            {
                // Construir query con filtros
                StringBuilder whereClause = new StringBuilder("WHERE 1=1");
                
                // Filtro de fechas
                whereClause.Append($" AND fecha_hora >= '{dtpFechaInicio.Value:yyyy-MM-dd 00:00:00}'");
                whereClause.Append($" AND fecha_hora <= '{dtpFechaFin.Value:yyyy-MM-dd 23:59:59}'");

                // Filtro de usuario
                if (cmbUsuario.SelectedIndex > 0)
                {
                    whereClause.Append($" AND usuario = '{cmbUsuario.SelectedItem}'");
                }

                // Filtro de acción
                if (cmbAccion.SelectedIndex > 0)
                {
                    whereClause.Append($" AND accion = '{cmbAccion.SelectedItem}'");
                }

                // Filtro de búsqueda
                if (!string.IsNullOrWhiteSpace(txtBusqueda.Text))
                {
                    string busqueda = txtBusqueda.Text.Trim();
                    whereClause.Append($" AND (detalles ILIKE '%{busqueda}%' OR " +
                                     $"ip_address ILIKE '%{busqueda}%' OR " +
                                     $"nombre_equipo ILIKE '%{busqueda}%')");
                }

                // Calcular total de registros
                string queryCount = $"SELECT COUNT(*) FROM auditoria_sistema {whereClause}";
                object resultCount = Database.ExecuteScalar(queryCount);
                int totalRegistros = resultCount != null ? Convert.ToInt32(resultCount) : 0;
                
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
                if (totalPaginas == 0) totalPaginas = 1;
                if (paginaActual > totalPaginas) paginaActual = totalPaginas;

                // Query principal con paginación
                int offset = (paginaActual - 1) * registrosPorPagina;
                string query = $@"
                    SELECT 
                        id_auditoria,
                        usuario,
                        email,
                        accion,
                        detalles,
                        TO_CHAR(fecha_hora, 'DD/MM/YYYY HH24:MI:SS') as fecha_hora,
                        ip_address,
                        nombre_equipo,
                        tipo_movimiento
                    FROM auditoria_sistema
                    {whereClause}
                    ORDER BY fecha_hora DESC
                    LIMIT {registrosPorPagina} OFFSET {offset}";

                DataTable dt = Database.ExecuteQuery(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dgvAuditoria.DataSource = dt;
                    ConfigurarColumnas();
                }
                else
                {
                    dgvAuditoria.DataSource = null;
                    MessageBox.Show("No se encontraron registros con los filtros aplicados.",
                        "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Actualizar labels
                lblTotalRegistros.Text = $"Total: {totalRegistros} registro(s)";
                lblPaginacion.Text = $"Página {paginaActual} de {totalPaginas}";
                btnAnterior.Enabled = paginaActual > 1;
                btnSiguiente.Enabled = paginaActual < totalPaginas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar auditoría: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColumnas()
        {
            if (dgvAuditoria.Columns.Count == 0) return;

            dgvAuditoria.Columns["id_auditoria"].HeaderText = "ID";
            dgvAuditoria.Columns["id_auditoria"].Width = 60;
            
            dgvAuditoria.Columns["usuario"].HeaderText = "Usuario";
            dgvAuditoria.Columns["usuario"].Width = 120;
            
            dgvAuditoria.Columns["email"].HeaderText = "Email";
            dgvAuditoria.Columns["email"].Width = 180;
            
            dgvAuditoria.Columns["accion"].HeaderText = "Acción";
            dgvAuditoria.Columns["accion"].Width = 150;
            
            dgvAuditoria.Columns["detalles"].HeaderText = "Detalles";
            dgvAuditoria.Columns["detalles"].Width = 300;
            
            dgvAuditoria.Columns["fecha_hora"].HeaderText = "Fecha y Hora";
            dgvAuditoria.Columns["fecha_hora"].Width = 150;
            
            dgvAuditoria.Columns["ip_address"].HeaderText = "IP";
            dgvAuditoria.Columns["ip_address"].Width = 120;
            
            dgvAuditoria.Columns["nombre_equipo"].HeaderText = "Equipo";
            dgvAuditoria.Columns["nombre_equipo"].Width = 150;
            
            dgvAuditoria.Columns["tipo_movimiento"].HeaderText = "Tipo Mov.";
            dgvAuditoria.Columns["tipo_movimiento"].Width = 100;
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            dtpFechaInicio.Value = DateTime.Now.AddDays(-30);
            dtpFechaFin.Value = DateTime.Now;
            cmbUsuario.SelectedIndex = 0;
            cmbAccion.SelectedIndex = 0;
            txtBusqueda.Clear();
            paginaActual = 1;
            CargarAuditoria();
        }

        private void CambiarPagina(int direccion)
        {
            int nuevaPagina = paginaActual + direccion;
            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginas)
            {
                paginaActual = nuevaPagina;
                CargarAuditoria();
            }
        }

        private void ExportarDatos(string formato)
        {
            try
            {
                // Obtener todos los datos (sin paginación)
                StringBuilder whereClause = new StringBuilder("WHERE 1=1");
                
                whereClause.Append($" AND fecha_hora >= '{dtpFechaInicio.Value:yyyy-MM-dd 00:00:00}'");
                whereClause.Append($" AND fecha_hora <= '{dtpFechaFin.Value:yyyy-MM-dd 23:59:59}'");

                if (cmbUsuario.SelectedIndex > 0)
                    whereClause.Append($" AND usuario = '{cmbUsuario.SelectedItem}'");

                if (cmbAccion.SelectedIndex > 0)
                    whereClause.Append($" AND accion = '{cmbAccion.SelectedItem}'");

                if (!string.IsNullOrWhiteSpace(txtBusqueda.Text))
                {
                    string busqueda = txtBusqueda.Text.Trim();
                    whereClause.Append($" AND (detalles ILIKE '%{busqueda}%' OR " +
                                     $"ip_address ILIKE '%{busqueda}%' OR " +
                                     $"nombre_equipo ILIKE '%{busqueda}%')");
                }

                string query = $@"
                    SELECT * FROM auditoria_sistema
                    {whereClause}
                    ORDER BY fecha_hora DESC";

                DataTable dt = Database.ExecuteQuery(query);

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = "";
                string content = "";

                switch (formato)
                {
                    case "PDF":
                        fileName = $"auditoria_{timestamp}.txt";
                        content = GenerarContenidoPDF(dt);
                        break;
                    case "Word":
                        fileName = $"auditoria_{timestamp}.doc";
                        content = GenerarContenidoWord(dt);
                        break;
                    case "Excel":
                        fileName = $"auditoria_{timestamp}.csv";
                        content = GenerarContenidoExcel(dt);
                        break;
                }

                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                File.WriteAllText(filePath, content, Encoding.UTF8);

                // Registrar exportación
                AuditLogger.Log(AuditLogger.AuditAction.ExportacionDatos,
                    $"Exportación de auditoría a {formato}: {dt.Rows.Count} registros");

                MessageBox.Show($"Archivo exportado exitosamente:\n{filePath}",
                    "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                System.Diagnostics.Process.Start("notepad.exe", filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerarContenidoPDF(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("           REPORTE DE AUDITORÍA DEL SISTEMA");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Período: {dtpFechaInicio.Value:dd/MM/yyyy} - {dtpFechaFin.Value:dd/MM/yyyy}");
            sb.AppendLine($"Total de registros: {dt.Rows.Count}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine();

            foreach (DataRow row in dt.Rows)
            {
                sb.AppendLine($"ID: {row["id_auditoria"]}");
                sb.AppendLine($"Usuario: {row["usuario"]}");
                sb.AppendLine($"Email: {row["email"]}");
                sb.AppendLine($"Acción: {row["accion"]}");
                sb.AppendLine($"Detalles: {row["detalles"]}");
                sb.AppendLine($"Fecha/Hora: {row["fecha_hora"]}");
                sb.AppendLine($"IP: {row["ip_address"]}");
                sb.AppendLine($"Equipo: {row["nombre_equipo"]}");
                sb.AppendLine($"Tipo Movimiento: {row["tipo_movimiento"]}");
                sb.AppendLine("───────────────────────────────────────────────────────────────");
            }

            return sb.ToString();
        }

        private string GenerarContenidoWord(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("REPORTE DE AUDITORÍA DEL SISTEMA");
            sb.AppendLine($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Período: {dtpFechaInicio.Value:dd/MM/yyyy} - {dtpFechaFin.Value:dd/MM/yyyy}");
            sb.AppendLine();

            foreach (DataRow row in dt.Rows)
            {
                sb.AppendLine($"{row["id_auditoria"]}\t{row["usuario"]}\t{row["accion"]}\t{row["fecha_hora"]}\t{row["ip_address"]}");
            }

            return sb.ToString();
        }

        private string GenerarContenidoExcel(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            
            // Encabezados
            sb.AppendLine("ID,Usuario,Email,Acción,Detalles,Fecha/Hora,IP,Equipo,Tipo Movimiento");

            // Datos
            foreach (DataRow row in dt.Rows)
            {
                sb.AppendLine($"{row["id_auditoria"]}," +
                            $"\"{row["usuario"]}\"," +
                            $"\"{row["email"]}\"," +
                            $"\"{row["accion"]}\"," +
                            $"\"{row["detalles"]}\"," +
                            $"\"{row["fecha_hora"]}\"," +
                            $"\"{row["ip_address"]}\"," +
                            $"\"{row["nombre_equipo"]}\"," +
                            $"\"{row["tipo_movimiento"]}\"");
            }

            return sb.ToString();
        }
    }
}
