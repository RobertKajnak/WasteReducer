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

        /// <summary>
        /// Apppends the products from <see cref="ZWB_candidates"/> based on criteria to ZWB.
        /// </summary>
        /// <param name="ZWB"></param>
        /// <param name="criteria"></param>
        private void AddToBags(List<List<Product>> ZWB, List<Predicate<Product>> criteria)
        {
            int n = this.config.NrOfBags;
            if (ZWB.Count == 0)
            {
                for (int i = 0; i < n; i++) ZWB.Add(new List<Product>());
            }

            foreach (var p in criteria)
            {
                var prods = ZWB_candidates.FindAll(p);
                if (prods.Count == 0)
                    continue;
                int ip = 0;

                bool nothingAdded = false;
                while (!nothingAdded)
                {
                    nothingAdded = true;
                    for (int i = 0; i < n && ip<prods.Count; i++)
                    {
                        if ((ZWB[i].FindAll(x=>x.Category.Equals(prods[ip].Category)).Count<prods[ip].Limit) &&
                            (ZWB[i].Sum(x => x.Price)+prods[ip].Price<config.PriceLimitUpper))
                        {
                            ZWB[i].Add(prods[ip++]);
                            ZWB_candidates.Remove(prods[i]);
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
                criteria.Add(p => p.Category.Equals("Salad"));
                criteria.Add(p => p.IsDiary == true);
                criteria.Add(p => p.Category.Equals("big meal"));

                //Shorthand for the rest:
                criteria.Add(p => p.Price > 0);
                AddToBags(ZWB,criteria);

                /*List<Product> leftovers = new List<Product>();

                ZWB_candidates.FindAll()
                ///Add all salads. 
                var salads = ZWB_candidates.FindAll(x => x.Category.Equals("Salad"));
                for (int i = 0; i < Math.Min(n,salads.Count); i++)
                {
                    for (j=0;j<salads[i].Limit;j++)
                    ZWB[i].Add(salads[i]);
                    ZWB_candidates.Remove(salads[i]);
                }
                ///Max 1 salad per bag
                foreach (var salad in salads)
                {
                    ZWB_candidates.Remove(salad);
                }

                ///Add a maximum of one diary per bag
                var diaries = ZWB_candidates.FindAll(x => x.IsDiary == true);
                for (var diary in diaries)
                {

                }*/

                /*bool[] used = new bool[ZWB_candidates.Count];
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
                }*/
                return ZWB;
            }
            
        }
        public List<Product> GetShelfItems() => shelf;
        public List<Product> GetExpiredItems() => expired;
        public List<Product> GetDiscountedProducts() => discounted;
        public List<List<Product>> GetWasteBags() => generateWasteBags();

    }
}
