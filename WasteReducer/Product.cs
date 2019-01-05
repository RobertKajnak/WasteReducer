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

        /// <summary>
        /// Creates a product based on specified parameters. The expiry date is accepted as dateTime.  
        /// To omit expiry date see <see cref="ProductBase"/>
        /// </summary>
        public Product(long id, string imageName, string label, string category, bool isDiary,
                int limit, double price, int facing, DateTime expiryDate) :
            base(id, imageName, label, category, isDiary, limit, price, facing)
        {
            this.ExpiryDate = expiryDate;
        }


        /// <summary>
        /// Creates a product based on specified parameters. The expiry date is accepted as an offset fromt he current date
        /// To omit expiry date see <see cref="ProductBase"/>
        /// <param name="daysUntilExpiry"> 0==today, 1==tomorrow etc. </param>
        /// </summary>

        public Product(long id, string imageName, string label, string category, bool isDiary,
                        int limit, double price, int facing, int daysUntilExpiry) :
            this(id, imageName, label, category, isDiary, limit, price, facing, DateTime.Today.AddDays(daysUntilExpiry))
        { }

        #region Overloaded variants that take a product or productBase as starting point
        public Product(Product p) : this(p.Id, p.ImageName, p.Label, p.Category, p.IsDiary, p.Limit, p.Price, p.Facing, p.ExpiryDate) { }

        public Product(ProductBase p, DateTime expiryDate) : this(p.Id, p.ImageName, p.Label, p.Category, p.IsDiary, p.Limit, p.Price, p.Facing, expiryDate) { }

        public Product(ProductBase p, int daysUntilExpiry) : this(p, DateTime.Today.AddDays(daysUntilExpiry)) { }

        #endregion

        #region Equals and Hash
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
        #endregion
    }

}
