using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using BackEndProducts.DTO;

namespace BackEndProducts.Application.Model
{
    public class InputUpdateProduct
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
