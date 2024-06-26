using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    public VisualElement root;
    public VisualElement healthBar;
    public Label healthLabel;
    public Label levelLabel;
    public Label xpLabel;

    // Start is called before the first frame update
    void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Zoek de UI-elementen voor de gezondheidsbalk en het label
        healthBar = root.Q<VisualElement>("HealthBar");
        healthLabel = root.Q<Label>("HealthText");
        levelLabel = root.Q<Label>("LevelText");
        xpLabel = root.Q<Label>("XPText");

        if (healthBar == null)
        {
            Debug.LogError("HealthBar element not found in the UI hierarchy!");
        }
    }

    public void SetValues(int currentHitPoints, int maxHitPoints)
    {
        // Bereken het percentage HP
        float percent = (float)currentHitPoints / maxHitPoints;

        // Converteer het percentage naar een breedte als percentage van het ouder-element
        Length width = new Length(percent, LengthUnit.Percent);

        // Pas de breedte van de balk aan op basis van het percentage HP
        healthBar.style.width = width;

        // Stel de tekst van het label in op de huidige HP
        healthLabel.text = $"{currentHitPoints}/{maxHitPoints} HP";
    }

    public void SetLevel(int level)
    {
        if (levelLabel != null)
        {
            levelLabel.text = $"Level: {level}";
        }
    }

    public void SetXP(int xp)
    {
        if (xpLabel != null)
        {
            xpLabel.text = $"XP: {xp}";
        }
    }
}
