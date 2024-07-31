namespace BackEndProducts.Api.Model
{
    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string? AditionalData { get; set; }
    }

}
