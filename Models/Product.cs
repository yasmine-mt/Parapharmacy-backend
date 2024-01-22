namespace backend.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public string Category { get; set; } 

        public Product(int productID, string name, string brand, string description, string imageURL, string category)
        {
            ProductID = productID;
            Name = name;
            Brand = brand;
            Description = description;
            ImageURL = imageURL;
            Category = category;
        }
    }
}
