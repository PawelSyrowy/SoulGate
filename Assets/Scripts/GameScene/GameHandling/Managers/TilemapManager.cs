using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.PlayerSettings;
using Vector3 = UnityEngine.Vector3;

public class TilemapManager : MonoBehaviour
{
    internal Tilemap TilemapBackground;
    Tilemap TilemapSafe;
    Tilemap TilemapBorder; 
    Tilemap TilemapGhost;
    TileBase TileToSpawn;

    List<Vector3Int> TileWorldPositions;
    Vector2Int TileWorldSize;
    WinManager WinManager;
    PlayerControl Player;
    internal EnemyControl Enemy;

    bool isDrawing = false;
    bool startedFromBorder = false;
    bool finishedOnBorder = false;
    Vector3Int firstCell;
    Vector3Int lastCell;

    public void Setup(Tilemap tilemapBackground, Tilemap tilemapSafe, Tilemap tilemapBorder, Tilemap tilemapGhost, TileBase tileToSpawn, WinManager winManager, PlayerControl player, EnemyControl enemy)
    {
        TileWorldPositions = GameHandler.TileWorldPositions;
        TileWorldSize = GameHandler.TileWorldSize; 
        TilemapBackground = tilemapBackground;
        TilemapSafe = tilemapSafe;
        TilemapBorder = tilemapBorder;
        TilemapGhost = tilemapGhost;
        TileToSpawn = tileToSpawn;
        WinManager = winManager;
        Player = player;
        Enemy = enemy;
    }

    void LateUpdate()
    {
        if (Enemy!=null)
        {
            if (Enemy.HasCollisionWithGhostTile == true)
            {
                GameHandler.RemoveScore(500, Player);
                DestroyGhostTiles();
                Enemy.HasCollisionWithGhostTile = false;
                if (Player != null)
                {
                    Player.DrawingBan = true;
                }
            }
        }
        if (Player != null && Player.state.ToString()=="Alive")
        {
            Player.CheckCanPlayerDraw(IsPlayerOnGhostTiles(), IsPlayerOnSafeTiles());

            if (Player.CheckPlayerCanFinishDrawing(IsPlayerOnSafeTiles()))
            {
                FinishDrawing();
                if (WinManager.CheckWin(TilemapSafe))
                {
                    Player.PlayerWin();
                    GameHandler.PlayerWin();
                }
            }

            Player.CheckIsPlayerDrawing(CheckGhostTilesExist());

            if (Player.CanDraw == true)
            {
                SpawnGhostTileAtPlayerPosition();
            }

            else if (Player.IsDrawing == false)
            {
                startedFromBorder = !Player.IsOnBackground;
            }
        }
    }

    internal void DestroyGhostTiles()
    {
        isDrawing = false;
        DestroyAllTiles(TilemapGhost);
    }

    private bool IsPlayerOnGhostTiles()
    {
        return CheckTileExists(TilemapGhost, Player.transform.position);
    }

    private bool IsPlayerOnSafeTiles()
    {
        return CheckTileExists(TilemapSafe, Player.transform.position);
    }

    private void FinishDrawing()
    {
        finishedOnBorder = !Player.IsOnBackground;

        isDrawing = false;

        List<Vector3Int> pointsToDrawBorder = new();
        if (startedFromBorder && finishedOnBorder)
        {
            ConnectPointsAlgorithm connectPointsManager = new(firstCell, lastCell, TileWorldPositions, TileWorldSize);
            pointsToDrawBorder = connectPointsManager.FindWayBetweenPoints();
        }
        else if (finishedOnBorder != startedFromBorder)
        {
            Vector3Int cell = startedFromBorder ? firstCell : lastCell;
            ConnectPointsAlgorithm connectPointsManager = new(cell, TileWorldPositions, GetAllTilesPositions(TilemapBorder));
            pointsToDrawBorder = connectPointsManager.FindWayToBorder();
        }
        if (!Enemy.EnemyHasColisionWithTiles(pointsToDrawBorder, TilemapBackground))
        {
            AddTiles(pointsToDrawBorder, TilemapGhost);
        }

        AddTiles(GetAllTilesPositions(TilemapGhost), TilemapBorder);
        ReplaceAllTiles(TilemapGhost, TilemapSafe);

        CloseShapesAlgorithm closedShapesAlgorithm = new(GetAllTilesPositions(TilemapBorder), -TileWorldPositions[1].x, -TileWorldPositions[1].y, TileWorldSize);
        List<List<Vector3Int>> pointsToFillShape = closedShapesAlgorithm.GetEmptyPositions(Enemy.GetEnemyPoint(TilemapBackground));

        AddTiles(pointsToFillShape[0], TilemapGhost);
        RemoveTiles(pointsToFillShape[1], TilemapGhost);
        ReplaceAllTiles(TilemapGhost, TilemapSafe);
    }

    private bool CheckGhostTilesExist()
    {
        BoundsInt bounds = TilemapGhost.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (TilemapGhost.HasTile(pos))
            {
                return true;
            }
        }

        return false;
    }

    private void SpawnGhostTileAtPlayerPosition()
    {
        Transform playerPosition = Player.transform;
        Vector3Int playerCellPosition = TilemapGhost.WorldToCell(playerPosition.position);
        TilemapGhost.SetTile(playerCellPosition, TileToSpawn);

        if (!isDrawing)
        {
            firstCell = TilemapGhost.WorldToCell(playerPosition.position);
            isDrawing = true;
        }
        else
        {
            lastCell = TilemapGhost.WorldToCell(playerPosition.position);
        }
    }

    private void AddTiles(List<Vector3Int> tilePositions, Tilemap tilemap)
    {
        foreach (Vector3Int position in tilePositions)
        {
            tilemap.SetTile(position, TileToSpawn);
        }
    }

    private void RemoveTiles(List<Vector3Int> tilePositions, Tilemap tilemap)
    {
        foreach (Vector3Int position in tilePositions)
        {
            tilemap.SetTile(position, null);
        }
    }

    private void ReplaceAllTiles(Tilemap firstTilemap, Tilemap secondTilemap)
    {
        List<Vector3Int> positions = GetAllTilesPositions(firstTilemap);

        foreach (Vector3Int pos in positions)
        {
            secondTilemap.SetTile(pos, TileToSpawn);
            firstTilemap.SetTile(pos, null);
        }
    }

    internal void DestroyAllTiles(Tilemap tilemap)
    {
        List<Vector3Int> positions = GetAllTilesPositions(tilemap);

        foreach (Vector3Int pos in positions)
        {
            tilemap.SetTile(pos, null);
        }
    }

    private bool CheckTileExists(Tilemap tilemap, Vector3 positionToCheck)
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

    private List<Vector3Int> GetAllTilesPositions(Tilemap tilemap)
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
}

//public void DestroyGreenTile(Vector2 playerCenter, Vector2 contactPosition)
//{
//    Vector2 direction = (contactPosition - playerCenter).normalized;
//    Vector2 offsetPosition = playerCenter + direction; 
//    Vector3Int tilePosition = TilemapGreen.WorldToCell(offsetPosition);
//    TilemapGreen.SetTile(tilePosition, null);
//}