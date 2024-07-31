using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndProducts.Sql.Queries
{
    [ExcludeFromCodeCoverage]
    public static class ProductQueries
    {
        public static string AllProduct => @"SELECT 
                                                p.ProductId, p.Name, p.Status, p.Stock, p.Description, p.Price, p.Discount, p.FinalPrice 
                                                FROM [Product] p (NOLOCK) 
                                                order by p.ProductId asc 
											    OFFSET @PageSize * (@PageNumber-1) ROWS FETCH NEXT @PageSize ROWS ONLY";

        public static string AllProductById => @"SELECT 
                                                p.ProductId, p.Name, p.Status, p.Stock, p.Description, p.Price, p.Discount, p.FinalPrice 
                                                FROM [Product] p (NOLOCK) 
                                                where p.ProductId = @ProductId 
                                                order by p.ProductId";

    }
}