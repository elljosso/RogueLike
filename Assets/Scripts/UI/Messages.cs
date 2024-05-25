using UnityEngine;
using UnityEngine.UIElements;

public class Messages : MonoBehaviour
{
    private Label[] labels = new Label[5];
    private VisualElement root;

    // Start is called before the first frame update
    void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Assuming you have labels named "label0", "label1", "label2", etc.
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i] = root.Q<Label>($"Label{i + 1}");
            if (labels[i] == null)
            {
                Debug.LogError($"Label Label{i} not found in UI.");
            }
        }

        Clear();
        AddMessage("Welcome to the dungeon, Adventurer!", Color.green);
    }

    public void Clear()
    {
        foreach (var label in labels)
        {
            if (label != null)
            {
                label.text = string.Empty;
            }
        }
    }

    public void MoveUp()
    {
        for (int i = labels.Length - 1; i > 0; i--)
        {
            if (labels[i] != null && labels[i - 1] != null)
            {
                labels[i].text = labels[i - 1].text;
                labels[i].style.color = labels[i - 1].style.color;
            }
        }

        if (labels[0] != null)
        {
            labels[0].text = string.Empty;
        }
    }

    public void AddMessage(string content, Color color)
    {
        MoveUp();
        if (labels[0] != null)
        {
            labels[0].text = content;
            labels[0].style.color = new StyleColor(color);
        }
    }
}
