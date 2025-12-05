using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SistemaBanco
{
    public static class BankTheme
    {

        public static readonly Color PrimaryBlue = Color.FromArgb(0, 51, 102);      // Azul oscuro corporativo
        public static readonly Color SecondaryBlue = Color.FromArgb(0, 102, 204);   // Azul medio
        public static readonly Color AccentGold = Color.FromArgb(212, 175, 55);     // Dorado elegante
        public static readonly Color LightGray = Color.FromArgb(245, 245, 245);     // Gris claro
        public static readonly Color DarkGray = Color.FromArgb(64, 64, 64);         // Gris oscuro
        public static readonly Color Success = Color.FromArgb(40, 167, 69);         // Verde éxito
        public static readonly Color Danger = Color.FromArgb(220, 53, 69);          // Rojo peligro
        public static readonly Color Warning = Color.FromArgb(255, 193, 7);         // Amarillo advertencia
        public static readonly Color White = Color.White;
        public static readonly Color TextPrimary = Color.FromArgb(33, 37, 41);
        public static readonly Color TextSecondary = Color.FromArgb(108, 117, 125);

        public static readonly Font TitleFont = new Font("Segoe UI", 18F, FontStyle.Bold);
        public static readonly Font SubtitleFont = new Font("Segoe UI", 14F, FontStyle.Bold);
        public static readonly Font HeaderFont = new Font("Segoe UI", 12F, FontStyle.Bold);
        public static readonly Font BodyFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font SmallFont = new Font("Segoe UI", 9F, FontStyle.Regular);
        public static readonly Font MoneyFont = new Font("Segoe UI", 24F, FontStyle.Bold);

        public static void StyleButton(Button btn, bool isPrimary = true)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = HeaderFont;
            btn.Cursor = Cursors.Hand;
            btn.ForeColor = White;

            if (isPrimary)
            {
                btn.BackColor = PrimaryBlue;
                btn.FlatAppearance.MouseOverBackColor = SecondaryBlue;
            }
            else
            {
                btn.BackColor = DarkGray;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(90, 90, 90);
            }
        }

        public static void StyleTextBox(TextBox txt)
        {
            txt.Font = BodyFont;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = White;
        }

        public static void StyleLabel(Label lbl, bool isTitle = false)
        {
            lbl.Font = isTitle ? TitleFont : BodyFont;
            lbl.ForeColor = isTitle ? PrimaryBlue : TextPrimary;
        }

        public static Panel CreateCard(int x, int y, int width, int height)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = White,
                BorderStyle = BorderStyle.None
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    Rectangle rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };

            return card;
        }
    }
}
