using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaBanco
{
    /// <summary>
    /// Cuadro de mensaje personalizado con el tema corporativo del banco.
    /// Reemplaza el MessageBox estándar de Windows con un diseño más profesional.
    /// </summary>
    public class CustomMessageBox : Form
    {
        /// <summary>
        /// Constructor que inicializa el cuadro de mensaje personalizado.
        /// </summary>
        /// <param name="titulo">Título del mensaje</param>
        /// <param name="mensaje">Contenido del mensaje</param>
        /// <param name="icon">Icono a mostrar (Error, Warning, Information)</param>
        public CustomMessageBox(string titulo, string mensaje, MessageBoxIcon icon)
        {
            InitializeComponent(titulo, mensaje, icon);
        }

        /// <summary>
        /// Inicializa los componentes visuales del cuadro de mensaje.
        /// </summary>
        private void InitializeComponent(string titulo, string mensaje, MessageBoxIcon icon)
        {
            this.Text = "";
            this.ClientSize = new Size(450, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = BankTheme.White;
            this.ShowInTaskbar = false;

            Panel borderPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(450, 250),
                BackColor = BankTheme.PrimaryBlue,
                Padding = new Padding(2)
            };

            Panel contentPanel = new Panel
            {
                Location = new Point(2, 2),
                Size = new Size(446, 246),
                BackColor = BankTheme.White
            };

            Panel headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(446, 50),
                BackColor = icon == MessageBoxIcon.Error ? BankTheme.Danger : 
                           icon == MessageBoxIcon.Warning ? BankTheme.Warning : 
                           BankTheme.PrimaryBlue
            };

            Label lblTitulo = new Label
            {
                Text = titulo,
                Location = new Point(15, 15),
                Size = new Size(380, 20),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.White
            };

            Button btnClose = new Button
            {
                Text = "✕",
                Location = new Point(410, 10),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(200, 0, 0),
                ForeColor = BankTheme.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            headerPanel.Controls.AddRange(new Control[] { lblTitulo, btnClose });

            string iconText = icon == MessageBoxIcon.Error ? "⚠️" :
                            icon == MessageBoxIcon.Warning ? "⚠️" :
                            icon == MessageBoxIcon.Information ? "ℹ️" : "✓";

            Label lblIcon = new Label
            {
                Text = iconText,
                Location = new Point(30, 70),
                Size = new Size(60, 60),
                Font = new Font("Segoe UI", 36F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblMensaje = new Label
            {
                Text = mensaje,
                Location = new Point(100, 70),
                Size = new Size(330, 100),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new Point(150, 190),
                Size = new Size(150, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = BankTheme.DarkGray,
                ForeColor = BankTheme.White,
                Font = BankTheme.BodyFont,
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();

            contentPanel.Controls.AddRange(new Control[] { headerPanel, lblIcon, lblMensaje, btnCerrar });
            borderPanel.Controls.Add(contentPanel);
            this.Controls.Add(borderPanel);
        }

        /// <summary>
        /// Muestra un cuadro de mensaje personalizado de forma modal.
        /// </summary>
        /// <param name="titulo">Título del mensaje</param>
        /// <param name="mensaje">Contenido del mensaje</param>
        /// <param name="icon">Icono a mostrar (por defecto Information)</param>
        public static void Show(string titulo, string mensaje, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            using (CustomMessageBox msgBox = new CustomMessageBox(titulo, mensaje, icon))
            {
                msgBox.ShowDialog();
            }
        }
    }
}
