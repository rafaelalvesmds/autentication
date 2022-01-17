namespace E_Commerce.API.Data.ValueObjects
{
    public class ProductVO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? CategotyName { get; set; }
        public string? imageUrl { get; set; }
    }
}
