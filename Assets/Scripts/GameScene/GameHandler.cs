using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private PlayerControl player;

    private LevelGrid levelGrid;

    private void Start()
    {
        levelGrid = new LevelGrid(71, 35);

        player.Setup(levelGrid);
        levelGrid.Setup(player);
    }
}
