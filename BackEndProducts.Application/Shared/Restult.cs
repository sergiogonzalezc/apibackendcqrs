using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndProducts.Application.Shared
{
    public class Result<T>
    {
        private readonly bool _isSuccess;
        private readonly string _error;
        private readonly T? _value;

        private Result(bool isSuccess, T? value, string error)
        {
            _isSuccess = isSuccess;
            _value = value;
            _error = error;
        }

        //Creamos dos respuesta para Sucess: una con tipo y otra sin argumento

        public static Result<T> Success() => new Result<T>(true, default, null);

        public static Result<T> Success(T value) => new Result<T>(true, value, null);        

        public static Result<T> Failure(string error) => new Result<T>(false, default, error);

        public bool IsSucess => _isSuccess;

        public string Error => _error;        
    }   
}
