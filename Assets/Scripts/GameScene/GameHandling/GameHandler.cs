using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private WinManager winManager;
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private PlayerControl player;
    [SerializeField] private Tilemap tilemapBackground;
    [SerializeField] private Tilemap tilemapSafe;

    private LevelGrid levelGrid;
    List<Vector3Int> TileWorldPositions;
    Vector2Int TileWorldSize;

    private static GameHandler instance;
    private static int score;

    private void Awake()
    {
        instance = this;
        InitializeStatic();

        TileWorldPositions = new List<Vector3Int>
        {
            new(-36, 17, 0),
            new(-36, -18, 0),
            new(35, -18, 0),
            new(35, 17, 0)
        };
        TileWorldSize = new Vector2Int(71, 35);
    }

    private void Start()
    {
        levelGrid = new LevelGrid(TileWorldSize.x, TileWorldSize.y);

        player.Setup(tilemapBackground, levelGrid, tilemapManager);
        levelGrid.Setup(player);

        tilemapManager.Setup(TileWorldPositions, TileWorldSize, player, tilemapSafe);
        winManager.Setup(tilemapBackground, tilemapSafe, player);
    }

    public static int GetScore()
    {
        return score;
    }

    public static void AddScore()
    {
        score += 100;
    }

    public static void RemoveScore(int amount, PlayerControl player)
    {
        score -= amount;
        if (score<0)
        {
            player.PlayerDied();
        }
    }

    private static void InitializeStatic()
    {
        score = 1000;
    }
    
    public static void ReloadScene()
    {
        Loader.Load(Loader.Scene.GameScene);
    }

    public static void GoToMainMenu()
    {
        Loader.Load(Loader.Scene.MainMenu);
    }

    public void PlayerNewLife()
    {
        if (score>=1000)
        {
            RemoveScore(1000, null);
            player.NewLife();
            GameOverWindow.HideStatic();
        }
    }

    public static void PlayerDied(TilemapManager tilemapManager)
    {
        GameOverWindow.ShowStatic();
        tilemapManager.DestroyGhostTiles();
    }

    public static void DestroyedBlueTiles(PlayerControl player)
    {
        RemoveScore(500, player);
    }

    public static void PlayerWin()
    {
        WinGameWindow.ShowStatic();
    }
}
