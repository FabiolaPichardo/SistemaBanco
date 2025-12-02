using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormHistorial : Form
    {
        private DataGridView dgvMovimientos;

        public FormHistorial()
        {
            InitializeComponent();
            CargarMovimientos();
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Historial de Movimientos";
            this.ClientSize = new System.Drawing.Size(1100, 660);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Header
            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(1100, 80),
                BackColor = BankTheme.PrimaryBlue
            };

            Label lblTitulo = new Label
            {
                Text = "📊 HISTORIAL DE MOVIMIENTOS",
                Location = new System.Drawing.Point(350, 25),
                Size = new System.Drawing.Size(400, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.Add(lblTitulo);

            // Card con DataGridView
            Panel mainCard = BankTheme.CreateCard(30, 110, 1040, 450);

            dgvMovimientos = new DataGridView
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(1020, 430),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = BankTheme.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BankTheme.PrimaryBlue,
                    ForeColor = BankTheme.White,
                    Font = BankTheme.HeaderFont,
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = BankTheme.BodyFont,
                    SelectionBackColor = BankTheme.SecondaryBlue,
                    SelectionForeColor = BankTheme.White
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BankTheme.LightGray
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            mainCard.Controls.Add(dgvMovimientos);

            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new System.Drawing.Point(450, 590),
                Size = new System.Drawing.Size(200, 50)
            };
            BankTheme.StyleButton(btnCerrar, false);
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, mainCard, btnCerrar });
        }

        private void CargarMovimientos()
        {
            try
            {
                string query = @"SELECT 
                                    fecha::date as Fecha,
                                    tipo as Tipo,
                                    monto as Monto,
                                    concepto as Concepto,
                                    saldo_anterior as ""Saldo Anterior"",
                                    saldo_nuevo as ""Saldo Nuevo""
                                FROM movimientos 
                                WHERE id_cuenta = @id 
                                ORDER BY fecha DESC";
                
                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));
                dgvMovimientos.DataSource = dt;

                // Formato de columnas
                if (dgvMovimientos.Columns.Count > 0)
                {
                    dgvMovimientos.Columns["Monto"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.Columns["Saldo Anterior"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.Columns["Saldo Nuevo"].DefaultCellStyle.Format = "C2";
                    dgvMovimientos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
