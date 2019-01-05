using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasteReducer
{
    /// <summary>
    /// Represents the global configuration values of the wastebags. These include parameters such as the number of waste bags and price limits.
    /// </summary>
    class WasteBagConfiguration
    {
        private int nrOfBags;
        private double priceLimitUpper;
        private double priceLimitLower;
        
        public WasteBagConfiguration()
        {
            nrOfBags = 5;
            priceLimitLower = 10;
            priceLimitUpper = 13;
        }

        public int NrOfBags { get => nrOfBags; set => nrOfBags = Math.Max(value,1); }
        public double PriceLimitLower { get => priceLimitLower; set => priceLimitLower = Math.Max(0, value); }
        public double PriceLimitUpper { get => priceLimitUpper; set => priceLimitUpper = Math.Max(1,value); }
    }
}
