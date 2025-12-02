using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormTransferencia : Form
    {
        private TextBox txtCuentaDestino;
        private TextBox txtMonto;
        private TextBox txtConcepto;
        private Label lblNombreDestino;

        public FormTransferencia()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Transferencia";
            this.ClientSize = new System.Drawing.Size(600, 630);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Header
            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(600, 80),
                BackColor = BankTheme.PrimaryBlue
            };

            Label lblTitulo = new Label
            {
                Text = "🔄 TRANSFERENCIA BANCARIA",
                Location = new System.Drawing.Point(120, 25),
                Size = new System.Drawing.Size(360, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Botón de inicio
            HomeButton.AddToForm(this, headerPanel);

            headerPanel.Controls.Add(lblTitulo);

            // Card principal
            Panel mainCard = BankTheme.CreateCard(50, 110, 500, 400);

            Label lblCuentaDestino = new Label
            {
                Text = "Cuenta Destino",
                Location = new System.Drawing.Point(40, 30),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtCuentaDestino = new TextBox
            {
                Location = new System.Drawing.Point(40, 55),
                Size = new System.Drawing.Size(420, 30),
                Font = new System.Drawing.Font("Segoe UI", 11F)
            };
            BankTheme.StyleTextBox(txtCuentaDestino);

            lblNombreDestino = new Label
            {
                Text = "",
                Location = new System.Drawing.Point(40, 90),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.Success
            };

            txtCuentaDestino.Leave += ValidarCuentaDestino;

            Label lblMonto = new Label
            {
                Text = "Monto a Transferir ($)",
                Location = new System.Drawing.Point(40, 130),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtMonto = new TextBox
            {
                Location = new System.Drawing.Point(40, 155),
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
                Text = "Concepto / Referencia",
                Location = new System.Drawing.Point(40, 205),
                Size = new System.Drawing.Size(420, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            txtConcepto = new TextBox
            {
                Location = new System.Drawing.Point(40, 230),
                Size = new System.Drawing.Size(420, 80),
                Multiline = true,
                Font = BankTheme.BodyFont,
                ScrollBars = ScrollBars.Vertical
            };
            BankTheme.StyleTextBox(txtConcepto);

            Panel warningPanel = new Panel
            {
                Location = new System.Drawing.Point(40, 325),
                Size = new System.Drawing.Size(420, 50),
                BackColor = System.Drawing.Color.FromArgb(255, 243, 205)
            };

            Label lblWarning = new Label
            {
                Text = "⚠️ Verifique los datos antes de confirmar.\nLas transferencias son irreversibles.",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(400, 30),
                Font = BankTheme.SmallFont,
                ForeColor = System.Drawing.Color.FromArgb(133, 100, 4)
            };

            warningPanel.Controls.Add(lblWarning);

            mainCard.Controls.AddRange(new Control[] { lblCuentaDestino, txtCuentaDestino, lblNombreDestino, lblMonto, txtMonto, lblConcepto, txtConcepto, warningPanel });

            // Botones
            Button btnTransferir = new Button
            {
                Text = "✓ TRANSFERIR",
                Location = new System.Drawing.Point(150, 560),
                Size = new System.Drawing.Size(150, 45)
            };
            BankTheme.StyleButton(btnTransferir, true);

            Button btnCancelar = new Button
            {
                Text = "✗ CANCELAR",
                Location = new System.Drawing.Point(310, 560),
                Size = new System.Drawing.Size(150, 45)
            };
            BankTheme.StyleButton(btnCancelar, false);

            btnTransferir.Click += BtnTransferir_Click;
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { headerPanel, mainCard, btnTransferir, btnCancelar });
        }

        private void ValidarCuentaDestino(object sender, EventArgs e)
        {
            string cuentaDestino = txtCuentaDestino.Text.Trim();
            if (string.IsNullOrEmpty(cuentaDestino))
            {
                lblNombreDestino.Text = "";
                return;
            }

            try
            {
                string query = @"SELECT u.nombre_completo 
                                FROM cuentas c 
                                INNER JOIN usuarios u ON c.id_usuario = u.id_usuario 
                                WHERE c.numero_cuenta = @cuenta AND c.id_cuenta != @miCuenta";
                DataTable dt = Database.ExecuteQuery(query,
                    new NpgsqlParameter("@cuenta", cuentaDestino),
                    new NpgsqlParameter("@miCuenta", FormLogin.IdCuentaActual));

                if (dt.Rows.Count > 0)
                {
                    lblNombreDestino.Text = "✓ " + dt.Rows[0]["nombre_completo"].ToString();
                    lblNombreDestino.ForeColor = BankTheme.Success;
                }
                else
                {
                    lblNombreDestino.Text = "✗ Cuenta no encontrada";
                    lblNombreDestino.ForeColor = BankTheme.Danger;
                }
            }
            catch (Exception ex)
            {
                lblNombreDestino.Text = "Error al validar cuenta";
                lblNombreDestino.ForeColor = BankTheme.Danger;
            }
        }

        private void BtnTransferir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCuentaDestino.Text))
            {
                MessageBox.Show("Ingrese la cuenta destino", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lblNombreDestino.Text.Contains("✗"))
            {
                MessageBox.Show("La cuenta destino no es válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

            var confirmResult = MessageBox.Show(
                $"¿Confirma la transferencia de ${monto:N2}?\n\nDestino: {lblNombreDestino.Text.Replace("✓ ", "")}",
                "Confirmar Transferencia",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult != DialogResult.Yes)
                return;

            try
            {
                // Obtener saldo actual
                string querySaldo = "SELECT saldo FROM cuentas WHERE id_cuenta = @id";
                DataTable dt = Database.ExecuteQuery(querySaldo, new NpgsqlParameter("@id", FormLogin.IdCuentaActual));
                decimal saldoActual = Convert.ToDecimal(dt.Rows[0]["saldo"]);

                if (saldoActual < monto)
                {
                    MessageBox.Show("Saldo insuficiente para realizar la transferencia", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Obtener ID cuenta destino
                string queryDestino = "SELECT id_cuenta, saldo FROM cuentas WHERE numero_cuenta = @cuenta";
                DataTable dtDestino = Database.ExecuteQuery(queryDestino, new NpgsqlParameter("@cuenta", txtCuentaDestino.Text.Trim()));
                int idCuentaDestino = Convert.ToInt32(dtDestino.Rows[0]["id_cuenta"]);
                decimal saldoDestino = Convert.ToDecimal(dtDestino.Rows[0]["saldo"]);

                decimal nuevoSaldoOrigen = saldoActual - monto;
                decimal nuevoSaldoDestino = saldoDestino + monto;

                // Registrar movimiento de salida (origen)
                string queryMovOrigen = @"INSERT INTO movimientos (id_cuenta, tipo, monto, concepto, saldo_anterior, saldo_nuevo) 
                                         VALUES (@cuenta, 'TRANSFERENCIA ENVIADA', @monto, @concepto, @saldoAnt, @saldoNuevo)";
                Database.ExecuteNonQuery(queryMovOrigen,
                    new NpgsqlParameter("@cuenta", FormLogin.IdCuentaActual),
                    new NpgsqlParameter("@monto", monto),
                    new NpgsqlParameter("@concepto", $"Transferencia a {txtCuentaDestino.Text.Trim()} - {txtConcepto.Text}"),
                    new NpgsqlParameter("@saldoAnt", saldoActual),
                    new NpgsqlParameter("@saldoNuevo", nuevoSaldoOrigen));

                // Registrar movimiento de entrada (destino)
                string queryMovDestino = @"INSERT INTO movimientos (id_cuenta, tipo, monto, concepto, saldo_anterior, saldo_nuevo) 
                                          VALUES (@cuenta, 'TRANSFERENCIA RECIBIDA', @monto, @concepto, @saldoAnt, @saldoNuevo)";
                Database.ExecuteNonQuery(queryMovDestino,
                    new NpgsqlParameter("@cuenta", idCuentaDestino),
                    new NpgsqlParameter("@monto", monto),
                    new NpgsqlParameter("@concepto", $"Transferencia recibida - {txtConcepto.Text}"),
                    new NpgsqlParameter("@saldoAnt", saldoDestino),
                    new NpgsqlParameter("@saldoNuevo", nuevoSaldoDestino));

                // Actualizar saldos
                string queryUpdateOrigen = "UPDATE cuentas SET saldo = @saldo WHERE id_cuenta = @id";
                Database.ExecuteNonQuery(queryUpdateOrigen,
                    new NpgsqlParameter("@saldo", nuevoSaldoOrigen),
                    new NpgsqlParameter("@id", FormLogin.IdCuentaActual));

                string queryUpdateDestino = "UPDATE cuentas SET saldo = @saldo WHERE id_cuenta = @id";
                Database.ExecuteNonQuery(queryUpdateDestino,
                    new NpgsqlParameter("@saldo", nuevoSaldoDestino),
                    new NpgsqlParameter("@id", idCuentaDestino));

                MessageBox.Show($"Transferencia realizada exitosamente\n\nMonto: ${monto:N2}\nNuevo saldo: ${nuevoSaldoOrigen:N2}",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
