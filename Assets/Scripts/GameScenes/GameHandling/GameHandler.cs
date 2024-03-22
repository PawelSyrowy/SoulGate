using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameHandler : MonoBehaviour
{

    // todo 1. Highscore dla ka¿dej sceny osobny
    [SerializeField] LevelConfiguration levelConfiguration;

    [SerializeField] internal Tilemap tilemapBackground;
    [SerializeField] internal Tilemap tilemapSafe;
    [SerializeField] internal Tilemap tilemapBorder;
    [SerializeField] internal Tilemap tilemapGhost;
    [SerializeField] internal TileBase tileToSpawn;

    [SerializeField] private PlayerControl player;
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private WinManager winManager;
    [SerializeField] private EnemyManager enemyManager;

    private LevelGrid levelGrid;
    private static GameHandler instance;

    internal static List<Vector3Int> TileWorldPositions;
    internal static Vector2Int TileWorldSize;
    internal static int LevelNumber;
    internal static int MaxLevel; 
    private static State state;

    internal enum State
    {
        Paused,
        Active,
        Dead,
        Win,
    }

    private void Awake()
    {
        instance = this;
        InitializeStatic();

        LevelNumber = levelConfiguration.LevelNumber;
        MaxLevel = levelConfiguration.MaxLevel;

        TileWorldSize = new Vector2Int(71, 35);
        TileWorldPositions = new List<Vector3Int>
        {
            new(-36, 17, 0),
            new(-36, -18, 0),
            new(35, -18, 0),
            new(35, 17, 0)
        };
    }

    private static void InitializeStatic()
    {
        state = State.Active;
        Score.InitializeStatic();
    }

    private void Start()
    {
        levelGrid = new LevelGrid(TileWorldSize.x, TileWorldSize.y, player);

        player.Setup(levelGrid, tilemapManager);
        tilemapManager.Setup(tilemapBackground, tilemapSafe, tilemapBorder, tilemapGhost, tileToSpawn, winManager, player, enemyManager);
        winManager.Setup(tilemapBackground, levelConfiguration.WinExpectation);
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

    public static void NextLevel()
    {
        if (LevelNumber < MaxLevel)
        {
            Loader.Scene parsedEnum;
            Enum.TryParse("Level" + (LevelNumber + 1).ToString(), out parsedEnum);
            Loader.Load(parsedEnum); 
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.BadClick);
        }
    }

    public static void ReloadScene()
    {
        Loader.Scene parsedEnum;
        Enum.TryParse("Level" + LevelNumber.ToString(), out parsedEnum);
        Loader.Load(parsedEnum);
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
