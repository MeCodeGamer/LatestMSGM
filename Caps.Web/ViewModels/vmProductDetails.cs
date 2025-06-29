namespace MSGM.Web.ViewModels
{
    public class vmProductDetails
    {
        public int productId { get; set; }
        public string productCategory { get; set; }
        public string? productTitle { get; set; }
        public string? productDescription { get; set; }
        public double productPrice { get; set; }
        public string? productImage { get; set; }
        public bool productStatus { get; set; }
    }
}
