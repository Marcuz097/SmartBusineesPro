using System;
using System.Collections.Generic;
using System.Text;

namespace SmartBusineesPro.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Propiedad de navegación hacia Productos
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
