using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormMovimiento : Form
    {
        private ComboBox cmbTipo;
        private TextBox txtMonto;
        private TextBox txtConcepto;

        public FormMovimiento()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Registrar Movimiento";
            this.ClientSize = new System.Drawing.Size(600, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(600, 80),
                BackColor = BankTheme.PrimaryBlue
            };

            Label lblTitulo = new Label
            {
                Text = "💳 REGISTRAR MOVIMIENTO",
                Location = new System.Drawing.Point(120, 25),
                Size = new System.Drawing.Size(360, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            HomeButton.AddToForm(this, headerPanel);

            headerPanel.Controls.Add(lblTitulo);

            Panel mainCard = BankTheme.CreateCard(50, 110, 500, 350);

            Label lblTipo = new Label
            {
                Text = "Tipo de Movimiento",
                Location = new System.Drawing.Point(40, 30),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            cmbTipo = new ComboBox
            {
                Location = new System.Drawing.Point(40, 55),
                Size = new System.Drawing.Size(420, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = BankTheme.BodyFont
            };
            cmbTipo.Items.AddRange(new string[] { "DEPOSITO", "RETIRO", "CARGO", "ABONO" });
            cmbTipo.SelectedIndex = 0;

            Label lblMonto = new Label
            {
                Text = "Monto ($)",
                Location = new System.Drawing.Point(40, 105),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtMonto = new TextBox
            {
                Location = new System.Drawing.Point(40, 130),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 12F)
            };
            BankTheme.StyleTextBox(txtMonto);
            txtMonto.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                    e.Handled = true;
                if (e.KeyChar == '.' && txtMonto.Text.Contains("."))
                    e.Handled = true;
            };

            Label lblConcepto = new Label
            {
                Text = "Concepto / Descripción",
                Location = new System.Drawing.Point(40, 180),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtConcepto = new TextBox
            {
                Location = new System.Drawing.Point(40, 205),
                Size = new System.Drawing.Size(420, 80),
                Multiline = true,
                Font = BankTheme.BodyFont,
                ScrollBars = ScrollBars.Vertical
            };
            BankTheme.StyleTextBox(txtConcepto);

            mainCard.Controls.AddRange(new Control[] { lblTipo, cmbTipo, lblMonto, txtMonto, lblConcepto, txtConcepto });

            Button btnGuardar = new Button
            {
                Text = "✓ GUARDAR",
                Location = new System.Drawing.Point(150, 490),
                Size = new System.Drawing.Size(150, 45)
            };
            BankTheme.StyleButton(btnGuardar, true);

            Button btnCancelar = new Button
            {
                Text = "✗ CANCELAR",
                Location = new System.Drawing.Point(310, 490),
                Size = new System.Drawing.Size(150, 45)
            };
            BankTheme.StyleButton(btnCancelar, false);

            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, mainCard, btnGuardar, btnCancelar });
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMonto.Text))
            {
                MessageBox.Show("Ingrese el monto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtMonto.Text, out decimal monto) || monto <= 0)
            {
                MessageBox.Show("Ingrese un monto válido mayor a 0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {

                string querySaldo = "SELECT saldo FROM cuentas WHERE id_cuenta = @id";
                DataTable dt = Database.ExecuteQuery(querySaldo, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));
                decimal saldoActual = Convert.ToDecimal(dt.Rows[0]["saldo"]);

                string tipo = cmbTipo.SelectedItem.ToString();
                decimal nuevoSaldo = saldoActual;

                if (tipo == "DEPOSITO" || tipo == "ABONO")
                    nuevoSaldo = saldoActual + monto;
                else if (tipo == "RETIRO" || tipo == "CARGO")
                {
                    if (saldoActual < monto)
                    {
                        MessageBox.Show("Saldo insuficiente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    nuevoSaldo = saldoActual - monto;
                }

                string queryMov = @"INSERT INTO movimientos (id_cuenta, tipo, monto, concepto, saldo_anterior, saldo_nuevo) 
                                   VALUES (@cuenta, @tipo, @monto, @concepto, @saldoAnt, @saldoNuevo)";
                Database.ExecuteNonQuery(queryMov,
                    new NpgsqlParameter("@cuenta", FormLogin.IdCuentaActual),
                    new NpgsqlParameter("@tipo", tipo),
                    new NpgsqlParameter("@monto", monto),
                    new NpgsqlParameter("@concepto", txtConcepto.Text),
                    new NpgsqlParameter("@saldoAnt", saldoActual),
                    new NpgsqlParameter("@saldoNuevo", nuevoSaldo));

                string queryUpdate = "UPDATE cuentas SET saldo = @saldo WHERE id_cuenta = @id";
                Database.ExecuteNonQuery(queryUpdate,
                    new NpgsqlParameter("@saldo", nuevoSaldo),
                    new NpgsqlParameter("@id", FormLogin.IdCuentaActual));

                MessageBox.Show($"Movimiento registrado exitosamente\nNuevo saldo: ${nuevoSaldo:N2}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
