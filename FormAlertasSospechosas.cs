using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaBanco
{

    public partial class FormAlertasSospechosas : Form
    {
        private DataGridView dgvAlertas;
        private DateTimePicker dtpFechaInicio;
        private DateTimePicker dtpFechaFin;
        private TextBox txtBuscarID;
        private TextBox txtBuscarNombre;
        private ComboBox cmbEstado;
        private Button btnBuscar;
        private DateTimePicker dtpExpiracion;
        private Button btnAplicarExpiracion;
        private Button btnExportarExcel;
        private Button btnExportarWord;
        private Button btnExportarPDF;
        private Label lblTotalAlertas;

        private int paginaActual = 1;
        private int registrosPorPagina = 25;
        private int totalPaginas = 0;

        public FormAlertasSospechosas()
        {
            InitializeComponent();
            CargarAlertas();

            AuditLogger.Log(AuditLogger.AuditAction.ConsultaHistorial,
                "Acceso al módulo de alertas sospechosas");
        }

        private void InitializeComponent()
        {
            this.Text = "Alertas de Actividad Sospechosa";
            this.ClientSize = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Panel headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1400, 80),
                BackColor = Color.FromArgb(220, 53, 69) // Rojo para alertas
            };

            Button btnVolver = HomeButton.Create(this);
            btnVolver.Location = new Point(20, 20);

            Label lblTitulo = new Label
            {
                Text = "⚠️ Alertas de Actividad Sospechosa",
                Location = new Point(120, 20),
                Size = new Size(600, 40),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = BankTheme.White
            };

            headerPanel.Controls.AddRange(new Control[] { btnVolver, lblTitulo });

            Panel panelFiltros = BankTheme.CreateCard(20, 100, 1360, 140);
            CrearFiltros(panelFiltros);

            dgvAlertas = new DataGridView
            {
                Location = new Point(20, 260),
                Size = new Size(1360, 480),
                Font = BankTheme.BodyFont,
                BackgroundColor = BankTheme.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvAlertas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 53, 69);
            dgvAlertas.ColumnHeadersDefaultCellStyle.ForeColor = BankTheme.White;
            dgvAlertas.ColumnHeadersDefaultCellStyle.Font = BankTheme.HeaderFont;
            dgvAlertas.EnableHeadersVisualStyles = false;
            dgvAlertas.CellContentClick += DgvAlertas_CellContentClick;

            this.Controls.AddRange(new Control[] { headerPanel, panelFiltros, dgvAlertas });
        }

        private void CrearFiltros(Panel panel)
        {
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
                Size = new Size(130, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(-30)
            };

            Label lblFechaFin = new Label
            {
                Text = "Fecha Fin:",
                Location = new Point(270, 20),
                Size = new Size(80, 25),
                Font = BankTheme.BodyFont
            };

            dtpFechaFin = new DateTimePicker
            {
                Location = new Point(350, 18),
                Size = new Size(130, 25),
                Format = DateTimePickerFormat.Short
            };

            Label lblID = new Label
            {
                Text = "ID Alerta:",
                Location = new Point(500, 20),
                Size = new Size(80, 25),
                Font = BankTheme.BodyFont
            };

            txtBuscarID = new TextBox
            {
                Location = new Point(580, 18),
                Size = new Size(100, 25),
                PlaceholderText = "ID..."
            };

            Label lblNombre = new Label
            {
                Text = "Nombre:",
                Location = new Point(700, 20),
                Size = new Size(70, 25),
                Font = BankTheme.BodyFont
            };

            txtBuscarNombre = new TextBox
            {
                Location = new Point(770, 18),
                Size = new Size(200, 25),
                PlaceholderText = "Nombre del titular..."
            };

            Label lblEstado = new Label
            {
                Text = "Estado:",
                Location = new Point(990, 20),
                Size = new Size(60, 25),
                Font = BankTheme.BodyFont
            };

            cmbEstado = new ComboBox
            {
                Location = new Point(1050, 18),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbEstado.Items.AddRange(new object[] { "Todos", "Abierta", "En revisión", "Escalada", "Cerrada" });
            cmbEstado.SelectedIndex = 0;

            btnBuscar = new Button
            {
                Text = "🔍 Buscar",
                Location = new Point(1220, 15),
                Size = new Size(120, 30)
            };
            BankTheme.StyleButton(btnBuscar, true);
            btnBuscar.Click += (s, e) => CargarAlertas();

            Label lblExpiracion = new Label
            {
                Text = "Tiempo de expiración:",
                Location = new Point(20, 70),
                Size = new Size(150, 25),
                Font = BankTheme.BodyFont
            };

            dtpExpiracion = new DateTimePicker
            {
                Location = new Point(170, 68),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(7)
            };

            btnAplicarExpiracion = new Button
            {
                Text = "✓ Aplicar",
                Location = new Point(330, 65),
                Size = new Size(100, 30)
            };
            BankTheme.StyleButton(btnAplicarExpiracion, true);
            btnAplicarExpiracion.Click += BtnAplicarExpiracion_Click;

            lblTotalAlertas = new Label
            {
                Text = "Total: 0 alertas",
                Location = new Point(450, 70),
                Size = new Size(200, 25),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            btnExportarExcel = new Button
            {
                Text = "📊 Excel",
                Location = new Point(1050, 65),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(33, 115, 70)
            };
            BankTheme.StyleButton(btnExportarExcel, true);
            btnExportarExcel.BackColor = Color.FromArgb(33, 115, 70);

            btnExportarWord = new Button
            {
                Text = "📝 Word",
                Location = new Point(1150, 65),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(0, 112, 192)
            };
            BankTheme.StyleButton(btnExportarWord, true);
            btnExportarWord.BackColor = Color.FromArgb(0, 112, 192);

            btnExportarPDF = new Button
            {
                Text = "📄 PDF",
                Location = new Point(1250, 65),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(220, 53, 69)
            };
            BankTheme.StyleButton(btnExportarPDF, true);
            btnExportarPDF.BackColor = Color.FromArgb(220, 53, 69);

            panel.Controls.AddRange(new Control[] {
                lblFechaInicio, dtpFechaInicio, lblFechaFin, dtpFechaFin,
                lblID, txtBuscarID, lblNombre, txtBuscarNombre,
                lblEstado, cmbEstado, btnBuscar,
                lblExpiracion, dtpExpiracion, btnAplicarExpiracion,
                lblTotalAlertas, btnExportarExcel, btnExportarWord, btnExportarPDF
            });
        }

        private void CargarAlertas()
        {
            try
            {
                string whereClause = "WHERE 1=1";

                whereClause += $" AND fecha_alerta >= '{dtpFechaInicio.Value:yyyy-MM-dd}'";
                whereClause += $" AND fecha_alerta <= '{dtpFechaFin.Value:yyyy-MM-dd 23:59:59}'";

                if (!string.IsNullOrWhiteSpace(txtBuscarID.Text))
                    whereClause += $" AND id_alerta = {txtBuscarID.Text}";

                if (!string.IsNullOrWhiteSpace(txtBuscarNombre.Text))
                    whereClause += $" AND nombre_titular ILIKE '%{txtBuscarNombre.Text}%'";

                if (cmbEstado.SelectedIndex > 0)
                    whereClause += $" AND estado = '{cmbEstado.SelectedItem}'";

                string query = $@"
                    SELECT 
                        id_alerta,
                        nombre_titular,
                        monto,
                        estado,
                        TO_CHAR(fecha_alerta, 'DD/MM/YYYY HH24:MI') as fecha,
                        tipo_alerta,
                        descripcion
                    FROM alertas_sospechosas
                    {whereClause}
                    ORDER BY fecha_alerta DESC";

                DataTable dt = Database.ExecuteQuery(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dgvAlertas.DataSource = dt;
                    ConfigurarColumnas();
                    AgregarBotonesAccion();
                    lblTotalAlertas.Text = $"Total: {dt.Rows.Count} alerta(s)";
                }
                else
                {
                    dgvAlertas.DataSource = null;
                    lblTotalAlertas.Text = "Total: 0 alertas";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar alertas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColumnas()
        {
            dgvAlertas.Columns["id_alerta"].HeaderText = "ID";
            dgvAlertas.Columns["id_alerta"].Width = 60;
            dgvAlertas.Columns["nombre_titular"].HeaderText = "Titular";
            dgvAlertas.Columns["nombre_titular"].Width = 200;
            dgvAlertas.Columns["monto"].HeaderText = "Monto";
            dgvAlertas.Columns["monto"].Width = 120;
            dgvAlertas.Columns["monto"].DefaultCellStyle.Format = "C2";
            dgvAlertas.Columns["estado"].HeaderText = "Estado";
            dgvAlertas.Columns["estado"].Width = 120;
            dgvAlertas.Columns["fecha"].HeaderText = "Fecha";
            dgvAlertas.Columns["fecha"].Width = 150;
            dgvAlertas.Columns["tipo_alerta"].Visible = false;
            dgvAlertas.Columns["descripcion"].Visible = false;
        }

        private void AgregarBotonesAccion()
        {
            if (dgvAlertas.Columns.Contains("btnDetalle"))
                dgvAlertas.Columns.Remove("btnDetalle");

            DataGridViewButtonColumn btnDetalle = new DataGridViewButtonColumn
            {
                Name = "btnDetalle",
                HeaderText = "Detalle",
                Text = "👁️ Ver",
                UseColumnTextForButtonValue = true,
                Width = 100
            };
            dgvAlertas.Columns.Add(btnDetalle);
        }

        private void DgvAlertas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvAlertas.Columns[e.ColumnIndex].Name == "btnDetalle")
            {
                int idAlerta = Convert.ToInt32(dgvAlertas.Rows[e.RowIndex].Cells["id_alerta"].Value);
                MostrarDetalleAlerta(idAlerta);
            }
        }

        private void MostrarDetalleAlerta(int idAlerta)
        {

            MessageBox.Show($"Detalle de alerta {idAlerta} - Por implementar",
                "Detalle", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnAplicarExpiracion_Click(object sender, EventArgs e)
        {
            if (dgvAlertas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una alerta primero.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idAlerta = Convert.ToInt32(dgvAlertas.SelectedRows[0].Cells["id_alerta"].Value);

            try
            {
                string query = $@"
                    UPDATE alertas_sospechosas 
                    SET fecha_expiracion = '{dtpExpiracion.Value:yyyy-MM-dd}'
                    WHERE id_alerta = {idAlerta}";

                Database.ExecuteNonQuery(query);

                MessageBox.Show("Fecha de expiración actualizada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarAlertas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
