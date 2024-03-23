using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMExt = TilemapManagerExtension;

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
        Vector3Int tilePosition = TMExt.GetTileFromPlayerCollision(playerCenter, contactPosition, TilemapDestructables);
        TilemapDestructables.SetTile(tilePosition, null);
    }
}
