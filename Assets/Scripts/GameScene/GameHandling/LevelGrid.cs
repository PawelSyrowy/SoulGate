using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGrid
{
    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private int width;
    private int height;
    private PlayerControl player;

    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void Setup(PlayerControl player)
    {
        this.player = player;

        SpawnFood();
    }

    private void SpawnFood()
    {
        do
        {
            foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (player.GetGridPosition() == foodGridPosition);

        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.foodSprite;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    public void SnakeMoved(Vector2Int playerGridPosition)
    {
        Vector2 playerGridPositionBig = new Vector2Int ((int)math.floor(playerGridPosition.x / 3), (int)math.floor(playerGridPosition.y / 3));
        Vector2 foodGridPositionBig = new Vector2Int((int)math.floor(foodGridPosition.x / 3), (int)math.floor(foodGridPosition.y / 3));
        if (playerGridPositionBig == foodGridPositionBig)
        {
            Object.Destroy(foodGameObject);
            SpawnFood();
            GameHandler.AddScore();
        }
    }
}
