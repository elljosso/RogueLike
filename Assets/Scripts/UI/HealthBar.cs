using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    private VisualElement root;
    private VisualElement healthBar;
    private Label healthLabel;

    // Start is called before the first frame update
    void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Assuming you have elements in your UXML with these names
        healthBar = root.Q<VisualElement>("healthBar");
        healthLabel = root.Q<Label>("healthLabel");
    }

    public void SetValues(int currentHitPoints, int maxHitPoints)
    {
        float percent = (float)currentHitPoints / maxHitPoints * 100;
        healthBar.style.width = new Length(percent, LengthUnit.Percent);
        healthLabel.text = $"{currentHitPoints}/{maxHitPoints} HP";
    }
}