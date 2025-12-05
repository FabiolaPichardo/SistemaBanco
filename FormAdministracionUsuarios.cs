using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaBanco
{
    public partial class FormAdministracionUsuarios : Form
    {
        private DataGridView dgvUsuarios;
        private TextBox txtBusqueda;
        private ComboBox cmbFiltroRol;
        private ComboBox cmbFiltroEstado;
        private Button btnAplicarFiltros;
        private Button btnLimpiarFiltros;
        private Label lblTotalRegistros;
        private Label lblPaginacion;
        private Button btnAnterior;
        private Button btnSiguiente;

        private int paginaActual = 1;
        private int registrosPorPagina = 25;
        private int totalPaginas = 0;
        private string ordenColumna = "fecha_registro";
        private string ordenDireccion = "DESC";

        public FormAdministracionUsuarios()
        {
            InitializeComponent();

            this.Load += (s, e) => CargarUsuarios();
        }

        private void InitializeComponent()
        {
            this.Text = "Administración de Usuarios";
            this.ClientSize = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BankTheme.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Panel headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1200, 80),
                BackColor = BankTheme.PrimaryBlue
            };

            Button btnVolver = HomeButton.Create(this);
            btnVolver.Location = new Point(20, 20);

            Label lblTitulo = new Label
            {
                Text = "👥 Administración de Usuarios",
                Location = new Point(120, 20),
                Size = new Size(500, 40),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = BankTheme.White
            };

            headerPanel.Controls.AddRange(new Control[] { btnVolver, lblTitulo });

            Panel panelFiltros = BankTheme.CreateCard(20, 100, 1160, 100);

            Label lblBusqueda = new Label
            {
                Text = "🔍 Búsqueda:",
                Location = new Point(20, 20),
                Size = new Size(100, 25),
                Font = BankTheme.BodyFont
            };

            txtBusqueda = new TextBox
            {
                Location = new Point(120, 18),
                Size = new Size(250, 25),
                Font = BankTheme.BodyFont
            };
            txtBusqueda.TextChanged += (s, e) => AplicarFiltros();

            Label lblRol = new Label
            {
                Text = "Rol:",
                Location = new Point(400, 20),
                Size = new Size(50, 25),
                Font = BankTheme.BodyFont
            };

            cmbFiltroRol = new ComboBox
            {
                Location = new Point(450, 18),
                Size = new Size(150, 25),
                Font = BankTheme.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFiltroRol.Items.AddRange(new object[] { "Todos", "Cliente", "Cajero", "Ejecutivo", "Gerente", "Administrador" });
            cmbFiltroRol.SelectedIndex = 0;
            cmbFiltroRol.SelectedIndexChanged += (s, e) => AplicarFiltros();

            Label lblEstado = new Label
            {
                Text = "Estado:",
                Location = new Point(630, 20),
                Size = new Size(60, 25),
                Font = BankTheme.BodyFont
            };

            cmbFiltroEstado = new ComboBox
            {
                Location = new Point(690, 18),
                Size = new Size(120, 25),
                Font = BankTheme.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFiltroEstado.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cmbFiltroEstado.SelectedIndex = 0;
            cmbFiltroEstado.SelectedIndexChanged += (s, e) => AplicarFiltros();

            btnLimpiarFiltros = new Button
            {
                Text = "🔄 Limpiar",
                Location = new Point(840, 15),
                Size = new Size(120, 30)
            };
            BankTheme.StyleButton(btnLimpiarFiltros, false);
            btnLimpiarFiltros.Click += BtnLimpiarFiltros_Click;

            lblTotalRegistros = new Label
            {
                Text = "Total: 0 usuarios",
                Location = new Point(20, 60),
                Size = new Size(300, 25),
                Font = BankTheme.BodyFont,
                ForeColor = BankTheme.TextSecondary
            };

            panelFiltros.Controls.AddRange(new Control[] { 
                lblBusqueda, txtBusqueda, lblRol, cmbFiltroRol, 
                lblEstado, cmbFiltroEstado, btnLimpiarFiltros, lblTotalRegistros 
            });

            dgvUsuarios = new DataGridView
            {
                Location = new Point(20, 220),
                Size = new Size(1160, 400),
                Font = BankTheme.BodyFont,
                BackgroundColor = BankTheme.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 35 }
            };

            dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor = BankTheme.PrimaryBlue;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = BankTheme.White;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.Font = BankTheme.HeaderFont;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvUsuarios.EnableHeadersVisualStyles = false;
            dgvUsuarios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvUsuarios.ColumnHeaderMouseClick += DgvUsuarios_ColumnHeaderMouseClick;

            Panel panelPaginacion = new Panel
            {
                Location = new Point(20, 630),
                Size = new Size(1160, 50),
                BackColor = BankTheme.White
            };

            btnAnterior = new Button
            {
                Text = "◀ Anterior",
                Location = new Point(20, 10),
                Size = new Size(120, 30)
            };
            BankTheme.StyleButton(btnAnterior, false);
            btnAnterior.Click += (s, e) => CambiarPagina(-1);

            lblPaginacion = new Label
            {
                Text = "Página 1 de 1",
                Location = new Point(480, 10),
                Size = new Size(200, 30),
                Font = BankTheme.BodyFont,
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnSiguiente = new Button
            {
                Text = "Siguiente ▶",
                Location = new Point(1020, 10),
                Size = new Size(120, 30)
            };
            BankTheme.StyleButton(btnSiguiente, false);
            btnSiguiente.Click += (s, e) => CambiarPagina(1);

            panelPaginacion.Controls.AddRange(new Control[] { btnAnterior, lblPaginacion, btnSiguiente });

            this.Controls.AddRange(new Control[] { 
                headerPanel, panelFiltros, dgvUsuarios, panelPaginacion 
            });
        }

        private void CargarUsuarios()
        {
            try
            {

                string whereClause = "WHERE 1=1";

                if (!string.IsNullOrWhiteSpace(txtBusqueda.Text))
                {
                    string busqueda = txtBusqueda.Text.Trim();
                    whereClause += $" AND (LOWER(usuario) LIKE '%{busqueda.ToLower()}%' " +
                                  $"OR LOWER(nombre_completo) LIKE '%{busqueda.ToLower()}%' " +
                                  $"OR LOWER(email) LIKE '%{busqueda.ToLower()}%')";
                }

                if (cmbFiltroRol.SelectedIndex > 0)
                {
                    whereClause += $" AND rol = '{cmbFiltroRol.SelectedItem}'";
                }

                if (cmbFiltroEstado.SelectedIndex > 0)
                {
                    bool activo = cmbFiltroEstado.SelectedItem.ToString() == "Activo";
                    whereClause += $" AND estatus = {activo}";
                }

                string queryCount = $"SELECT COUNT(*) FROM usuarios {whereClause}";
                object resultCount = Database.ExecuteScalar(queryCount);
                int totalRegistros = resultCount != null ? Convert.ToInt32(resultCount) : 0;

                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
                if (totalPaginas == 0) totalPaginas = 1;
                if (paginaActual > totalPaginas) paginaActual = totalPaginas;

                int offset = (paginaActual - 1) * registrosPorPagina;
                string query = $@"
                    SELECT 
                        id_usuario,
                        usuario,
                        nombre_completo,
                        email,
                        rol,
                        TO_CHAR(fecha_registro, 'DD/MM/YYYY') as fecha_registro,
                        CASE WHEN estatus THEN 'Activo' ELSE 'Inactivo' END as estado,
                        estatus
                    FROM usuarios
                    {whereClause}
                    ORDER BY {ordenColumna} {ordenDireccion}
                    LIMIT {registrosPorPagina} OFFSET {offset}";

                DataTable dt = Database.ExecuteQuery(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dgvUsuarios.DataSource = dt;
                    ConfigurarColumnas();
                    AgregarBotonesAccion();
                }
                else
                {
                    dgvUsuarios.DataSource = null;
                    if (!string.IsNullOrWhiteSpace(txtBusqueda.Text) || 
                        cmbFiltroRol.SelectedIndex > 0 || 
                        cmbFiltroEstado.SelectedIndex > 0)
                    {
                        MessageBox.Show("No se encontraron registros que coincidan con los filtros aplicados.",
                            "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                lblTotalRegistros.Text = $"Total: {totalRegistros} usuario(s)";
                lblPaginacion.Text = $"Página {paginaActual} de {totalPaginas}";
                btnAnterior.Enabled = paginaActual > 1;
                btnSiguiente.Enabled = paginaActual < totalPaginas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColumnas()
        {
            if (dgvUsuarios.Columns.Count == 0) return;

            dgvUsuarios.Columns["id_usuario"].Visible = false;
            dgvUsuarios.Columns["estatus"].Visible = false;

            dgvUsuarios.Columns["usuario"].HeaderText = "Usuario";
            dgvUsuarios.Columns["nombre_completo"].HeaderText = "Nombre Completo";
            dgvUsuarios.Columns["email"].HeaderText = "Correo Electrónico";
            dgvUsuarios.Columns["rol"].HeaderText = "Rol";
            dgvUsuarios.Columns["fecha_registro"].HeaderText = "Fecha de Alta";
            dgvUsuarios.Columns["estado"].HeaderText = "Estado";

            dgvUsuarios.Columns["usuario"].Width = 120;
            dgvUsuarios.Columns["nombre_completo"].Width = 200;
            dgvUsuarios.Columns["email"].Width = 220;
            dgvUsuarios.Columns["rol"].Width = 120;
            dgvUsuarios.Columns["fecha_registro"].Width = 120;
            dgvUsuarios.Columns["estado"].Width = 100;
        }

        private void AgregarBotonesAccion()
        {

            if (dgvUsuarios.Columns.Contains("btnEditar"))
                dgvUsuarios.Columns.Remove("btnEditar");
            if (dgvUsuarios.Columns.Contains("btnEliminar"))
                dgvUsuarios.Columns.Remove("btnEliminar");

            DataGridViewButtonColumn btnEditar = new DataGridViewButtonColumn
            {
                Name = "btnEditar",
                HeaderText = "Editar",
                Text = "✏️ Editar",
                UseColumnTextForButtonValue = true,
                Width = 100
            };
            dgvUsuarios.Columns.Add(btnEditar);

            DataGridViewButtonColumn btnEliminar = new DataGridViewButtonColumn
            {
                Name = "btnEliminar",
                HeaderText = "Eliminar",
                Text = "🗑️ Eliminar",
                UseColumnTextForButtonValue = true,
                Width = 100
            };
            dgvUsuarios.Columns.Add(btnEliminar);

            dgvUsuarios.CellContentClick -= DgvUsuarios_CellContentClick;

            dgvUsuarios.CellContentClick += DgvUsuarios_CellContentClick;
        }

        private void DgvUsuarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvUsuarios.Rows[e.RowIndex];
            int idUsuario = Convert.ToInt32(row.Cells["id_usuario"].Value);
            string nombreUsuario = row.Cells["usuario"].Value.ToString();

            if (dgvUsuarios.Columns[e.ColumnIndex].Name == "btnEditar")
            {
                EditarUsuario(idUsuario);
            }
            else if (dgvUsuarios.Columns[e.ColumnIndex].Name == "btnEliminar")
            {
                EliminarUsuario(idUsuario, nombreUsuario);
            }
        }

        private void EditarUsuario(int idUsuario)
        {
            try
            {

                string query = $@"SELECT * FROM usuarios WHERE id_usuario = {idUsuario}";
                DataTable dt = Database.ExecuteQuery(query);

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontró el usuario.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow usuario = dt.Rows[0];

                Form formEditar = new Form
                {
                    Text = "Editar Usuario",
                    Size = new Size(500, 450),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = BankTheme.LightGray
                };

                Panel panelHeader = new Panel
                {
                    Location = new Point(0, 0),
                    Size = new Size(500, 60),
                    BackColor = BankTheme.PrimaryBlue
                };

                Label lblTituloModal = new Label
                {
                    Text = "✏️ Editar Usuario",
                    Location = new Point(20, 15),
                    Size = new Size(400, 30),
                    Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                    ForeColor = BankTheme.White
                };
                panelHeader.Controls.Add(lblTituloModal);

                int yPos = 80;

                Label lblUsuario = new Label
                {
                    Text = "Usuario:",
                    Location = new Point(30, yPos),
                    Size = new Size(150, 25),
                    Font = BankTheme.BodyFont
                };

                TextBox txtUsuario = new TextBox
                {
                    Location = new Point(200, yPos),
                    Size = new Size(250, 25),
                    Font = BankTheme.BodyFont,
                    Text = usuario["usuario"].ToString(),
                    ReadOnly = true,
                    BackColor = Color.LightGray
                };

                yPos += 40;
                Label lblNombre = new Label
                {
                    Text = "Nombre Completo:",
                    Location = new Point(30, yPos),
                    Size = new Size(150, 25),
                    Font = BankTheme.BodyFont
                };

                TextBox txtNombre = new TextBox
                {
                    Location = new Point(200, yPos),
                    Size = new Size(250, 25),
                    Font = BankTheme.BodyFont,
                    Text = usuario["nombre_completo"].ToString()
                };

                yPos += 40;
                Label lblEmail = new Label
                {
                    Text = "Correo Electrónico:",
                    Location = new Point(30, yPos),
                    Size = new Size(150, 25),
                    Font = BankTheme.BodyFont
                };

                TextBox txtEmail = new TextBox
                {
                    Location = new Point(200, yPos),
                    Size = new Size(250, 25),
                    Font = BankTheme.BodyFont,
                    Text = usuario["email"].ToString()
                };

                yPos += 40;
                Label lblRol = new Label
                {
                    Text = "Rol:",
                    Location = new Point(30, yPos),
                    Size = new Size(150, 25),
                    Font = BankTheme.BodyFont
                };

                ComboBox cmbRol = new ComboBox
                {
                    Location = new Point(200, yPos),
                    Size = new Size(250, 25),
                    Font = BankTheme.BodyFont,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                cmbRol.Items.AddRange(new object[] { "Cliente", "Cajero", "Ejecutivo", "Gerente", "Administrador" });
                cmbRol.SelectedItem = usuario["rol"].ToString();

                yPos += 40;
                Label lblEstado = new Label
                {
                    Text = "Estado:",
                    Location = new Point(30, yPos),
                    Size = new Size(150, 25),
                    Font = BankTheme.BodyFont
                };

                ComboBox cmbEstado = new ComboBox
                {
                    Location = new Point(200, yPos),
                    Size = new Size(250, 25),
                    Font = BankTheme.BodyFont,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                cmbEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });
                cmbEstado.SelectedItem = Convert.ToBoolean(usuario["estatus"]) ? "Activo" : "Inactivo";

                Button btnGuardar = new Button
                {
                    Text = "✅ Guardar Cambios",
                    Location = new Point(120, 350),
                    Size = new Size(150, 40)
                };
                BankTheme.StyleButton(btnGuardar, true);

                Button btnCancelar = new Button
                {
                    Text = "❌ Cancelar",
                    Location = new Point(290, 350),
                    Size = new Size(150, 40)
                };
                BankTheme.StyleButton(btnCancelar, false);

                btnGuardar.Click += (s, e) =>
                {

                    if (string.IsNullOrWhiteSpace(txtNombre.Text))
                    {
                        MessageBox.Show("El nombre completo es obligatorio.", "Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@"))
                    {
                        MessageBox.Show("Ingrese un correo electrónico válido.", "Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (cmbRol.SelectedIndex < 0)
                    {
                        MessageBox.Show("Seleccione un rol.", "Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        string queryUpdate = $@"
                            UPDATE usuarios 
                            SET nombre_completo = '{txtNombre.Text.Trim()}',
                                email = '{txtEmail.Text.Trim()}',
                                rol = '{cmbRol.SelectedItem}',
                                estatus = {(cmbEstado.SelectedItem.ToString() == "Activo" ? "TRUE" : "FALSE")}
                            WHERE id_usuario = {idUsuario}";

                        int filasAfectadas = Database.ExecuteNonQuery(queryUpdate);

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("✅ Usuario actualizado correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            formEditar.DialogResult = DialogResult.OK;
                            formEditar.Close();
                            CargarUsuarios();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar el usuario.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al actualizar usuario: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                btnCancelar.Click += (s, e) => formEditar.Close();

                formEditar.Controls.AddRange(new Control[] {
                    panelHeader, lblUsuario, txtUsuario, lblNombre, txtNombre,
                    lblEmail, txtEmail, lblRol, cmbRol, lblEstado, cmbEstado,
                    btnGuardar, btnCancelar
                });

                formEditar.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir formulario de edición: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarUsuario(int idUsuario, string nombreUsuario)
        {
            try
            {

                string queryDependencias = $@"
                    SELECT 
                        (SELECT COUNT(*) FROM movimientos_financieros WHERE id_usuario = {idUsuario}) as movimientos,
                        (SELECT COUNT(*) FROM cuentas WHERE id_usuario = {idUsuario}) as cuentas";

                DataTable dtDep = Database.ExecuteQuery(queryDependencias);

                if (dtDep != null && dtDep.Rows.Count > 0)
                {
                    int movimientos = Convert.ToInt32(dtDep.Rows[0]["movimientos"]);
                    int cuentas = Convert.ToInt32(dtDep.Rows[0]["cuentas"]);

                    if (movimientos > 0 || cuentas > 0)
                    {
                        string mensaje = $"⚠️ No se puede eliminar el usuario '{nombreUsuario}' porque tiene:\n\n";
                        if (cuentas > 0) mensaje += $"• {cuentas} cuenta(s) asociada(s)\n";
                        if (movimientos > 0) mensaje += $"• {movimientos} movimiento(s) financiero(s)\n";
                        mensaje += "\nPrimero debe resolver estas dependencias o desactivar el usuario.";

                        MessageBox.Show(mensaje, "No se puede eliminar",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                DialogResult resultado = MessageBox.Show(
                    $"¿Está seguro de eliminar el usuario '{nombreUsuario}'?\n\n" +
                    "⚠️ Esta acción es IRREVERSIBLE y se registrará en los logs de auditoría.\n\n" +
                    "El usuario será eliminado permanentemente del sistema.",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (resultado == DialogResult.Yes)
                {

                    string queryAuditoria = $@"
                        INSERT INTO historial_movimientos 
                        (id_movimiento, campo_modificado, valor_anterior, valor_nuevo, 
                         usuario_modificacion, fecha_modificacion, comentarios)
                        VALUES 
                        (0, 'usuario_eliminado', '{nombreUsuario}', 'ELIMINADO', 
                         '{FormLogin.UsuarioActual}', CURRENT_TIMESTAMP, 
                         'Usuario eliminado por administrador ID: {FormLogin.IdUsuario}')";

                    try
                    {
                        Database.ExecuteNonQuery(queryAuditoria);
                    }
                    catch
                    {

                    }

                    string queryDelete = $"DELETE FROM usuarios WHERE id_usuario = {idUsuario}";
                    int filasAfectadas = Database.ExecuteNonQuery(queryDelete);

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show($"✅ Usuario '{nombreUsuario}' eliminado correctamente.\n\n" +
                            "La acción ha sido registrada en los logs de auditoría.",
                            "Usuario Eliminado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        CargarUsuarios();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el usuario.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar usuario: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvUsuarios_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0) return;

            string columnName = dgvUsuarios.Columns[e.ColumnIndex].Name;

            if (columnName == "btnEditar" || columnName == "btnEliminar") return;

            if (ordenColumna == columnName)
            {
                ordenDireccion = ordenDireccion == "ASC" ? "DESC" : "ASC";
            }
            else
            {
                ordenColumna = columnName;
                ordenDireccion = "ASC";
            }

            paginaActual = 1;
            CargarUsuarios();
        }

        private void AplicarFiltros()
        {
            paginaActual = 1;
            CargarUsuarios();
        }

        private void BtnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtBusqueda.Clear();
            cmbFiltroRol.SelectedIndex = 0;
            cmbFiltroEstado.SelectedIndex = 0;
            paginaActual = 1;
            CargarUsuarios();
        }

        private void CambiarPagina(int direccion)
        {
            int nuevaPagina = paginaActual + direccion;
            if (nuevaPagina >= 1 && nuevaPagina <= totalPaginas)
            {
                paginaActual = nuevaPagina;
                CargarUsuarios();
            }
        }
    }
}
