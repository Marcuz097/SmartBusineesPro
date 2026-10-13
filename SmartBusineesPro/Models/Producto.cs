using System;
using System.Collections.Generic;
using System.Text;

namespace SmartBusineesPro.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        // Clave foránea y navegación hacia Categoria
        public int CategoriaId { get; set; }
        public virtual Categoria? Categoria { get; set; }
    }
}

