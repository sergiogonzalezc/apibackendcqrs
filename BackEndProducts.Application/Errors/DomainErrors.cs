using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndProducts.Application.Errors
{
    public static class DomainErrors
    {
        public static readonly Error ProductCreationId = new Error() { code = 10, message = "Error: valor inválido" };
        public static readonly Error ProductCreationIdEmpty = new Error() { code = 11, message = "Valor no válido. Vuelva a intentar ingresando un valor mayor que cero" };

        public static readonly Error ProductCreationNameInvalid = new Error() { code = 20, message = "Valor debe ser inferior a 50 caracteres" };
        public static readonly Error ProductCreationNameIsEmpty = new Error() { code = 21, message = "Valor no válido. Vuelva a intentar ingresando un valor mayor que cero" };

        public static readonly Error ProductCreationInvalidStock = new Error() { code = 30, message = "Error: stock no válido" };
        public static readonly Error ProductCreationStockIsEmpty = new Error() { code = 31, message = "Error: stock no válido" };

        public static readonly Error ProductCreationPriceInvalid = new Error() { code = 40, message = "Valor no válido. Vuelva a intentar ingresando un valor mayor que cero" };
        public static readonly Error ProductCreationPriceIsEmpty = new Error() { code = 41, message = "Error: precio unitario no válido" };

        public static readonly Error ProductCreationDescriptionInvalid = new Error() { code = 51, message = "Valor no válido. Valor debe ser inferior a 100 caracteres" };

    }

    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}
