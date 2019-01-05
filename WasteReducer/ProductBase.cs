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

        /// <summary>
        /// Creates a database version of a Product. To add Expiry date see <see cref="Product"/>
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <param name="imageName">The filename of the product icon.</param>
        /// <param name="label">The long name of the product</param>
        /// <param name="category">The category of the product</param>
        /// <param name="isDiary">Specifies wheater the product should be considered for lactose sensitive customers</param>
        /// <param name="limit">How many instances of this category should be in a single bag</param>
        /// <param name="price">Price of the product</param>
        /// <param name="facing">Specifies how highly a product is sold. 5=max, 0=min</param>
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

        /// <summary>
        /// Creates a copy of the ProductBase
        /// </summary>
        /// <param name="p">The object to be copied</param>
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
