using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDestructablesPlugin : MonoBehaviour
{
    Tilemap TilemapDestructables;
    TileBase TileToSpawn;

    internal void Setup(Tilemap tilemapDestructables, TileBase tileToSpawn)
    {
        TilemapDestructables = tilemapDestructables;
        TileToSpawn = tileToSpawn;
    }

    public void DestroyDestructables(Vector2 playerCenter, Vector2 contactPosition)
    {
        Vector2 direction = (contactPosition - playerCenter).normalized;
        Vector2 offsetPosition = contactPosition + direction/2;
        Vector3Int tilePosition = TilemapDestructables.WorldToCell(offsetPosition);
        TilemapDestructables.SetTile(tilePosition, null);
    }
}
