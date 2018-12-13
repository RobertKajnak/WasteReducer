using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasteReducer
{
    public class Product
    {
        public long Id { get; }
        public string Label { get; }
        public string Category { get; }
        public bool IsDiary { get; }
        public int Limit { get; }
        public string ImageName { get; }
        public double Price { get; }
        public int Facing { get; }
        private DateTime expiryDate;
        public DateTime ExpiryDate { get => expiryDate; set => SetExpiry(value); }

        public Product(long id, string imageName, string label, string category, bool isDiary, 
                        int limit, double price,int facing, DateTime expiryDate)
        {
            this.Id = id;
            this.Label = label;
            this.Category = category;
            this.ImageName = imageName;
            this.IsDiary = isDiary;
            this.Limit = limit;
            this.Facing = facing;
            this.Price = price;
            this.expiryDate = expiryDate;
        }

        public Product(long id, string imageName, string label, string category, bool isDiary,
                        int limit, double price, int facing, int daysUntilExpiry) :
            this(id, imageName, label, category, isDiary, limit, price, facing, DateTime.Today.AddDays(daysUntilExpiry)){ }

        public Product(Product p) : this(p.Id, p.ImageName, p.Label, p.Category, p.IsDiary, p.Limit, p.Price, p.Facing, p.ExpiryDate) { }

        public void SetExpiry(int daysUntilExpiry)
        {
            SetExpiry(DateTime.Today.AddDays(daysUntilExpiry));
        }
        public void SetExpiry(DateTime expirydate)
        {
            this.expiryDate = expirydate;
        }
    }

}
