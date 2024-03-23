using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMExt = TilemapManagerExtension;

public class TilemapManagerDLC : MonoBehaviour
{
    [SerializeField] Tilemap TilemapDestructables;
    [SerializeField] Tilemap TilemapFakeGhost;
    TileBase TileToSpawn;

    internal void Setup(EnemyManager EnemyManager, TileBase tileToSpawn)
    {
        foreach (EnemyControl enemy in EnemyManager.enemyArray)
        {
            enemy.OnCollisionWithFakeGhostTile += _OnCollisionWithFakeGhostTile;
            TileToSpawn = tileToSpawn;
        }
    }

    private void _OnCollisionWithFakeGhostTile(object sender, EnemyControl.OnCollisionWithFakeGhostTileEventArgs e)
    {
        DestroyFakeGhost(e.Transform.position, e.Point.point);
    }

    public void DestroyFakeGhost(Vector2 playerCenter, Vector2 contactPosition)
    {
        Vector3Int tilePosition = TMExt.GetTileFromPlayerCollision(playerCenter, contactPosition, TilemapDestructables);
        TilemapFakeGhost.SetTile(tilePosition, null);
    }

    public void DestroyDestructables(Vector2 playerCenter, Vector2 contactPosition)
    {
        Vector3Int tilePosition = TMExt.GetTileFromPlayerCollision(playerCenter, contactPosition, TilemapDestructables);
        TilemapDestructables.SetTile(tilePosition, null);
    }
}
