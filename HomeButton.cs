using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaBanco
{
    public static class HomeButton
    {
        public static Button Create(Form currentForm)
        {
            Button btnHome = new Button
            {
                Text = "🏠",
                Location = new Point(10, 10),
                Size = new Size(50, 50),
                Font = new Font("Segoe UI", 20F),
                BackColor = BankTheme.PrimaryBlue,
                ForeColor = BankTheme.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TabStop = false
            };

            btnHome.FlatAppearance.BorderSize = 0;
            btnHome.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 58, 138); // Azul más oscuro

            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(btnHome, "Regresar al Dashboard");

            btnHome.Click += (s, e) =>
            {
                currentForm.Close();
            };

            return btnHome;
        }

        public static void AddToForm(Form form, Panel headerPanel)
        {
            Button btnHome = Create(form);
            headerPanel.Controls.Add(btnHome);
            btnHome.BringToFront();
        }
    }
}
