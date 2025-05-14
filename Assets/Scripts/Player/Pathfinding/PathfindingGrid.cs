using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingGrid : MonoBehaviour
{
    [SerializeField] private List<Tilemap> walkableTileMaps;
    private HashSet<Vector3Int> walkableTiles;

    void Start()
    {
        walkableTiles = new HashSet<Vector3Int>();
        foreach (Tilemap tilemap in walkableTileMaps)
        {
            CheckWalkableAllTiles(tilemap);
        }
    }

    private void CheckWalkableAllTiles(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(position))
            {
                walkableTiles.Add(position);
            }
        }
    }

    public bool IsWalkable(Vector3Int tilePosition)
    {
        return walkableTiles.Contains(tilePosition);
    }

    public List<Tilemap> GetTileMaps()
    {
        return walkableTileMaps;
    }
}