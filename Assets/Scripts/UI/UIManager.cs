using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Documents")]
    public GameObject HealthBar;
    public GameObject Messages;

    private HealthBar healthBarScript;
    private Messages messagesScript;

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
    }

    public void UpdateHealth(int current, int max)
    {
        if (healthBarScript != null)
        {
            healthBarScript.SetValues(current, max);
        }
    }

    public void AddMessage(string message, Color color)
    {
        if (messagesScript != null)
        {
            messagesScript.AddMessage(message, color);
        }
    }
}
