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
            this.Text = "Módulo Banco - Portal de Cliente";
            this.ClientSize = new System.Drawing.Size(1000, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Header
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

            // Panel principal con opciones
            Label lblTitulo = new Label
            {
                Text = "Panel de Control",
                Location = new System.Drawing.Point(50, 130),
                Size = new System.Drawing.Size(300, 35),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            // Tarjetas de opciones - Primera fila
            Panel cardSaldo = CreateMenuCard(50, 180, 280, 180, "💰", "Consultar Saldo", "Ver el saldo actual de tu cuenta", "ConsultarSaldo");
            Panel cardMovimiento = CreateMenuCard(360, 180, 280, 180, "💳", "Movimientos Financieros", "Registrar cargos y abonos", "RegistrarMovimientos");
            Panel cardTransferencia = CreateMenuCard(670, 180, 280, 180, "🔄", "Transferencias", "Transferir entre cuentas", "Transferencias");

            // Segunda fila
            Panel cardHistorial = CreateMenuCard(50, 390, 280, 180, "📊", "Historial", "Ver movimientos realizados", "Historial");
            Panel cardEstado = CreateMenuCard(360, 390, 280, 180, "📄", "Estado de Cuenta", "Generar reporte detallado", "EstadoCuenta");
            Panel cardAdminUsuarios = CreateMenuCard(670, 390, 280, 180, "👥", "Admin. Usuarios", "Gestionar usuarios del sistema", "AdministrarUsuarios");

            // Tercera fila - Módulos especializados (centrado)
            Panel cardDivisas = CreateMenuCard(360, 600, 280, 180, "💱", "Autorización Divisas", "Gestionar operaciones en moneda extranjera", "ConsultarSolicitudesDivisas");

            // Botón cerrar sesión
            Button btnSalir = new Button
            {
                Text = "🚪 CERRAR SESIÓN",
                Location = new System.Drawing.Point(400, 820),
                Size = new System.Drawing.Size(200, 50)
            };
            BankTheme.StyleButton(btnSalir, false);

            // Eventos - Asignar a las tarjetas y sus controles hijos
            AsignarEventoCard(cardSaldo, () => new FormSaldo().ShowDialog());
            AsignarEventoCard(cardMovimiento, () => new FormMovimientoFinanciero().ShowDialog());
            AsignarEventoCard(cardTransferencia, () => new FormTransferencia().ShowDialog());
            AsignarEventoCard(cardHistorial, () => new FormRevisionMovimientos().ShowDialog());
            AsignarEventoCard(cardEstado, () => new FormEstadoCuenta().ShowDialog());
            AsignarEventoCard(cardAdminUsuarios, () => new FormAdministracionUsuarios().ShowDialog());
            AsignarEventoCard(cardDivisas, () => new FormAutorizacionDivisas().ShowDialog());
            btnSalir.Click += (s, e) => this.Close();

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

        private Panel CreateMenuCard(int x, int y, int width, int height, string icon, string title, string description, string permiso)
        {
            // Verificar si el usuario tiene permiso
            bool tienePermiso = RoleManager.TienePermiso(FormLogin.RolUsuario, permiso);
            
            // Si no tiene permiso, no mostrar la tarjeta
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

            // Efecto hover
            card.MouseEnter += (s, e) => card.BackColor = BankTheme.LightGray;
            card.MouseLeave += (s, e) => card.BackColor = BankTheme.White;

            // Propagar eventos de hover a los controles hijos
            foreach (Control ctrl in card.Controls)
            {
                ctrl.MouseEnter += (s, e) => card.BackColor = BankTheme.LightGray;
                ctrl.MouseLeave += (s, e) => card.BackColor = BankTheme.White;
            }

            return card;
        }
    }
}
