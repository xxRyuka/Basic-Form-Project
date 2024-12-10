namespace FormApp.Models
{
    public static class Repository
    {
        private static readonly List<Product> _products = new List<Product>();
        public static List<Product> Products => _products;

        private static readonly List<Category> _category = new List<Category>();

        public static List<Category> Categories => _category;


        static Repository()
        {
            _category.Add(new Category() { CategoryID = 1, NameCategory = "Telefon" });
            _category.Add(new Category() { CategoryID = 2, NameCategory = "Bilgisayar" });


            _products.Add(new Product() { CategoryID = 1, ProductID = 1, Name = "Xiaomi Note 13 Pro", Price = 20000, IsActive = true, Image = "1.jpg" });
            _products.Add(new Product() { CategoryID = 1, ProductID = 2, Name = "Xiaomi Note 8 Pro", Price = 15000, IsActive = true, Image = "2.jpg" });
            _products.Add(new Product() { CategoryID = 1, ProductID = 3, Name = "Iphone X Max", Price = 35000, IsActive = true, Image = "3.jpg" });
            _products.Add(new Product() { CategoryID = 1, ProductID = 4, Name = "Samsung Galaxy Z Fold", Price = 22000, IsActive = true, Image = "4.jpg" });
            _products.Add(new Product() { CategoryID = 2, ProductID = 6, Name = "Mac Air M2", Price = 75000, IsActive = true, Image = "5.jpg" });
            _products.Add(new Product() { CategoryID = 2, ProductID = 7, Name = "HP Victus 16", Price = 850000, IsActive = true, Image = "6.jpg" });
        }


        public static void EditProduct(Product UpdatedProduct)
        {

            Product currentPR = Repository._products.FirstOrDefault(c => c.ProductID == UpdatedProduct.ProductID)!;

            if (currentPR != null)
            {
                currentPR.Image = UpdatedProduct.Image;
                currentPR.IsActive = UpdatedProduct.IsActive;
                currentPR.Name = UpdatedProduct.Name;
                currentPR.Price = UpdatedProduct.Price;
                currentPR.CategoryID = UpdatedProduct.CategoryID;
            }


        }


        public static void AddProduct(Product item)
        {


            _products.Add(item);
            item.ProductID = Repository.Products.Count() + 1;



        }
    }
}