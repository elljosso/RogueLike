using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;

    public Text floorText; // Reference to the UI text for displaying the current floor

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static MapManager Get { get => instance; }

    [Header("TileMaps")]
    public Tilemap FloorMap;
    public Tilemap ObstacleMap;
    public Tilemap FogMap;

    [Header("Tiles")]
    public TileBase FloorTile;
    public TileBase WallTile;
    public TileBase FogTile;

    [Header("Features")]
    public Dictionary<Vector2Int, Node> Nodes = new Dictionary<Vector2Int, Node>();
    public List<Vector3Int> VisibleTiles;
    public Dictionary<Vector3Int, TileData> Tiles = new Dictionary<Vector3Int, TileData>();

    [Header("Map Settings")]
    public int width = 80;
    public int height = 45;
    public int roomMaxSize = 10;
    public int roomMinSize = 6;
    public int maxRooms = 30;
    public int maxEnemies = 2; // Variabele voor maximum aantal vijanden
    public int maxItems = 2; // Variabele voor maximum aantal items
    public int floor = 0; // Variabele voor de huidige verdieping

    private void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        ClearFloor();

        Tiles = new Dictionary<Vector3Int, TileData>();
        VisibleTiles = new List<Vector3Int>();

        // Instellen van de dungeon generator
        DungeonGenerator dungeonGenerator = new DungeonGenerator();
        dungeonGenerator.SetSize(width, height);
        dungeonGenerator.SetRoomSize(roomMinSize, roomMaxSize);
        dungeonGenerator.SetMaxRooms(maxRooms);
        dungeonGenerator.SetMaxEnemies(maxEnemies);
        dungeonGenerator.SetMaxItems(maxItems); // Zet het maximale aantal items
        dungeonGenerator.SetCurrentFloor(floor); // Stel de huidige verdieping in
        dungeonGenerator.Generate();

        AddTileMapToDictionary(FloorMap);
        AddTileMapToDictionary(ObstacleMap);
        SetupFogMap();

        UpdateFloorText(); // Update the UI text for the current floor
    }

    public void MoveUp()
    {
        ClearFloor();
        floor--;
        GenerateDungeon();
        UpdateFloorText(); // Update the UI text for the current floor
    }

    public void MoveDown()
    {
        ClearFloor();
        floor++;
        GenerateDungeon();
        UpdateFloorText(); // Update the UI text for the current floor
    }

    public bool InBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

    public bool IsWalkable(Vector3 position)
    {
        Vector3Int gridPosition = FloorMap.WorldToCell(position);
        if (!InBounds(gridPosition.x, gridPosition.y) || ObstacleMap.HasTile(gridPosition))
        {
            return false;
        }
        return true;
    }

    private void AddTileMapToDictionary(Tilemap tilemap)
    {
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos))
            {
                continue;
            }
            TileData tile = new TileData(
                name: tilemap.GetTile(pos).name,
                isExplored: false,
                isVisible: false
            );

            Tiles.Add(pos, tile);
        }
    }

    private void SetupFogMap()
    {
        foreach (Vector3Int pos in Tiles.Keys)
        {
            if (!FogMap.HasTile(pos))
            {
                FogMap.SetTile(pos, FogTile);
                FogMap.SetTileFlags(pos, TileFlags.None);
            }

            if (Tiles[pos].IsExplored)
            {
                FogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
            }
            else
            {
                FogMap.SetColor(pos, Color.white);
            }
        }
    }

    public void UpdateFogMap(List<Vector3Int> playerFOV)
    {
        foreach (var pos in VisibleTiles)
        {
            if (!Tiles[pos].IsExplored)
            {
                Tiles[pos].IsExplored = true;
            }

            Tiles[pos].IsVisible = false;
            FogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
        }

        VisibleTiles.Clear();

        foreach (var pos in playerFOV)
        {
            Tiles[pos].IsVisible = true;
            FogMap.SetColor(pos, Color.clear);
            VisibleTiles.Add(pos);
        }
    }

    // Update the UI text to display the current floor
    private void UpdateFloorText()
    {
        if (floorText != null)
        {
            floorText.text = "Floor " + floor;
        }
    }

    // Clear the floor by removing all tiles from the maps
    private void ClearFloor()
    {
        FloorMap.ClearAllTiles();
        ObstacleMap.ClearAllTiles();
        FogMap.ClearAllTiles();
        Tiles.Clear();
        VisibleTiles.Clear();
    }
}
