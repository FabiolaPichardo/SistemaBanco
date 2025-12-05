using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SistemaBanco
{
    public static class IconHelper
    {
        private static Icon? _appIcon;

        public static void SetFormIcon(Form form)
        {
            try
            {
                if (_appIcon == null)
                {
                    string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "imagenes", "logo.png");

                    if (File.Exists(iconPath))
                    {
                        using (var bitmap = new Bitmap(iconPath))
                        {

                            IntPtr hIcon = bitmap.GetHicon();
                            _appIcon = Icon.FromHandle(hIcon);
                        }
                    }
                }

                if (_appIcon != null)
                {
                    form.Icon = _appIcon;
                }
            }
            catch
            {

            }
        }
    }
}
