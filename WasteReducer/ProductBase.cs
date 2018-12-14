using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasteReducer
{
    public class ProductBase
    {
        public long Id { get; }
        public string Label { get; }
        public string Category { get; }
        public bool IsDiary { get; }
        public int Limit { get; }
        public string ImageName { get; }
        public double Price { get; }
        public int Facing { get; }

        public ProductBase(long id, string imageName, string label, string category, bool isDiary,
                        int limit, double price, int facing)
        {
            this.Id = id;
            this.Label = label;
            this.Category = category;
            this.ImageName = imageName;
            this.IsDiary = isDiary;
            this.Limit = limit;
            this.Facing = facing;
            this.Price = price;
        }

        public ProductBase(long id, string imageName, string label, string category, bool isDiary,
                        int limit, double price, int facing, int daysUntilExpiry) :
            this(id, imageName, label, category, isDiary, limit, price, facing)
        { }

        public ProductBase(ProductBase p) : this(p.Id, p.ImageName, p.Label, p.Category, p.IsDiary, p.Limit, p.Price, p.Facing) { }

        public override bool Equals(object obj)
        {
            return this.Id == ((ProductBase)obj).Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }
    }
}
