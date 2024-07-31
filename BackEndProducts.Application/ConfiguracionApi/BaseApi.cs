using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndProducts.Application.ConfiguracionApi
{
    public abstract class BaseApi
    {
        public string UrlWeb
        {
            get;
            set;
        }

        public string UrlApi
        {
            get;
            set;
        }

        public string ApiKey
        {
            get;
            set;
        }

        public string User
        {
            get;
            set;
        }

        public string Pass
        {
            get;
            set;
        }
    }
}
