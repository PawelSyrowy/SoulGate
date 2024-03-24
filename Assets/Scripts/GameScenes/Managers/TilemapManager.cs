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
using static UnityEngine.EventSystems.EventTrigger;
using Vector3 = UnityEngine.Vector3;
using TMExt = TilemapManagerExtension;

public class TilemapManager : MonoBehaviour
{    
    List<Vector3Int> TileWorldPositions;
    Vector2Int TileWorldSize;
    [SerializeField] internal Tilemap TilemapBackground;
    [SerializeField] internal Tilemap TilemapSafe;
    [SerializeField] internal Tilemap TilemapBorder;
    [SerializeField] internal Tilemap TilemapGhost;
    [SerializeField] internal TileBase TileToSpawn;
    PlayerControl Player;
    internal EnemyManager EnemyManager;
    [SerializeField] internal TilemapManagerDLC TilemapManagerDLC;

    bool isDrawing = false;
    bool startedFromBorder = false;
    bool finishedOnBorder = false;
    Vector3Int firstCell;
    Vector3Int lastCell;

    public void Setup(PlayerControl player, EnemyManager enemyManager)
    {
        TileWorldPositions = GameHandler.TileWorldPositions;
        TileWorldSize = GameHandler.TileWorldSize;
        Player = player;
        EnemyManager = enemyManager;

        foreach (EnemyControl enemy in EnemyManager.enemyArray)
        {
            enemy.OnCollisionWithGhostTile += _OnCollisionWithGhostTile;
        }
        TilemapManagerDLC.Setup(EnemyManager, TileToSpawn);
    }

    void LateUpdate()
    {
        if (Player.state.ToString() == "Playing")
        {
            Player.CheckCanPlayerDraw(IsPlayerOnGhostTiles(), IsPlayerOnSafeTiles());

            if (Player.CheckPlayerCanFinishDrawing(IsPlayerOnSafeTiles()))
            {
                FinishDrawing();
                if (Progress.CheckWin(TilemapSafe))
                {
                    Player.PlayerWin();
                    GameHandler.PlayerWin();
                }
                else
                {
                    SoundManager.PlaySound(SoundManager.Sound.Complete);
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
        TMExt.DestroyAllTiles(TilemapGhost);
    }

    private bool IsPlayerOnGhostTiles()
    {
        return TMExt.CheckTileExists(TilemapGhost, Player.transform.position);
    }

    private bool IsPlayerOnSafeTiles()
    {
        return TMExt.CheckTileExists(TilemapSafe, Player.transform.position);
    }

    private void FinishDrawing()
    {
        finishedOnBorder = !Player.IsOnBackground;

        isDrawing = false;

        List<Vector3Int> pointsToDrawBorder = new();
        if (startedFromBorder && finishedOnBorder)
        {
            ConnectPointsAlgorithm connectPointsManager = new(firstCell, lastCell, TileWorldPositions, TileWorldSize, TMExt.GetAllTilesPositions(TilemapBorder));
            pointsToDrawBorder = connectPointsManager.FindWayBetweenPoints();
        }
        else if (finishedOnBorder != startedFromBorder)
        {
            Vector3Int onlyCell = startedFromBorder ? firstCell : lastCell;
            ConnectPointsAlgorithm connectPointsManager = new(firstCell, lastCell, TileWorldPositions, TileWorldSize, TMExt.GetAllTilesPositions(TilemapBorder));
            pointsToDrawBorder = connectPointsManager.FindWayToBorder(onlyCell);
        }
        foreach (EnemyControl enemy in EnemyManager.enemyArray)
        {
            if (!enemy.CheckColisionWithTiles(pointsToDrawBorder, TilemapBackground))
            {
                TMExt.AddTiles(pointsToDrawBorder, TilemapGhost, TileToSpawn);
            }
        }
        
        TMExt.AddTiles(TMExt.GetAllTilesPositions(TilemapGhost), TilemapBorder, TileToSpawn);
        TMExt.ReplaceAllTiles(TilemapGhost, TilemapSafe, TileToSpawn);

        CloseShapesAlgorithm closedShapesAlgorithm = new(TMExt.GetAllTilesPositions(TilemapBorder), -TileWorldPositions[1].x, -TileWorldPositions[1].y, TileWorldSize);
        List<Vector3Int> pointsToFillShape = closedShapesAlgorithm.GetEmptyPositions();
        TMExt.AddTiles(pointsToFillShape, TilemapGhost, TileToSpawn);

        foreach (EnemyControl enemy in EnemyManager.enemyArray)
        {
            List<Vector3Int> pointsToUnfillShape = closedShapesAlgorithm.GetEnemyPositions(enemy.GetEnemyPoint(TilemapBackground));
            TMExt.RemoveTiles(pointsToUnfillShape, TilemapGhost);
        }

        TMExt.ReplaceAllTiles(TilemapGhost, TilemapSafe, TileToSpawn);
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

    private void _OnCollisionWithGhostTile(object sender, EventArgs e)
    {
        Score.RemoveScore(100, Player);
        DestroyGhostTiles();
        if (Player != null)
        {
            Player.DrawingBan = true;
        }
        SoundManager.PlaySound(SoundManager.Sound.Hurt);
    }
}