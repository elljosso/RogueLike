using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using Items;

public class InventoryUI : MonoBehaviour
{
    public Label[] labels = new Label[8];
    public VisualElement root;
    private int selected;
    private int numItems;

    public int Selected
    {
        get { return selected; }
    }

    private void Clear()
    {
        foreach (var label in labels)
        {
            label.text = "";
        }
    }

    private void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        // Opslaan van alle labels in de array en aanvankelijke instellingen
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i] = root.Q<Label>("Item" + (i + 1));
        }
        Clear();
        Hide();
    }

    private void UpdateSelected()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            if (i == selected)
            {
                labels[i].style.backgroundColor = Color.green;
            }
            else
            {
                labels[i].style.backgroundColor = Color.clear;
            }
        }
    }

    public void SelectNextItem()
    {
        selected = Mathf.Min(selected + 1, numItems - 1);
        UpdateSelected();
    }

    public void SelectPreviousItem()
    {
        selected = Mathf.Max(selected - 1, 0);
        UpdateSelected();
    }

    public void Show(List<Consumable> list)
    {
        selected = 0;
        numItems = list.Count;
        Clear();

        // Set de tekst van de labels gelijk aan de naam van de consumables in de lijst
        for (int i = 0; i < numItems; i++)
        {
            labels[i].text = list[i].name;
        }

        UpdateSelected();
        // Toon de GUI
        root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        // Verberg de GUI
        root.style.display = DisplayStyle.None;
    }
}