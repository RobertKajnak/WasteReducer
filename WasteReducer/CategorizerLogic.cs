using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasteReducer
{
    class CategorizerLogic
    {
        private readonly List<Product> products; 
        private readonly WasteBagConfiguration config;
        private readonly List<Product> shelf;
        private readonly List<Product> expired;
        private readonly List<Product> discounted;
        private readonly List<Product> ZWB_candidates;

        /// <summary>
        /// Generates the items that should be discounted and those that should be left on
        /// the shelfes, those that should get a discount and those that go into the Zero Waste Bags
        /// </summary>
        /// <param name="products">All products from the crate</param>
        /// <param name="config">Class containing all the configurations relevant to the set</param>
        public CategorizerLogic(List<Product> products,WasteBagConfiguration config)
        {
            this.products = products;
            this.config = config;
            expired = new List<Product>();
            shelf = new List<Product>();
            discounted = new List<Product>();
            ZWB_candidates = new List<Product>();

        ///Not only shorthand, but if the date changes during run, the results will be consistent
        DateTime today = DateTime.Today;
            foreach (Product product in products)
            {
                if (product.ExpiryDate <= today)
                {
                    expired.Add(product);
                }
                else if (product.ExpiryDate <= today.AddDays(1))
                {
                    discounted.Add(product);
                    
                }
                else if (product.ExpiryDate <= today.AddDays(3))
                {
                    ZWB_candidates.Add(product);
                }
                else
                {
                    shelf.Add(product);
                }

            }
        }

        private List<List<Product>> generateWasteBags()
        {
            if (ZWB_candidates == null)
                return null;
            else
            {
                List<List<Product>> ZWB = new List<List<Product>>();

                bool[] used = new bool[ZWB_candidates.Count];
                for (int i = 0; i < config.NrOfBags;i++)
                {
                    List<Product> zwb = new List<Product>();
                    for (int j=0;j<ZWB_candidates.Count;j++)
                    {
                        double sum = zwb.Sum(x => x.Price);
                        if (sum <= config.PriceLimitUpper && !used[j])
                        {
                            zwb.Add(ZWB_candidates[j]);
                            used[j] = true;
                        }
                        if (sum >= config.PriceLimitLower)
                        {
                            break;
                        }
                    }

                    ZWB.Add(zwb);
                }
                return ZWB;
            }
        }
        public List<Product> GetShelfItems() => shelf;
        public List<Product> GetExpiredItems() => expired;
        public List<Product> GetDiscountedProducts() => discounted;
        public List<List<Product>> GetWasteBags() => generateWasteBags();

    }
}
