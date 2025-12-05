using System;
using System.Linq;
using System.Windows.Forms;

namespace SistemaBanco
{
    public partial class FormMenu : Form
    {
        public FormMenu()
        {
            InitializeComponent();
            IconHelper.SetFormIcon(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Portal de Cliente";
            this.ClientSize = new System.Drawing.Size(1000, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Panel headerPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(1000, 100),
                BackColor = BankTheme.PrimaryBlue
            };

            Label lblLogo = new Label
            {
                Text = "🏦 MÓDULO BANCO",
                Location = new System.Drawing.Point(30, 25),
                Size = new System.Drawing.Size(300, 50),
                Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold),
                ForeColor = BankTheme.White
            };

            Label lblBienvenida = new Label
            {
                Text = $"Bienvenido, {FormLogin.NombreUsuario}",
                Location = new System.Drawing.Point(650, 15),
                Size = new System.Drawing.Size(320, 25),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.AccentGold,
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };

            Label lblRol = new Label
            {
                Text = $"Rol: {FormLogin.RolUsuario}",
                Location = new System.Drawing.Point(650, 45),
                Size = new System.Drawing.Size(320, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };

            Label lblFecha = new Label
            {
                Text = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy"),
                Location = new System.Drawing.Point(650, 70),
                Size = new System.Drawing.Size(320, 20),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };

            headerPanel.Controls.AddRange(new Control[] { lblLogo, lblBienvenida, lblRol, lblFecha });

            Label lblTitulo = new Label
            {
                Text = "Panel de Control",
                Location = new System.Drawing.Point(50, 130),
                Size = new System.Drawing.Size(300, 35),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            Button btnSalir = new Button
            {
                Text = "🚪 CERRAR SESIÓN",
                Location = new System.Drawing.Point(750, 125),
                Size = new System.Drawing.Size(200, 45)
            };
            BankTheme.StyleButton(btnSalir, false);

            Panel cardSaldo = CreateMenuCard(60, 180, 280, 170, "💰", "Consultar Saldo", "Ver el saldo actual de tu cuenta", "ConsultarSaldo");
            Panel cardMovimiento = CreateMenuCard(360, 180, 280, 170, "💳", "Movimientos Financieros", "Registrar cargos y abonos", "RegistrarMovimientos");
            Panel cardTransferencia = CreateMenuCard(660, 180, 280, 170, "🔄", "Transferencias", "Transferir entre cuentas", "Transferencias");
            Panel cardHistorial = CreateMenuCard(60, 370, 280, 170, "📊", "Historial", "Ver movimientos realizados", "Historial");
            Panel cardEstado = CreateMenuCard(360, 370, 280, 170, "📄", "Estado de Cuenta", "Generar reporte detallado", "EstadoCuenta");
            Panel cardDivisas = CreateMenuCard(660, 370, 280, 170, "💱", "Autorización Divisas", "Gestionar operaciones en moneda extranjera", "ConsultarSolicitudesDivisas");
            Panel cardAdminUsuarios = CreateMenuCard(360, 560, 280, 170, "👥", "Admin. Usuarios", "Gestionar usuarios del sistema", "AdministrarUsuarios");

            ReorganizarTarjetas(new Panel[] { cardSaldo, cardMovimiento, cardTransferencia, cardHistorial, cardEstado, cardDivisas, cardAdminUsuarios });

            AsignarEventoCard(cardSaldo, () => new FormSaldo().ShowDialog());
            AsignarEventoCard(cardMovimiento, () => new FormMovimientoFinanciero().ShowDialog());
            AsignarEventoCard(cardTransferencia, () => new FormTransferencia().ShowDialog());
            AsignarEventoCard(cardHistorial, () => new FormHistorial().ShowDialog());
            AsignarEventoCard(cardEstado, () => new FormEstadoCuenta().ShowDialog());
            AsignarEventoCard(cardAdminUsuarios, () => new FormAdministracionUsuarios().ShowDialog());
            AsignarEventoCard(cardDivisas, () => new FormAutorizacionDivisas().ShowDialog());
            btnSalir.Click += BtnCerrarSesion_Click;

            this.Controls.AddRange(new Control[] { headerPanel, lblTitulo, cardSaldo, cardMovimiento, cardTransferencia, cardHistorial, cardEstado, cardAdminUsuarios, cardDivisas, btnSalir });
        }

        private void AsignarEventoCard(Panel card, Action action)
        {
            card.Click += (s, e) => action();
            foreach (Control ctrl in card.Controls)
            {
                ctrl.Click += (s, e) => action();
            }
        }

        private void ReorganizarTarjetas(Panel[] tarjetas)
        {

            var tarjetasVisibles = tarjetas.Where(t => t.Visible).ToList();

            int columna = 0;
            int fila = 0;
            int xInicial = 60;
            int yInicial = 180;
            int anchoTarjeta = 280;
            int altoTarjeta = 170;
            int espacioX = 300; // 280 + 20 de margen
            int espacioY = 190; // 170 + 20 de margen

            foreach (var tarjeta in tarjetasVisibles)
            {
                int x = xInicial + (columna * espacioX);
                int y = yInicial + (fila * espacioY);

                tarjeta.Location = new System.Drawing.Point(x, y);

                columna++;
                if (columna >= 3)
                {
                    columna = 0;
                    fila++;
                }
            }
        }

        private Panel CreateMenuCard(int x, int y, int width, int height, string icon, string title, string description, string permiso)
        {

            bool tienePermiso = RoleManager.TienePermiso(FormLogin.RolUsuario, permiso);

            if (!tienePermiso)
            {
                Panel emptyCard = new Panel
                {
                    Location = new System.Drawing.Point(x, y),
                    Size = new System.Drawing.Size(width, height),
                    Visible = false
                };
                return emptyCard;
            }

            Panel card = BankTheme.CreateCard(x, y, width, height);
            card.Cursor = Cursors.Hand;
            card.Tag = title; // Guardar el título para identificar la acción

            Label lblIcon = new Label
            {
                Text = icon,
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(60, 60),
                Font = new System.Drawing.Font("Segoe UI", 36F),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };

            Label lblTitle = new Label
            {
                Text = title,
                Location = new System.Drawing.Point(20, 90),
                Size = new System.Drawing.Size(240, 30),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.PrimaryBlue,
                Cursor = Cursors.Hand
            };

            Label lblDescription = new Label
            {
                Text = description,
                Location = new System.Drawing.Point(20, 125),
                Size = new System.Drawing.Size(240, 40),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary,
                Cursor = Cursors.Hand
            };

            card.Controls.AddRange(new Control[] { lblIcon, lblTitle, lblDescription });

            card.MouseEnter += (s, e) => card.BackColor = BankTheme.LightGray;
            card.MouseLeave += (s, e) => card.BackColor = BankTheme.White;

            foreach (Control ctrl in card.Controls)
            {
                ctrl.MouseEnter += (s, e) => card.BackColor = BankTheme.LightGray;
                ctrl.MouseLeave += (s, e) => card.BackColor = BankTheme.White;
            }

            return card;
        }

        private void BtnCerrarSesion_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
                "¿Está seguro que desea cerrar sesión?\n\nSe cerrará su sesión actual y regresará a la pantalla de inicio de sesión.",
                "Confirmar Cierre de Sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (resultado == DialogResult.Yes)
            {

                this.Hide();
                FormLogin loginForm = new FormLogin();
                loginForm.ShowDialog();
                this.Close();
            }
        }
    }
}
