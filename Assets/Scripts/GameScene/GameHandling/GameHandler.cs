using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameHandler : MonoBehaviour
{
    [SerializeField] internal Tilemap tilemapBackground;
    [SerializeField] internal Tilemap tilemapSafe;
    [SerializeField] internal Tilemap tilemapBorder;
    [SerializeField] internal Tilemap tilemapGhost;
    [SerializeField] internal TileBase tileToSpawn;

    [SerializeField] private WinManager winManager;
    [SerializeField] private TilemapManager tilemapManager; 
    [SerializeField] private PlayerControl player;
    [SerializeField] private EnemyControl enemy;

    private LevelGrid levelGrid;
    
    internal static List<Vector3Int> TileWorldPositions;
    internal static Vector2Int TileWorldSize;
    private static int score;
    private static State state;

    internal enum State
    {
        Paused,
        Active,
        Dead,
        Win,
    }

    private static GameHandler instance;

    //TODO 1. Odbijanie wroga od zielonego pola, spawner dla wrogów, po¿eranie jedzenia, grafika, dŸwiêk, portowanie na androida

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
        levelGrid = new LevelGrid(TileWorldSize.x, TileWorldSize.y, player);

        player.Setup(levelGrid, tilemapManager);
        tilemapManager.Setup(tilemapBackground, tilemapSafe, tilemapBorder, tilemapGhost, tileToSpawn, winManager, player, enemy);
        winManager.Setup(tilemapBackground);
    }

    private void Update()
    {
        if (state == State.Paused || state==State.Active)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (state == State.Paused)
                {
                    GameHandler.ResumeGame(player);
                }
                else
                {
                    GameHandler.PauseGame(player);
                }
            }
        }
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
        state = State.Active;
    }
    
    public static void ReloadScene()
    {
        Loader.Load(Loader.Scene.GameScene);
    }

    public static void GoToMainMenu()
    {
        Loader.Load(Loader.Scene.MainMenu);
    }

    public static void PauseGame(PlayerControl player)
    {
        PauseGameWindow.ShowStatic();
        state = State.Paused;
        player.PlayerPause();
    }

    public static void ResumeGame(PlayerControl player)
    {
        PauseGameWindow.HideStatic();
        state= State.Active;
        player.PlayerResume();
    }

    public void PlayerNewLife()
    {
        if (score>=1000)
        {
            RemoveScore(1000, null);
            player.NewLife();
            state=State.Active;
            GameOverWindow.HideStatic();
        }
    }

    public static void PlayerDied(TilemapManager tilemapManager)
    {
        state = State.Dead;
        GameOverWindow.ShowStatic();
        tilemapManager.DestroyGhostTiles();
    }

    public static void PlayerWin()
    {
        state = State.Win;
        WinGameWindow.ShowStatic();
    }
}
