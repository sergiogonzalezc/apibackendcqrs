using System.Runtime.Serialization;

namespace BackEndProducts.Api.Model
{

    public abstract class BaseModel
    {
        [DataMember]
        public bool Success
        {
            get;
            set;
        }

        [DataMember]
        public string Status
        {
            get;
            set;
        }

        [DataMember]
        public string SubStatus
        {
            get;
            set;
        }

        [DataMember]
        public string Code
        {
            get;
            set;
        }

        //[DataMember]
        //public string ClientMessage
        //{
        //    get
        //    {
        //        return this.Success && this.Code == null ? "Solicitud exitosa." : ClientMessageCodes.Get(this.Code);
        //    }
        //}

        [DataMember]
        public string Message
        {
            get;
            set;
        }

        [DataMember]
        public string Token
        {
            get;
            set;
        }
    }
}

