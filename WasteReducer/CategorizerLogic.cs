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
        private readonly List<List<Product>> ZWB_final;

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
                    else if (product.ExpiryDate <= today.AddDays(1) || product.Facing>4)
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
            ZWB_final = generateWasteBags();
            foreach (var p in ZWB_candidates)
                discounted.Add(p);
            ZWB_candidates.Clear();
        }

        /// <summary>
        /// Apppends the products from <see cref="ZWB_candidates"/> based on criteria to ZWB.
        /// </summary>
        /// <param name="ZWB"></param>
        /// <param name="criteria"></param>
        private void AddToBags(List<List<Product>> ZWB, List<Predicate<Product>> criteria)
        {
            int n = this.config.NrOfBags;
            ///Initialized the empty bags, if they have not been yet
            if (ZWB.Count == 0)
            {
                for (int i = 0; i < n; i++) ZWB.Add(new List<Product>());
            }

            foreach (var p in criteria)
            {
                ///Filters the candidates by the criteria, in order
                ///Then sorts them by price
                var prods = ZWB_candidates.FindAll(p).OrderBy(x=>-x.Price).ToList();
                if (prods.Count == 0)
                    continue;
                int ip = 0;

                bool nothingAdded = false;
                while (ip<prods.Count)
                {
                    if (nothingAdded)
                        ip++;
                    nothingAdded = true;
                    for (int i = 0; i < n && ip<prods.Count; i++)
                    {
                        ///checks for items within the category of the to-be-added item and ensures it doesn't go over limit
                        if ((ZWB[i].FindAll(x=>x.Category.Equals(prods[ip].Category)).Count<prods[ip].Limit) &&
                            ///Checks to be within price, but try to be close to the lower price
                            (ZWB[i].Sum(x => x.Price)+prods[ip].Price<config.PriceLimitUpper) &&
                            (ZWB[i].Sum(x => x.Price) < config.PriceLimitLower))
                        {
                            ZWB[i].Add(prods[ip]);
                            ZWB_candidates.Remove(prods[ip++]);
                            nothingAdded = false;
                        }
                    }
                }
            }
        }

        private List<List<Product>> generateWasteBags()
        {
            if (ZWB_candidates == null)
                return null;
            else
            {
                int n = this.config.NrOfBags;
                var ZWB = new List<List<Product>>();
                var criteria = new List<Predicate<Product>>();
                criteria.Add(p => p.Category.Equals("salad"));
                criteria.Add(p => p.IsDiary == true);
                criteria.Add(p => p.Category.Equals("big meal"));
                //criteria.Add(p => p.Category.Equals("snack"));

                //Shorthand for the rest:
                criteria.Add(p => p.Price > 0);
                AddToBags(ZWB,criteria);

                return ZWB;
            }
            
        }
        public List<Product> GetShelfItems() => shelf;
        public List<Product> GetExpiredItems() => expired;
        public List<Product> GetDiscountedProducts() => discounted;
        public List<List<Product>> GetWasteBags() => ZWB_final ;

    }
}
