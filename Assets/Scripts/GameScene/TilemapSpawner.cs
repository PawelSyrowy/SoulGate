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
using static UnityEditor.PlayerSettings;
using Vector3 = UnityEngine.Vector3;

public class TilemapSpawner : MonoBehaviour
{
    [SerializeField] internal Tilemap TilemapSafe;
    [SerializeField] Tilemap TilemapGhost;
    [SerializeField] internal Tilemap TilemapBackground;
    [SerializeField] Tilemap TilemapBorder;

    //test
    [SerializeField] Tilemap TilemapBusyTest;
    [SerializeField] Tilemap TilemapForbiddenTest;
    [SerializeField] Tilemap TilemapEmptyTest;
    [SerializeField] Tilemap TilemapBorderTest;

    [SerializeField] TileBase tileToSpawn;

    [SerializeField] EnemyControl Enemy;
    [SerializeField] PlayerControl Player;

    List<Vector3Int> TileWorldPositions;

    bool isDrawing = false;
    bool startedFromBorder = false;
    bool finishedOnBorder = false;
    Vector3Int firstCell;
    Vector3Int lastCell;

    private void Start()
    {
        TileWorldPositions = new List<Vector3Int>
        {
            new(-36, 17, 0),
            new(-36, -18, 0),
            new(35, -18, 0),
            new(35, 17, 0)
        };
    }

    void LateUpdate()
    {
        if (Enemy.HasCollisionWithGhostTile == true)
        {
            DestroyGhostTiles();
            Enemy.HasCollisionWithGhostTile = false;
            if (Player != null)
            {
                Player.DrawingBan = true;
            }
        }
        if (Player != null)
        {
            Player.CheckCanPlayerDraw(IsPlayerOnGhostTiles(), IsPlayerOnSafeTiles());

            if (Player.CheckPlayerCanFinishDrawing(IsPlayerOnSafeTiles()))
            {
                FinishDrawing();
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

    private void DestroyGhostTiles()
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
            ConnectPointsManager connectPointsManager = new(firstCell, lastCell, TileWorldPositions);
            pointsToDrawBorder = connectPointsManager.FindWayBetweenPoints();
        }
        else if (finishedOnBorder != startedFromBorder)
        {
            Vector3Int cell = startedFromBorder ? firstCell : lastCell;
            ConnectPointsManager connectPointsManager = new(cell, TileWorldPositions, GetAllTilesPositions(TilemapBorder));
            pointsToDrawBorder = connectPointsManager.FindWayToBorder();
        }
        if (!Enemy.EnemyHasColisionWithTiles(pointsToDrawBorder))
        {
            AddTiles(pointsToDrawBorder, TilemapGhost);
        }

        AddTiles(GetAllTilesPositions(TilemapGhost), TilemapBorder);
        ReplaceAllTiles(TilemapGhost, TilemapSafe);

        ClosedShapesAlgorithm closedShapesAlgorithm = new(GetAllTilesPositions(TilemapBorder), 36, 18);
        List<List<Vector3Int>> pointsToFillShape = closedShapesAlgorithm.GetEmptyPositions(Enemy.GetEnemyPoint());

        AddTiles(pointsToFillShape[0], TilemapGhost);
        RemoveTiles(pointsToFillShape[1], TilemapGhost);
        ReplaceAllTiles(TilemapGhost, TilemapSafe);

        //DestroyAllTiles(TilemapBusyTest);
        //DestroyAllTiles(TilemapEmptyTest);
        //DestroyAllTiles(TilemapBorderTest);
        //DestroyAllTiles(TilemapForbiddenTest);

        //AddTiles(closedShapesAlgorithmNegative.BusyPositionsTest, TilemapBusyTest);
        //AddTiles(closedShapesAlgorithmNegative.EmptyPositionsTest, TilemapEmptyTest);
        //AddTiles(closedShapesAlgorithmNegative.BorderPositionsTest, TilemapBorderTest);
        //AddTiles(closedShapesAlgorithmNegative.ForbiddenPositionsTest, TilemapForbiddenTest);
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
        TilemapGhost.SetTile(playerCellPosition, tileToSpawn);

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
            tilemap.SetTile(position, tileToSpawn);
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
            secondTilemap.SetTile(pos, tileToSpawn);
            firstTilemap.SetTile(pos, null);
        }
    }

    private void DestroyAllTiles(Tilemap tilemap)
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