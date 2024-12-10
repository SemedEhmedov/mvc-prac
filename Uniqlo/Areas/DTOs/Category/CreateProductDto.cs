namespace Uniqlo.Areas.DTOs.Category
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double CostPrice { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public int CategoryId { get; set; }
    }
}
