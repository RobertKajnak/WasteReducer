using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WasteReducer.Entities;

namespace WasteReducer
{
    class CategorizerLogic
    {
        private readonly List<Product> products; 
        private readonly WasteBagConfiguration config;
        private readonly ShelfProducts shelf;
        ///Backup variable, not expected to be used
        private readonly List<Product> expired;
        private readonly DiscountedProducts discounted;
        private readonly ZeroWasteBag ZWB_candidates;
        private readonly ZeroWasteBagsAll ZWB_final;

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
            shelf = new ShelfProducts();
            discounted = new DiscountedProducts();
            ZWB_candidates = new ZeroWasteBag();

            ///Not only shorthand, but if the date changes during run, the results will be consistent
            DateTime today = DateTime.Today;
            ///ON the INFERENCE model: obtain object
            foreach (Product product in products)
            {
                /// In reality, this should not happen. Added for future-proofing
                if (product.ExpiryDate <= today) ///INFERENCE: obtain all attributes -> obtain feature
                {
                    expired.Add(product);///INFERENCE:  match Truth value => true; if false move to next clause
                }
                ///DISCOUNT: Expires tomorrow
                else if (product.ExpiryDate <= today.AddDays(1))
                {
                    discounted.Add(product);
                }
                ///SHELF: expires in 2 days, but has high facing or expires in more than 2 days
                else if (((product.ExpiryDate <= today.AddDays(2)) && (product.Facing >= 4))
                            || (product.ExpiryDate > today.AddDays(2)))
                {
                    shelf.Add(product);
                }
                ///ZWB: Expires in 2 days but has low facing
                else if ((product.ExpiryDate <= today.AddDays(2)) && (product.Facing <= 4))
                {
                    ZWB_candidates.Add(product);
                }

            }
            ///Sorts the ZWB products into the 5(or otherwise set by parameter) bags
            ZWB_final = generateWasteBags();
            foreach (var p in ZWB_candidates)
                discounted.Add(p);
            ZWB_candidates.Clear();
        }

        /// <summary>
        /// Apppends the products from <see cref="ZWB_candidates"/> based on criteria to ZWB.
        /// </summary>
        /// <param name="ZWB">The batch of waste bags. Can be empty or already have items in it</param>
        /// <param name="criteriaImportance">Predicates that define the preference order</param>
        /// <param name="criteriaFilter">Predicates that define the filtering criteria </param>
        private void AddToBags(ZeroWasteBagsAll ZWB, List<Predicate<Product>> criteriaImportance, List<Predicate<Product>> criteriaFilter)
        {
            int n = this.config.NrOfBags;
            ///Initialized the empty bags, if they have not been yet
            if (ZWB.Count == 0)
            {
                for (int i = 0; i < n; i++) ZWB.Add(new ZeroWasteBag());
            }

            ///Restricts the items to only those that pass the criteria
            var ZWB_passed = new List<Product>();
            foreach (var crit in criteriaFilter)
            {
                var tempBag = ZWB_candidates.FindAll(crit);
                ZWB_passed.AddRange(tempBag);
                tempBag.ForEach(x=>ZWB_candidates.Remove(x));
            }

            ///This adds the reamining items, that have not been added as a preference
            criteriaImportance.Add(x=>true);
            foreach (var p in criteriaImportance)
            {
                ///Filters the candidates by the criteria, in order
                ///Then sorts them by price
                var prods = ZWB_passed.FindAll(p).OrderBy(x=>-x.Price).ToList();
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
                        ///INFERENCE: propose extension
                        ///checks for items within the category of the to-be-added item and ensures it doesn't go over limit
                        if ((ZWB[i].FindAll(x=>x.Category.Equals(prods[ip].Category)).Count<prods[ip].Limit) &&
                            ///Checks to be within price, but try to be close to the lower price
                            (ZWB[i].Sum(x => x.Price)+prods[ip].Price<config.PriceLimitUpper) &&
                            (ZWB[i].Sum(x => x.Price) < config.PriceLimitLower))
                        {
                            ///INFERENCE: verify: truth value
                            ZWB[i].Add(prods[ip]);
                            ZWB_passed.Remove(prods[ip++]);
                            nothingAdded = false;
                        }///INFERENCE: critique:=> take new product -> modify design by adding trying a new Product
                    }
                }
            }
            criteriaImportance.RemoveAt(criteriaImportance.Count-1);
            ZWB_passed.ForEach(x=>ZWB_candidates.Add(x));
        }

        private ZeroWasteBagsAll generateWasteBags()
        {
            if (ZWB_candidates == null)
                return null;
            else
            {
                int n = this.config.NrOfBags;
                var ZWB = new ZeroWasteBagsAll();
                ///The preferences by which order the products will be added in order
                ///INFERENCE: This defines the skeletal design of the bags
                var pref = new List<Predicate<Product>>();
                pref.Add(p => p.Category.Equals("salad"));
                pref.Add(p => p.IsDiary == true);
                pref.Add(p => p.Category.Equals("big meal"));

                //Items that should be not considered 
                var filt = new List<Predicate<Product>>();
                filt.Add(x => x.Facing <= 4);
                AddToBags(ZWB, pref, filt);

                return ZWB;
            }
            
        }
        public ShelfProducts GetShelfItems() => shelf;
        ///Backup variable, not expected to be used
        public List<Product> GetExpiredItems() => expired;
        public DiscountedProducts GetDiscountedProducts() => discounted;
        public ZeroWasteBagsAll GetWasteBags() => ZWB_final ;

    }
}
