using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PurchaseList.Models
{
    public class Item
    {
        public Package Package
        {
            get { return package; }
            set { package = value; }
        }
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        private Package package = new Package();
        private int quantity;
        public Item() { }
        public Item(Package package, int quantity)
        {
            this.package = package;
            this.quantity = quantity;
        }

    }
}