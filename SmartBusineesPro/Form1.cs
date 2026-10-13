using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SmartBusineesPro.Data;

namespace SmartBusineesPro
{
    public partial class Form1 : Form
    {
        // Controles dinámicos creados para el diseño profesional
        private Panel panelHeader = null!;
        private Label lblHeaderTitle = null!;
        private Panel cardTotalProductos = null!;
        private Label lblTotalNum = null!;
        private Panel cardTotalStock = null!;
        private Label lblStockNum = null!;
        private Panel panelGridContainer = null!;

        public Form1()
        {
            InitializeComponent();
            ConfigurarPropiedadesVentana();
            ConstruirComponentesDinamicos();
        }

        private void ConfigurarPropiedadesVentana()
        {
            this.Text = "SmartBusinessPro | Dashboard de Inventario";
            this.BackColor = Color.FromArgb(241, 245, 249); // Slate 100
            this.Size = new Size(900, 620);
            this.MinimumSize = new Size(800, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void ConstruirComponentesDinamicos()
        {
            // 1. Panel de Encabezado Superior (Header)
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(15, 23, 42) // Slate 900 (Oscuro Elegante)
            };

            lblHeaderTitle = new Label
            {
                Text = "📦 Control de Inventario y Stock",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 18)
            };
            panelHeader.Controls.Add(lblHeaderTitle);
            this.Controls.Add(panelHeader);

            // 2. Tarjeta KPI 1: Total Productos
            cardTotalProductos = CrearTarjetaKpi("Total Productos", "0", new Point(30, 80), Color.FromArgb(37, 99, 235));
            lblTotalNum = (Label)cardTotalProductos.Controls[1];
            this.Controls.Add(cardTotalProductos);

            // 3. Tarjeta KPI 2: Unidades en Stock
            cardTotalStock = CrearTarjetaKpi("Total en Stock", "0", new Point(230, 80), Color.FromArgb(16, 185, 129));
            lblStockNum = (Label)cardTotalStock.Controls[1];
            this.Controls.Add(cardTotalStock);

            // 4. Panel Contenedor para el DataGridView (Le da efecto de tarjeta blanca)
            panelGridContainer = new Panel
            {
                Location = new Point(30, 160),
                Size = new Size(825, 330),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(panelGridContainer);

            // Mover el DataGridView dentro del panel contenedor
            if (dataGridViewProductos != null)
            {
                this.Controls.Remove(dataGridViewProductos);
                panelGridContainer.Controls.Add(dataGridViewProductos);
                dataGridViewProductos.Dock = DockStyle.Fill;
            }

            // Reposicionar Botón
            if (button1 != null)
            {
                button1.Location = new Point(695, 510);
                button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            }
        }

        private Panel CrearTarjetaKpi(string titulo, string valorInicial, Point ubicacion, Color colorAcento)
        {
            Panel card = new Panel
            {
                Location = ubicacion,
                Size = new Size(180, 65),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            Label lblTitulo = new Label
            {
                Text = titulo.ToUpper(),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(12, 10),
                AutoSize = true
            };

            Label lblValor = new Label
            {
                Text = valorInicial,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = colorAcento,
                Location = new Point(10, 28),
                AutoSize = true
            };

            card.Controls.Add(lblTitulo);
            card.Controls.Add(lblValor);
            return card;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AplicarEstilosComponentes();
            CargarProductos();
        }

        private void AplicarEstilosComponentes()
        {
            // Estilo del Botón de Acción
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = Color.FromArgb(37, 99, 235);
            button1.ForeColor = Color.White;
            button1.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            button1.Cursor = Cursors.Hand;
            button1.Size = new Size(160, 42);
            button1.Text = "🔄 Cargar Datos";

            // Estilo Avanzado del DataGridView
            dataGridViewProductos.BorderStyle = BorderStyle.None;
            dataGridViewProductos.BackgroundColor = Color.White;
            dataGridViewProductos.EnableHeadersVisualStyles = false;
            dataGridViewProductos.GridColor = Color.FromArgb(241, 245, 249);
            dataGridViewProductos.RowHeadersVisible = false;
            dataGridViewProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewProductos.RowTemplate.Height = 38;
            dataGridViewProductos.AllowUserToAddRows = false;

            // Encabezados
            dataGridViewProductos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewProductos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dataGridViewProductos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewProductos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            dataGridViewProductos.ColumnHeadersHeight = 42;

            // Filas y Selección
            dataGridViewProductos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 231, 255);
            dataGridViewProductos.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
            dataGridViewProductos.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dataGridViewProductos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        private void CargarProductos()
        {
            try
            {
                using (var context = new SmartBusinessContext())
                {
                    if (!context.Database.CanConnect())
                    {
                        MessageBox.Show("No se pudo conectar a SQL Server.", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var productos = context.Productos
                        .Include(p => p.Categoria)
                        .Select(p => new
                        {
                            NombreProducto = p.Nombre,
                            Precio = p.Precio,
                            Existencia = p.Stock,
                            Categoria = p.Categoria != null ? p.Categoria.Nombre : "Sin Categoría"
                        })
                        .ToList();

                    dataGridViewProductos.DataSource = productos;
                    FormatearColumnasGrid();

                    // Actualizar métricas dinámicas
                    lblTotalNum.Text = productos.Count.ToString();
                    lblStockNum.Text = productos.Sum(p => p.Existencia).ToString("N0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar datos:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FormatearColumnasGrid()
        {
            if (dataGridViewProductos.Columns["NombreProducto"] != null)
                dataGridViewProductos.Columns["NombreProducto"].HeaderText = "Producto";

            if (dataGridViewProductos.Columns["Categoria"] != null)
                dataGridViewProductos.Columns["Categoria"].HeaderText = "Categoría";

            if (dataGridViewProductos.Columns["Precio"] != null)
            {
                dataGridViewProductos.Columns["Precio"].DefaultCellStyle.Format = "C2";
                dataGridViewProductos.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridViewProductos.Columns["Precio"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridViewProductos.Columns["Existencia"] != null)
            {
                dataGridViewProductos.Columns["Existencia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewProductos.Columns["Existencia"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CargarProductos();
        }
    }
}