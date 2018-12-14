using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasteReducer
{
    public class Product : ProductBase
    {
        public DateTime ExpiryDate { get; }
        public int Count { get; set; } = 1;

        public Product(long id, string imageName, string label, string category, bool isDiary,
                int limit, double price, int facing, DateTime expiryDate) :
            base(id, imageName, label, category, isDiary, limit, price, facing)
        {
            this.ExpiryDate = expiryDate;
        }

        public Product(long id, string imageName, string label, string category, bool isDiary,
                        int limit, double price, int facing, int daysUntilExpiry) :
            this(id, imageName, label, category, isDiary, limit, price, facing, DateTime.Today.AddDays(daysUntilExpiry))
        { }

        public Product(Product p) : this(p.Id, p.ImageName, p.Label, p.Category, p.IsDiary, p.Limit, p.Price, p.Facing, p.ExpiryDate) { }

        public Product(ProductBase p, DateTime expiryDate) : this(p.Id, p.ImageName, p.Label, p.Category, p.IsDiary, p.Limit, p.Price, p.Facing, expiryDate) { }

        public Product(ProductBase p, int daysUntilExpiry) : this(p, DateTime.Today.AddDays(daysUntilExpiry)) { }

        public override bool Equals(object obj)
        {
            Product other = (Product)obj;
            return other.Id == this.Id && other.ExpiryDate == this.ExpiryDate;
        }

        public override int GetHashCode()
        {
            var hashCode = 679641088;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + ExpiryDate.GetHashCode();
            return hashCode;
        }
    }

}
