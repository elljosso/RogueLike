using UnityEngine;

namespace Items
{
    public class Consumable : MonoBehaviour
    {
        public enum ItemType
        {
            HealthPotion,
            Fireball,
            ScrollOfConfusion
        }

        [SerializeField]
        private ItemType type;

        public ItemType Type
        {
            get { return type; }
        }

        public bool IsType(ItemType itemType)
        {
            return type == itemType;
        }

        public ItemType GetItemType()
        {
            return type;
        }

        private void Start()
        {
            // Voeg het item toe aan de GameManager's Items lijst bij het starten
            GameManager.Get.AddItem(this);
        }
    }
}