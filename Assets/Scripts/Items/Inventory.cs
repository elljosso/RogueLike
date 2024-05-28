using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class Inventory
    {
        private List<Consumable> items = new List<Consumable>();
        private int maxItems = 2; // Default value

        // Set the maximum number of items
        public void SetMaxItems(int max)
        {
            maxItems = max;
        }

        public bool AddItem(Consumable item)
        {
            if (items.Count < maxItems)
            {
                items.Add(item);
                return true;
            }
            return false;
        }

        public void DropItem(Consumable item)
        {
            items.Remove(item);
        }
    }
}