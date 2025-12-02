using System;
using System.Net;
using System.Net.Mail;

namespace SistemaBanco
{
    public static class EmailService
    {
        // Configuración del servidor SMTP
        // NOTA: Estos valores deben configurarse según el proveedor de correo
        private static string smtpServer = "smtp.gmail.com"; // Para Gmail
        private static int smtpPort = 587;
        private static string smtpUser = "tu_correo@gmail.com"; // Cambiar por el correo del sistema
        private static string smtpPassword = "tu_contraseña_app"; // Usar contraseña de aplicación
        private static string fromEmail = "tu_correo@gmail.com";
        private static string fromName = "Módulo Banco";

        /// <summary>
        /// Envía un correo electrónico
        /// </summary>
        /// <param name="toEmail">Correo del destinatario</param>
        /// <param name="subject">Asunto del correo</param>
        /// <param name="body">Cuerpo del mensaje</param>
        /// <returns>True si se envió correctamente, False si hubo error</returns>
        public static bool EnviarCorreo(string toEmail, string subject, string body)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail, fromName);
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.High;

                    using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 10000; // 10 segundos

                        smtp.Send(mail);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Envía un correo con el código de recuperación de contraseña
        /// </summary>
        public static bool EnviarCodigoRecuperacion(string toEmail, string nombreUsuario, string codigo)
        {
            string subject = "Código de Recuperación de Contraseña - Módulo Banco";
            
            string body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f5f5; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }}
                        .header {{ background-color: #003366; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ padding: 30px; }}
                        .code {{ font-size: 32px; font-weight: bold; color: #003366; text-align: center; padding: 20px; background-color: #f0f0f0; border-radius: 5px; letter-spacing: 5px; }}
                        .warning {{ color: #dc3545; font-size: 14px; margin-top: 20px; }}
                        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🏦 Módulo Banco</h1>
                            <p>Recuperación de Contraseña</p>
                        </div>
                        <div class='content'>
                            <h2>Hola, {nombreUsuario}</h2>
                            <p>Hemos recibido una solicitud para recuperar la contraseña de tu cuenta.</p>
                            <p>Tu código de verificación es:</p>
                            <div class='code'>{codigo}</div>
                            <p>Este código es válido por <strong>15 minutos</strong>.</p>
                            <p class='warning'>
                                ⚠️ Si no solicitaste este código, ignora este correo. Tu cuenta permanece segura.
                            </p>
                            <p>Por seguridad, nunca compartas este código con nadie.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2025 Módulo Banco. Todos los derechos reservados.</p>
                            <p>Este es un correo automático, por favor no responder.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            return EnviarCorreo(toEmail, subject, body);
        }

        /// <summary>
        /// Verifica si la configuración de correo está lista
        /// </summary>
        public static bool ConfiguracionValida()
        {
            return !smtpUser.Contains("tu_correo") && !smtpPassword.Contains("tu_contraseña");
        }
    }
}
