using UnityEngine;
using UnityEngine.UI;

public class FloorInfo : MonoBehaviour
{
    public Text floorText;
    public Text enemiesText;

    private void Start()
    {
        UpdateFloorText();
        UpdateEnemiesText();
    }

    public void UpdateFloorText()
    {
        if (MapManager.Get != null)
        {
            floorText.text = "Floor " + MapManager.Get.floor;
        }
    }

    public void UpdateEnemiesText()
    {
        if (GameManager.Get != null)
        {
            int remainingEnemies = GameManager.Get.Enemies.Count;
            enemiesText.text = remainingEnemies + " enemies left";
        }
    }
}