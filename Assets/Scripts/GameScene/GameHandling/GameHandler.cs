using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private WinningManager winningManager;
    [SerializeField] private TilemapSpawner tilemapSpawner;
    [SerializeField] private PlayerControl player;

    private LevelGrid levelGrid;
    List<Vector3Int> TileWorldPositions;
    Vector2Int TileWorldSize;

    private void Awake()
    {
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

        player.Setup(levelGrid);
        levelGrid.Setup(player);

        tilemapSpawner.Setup(TileWorldPositions, TileWorldSize, player);
        winningManager.Setup(tilemapSpawner);

    }
}
