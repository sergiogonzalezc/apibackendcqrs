using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BackEndProducts.Application.Model
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public int Stock { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public decimal FinalPrice { get; }

    }
}
