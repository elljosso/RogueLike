using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator
{
    private int width, height;
    private int maxRoomSize, minRoomSize;
    private int maxRooms;
    private int maxEnemies; // Variabele voor maximum aantal vijanden
    private int maxItems; // Variabele voor maximum aantal items
    private int currentFloor; // Variabele voor de huidige verdieping
    List<Room> rooms = new List<Room>();

    // List of enemy names in order of strength
    private List<string> enemyNames = new List<string>
    {
        "Spin",
        "Schorpioen",
        "Mier",
        "Crab",
        "Hond",
        "Vos",
        "Wolf",
        "Tijger"
    };

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void SetRoomSize(int min, int max)
    {
        minRoomSize = min;
        maxRoomSize = max;
    }

    public void SetMaxRooms(int max)
    {
        maxRooms = max;
    }

    public void SetMaxEnemies(int max)
    {
        maxEnemies = max;
    }

    public void SetMaxItems(int max)
    {
        maxItems = max;
    }

    public void SetCurrentFloor(int floor)
    {
        currentFloor = floor;
    }

    public void Generate()
    {
        rooms.Clear();

        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);

            int roomX = Random.Range(0, width - roomWidth - 1);
            int roomY = Random.Range(0, height - roomHeight - 1);

            var room = new Room(roomX, roomY, roomWidth, roomHeight);

            if (room.Overlaps(rooms))
            {
                continue;
            }

            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        if (!TrySetWallTile(new Vector3Int(x, y)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y, 0));
                    }
                }
            }

            PlaceEnemies(room, maxEnemies);
            PlaceItems(room, maxItems);

            if (rooms.Count != 0)
            {
                TunnelBetween(rooms[rooms.Count - 1], room);
            }

            rooms.Add(room);
        }

        var lastRoom = rooms[rooms.Count - 1];
        var firstRoom = rooms[0];

        // Plaats een ladder naar beneden in de laatste kamer
        GameManager.Get.CreateLadder("LadderDown", lastRoom.Center());

        // Speler positie aanpassen of aanmaken
        if (GameManager.Get.Player != null)
        {
            GameManager.Get.Player.transform.position = new Vector3(firstRoom.Center().x, firstRoom.Center().y, 0);
        }
        else
        {
            GameManager.Get.CreateActor("Player", firstRoom.Center());
        }

        // Plaats een ladder naar boven in de eerste kamer als de verdieping groter is dan 0
        if (currentFloor > 0)
        {
            GameManager.Get.CreateLadder("LadderUp", firstRoom.Center());
        }
    }

    private bool TrySetWallTile(Vector3Int pos)
    {
        if (MapManager.Get.FloorMap.GetTile(pos))
        {
            return false;
        }
        else
        {
            MapManager.Get.ObstacleMap.SetTile(pos, MapManager.Get.WallTile);
            return true;
        }
    }

    private void SetFloorTile(Vector3Int pos)
    {
        if (MapManager.Get.ObstacleMap.GetTile(pos))
        {
            MapManager.Get.ObstacleMap.SetTile(pos, null);
        }
        MapManager.Get.FloorMap.SetTile(pos, MapManager.Get.FloorTile);
    }

    private void PlaceEnemies(Room room, int maxEnemies)
    {
        int num = Random.Range(0, maxEnemies + 1);

        for (int counter = 0; counter < num; counter++)
        {
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            // Determine the range of enemy indices based on the current floor
            int maxEnemyIndex = Mathf.Clamp(currentFloor + 1, 1, enemyNames.Count);
            int enemyIndex = Random.Range(0, maxEnemyIndex);
            string enemyName = enemyNames[enemyIndex];

            GameManager.Get.CreateActor(enemyName, new Vector2(x, y));
        }
    }

    private void PlaceItems(Room room, int maxItems)
    {
        int num = Random.Range(0, maxItems + 1);

        for (int counter = 0; counter < num; counter++)
        {
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            if (Random.value < 0.33f)
            {
                GameManager.Get.CreateItem("HealthPotion", new Vector2(x, y));
            }
            else if (Random.value < 0.66f)
            {
                GameManager.Get.CreateItem("Fireball", new Vector2(x, y));
            }
            else
            {
                GameManager.Get.CreateItem("ScrollOfConfusion", new Vector2(x, y));
            }
        }
    }

    private void TunnelBetween(Room oldRoom, Room newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (Random.value < 0.5f)
        {
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }

        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (!TrySetWallTile(new Vector3Int(x, y, 0)))
                    {
                        continue;
                    }
                }
            }
        }
    }
}
