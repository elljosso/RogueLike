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
            labels[i] = root.Q<Label>($"label{i}");
        }

        Clear();
        AddMessage("Welcome to the dungeon, Adventurer!", Color.green);
    }

    public void Clear()
    {
        foreach (var label in labels)
        {
            label.text = string.Empty;
        }
    }

    public void MoveUp()
    {
        for (int i = labels.Length - 1; i > 0; i--)
        {
            labels[i].text = labels[i - 1].text;
            labels[i].style.color = labels[i - 1].style.color;
        }

        labels[0].text = string.Empty;
    }

    public void AddMessage(string content, Color color)
    {
        MoveUp();
        labels[0].text = content;
        labels[0].style.color = new StyleColor(color);
    }
}