using System.Runtime.Serialization;

namespace BackEndProducts.Api.Model
{
    public abstract class GenericBaseModel<T> : BaseModel
    {
        public T Data
        {
            get;
            set;
        }

        public List<T> DataList
        {
            get;
            set;
        }
    }
}

