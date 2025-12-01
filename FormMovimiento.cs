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
            this.Text = "Registrar Movimiento";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTitulo = new Label { Text = "REGISTRAR MOVIMIENTO", Location = new System.Drawing.Point(120, 20), Size = new System.Drawing.Size(260, 30), Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold) };
            
            Label lblTipo = new Label { Text = "Tipo:", Location = new System.Drawing.Point(80, 70), Size = new System.Drawing.Size(100, 20) };
            cmbTipo = new ComboBox { Location = new System.Drawing.Point(180, 68), Size = new System.Drawing.Size(250, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipo.Items.AddRange(new string[] { "DEPOSITO", "RETIRO", "CARGO", "ABONO" });
            cmbTipo.SelectedIndex = 0;

            Label lblMonto = new Label { Text = "Monto:", Location = new System.Drawing.Point(80, 110), Size = new System.Drawing.Size(100, 20) };
            txtMonto = new TextBox { Location = new System.Drawing.Point(180, 108), Size = new System.Drawing.Size(250, 20) };

            Label lblConcepto = new Label { Text = "Concepto:", Location = new System.Drawing.Point(80, 150), Size = new System.Drawing.Size(100, 20) };
            txtConcepto = new TextBox { Location = new System.Drawing.Point(180, 148), Size = new System.Drawing.Size(250, 60), Multiline = true };

            Button btnGuardar = new Button { Text = "Guardar", Location = new System.Drawing.Point(180, 250), Size = new System.Drawing.Size(120, 40), Font = new System.Drawing.Font("Arial", 11) };
            Button btnCancelar = new Button { Text = "Cancelar", Location = new System.Drawing.Point(310, 250), Size = new System.Drawing.Size(120, 40), Font = new System.Drawing.Font("Arial", 11) };

            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitulo, lblTipo, cmbTipo, lblMonto, txtMonto, lblConcepto, txtConcepto, btnGuardar, btnCancelar });
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
                // Obtener saldo actual
                string querySaldo = "SELECT saldo FROM cuentas WHERE id_cuenta = @id";
                DataTable dt = Database.ExecuteQuery(querySaldo, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));
                decimal saldoActual = Convert.ToDecimal(dt.Rows[0]["saldo"]);

                string tipo = cmbTipo.SelectedItem.ToString();
                decimal nuevoSaldo = saldoActual;

                // Calcular nuevo saldo
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

                // Registrar movimiento
                string queryMov = @"INSERT INTO movimientos (id_cuenta, tipo, monto, concepto, saldo_anterior, saldo_nuevo) 
                                   VALUES (@cuenta, @tipo, @monto, @concepto, @saldoAnt, @saldoNuevo)";
                Database.ExecuteNonQuery(queryMov,
                    new NpgsqlParameter("@cuenta", FormLogin.IdCuentaActual),
                    new NpgsqlParameter("@tipo", tipo),
                    new NpgsqlParameter("@monto", monto),
                    new NpgsqlParameter("@concepto", txtConcepto.Text),
                    new NpgsqlParameter("@saldoAnt", saldoActual),
                    new NpgsqlParameter("@saldoNuevo", nuevoSaldo));

                // Actualizar saldo
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
