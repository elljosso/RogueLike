using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Documents")]
    public GameObject HealthBar;
    public GameObject Messages;
    public GameObject InventoryUIObject;
    public GameObject FloorInfoObject; // Toegevoegd

    private HealthBar healthBarScript;
    private Messages messagesScript;
    private InventoryUI inventoryUIScript;
    private FloorInfo floorInfoScript; // Toegevoegd

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (HealthBar != null)
        {
            healthBarScript = HealthBar.GetComponent<HealthBar>();
            if (healthBarScript == null)
            {
                Debug.LogError("HealthBar script not found on HealthBar GameObject.");
            }
        }

        if (Messages != null)
        {
            messagesScript = Messages.GetComponent<Messages>();
            if (messagesScript == null)
            {
                Debug.LogError("Messages script not found on Messages GameObject.");
            }
        }

        if (InventoryUIObject != null)
        {
            inventoryUIScript = InventoryUIObject.GetComponent<InventoryUI>();
            if (inventoryUIScript == null)
            {
                Debug.LogError("InventoryUI script not found on InventoryUI GameObject.");
            }
        }

        if (FloorInfoObject != null)
        {
            floorInfoScript = FloorInfoObject.GetComponent<FloorInfo>();
            if (floorInfoScript == null)
            {
                Debug.LogError("FloorInfo script not found on FloorInfo GameObject.");
            }
        }
    }

    public InventoryUI InventoryUI
    {
        get { return inventoryUIScript; }
    }

    public void UpdateHealth(int current, int max)
    {
        if (healthBarScript != null)
        {
            healthBarScript.SetValues(current, max);
        }
    }

    public void SetLevel(int level)
    {
        if (healthBarScript != null)
        {
            healthBarScript.SetLevel(level);
        }
    }

    public void SetXP(int xp)
    {
        if (healthBarScript != null)
        {
            healthBarScript.SetXP(xp);
        }
    }

    public void AddMessage(string message, Color color)
    {
        if (messagesScript != null)
        {
            messagesScript.AddMessage(message, color);
        }
    }

    public void UpdateFloorInfo() // Toegevoegd
    {
        if (floorInfoScript != null)
        {
            floorInfoScript.UpdateFloorText();
            floorInfoScript.UpdateEnemiesText();
        }
    }
}
