using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace SistemaBanco
{
    public partial class FormConfigRolesDivisas : Form
    {
        private DataGridView dgvConfiguracion;
        private ComboBox cmbDivisa;
        private ComboBox cmbRol;
        private TextBox txtMontoMinimo;
        private TextBox txtMontoMaximo;
        private CheckBox chkActivo;

        public FormConfigRolesDivisas()
        {
            InitializeComponent();

            try
            {
                CargarDivisas();
                CargarConfiguracion();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Inicializar",
                    $"Error al inicializar el formulario.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo Banco - Configuración de Roles Autorizadores";
            this.ClientSize = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Panel headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1000, 70),
                BackColor = BankTheme.PrimaryBlue
            };

            Label lblTitulo = new Label
            {
                Text = "CONFIGURACIÓN DE ROLES AUTORIZADORES POR DIVISA",
                Location = new Point(20, 20),
                Size = new Size(960, 30),
                Font = BankTheme.SubtitleFont,
                ForeColor = BankTheme.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.Add(lblTitulo);

            Panel nuevoPanel = BankTheme.CreateCard(20, 90, 960, 180);

            Label lblNuevo = new Label
            {
                Text = "Nueva Configuración",
                Location = new Point(20, 15),
                Size = new Size(200, 25),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            Label lblDivisa = new Label
            {
                Text = "Divisa:",
                Location = new Point(20, 50),
                Size = new Size(100, 20),
                Font = BankTheme.BodyFont
            };

            cmbDivisa = new ComboBox
            {
                Location = new Point(120, 48),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = BankTheme.BodyFont
            };

            Label lblRol = new Label
            {
                Text = "Rol:",
                Location = new Point(290, 50),
                Size = new Size(80, 20),
                Font = BankTheme.BodyFont
            };

            cmbRol = new ComboBox
            {
                Location = new Point(370, 48),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = BankTheme.BodyFont
            };
            cmbRol.Items.AddRange(new string[] { "Ejecutivo", "Gerente", "Administrador" });
            cmbRol.SelectedIndex = 0;

            Label lblMontoMin = new Label
            {
                Text = "Monto Mínimo:",
                Location = new Point(540, 50),
                Size = new Size(110, 20),
                Font = BankTheme.BodyFont
            };

            txtMontoMinimo = new TextBox
            {
                Location = new Point(650, 48),
                Size = new Size(120, 25),
                Font = BankTheme.BodyFont,
                Text = "0"
            };

            Label lblMontoMax = new Label
            {
                Text = "Monto Máximo:",
                Location = new Point(20, 85),
                Size = new Size(110, 20),
                Font = BankTheme.BodyFont
            };

            txtMontoMaximo = new TextBox
            {
                Location = new Point(130, 83),
                Size = new Size(150, 25),
                Font = BankTheme.BodyFont,
                PlaceholderText = "Dejar vacío = sin límite"
            };

            chkActivo = new CheckBox
            {
                Text = "Activo",
                Location = new Point(300, 85),
                Size = new Size(100, 25),
                Font = BankTheme.BodyFont,
                Checked = true
            };

            Button btnAgregar = new Button
            {
                Text = "➕ Agregar Configuración",
                Location = new Point(400, 80),
                Size = new Size(200, 30),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnAgregar, true);
            btnAgregar.Click += BtnAgregar_Click;

            Label lblInfo = new Label
            {
                Text = "💡 Configure qué roles pueden autorizar operaciones en cada divisa y los rangos de monto permitidos.",
                Location = new Point(20, 125),
                Size = new Size(920, 40),
                Font = BankTheme.SmallFont,
                ForeColor = BankTheme.TextSecondary
            };

            nuevoPanel.Controls.AddRange(new Control[] {
                lblNuevo, lblDivisa, cmbDivisa, lblRol, cmbRol,
                lblMontoMin, txtMontoMinimo, lblMontoMax, txtMontoMaximo,
                chkActivo, btnAgregar, lblInfo
            });

            Label lblExistentes = new Label
            {
                Text = "Configuraciones Existentes",
                Location = new Point(20, 285),
                Size = new Size(250, 25),
                Font = BankTheme.HeaderFont,
                ForeColor = BankTheme.PrimaryBlue
            };

            dgvConfiguracion = new DataGridView
            {
                Location = new Point(20, 315),
                Size = new Size(960, 295), // Aumentar altura para abarcar más espacio
                Font = BankTheme.BodyFont,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false, // Permitir edición
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, // Cambiar a Fill para que abarque todo el ancho
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false
            };

            dgvConfiguracion.ColumnHeadersDefaultCellStyle.BackColor = BankTheme.PrimaryBlue;
            dgvConfiguracion.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvConfiguracion.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvConfiguracion.ColumnHeadersHeight = 40;
            dgvConfiguracion.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            Button btnEliminar = new Button
            {
                Text = "🗑 Eliminar",
                Location = new Point(20, 625),
                Size = new Size(150, 40),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnEliminar, false);
            btnEliminar.ForeColor = Color.White;
            btnEliminar.Click += BtnEliminar_Click;

            Button btnActualizar = new Button
            {
                Text = "🔄 Actualizar",
                Location = new Point(190, 625),
                Size = new Size(150, 40),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnActualizar, false);
            btnActualizar.Click += (s, e) => CargarConfiguracion();

            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new Point(830, 625),
                Size = new Size(150, 40),
                Font = BankTheme.BodyFont
            };
            BankTheme.StyleButton(btnCerrar, false);
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                headerPanel, nuevoPanel, lblExistentes, dgvConfiguracion,
                btnEliminar, btnActualizar, btnCerrar
            });
        }

        private void CargarDivisas()
        {
            try
            {

                if (cmbDivisa == null)
                {
                    System.Diagnostics.Debug.WriteLine("cmbDivisa no está inicializado");
                    return;
                }

                cmbDivisa.Items.Clear();

                string query = "SELECT codigo, nombre FROM divisas WHERE activa = TRUE ORDER BY codigo";
                DataTable dt = Database.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    cmbDivisa.Items.Add($"{row["codigo"]} - {row["nombre"]}");
                }

                if (cmbDivisa.Items.Count > 0)
                    cmbDivisa.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Cargar Divisas",
                    $"No se pudieron cargar las divisas.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        private void CargarConfiguracion()
        {
            try
            {

                if (dgvConfiguracion == null)
                {
                    System.Diagnostics.Debug.WriteLine("dgvConfiguracion no está inicializado");
                    return;
                }

                string query = @"
                    SELECT 
                        r.id_config,
                        d.codigo AS divisa,
                        d.nombre AS nombre_divisa,
                        r.rol,
                        r.monto_minimo,
                        r.monto_maximo,
                        r.activo,
                        r.fecha_creacion
                    FROM roles_autorizadores_divisas r
                    INNER JOIN divisas d ON r.id_divisa = d.id_divisa
                    ORDER BY d.codigo, r.rol";

                DataTable dt = Database.ExecuteQuery(query);
                dgvConfiguracion.DataSource = dt;

                ConfigurarColumnas();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Cargar Configuración",
                    $"No se pudo cargar la configuración.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColumnas()
        {
            try
            {
                if (dgvConfiguracion == null || dgvConfiguracion.Columns.Count == 0) 
                    return;

                if (dgvConfiguracion.Columns.Contains("id_config"))
                    dgvConfiguracion.Columns["id_config"].Visible = false;

                if (dgvConfiguracion.Columns.Contains("divisa"))
                {
                    dgvConfiguracion.Columns["divisa"].HeaderText = "Divisa";
                    dgvConfiguracion.Columns["divisa"].FillWeight = 10; // Proporción relativa
                }

                if (dgvConfiguracion.Columns.Contains("nombre_divisa"))
                {
                    dgvConfiguracion.Columns["nombre_divisa"].HeaderText = "Nombre Divisa";
                    dgvConfiguracion.Columns["nombre_divisa"].FillWeight = 20;
                }

                if (dgvConfiguracion.Columns.Contains("rol"))
                {
                    dgvConfiguracion.Columns["rol"].HeaderText = "Rol";
                    dgvConfiguracion.Columns["rol"].FillWeight = 15;
                }

                if (dgvConfiguracion.Columns.Contains("monto_minimo"))
                {
                    dgvConfiguracion.Columns["monto_minimo"].HeaderText = "Monto Mínimo";
                    dgvConfiguracion.Columns["monto_minimo"].FillWeight = 15;
                    dgvConfiguracion.Columns["monto_minimo"].DefaultCellStyle.Format = "C2";
                    dgvConfiguracion.Columns["monto_minimo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (dgvConfiguracion.Columns.Contains("monto_maximo"))
                {
                    dgvConfiguracion.Columns["monto_maximo"].HeaderText = "Monto Máximo";
                    dgvConfiguracion.Columns["monto_maximo"].FillWeight = 15;
                    dgvConfiguracion.Columns["monto_maximo"].DefaultCellStyle.Format = "C2";
                    dgvConfiguracion.Columns["monto_maximo"].DefaultCellStyle.NullValue = "Sin límite";
                    dgvConfiguracion.Columns["monto_maximo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (dgvConfiguracion.Columns.Contains("activo"))
                {
                    dgvConfiguracion.Columns["activo"].HeaderText = "Activo";
                    dgvConfiguracion.Columns["activo"].FillWeight = 8;
                    dgvConfiguracion.Columns["activo"].ReadOnly = false; // Permitir edición
                }

                if (dgvConfiguracion.Columns.Contains("fecha_creacion"))
                {
                    dgvConfiguracion.Columns["fecha_creacion"].HeaderText = "Fecha";
                    dgvConfiguracion.Columns["fecha_creacion"].FillWeight = 17;
                    dgvConfiguracion.Columns["fecha_creacion"].DefaultCellStyle.Format = "dd/MM/yy HH:mm";
                    dgvConfiguracion.Columns["fecha_creacion"].ReadOnly = true;
                }

                if (dgvConfiguracion.Columns.Contains("divisa"))
                    dgvConfiguracion.Columns["divisa"].ReadOnly = true;
                if (dgvConfiguracion.Columns.Contains("nombre_divisa"))
                    dgvConfiguracion.Columns["nombre_divisa"].ReadOnly = true;
                if (dgvConfiguracion.Columns.Contains("rol"))
                    dgvConfiguracion.Columns["rol"].ReadOnly = true;
                if (dgvConfiguracion.Columns.Contains("monto_minimo"))
                    dgvConfiguracion.Columns["monto_minimo"].ReadOnly = true;
                if (dgvConfiguracion.Columns.Contains("monto_maximo"))
                    dgvConfiguracion.Columns["monto_maximo"].ReadOnly = true;

                dgvConfiguracion.CellValueChanged -= DgvConfiguracion_CellValueChanged; // Evitar duplicados
                dgvConfiguracion.CellValueChanged += DgvConfiguracion_CellValueChanged;

                dgvConfiguracion.CurrentCellDirtyStateChanged -= DgvConfiguracion_CurrentCellDirtyStateChanged;
                dgvConfiguracion.CurrentCellDirtyStateChanged += DgvConfiguracion_CurrentCellDirtyStateChanged;

                dgvConfiguracion.CellFormatting -= DgvConfiguracion_CellFormatting; // Evitar duplicados
                dgvConfiguracion.CellFormatting += DgvConfiguracion_CellFormatting;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ConfigurarColumnas: {ex.Message}");
            }
        }

        private void DgvConfiguracion_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {

            if (dgvConfiguracion.IsCurrentCellDirty)
            {
                dgvConfiguracion.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvConfiguracion_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;

                if (dgvConfiguracion.Columns[e.ColumnIndex].Name == "activo")
                {
                    int idConfig = Convert.ToInt32(dgvConfiguracion.Rows[e.RowIndex].Cells["id_config"].Value);
                    bool nuevoEstado = Convert.ToBoolean(dgvConfiguracion.Rows[e.RowIndex].Cells["activo"].Value);

                    string query = "UPDATE roles_autorizadores_divisas SET activo = @activo WHERE id_config = @idConfig";
                    Database.ExecuteNonQuery(query,
                        new NpgsqlParameter("@activo", nuevoEstado),
                        new NpgsqlParameter("@idConfig", idConfig));

                    string divisa = dgvConfiguracion.Rows[e.RowIndex].Cells["divisa"].Value.ToString();
                    string rol = dgvConfiguracion.Rows[e.RowIndex].Cells["rol"].Value.ToString();
                    AuditLogger.Log(AuditLogger.AuditAction.CambioConfiguracion,
                        $"CONFIG_ROL_DIVISA_CAMBIO_ESTADO - Divisa: {divisa}, Rol: {rol}, Nuevo Estado: {(nuevoEstado ? "Activo" : "Inactivo")}");

                    CargarConfiguracion();
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Actualizar Estado",
                    $"No se pudo actualizar el estado.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);

                CargarConfiguracion();
            }
        }

        private void DgvConfiguracion_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (dgvConfiguracion.Columns.Contains("activo") && 
                    dgvConfiguracion.Rows[e.RowIndex].Cells["activo"].Value != null)
                {
                    bool activo = Convert.ToBoolean(dgvConfiguracion.Rows[e.RowIndex].Cells["activo"].Value);

                    if (!activo)
                    {

                        e.CellStyle.BackColor = Color.FromArgb(254, 226, 226);
                        e.CellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                    }
                }
            }
            catch
            {

            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            try
            {

                if (cmbDivisa.SelectedIndex < 0)
                {
                    CustomMessageBox.Show("Divisa Requerida",
                        "Por favor, seleccione una divisa.",
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtMontoMinimo.Text, out decimal montoMin) || montoMin < 0)
                {
                    CustomMessageBox.Show("Monto Inválido",
                        "El monto mínimo debe ser un número válido mayor o igual a cero.",
                        MessageBoxIcon.Warning);
                    return;
                }

                decimal? montoMax = null;
                if (!string.IsNullOrWhiteSpace(txtMontoMaximo.Text))
                {
                    if (!decimal.TryParse(txtMontoMaximo.Text, out decimal maxTemp) || maxTemp < montoMin)
                    {
                        CustomMessageBox.Show("Monto Inválido",
                            "El monto máximo debe ser un número válido mayor o igual al monto mínimo.",
                            MessageBoxIcon.Warning);
                        return;
                    }
                    montoMax = maxTemp;
                }

                string codigoDivisa = cmbDivisa.SelectedItem.ToString().Split('-')[0].Trim();
                string queryDivisa = "SELECT id_divisa FROM divisas WHERE codigo = @codigo";
                DataTable dtDivisa = Database.ExecuteQuery(queryDivisa, 
                    new NpgsqlParameter("@codigo", codigoDivisa));

                if (dtDivisa.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Error", "No se encontró la divisa seleccionada.", MessageBoxIcon.Error);
                    return;
                }

                int idDivisa = Convert.ToInt32(dtDivisa.Rows[0]["id_divisa"]);

                string queryExiste = @"SELECT COUNT(*) FROM roles_autorizadores_divisas 
                                      WHERE id_divisa = @idDivisa AND rol = @rol AND activo = TRUE";
                DataTable dtExiste = Database.ExecuteQuery(queryExiste,
                    new NpgsqlParameter("@idDivisa", idDivisa),
                    new NpgsqlParameter("@rol", cmbRol.SelectedItem.ToString()));

                if (Convert.ToInt32(dtExiste.Rows[0][0]) > 0)
                {
                    CustomMessageBox.Show("Configuración Existente",
                        "Ya existe una configuración activa para esta divisa y rol. Elimine o desactive la existente primero.",
                        MessageBoxIcon.Warning);
                    return;
                }

                string queryConflicto = @"
                    SELECT COUNT(*) FROM roles_autorizadores_divisas 
                    WHERE id_divisa = @idDivisa 
                    AND rol = @rol 
                    AND activo = TRUE
                    AND (
                        (@montoMin BETWEEN monto_minimo AND COALESCE(monto_maximo, 999999999))
                        OR (@montoMax BETWEEN monto_minimo AND COALESCE(monto_maximo, 999999999))
                        OR (monto_minimo BETWEEN @montoMin AND COALESCE(@montoMax, 999999999))
                    )";

                DataTable dtConflicto = Database.ExecuteQuery(queryConflicto,
                    new NpgsqlParameter("@idDivisa", idDivisa),
                    new NpgsqlParameter("@rol", cmbRol.SelectedItem.ToString()),
                    new NpgsqlParameter("@montoMin", montoMin),
                    new NpgsqlParameter("@montoMax", (object)montoMax ?? DBNull.Value));

                if (Convert.ToInt32(dtConflicto.Rows[0][0]) > 0)
                {
                    CustomMessageBox.Show("Conflicto de Rangos",
                        "Los rangos de montos se solapan con una configuración existente para esta divisa y rol.",
                        MessageBoxIcon.Warning);
                    return;
                }

                string queryInsert = @"
                    INSERT INTO roles_autorizadores_divisas 
                    (id_divisa, rol, monto_minimo, monto_maximo, activo) 
                    VALUES (@idDivisa, @rol, @montoMin, @montoMax, @activo)";

                Database.ExecuteNonQuery(queryInsert,
                    new NpgsqlParameter("@idDivisa", idDivisa),
                    new NpgsqlParameter("@rol", cmbRol.SelectedItem.ToString()),
                    new NpgsqlParameter("@montoMin", montoMin),
                    new NpgsqlParameter("@montoMax", (object)montoMax ?? DBNull.Value),
                    new NpgsqlParameter("@activo", chkActivo.Checked));

                AuditLogger.Log(AuditLogger.AuditAction.CambioConfiguracion,
                    $"CONFIG_ROL_DIVISA_AGREGAR - Divisa: {codigoDivisa}, Rol: {cmbRol.SelectedItem}, Monto: {montoMin}-{montoMax?.ToString() ?? "Sin límite"}");

                CustomMessageBox.Show("Configuración Agregada",
                    "La configuración se ha agregado correctamente.",
                    MessageBoxIcon.Information);

                txtMontoMinimo.Text = "0";
                txtMontoMaximo.Clear();
                chkActivo.Checked = true;

                CargarConfiguracion();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Agregar Configuración",
                    $"No se pudo agregar la configuración.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvConfiguracion.SelectedRows.Count == 0)
                {
                    CustomMessageBox.Show("Selección Requerida",
                        "Por favor, seleccione una configuración para eliminar.",
                        MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "¿Está seguro de eliminar esta configuración?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                int idConfig = Convert.ToInt32(dgvConfiguracion.SelectedRows[0].Cells["id_config"].Value);

                string query = "DELETE FROM roles_autorizadores_divisas WHERE id_config = @idConfig";
                Database.ExecuteNonQuery(query, new NpgsqlParameter("@idConfig", idConfig));

                AuditLogger.Log(AuditLogger.AuditAction.CambioConfiguracion,
                    $"CONFIG_ROL_DIVISA_ELIMINAR - ID Config: {idConfig}");

                CustomMessageBox.Show("Configuración Eliminada",
                    "La configuración se ha eliminado correctamente.",
                    MessageBoxIcon.Information);

                CargarConfiguracion();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error al Eliminar Configuración",
                    $"No se pudo eliminar la configuración.\n\nDetalle: {ex.Message}",
                    MessageBoxIcon.Error);
            }
        }
    }
}
