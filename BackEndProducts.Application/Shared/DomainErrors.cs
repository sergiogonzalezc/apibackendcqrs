using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndProducts.Application.Shared
{
    /// <summary>
    /// Clase que maneja los mensajes de error de forma centralizada
    /// </summary>
    public static class DomainErrors
    {
        public static readonly Error ProductCreationIdInvalid = new Error() { code = 10, message = "Error: valor inválido" };
        public static readonly Error ProductCreationIdEmpty = new Error() { code = 11, message = "Valor no válido. Vuelva a intentar ingresando un valor mayor que cero" };
        public static readonly Error ProductCreationIdDuplicated = new Error() { code = 12, message = "Error: código <ProductId> duplicado" };
        public static readonly Error ProductCreationGenericError= new Error() { code = 13, message = "Error desconocido al insertar producto. Favor revise los parámetros de entrada" };
        public static readonly Error ProductCreationNotFound = new Error() { code = 14, message = "Código de producto no encontrado" };


        public static readonly Error ProductCreationNameInvalid = new Error() { code = 20, message = "Valor debe ser inferior a 50 caracteres" };
        public static readonly Error ProductCreationNameIsEmpty = new Error() { code = 21, message = "Valor no válido. Vuelva a intentar ingresando un valor mayor que cero" };

        public static readonly Error ProductCreationStatusInvalid = new Error() { code = 20, message = "Valor no válido. Valor debe ser 0 o 1" };

        public static readonly Error ProductCreationInvalidStock = new Error() { code = 40, message = "Error: stock no válido" };
        public static readonly Error ProductCreationStockIsEmpty = new Error() { code = 41, message = "Error: stock no válido" };

        public static readonly Error ProductCreationPriceInvalid = new Error() { code = 50, message = "Valor no válido. Vuelva a intentar ingresando un valor mayor que cero" };
        public static readonly Error ProductCreationPriceIsEmpty = new Error() { code = 51, message = "Error: precio unitario no válido" };

        public static readonly Error ProductCreationDescriptionInvalid = new Error() { code = 61, message = "Valor no válido. Valor debe ser inferior a 100 caracteres" };

        // generic error
        public static readonly Error ReadingConfig= new Error() { code = 71, message = "Error desconocido al leer archivo de configuración" };
        public static readonly Error DiscountApiError = new Error() { code = 72, message = "Error leyendo data de endpoint de descuentos" };
        public static readonly Error ErrorUpdatingProduct = new Error() { code = 73, message = "Error al actualizar poroducto. Favro revise los parámetros." };
        public static readonly Error ProductEditionGenericError = new Error() { code = 74, message = "Error desconocido al editar producto. Favor revise los parámetros de entrada" };


    }

    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}
