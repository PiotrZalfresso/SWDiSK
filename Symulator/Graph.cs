using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    public class DeliveryItem : IEquatable<DeliveryItem>
    {
        public Package package { get; }
        public int carId { get; } = -1;

        public DeliveryItem(Package package)
        {
            this.package = package;
        }

        public DeliveryItem(Package package, int carId)
        {
            this.package = package;
            this.carId = carId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            DeliveryItem objAsItem = obj as DeliveryItem;
            if (objAsItem == null) return false;
            else return Equals(objAsItem);
        }

        public bool Equals(DeliveryItem other)
        {
            if (other == null) return false;
            return (this.package.Equals(other.package));
        }
    }

    public static class Graph
    {
        static public List<DeliveryItem> deliveredItems = new List<DeliveryItem>();
        static public int numberOfDelivered {
            get
            {
                return deliveredItems.Count;
            }
        }
    }
}
