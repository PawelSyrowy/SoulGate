using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapManagerExtension
{
    internal static void AddTiles(List<Vector3Int> tilePositions, Tilemap tilemap, TileBase tileToSpawn)
    {
        foreach (Vector3Int position in tilePositions)
        {
            tilemap.SetTile(position, tileToSpawn);
        }
    }

    internal static void RemoveTiles(List<Vector3Int> tilePositions, Tilemap tilemap)
    {
        foreach (Vector3Int position in tilePositions)
        {
            tilemap.SetTile(position, null);
        }
    }

    internal static void ReplaceAllTiles(Tilemap firstTilemap, Tilemap secondTilemap, TileBase tileToSpawn)
    {
        List<Vector3Int> positions = GetAllTilesPositions(firstTilemap);

        foreach (Vector3Int pos in positions)
        {
            secondTilemap.SetTile(pos, tileToSpawn);
            firstTilemap.SetTile(pos, null);
        }
    }

    internal static void DestroyAllTiles(Tilemap tilemap)
    {
        List<Vector3Int> positions = GetAllTilesPositions(tilemap);

        foreach (Vector3Int pos in positions)
        {
            tilemap.SetTile(pos, null);
        }
    }

    internal static bool CheckTileExists(Tilemap tilemap, Vector3 positionToCheck)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(positionToCheck);
        TileBase tile = tilemap.GetTile(cellPosition);
        if (tile != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal static List<Vector3Int> GetAllTilesPositions(Tilemap tilemap)
    {
        List<Vector3Int> positions = new();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (allTiles[pos.x - bounds.xMin + (pos.y - bounds.yMin) * bounds.size.x] != null)
            {
                positions.Add(pos);
            }
        }
        return positions;
    }

    internal static Vector3Int GetTileFromPlayerCollision(Vector2 playerCenter, Vector2 contactPosition, Tilemap tilemapDestructable)
    {
        Vector2 direction = (contactPosition - playerCenter).normalized;
        Vector2 offsetPosition = contactPosition + direction / 2;
        return tilemapDestructable.WorldToCell(offsetPosition);
    }
}
