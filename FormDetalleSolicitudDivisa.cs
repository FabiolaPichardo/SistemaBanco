using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormDetalleSolicitudDivisa : Form
    {
        private int idSolicitud;
        private string estadoActual;
        private TextBox txtComentarios;
        private TextBox txtMotivoRechazo;
        private Label lblEstadoActual;
        private Panel infoPanel;
        private Label lblMotivoRechazo;
        private Panel botonesPanel;

        public FormDetalleSolicitudDivisa(int idSolicitud)
        {
            this.idSolicitud = idSolicitud;
            InitializeComponent();
            this.Shown += (s, e) => CargarDetalles();
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Detalle de Solicitud de Divisa";
            this.ClientSize = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header
            Panel headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(800, 70),
                BackColor = BankTheme.PrimaryBlue
            };

            Label lblTitulo = new Label
            {
                Text = "DETALLE DE SOLICITUD DE DIVISA",
                Location = new Point(20, 20),
                Size = new Size(760, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.Add(lblTitulo);

            // Panel de detalles
            Panel detallesPanel = BankTheme.CreateCard(20, 90, 760, 450);

            int yPos = 20;

            // Estado actual
            Label lblEstadoLabel = new Label
            {
                Text = "Estado Actual:",
                Location = new Point(20, yPos),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };

            lblEstadoActual = new Label
            {
                Location = new Point(180, yPos),
                Size = new Size(560, 30),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 5, 10, 5)
            };

            yPos += 45;

            // Separador
            Panel separador1 = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(720, 2),
                BackColor = BankTheme.LightGray
            };

            yPos += 15;

            // Información de la solicitud (se llenará dinámicamente)
            infoPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(720, 200),
                AutoScroll = true
            };

            yPos += 210;

            // Comentarios de autorización
            Label lblComentarios = new Label
            {
                Text = "Comentarios de Autorización:",
                Location = new Point(20, yPos),
                Size = new Size(250, 20),
                Font = BankTheme.BodyFont
            };

            yPos += 25;

            txtComentarios = new TextBox
            {
                Location = new Point(20, yPos),
                Size = new Size(720, 60),
                Multiline = true,
                Font = BankTheme.BodyFont,
                ScrollBars = ScrollBars.Vertical
            };

            yPos += 70;

            // Motivo de rechazo (solo visible si se rechaza)
            lblMotivoRechazo = new Label
            {
                Text = "Motivo de Rechazo:",
                Location = new Point(20, yPos),
                Size = new Size(250, 20),
                Font = BankTheme.BodyFont,
                Visible = false
            };

            yPos += 25;

            txtMotivoRechazo = new TextBox
            {
                Location = new Point(20, yPos),
                Size = new Size(720, 60),
                Multiline = true,
                Font = BankTheme.BodyFont,
                ScrollBars = ScrollBars.Vertical,
                Visible = false
            };

            detallesPanel.Controls.AddRange(new Control[] {
                lblEstadoLabel, lblEstadoActual, separador1, infoPanel,
                lblComentarios, txtComentarios, lblMotivoRechazo, txtMotivoRechazo
            });

            // Botones de acción
            botonesPanel = new Panel
            {
                Location = new Point(20, 550),
                Size = new Size(760, 100),
                BackColor = Color.Transparent
            };

            Button btnMarcarRevision = new Button
            {
                Text = "En Revisión",
                Location = new Point(30, 10),
                Size = new Size(160, 45),
                Font = BankTheme.BodyFont,
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnMarcarRevision.FlatAppearance.BorderSize = 0;
            btnMarcarRevision.Click += (s, e) => CambiarEstado("En Revisión");

            Button btnAutorizar = new Button
            {
                Text = "✅ Autorizar",
                Location = new Point(210, 10),
                Size = new Size(160, 45),
                Font = BankTheme.BodyFont,
                BackColor = BankTheme.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAutorizar.FlatAppearance.BorderSize = 0;
            btnAutorizar.Click += (s, e) => CambiarEstado("Autorizada");

            Button btnRechazar = new Button
            {
                Text = "❌ Rechazar",
                Location = new Point(390, 10),
                Size = new Size(160, 45),
                Font = BankTheme.BodyFont,
                BackColor = BankTheme.Danger,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRechazar.FlatAppearance.BorderSize = 0;
            btnRechazar.Click += (s, e) =>
            {
                lblMotivoRechazo.Visible = true;
                txtMotivoRechazo.Visible = true;
                CambiarEstado("Rechazada");
            };

            Button btnCerrar = new Button
            {
                Text = "Cerrar",
                Location = new Point(570, 10),
                Size = new Size(160, 45),
                Font = BankTheme.BodyFont,
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();

            botonesPanel.Controls.AddRange(new Control[] {
                btnMarcarRevision, btnAutorizar, btnRechazar, btnCerrar
            });

            this.Controls.AddRange(new Control[] {
                headerPanel, detallesPanel, botonesPanel
            });
        }

        private void CargarDetalles()
        {
            try
            {
                string query = @"
                    SELECT 
                        s.id_transaccion,
                        s.descripcion,
                        c.numero_cuenta,
                        u_sol.nombre_completo AS solicitante,
                        u_sol.email AS email_solicitante,
                        d.codigo AS divisa,
                        d.nombre AS nombre_divisa,
                        d.simbolo,
                        s.monto_mxn,
                        s.tasa_cambio,
                        s.monto_divisa,
                        s.estado,
                        s.fecha_solicitud,
                        s.fecha_expiracion,
                        COALESCE(u_aut.nombre_completo, '-') AS autorizador,
                        s.fecha_autorizacion,
                        s.comentarios_autorizacion,
                        s.motivo_rechazo
                    FROM solicitudes_autorizacion_divisas s
                    INNER JOIN cuentas c ON s.id_cuenta = c.id_cuenta
                    INNER JOIN usuarios u_sol ON s.id_usuario_solicitante = u_sol.id_usuario
                    INNER JOIN divisas d ON s.id_divisa = d.id_divisa
                    LEFT JOIN usuarios u_aut ON s.id_usuario_autorizador = u_aut.id_usuario
                    WHERE s.id_solicitud = @idSolicitud";

                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@idSolicitud", idSolicitud));

                if (dt.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Solicitud No Encontrada",
                        "No se encontró la solicitud especificada.",
                        MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                DataRow row = dt.Rows[0];
                estadoActual = row["estado"].ToString();

                // Actualizar estado visual
                lblEstadoActual.Text = estadoActual;
                ActualizarColorEstado();

                // Llenar panel de información
                infoPanel.Controls.Clear();

                int yPos = 0;
                AgregarCampo(infoPanel, "ID Transacción:", row["id_transaccion"].ToString(), ref yPos);
                AgregarCampo(infoPanel, "Descripción:", row["descripcion"].ToString(), ref yPos);
                AgregarCampo(infoPanel, "Número de Cuenta:", row["numero_cuenta"].ToString(), ref yPos);
                AgregarCampo(infoPanel, "Solicitante:", row["solicitante"].ToString(), ref yPos);
                AgregarCampo(infoPanel, "Email:", row["email_solicitante"].ToString(), ref yPos);
                AgregarCampo(infoPanel, "Divisa:", $"{row["divisa"]} - {row["nombre_divisa"]} ({row["simbolo"]})", ref yPos);
                AgregarCampo(infoPanel, "Monto en MXN:", $"${Convert.ToDecimal(row["monto_mxn"]):N2}", ref yPos);
                AgregarCampo(infoPanel, "Tasa de Cambio:", Convert.ToDecimal(row["tasa_cambio"]).ToString("N4"), ref yPos);
                AgregarCampo(infoPanel, "Monto en Divisa:", $"{row["simbolo"]} {Convert.ToDecimal(row["monto_divisa"]):N2}", ref yPos);
                AgregarCampo(infoPanel, "Fecha Solicitud:", Convert.ToDateTime(row["fecha_solicitud"]).ToString("dd/MM/yyyy HH:mm"), ref yPos);
                
                if (row["fecha_expiracion"] != DBNull.Value)
                {
                    AgregarCampo(infoPanel, "Fecha Expiración:", Convert.ToDateTime(row["fecha_expiracion"]).ToString("dd/MM/yyyy HH:mm"), ref yPos);
                }

                if (row["autorizador"].ToString() != "-")
                {
                    AgregarCampo(infoPanel, "Autorizador:", row["autorizador"].ToString(), ref yPos);
                    if (row["fecha_autorizacion"] != DBNull.Value)
                    {
                        AgregarCampo(infoPanel, "Fecha Autorización:", Convert.ToDateTime(row["fecha_autorizacion"]).ToString("dd/MM/yyyy HH:mm"), ref yPos);
                    }
                }

                // Cargar comentarios existentes
                if (row["comentarios_autorizacion"] != DBNull.Value)
                {
                    txtComentarios.Text = row["comentarios_autorizacion"].ToString();
                }

                if (row["motivo_rechazo"] != DBNull.Value)
                {
                    txtMotivoRechazo.Text = row["motivo_rechazo"].ToString();
                    lblMotivoRechazo.Visible = true;
                    txtMotivoRechazo.Visible = true;
                }

                // Deshabilitar botones si ya está procesada
                if (estadoActual == "Autorizada" || estadoActual == "Rechazada" || estadoActual == "Expirada")
                {
                    foreach (Control ctrl in botonesPanel.Controls)
                    {
                        if (ctrl is Button && ctrl.Text != "Cerrar")
                        {
                            ctrl.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Cargar Detalles",
                    $"No se pudieron cargar los detalles de la solicitud.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void AgregarCampo(Panel panel, string etiqueta, string valor, ref int yPos)
        {
            Label lblEtiqueta = new Label
            {
                Text = etiqueta,
                Location = new Point(0, yPos),
                Size = new Size(180, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = BankTheme.TextSecondary
            };

            Label lblValor = new Label
            {
                Text = valor,
                Location = new Point(185, yPos),
                Size = new Size(520, 20),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextPrimary
            };

            panel.Controls.AddRange(new Control[] { lblEtiqueta, lblValor });
            yPos += 25;
        }

        private void ActualizarColorEstado()
        {
            switch (estadoActual)
            {
                case "Pendiente":
                    lblEstadoActual.ForeColor = Color.FromArgb(146, 64, 14);
                    lblEstadoActual.BackColor = Color.FromArgb(255, 243, 205);
                    break;
                case "En Revisión":
                    lblEstadoActual.ForeColor = Color.FromArgb(30, 64, 175);
                    lblEstadoActual.BackColor = Color.FromArgb(219, 234, 254);
                    break;
                case "Autorizada":
                    lblEstadoActual.ForeColor = Color.FromArgb(22, 101, 52);
                    lblEstadoActual.BackColor = Color.FromArgb(220, 252, 231);
                    break;
                case "Rechazada":
                    lblEstadoActual.ForeColor = Color.FromArgb(153, 27, 27);
                    lblEstadoActual.BackColor = Color.FromArgb(254, 226, 226);
                    break;
                case "Expirada":
                    lblEstadoActual.ForeColor = Color.FromArgb(64, 64, 64);
                    lblEstadoActual.BackColor = Color.FromArgb(229, 229, 229);
                    break;
            }
        }

        private void CambiarEstado(string nuevoEstado)
        {
            try
            {
                // Validaciones
                if (estadoActual == "Autorizada" || estadoActual == "Rechazada" || estadoActual == "Expirada")
                {
                    CustomMessageBox.Show("Operación No Permitida",
                        "Esta solicitud ya ha sido procesada y no puede modificarse.",
                        MessageBoxIcon.Warning);
                    return;
                }

                if (nuevoEstado == "Rechazada" && string.IsNullOrWhiteSpace(txtMotivoRechazo.Text))
                {
                    CustomMessageBox.Show("Motivo Requerido",
                        "Debe proporcionar un motivo para rechazar la solicitud.",
                        MessageBoxIcon.Warning);
                    return;
                }

                // Confirmar acción
                string mensaje = nuevoEstado == "Autorizada" 
                    ? "¿Está seguro de autorizar esta operación en divisa?" 
                    : nuevoEstado == "Rechazada"
                    ? "¿Está seguro de rechazar esta solicitud?"
                    : "¿Marcar esta solicitud como 'En Revisión'?";

                DialogResult result = MessageBox.Show(mensaje, "Confirmar Acción", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                // Actualizar en base de datos
                string query = @"
                    UPDATE solicitudes_autorizacion_divisas 
                    SET estado = @estado,
                        id_usuario_autorizador = @idUsuario,
                        fecha_autorizacion = @fechaAutorizacion,
                        comentarios_autorizacion = @comentarios,
                        motivo_rechazo = @motivoRechazo
                    WHERE id_solicitud = @idSolicitud";

                Database.ExecuteNonQuery(query,
                    new NpgsqlParameter("@estado", nuevoEstado),
                    new NpgsqlParameter("@idUsuario", FormLogin.IdUsuarioActual),
                    new NpgsqlParameter("@fechaAutorizacion", DateTime.Now),
                    new NpgsqlParameter("@comentarios", txtComentarios.Text ?? ""),
                    new NpgsqlParameter("@motivoRechazo", txtMotivoRechazo.Text ?? ""),
                    new NpgsqlParameter("@idSolicitud", idSolicitud));

                // Registrar en auditoría
                AuditLogger.Log(AuditLogger.AuditAction.CambioConfiguracion,
                    $"AUTORIZACION_DIVISA_{nuevoEstado.ToUpper()} - Solicitud ID {idSolicitud} - Estado: {nuevoEstado}");

                CustomMessageBox.Show("Operación Exitosa",
                    $"La solicitud ha sido marcada como '{nuevoEstado}'.",
                    MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Cambiar Estado",
                    $"No se pudo actualizar el estado de la solicitud.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }
    }
}
