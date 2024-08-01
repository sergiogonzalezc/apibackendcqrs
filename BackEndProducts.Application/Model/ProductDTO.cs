using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BackEndProducts.Application.Model
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public int Status { get; set; }

        public string StatusName
        {
            get
            {
                return (Status == 1) ? "Active" : "Inactive";
            }
        }

        public int Stock { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public decimal FinalPrice { get; }
    }
}
