using System;
using System.Windows.Forms;

namespace SistemaBanco
{
    public partial class FormMenu : Form
    {
        public FormMenu()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"Sistema Bancario - {FormLogin.NombreUsuario}";
            this.Size = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblBienvenida = new Label { Text = $"Bienvenido, {FormLogin.NombreUsuario}", Location = new System.Drawing.Point(150, 30), Size = new System.Drawing.Size(300, 30), Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold) };
            
            Button btnSaldo = new Button { Text = "Ver Saldo", Location = new System.Drawing.Point(200, 80), Size = new System.Drawing.Size(200, 50), Font = new System.Drawing.Font("Arial", 12) };
            Button btnMovimiento = new Button { Text = "Registrar Movimiento", Location = new System.Drawing.Point(200, 140), Size = new System.Drawing.Size(200, 50), Font = new System.Drawing.Font("Arial", 12) };
            Button btnHistorial = new Button { Text = "Ver Movimientos", Location = new System.Drawing.Point(200, 200), Size = new System.Drawing.Size(200, 50), Font = new System.Drawing.Font("Arial", 12) };
            Button btnSalir = new Button { Text = "Cerrar Sesión", Location = new System.Drawing.Point(200, 260), Size = new System.Drawing.Size(200, 50), Font = new System.Drawing.Font("Arial", 12) };

            btnSaldo.Click += (s, e) => new FormSaldo().ShowDialog();
            btnMovimiento.Click += (s, e) => new FormMovimiento().ShowDialog();
            btnHistorial.Click += (s, e) => new FormHistorial().ShowDialog();
            btnSalir.Click += (s, e) => { this.Close(); Application.Exit(); };

            this.Controls.AddRange(new Control[] { lblBienvenida, btnSaldo, btnMovimiento, btnHistorial, btnSalir });
        }
    }
}
