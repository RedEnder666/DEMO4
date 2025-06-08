using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEMO4.Utils
{
    internal class CostCalc
    {
        public static decimal? Calculate(DEMO_v3Entities db, partner partner) 
        {
            return db.partners_products
                .Where(pp => pp.partner_ID == partner.partner_ID)
                .Sum(pp => db.products.FirstOrDefault(p => p.product_ID == pp.product_ID).price*pp.quantity);
        }   
    }

}
