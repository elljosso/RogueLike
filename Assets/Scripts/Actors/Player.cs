using UnityEngine;
using UnityEngine.InputSystem;
using Items;
using Unity.VisualScripting;
using System.Collections.Generic;

[RequireComponent(typeof(Actor))]
public class Player : MonoBehaviour, Controls.IPlayerActions
{
    public Inventory inventory; // Public Inventory instance
    private Controls controls;

    private bool inventoryIsOpen = true;
    private bool droppingItem = true;
    private bool usingItem = false;

    private void Awake()
    {
        controls = new Controls();
    }

    private void Start()
    {
        // Set the player in the GameManager
        GameManager.Get.Player = GetComponent<Actor>();

        // Adjust the camera position
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);

        // Initialize the inventory
        inventory = new Inventory();
        inventory.SetMaxItems(2); // Example setting the max items to 2
    }

    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (inventoryIsOpen && context.started)
        {
            Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
            if (direction.y > 0)
            {
                UIManager.Instance.InventoryUI.SelectPreviousItem();
            }
            else if (direction.y < 0)
            {
                UIManager.Instance.InventoryUI.SelectNextItem();
            }
        }
        else
        {
            if (context.performed)
            {
                Move();
            }
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inventoryIsOpen)
            {
                UIManager.Instance.InventoryUI.Hide();
                inventoryIsOpen = false;
                droppingItem = false;
                usingItem = false;
            }
            else
            {
                // Handle other exit actions if needed
            }
        }
    }

    private void Move()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Debug.Log("Moving in direction: " + roundedDirection);
        Action.MoveOrHit(GetComponent<Actor>(), roundedDirection);
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Consumable item = GameManager.Get.GetItemAtLocation(transform.position);
            if (item == null)
            {
                Debug.Log("No item at this location");
            }
            else if (inventory.AddItem(item))
            {
                item.gameObject.SetActive(false);
                GameManager.Get.RemoveItem(item);
                Debug.Log("Item added to inventory");
            }
            else
            {
                Debug.Log("Inventory is full");
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inventoryIsOpen)
            {
                // Show the inventory via the UIManager
                UIManager.Instance.InventoryUI.Show(inventory.GetItems());

                // Set the inventoryIsOpen and droppingItem flags
                inventoryIsOpen = true;
                droppingItem = true;

                Debug.Log("Inventory opened for dropping items.");
            }
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inventoryIsOpen)
            {
                int selectedItemIndex = UIManager.Instance.InventoryUI.Selected;
                Consumable[] items = inventory.GetItems().ToArray();

                if (selectedItemIndex >= 0 && selectedItemIndex < items.Length)
                {
                    Consumable selectedItem = items[selectedItemIndex];

                    if (droppingItem)
                    {
                        DropItem(selectedItem);
                    }
                    else
                    {
                        UseItem(selectedItem);
                    }

                    UIManager.Instance.InventoryUI.Hide();
                    inventoryIsOpen = false;
                    droppingItem = false;
                    usingItem = false;
                }
            }
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inventoryIsOpen)
            {
                int selectedItemIndex = UIManager.Instance.InventoryUI.Selected;
                Consumable[] items = inventory.GetItems().ToArray();

                if (selectedItemIndex >= 0 && selectedItemIndex < items.Length)
                {
                    Consumable selectedItem = items[selectedItemIndex];

                    if (droppingItem)
                    {
                        DropItem(selectedItem);
                    }
                    else
                    {
                        UseItem(selectedItem);
                    }

                    UIManager.Instance.InventoryUI.Hide();
                    inventoryIsOpen = false;
                    droppingItem = false;
                    usingItem = false;
                }
            }
        }
    }

    private void DropItem(Consumable item)
    {
        // Place the item at the player's position
        item.transform.position = transform.position;

        // Add the item back to the GameManager
        GameManager.Get.AddItem(item);

        // Activate the item's GameObject
        item.gameObject.SetActive(true);
    }

    private void UseItem(Consumable item)
    {
        Actor playerActor = GetComponent<Actor>();
        
        if (item is HealthPotion)
        {
            HealthPotion potion = item as HealthPotion;
            int healedAmount = potion.HealAmount;
            playerActor.Heal(healedAmount);
            UIManager.Instance.AddMessage($"You healed for {healedAmount} HP.", Color.green);
        }
        else if (item is Fireball)
        {
            Fireball fireball = item as Fireball;
            List<Actor> nearbyEnemies = GameManager.Get.GetNearbyEnemies(transform.position);
            foreach (Actor enemy in nearbyEnemies)
            {
                enemy.DoDamage(fireball.DamageAmount);
            }
            UIManager.Instance.AddMessage($"You dealt {fireball.DamageAmount} damage to nearby enemies.", Color.red);
        }
        else if (item is ScrollOfConfusion)
        {
            ScrollOfConfusion scroll = item as ScrollOfConfusion;
            List<Actor> nearbyEnemies = GameManager.Get.GetNearbyEnemies(transform.position);
            foreach (Actor enemy in nearbyEnemies)
            {
                enemy.Confuse(scroll.Duration);
            }
            UIManager.Instance.AddMessage("You confused nearby enemies.", Color.blue);
        }

        // Destroy the item's GameObject
        Destroy(item.gameObject);
    }
}
