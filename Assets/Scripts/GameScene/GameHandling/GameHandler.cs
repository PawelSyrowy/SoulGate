using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameHandler : MonoBehaviour
{

    // todo 1. Retry Button i Next button do odpowiednich scen, zmieni� nazwe level1, nauczy� si� eventy i zrobic dla player dead i blue tiles destroy, highscore dla ka�dej sceny osobny
    [SerializeField] internal Tilemap tilemapBackground;
    [SerializeField] internal Tilemap tilemapSafe;
    [SerializeField] internal Tilemap tilemapBorder;
    [SerializeField] internal Tilemap tilemapGhost;
    [SerializeField] internal TileBase tileToSpawn;

    [SerializeField] private WinManager winManager;
    [SerializeField] private TilemapManager tilemapManager; 
    [SerializeField] private PlayerControl player;
    [SerializeField] private EnemyManager enemyManager;

    private LevelGrid levelGrid;
    
    internal static List<Vector3Int> TileWorldPositions;
    internal static Vector2Int TileWorldSize;
    private static State state;

    internal enum State
    {
        Paused,
        Active,
        Dead,
        Win,
    }

    private static GameHandler instance;

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
        tilemapManager.Setup(tilemapBackground, tilemapSafe, tilemapBorder, tilemapGhost, tileToSpawn, winManager, player, enemyManager);
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

    private static void InitializeStatic()
    {
        state = State.Active;
        Score.InitializeStatic();
    }

    public static void NextLevel()
    {
        Loader.Load(Loader.Scene.Level2);
        SoundManager.PlaySound(SoundManager.Sound.BadClick);
    }

    public static void ReloadScene()
    {
        Loader.Load(Loader.Scene.GameScene);
        SoundManager.PlaySound(SoundManager.Sound.Click);
    }

    public static void GoToMainMenu()
    {
        Loader.Load(Loader.Scene.MainMenu);
        SoundManager.PlaySound(SoundManager.Sound.Click);
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
        state = State.Active;
        player.PlayerResume();
    }

    public void PlayerNewLife()
    {
        if (Score.GetScore()>= 1000)
        {
            Score.RemoveScore(1000, null);
            player.NewLife();
            state=State.Active;
            GameOverWindow.HideStatic();
            SoundManager.PlaySound(SoundManager.Sound.NewLife);
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.BadClick);
        }
    }

    public static void PlayerDied(TilemapManager tilemapManager)
    {
        state = State.Dead;
        GameOverWindow.ShowStatic();
        tilemapManager.DestroyGhostTiles();
        SoundManager.PlaySound(SoundManager.Sound.Die);
    }

    public static void PlayerWin()
    {
        state = State.Win;
        WinGameWindow.ShowStatic();
        Score.TrySetNewHighscore();
        SoundManager.PlaySound(SoundManager.Sound.Win);
    }
}
