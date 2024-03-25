using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGrid
{
    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private readonly int width;
    private readonly int height;
    private readonly TilemapManager tilemapManager;
    private bool foodExists = false;
    private int waitingFood = 0;

    public LevelGrid(TilemapManager tilemapManager)
    {
        this.width = GameHandler.TileWorldSize.x;
        this.height = GameHandler.TileWorldSize.y;
        this.tilemapManager = tilemapManager;
    }

    internal void NewFood()
    {
        FindFoodPosition();
        SpawnFood();
    }

    private void FindFoodPosition()
    {
        do
        {
            foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (!TilemapManagerExtension.CheckTileExists(tilemapManager.TilemapSafe, new Vector3 (foodGridPosition.x, foodGridPosition.y, 0)));
    }

    private void SpawnFood()
    {
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.foodSprite;
        foodGameObject.GetComponent<SpriteRenderer>().material.color = Color.green;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
        foodExists = true;
        waitingFood--;
    }

    private void EatFood()
    {
        DestroyFood();
        Score.AddLifes();
        SoundManager.PlaySound(SoundManager.Sound.Food);
    }

    private void DestroyFood()
    {
        foodExists = false;
        Object.Destroy(foodGameObject);
    }

    public void PlayerMoved(Vector2Int playerGridPosition)
    {
        if (foodExists)
        {
            Vector2 playerGridPositionBig = new Vector2Int((int)math.floor(playerGridPosition.x / 3), (int)math.floor(playerGridPosition.y / 3));
            Vector2 foodGridPositionBig = new Vector2Int((int)math.floor(foodGridPosition.x / 3), (int)math.floor(foodGridPosition.y / 3));
            if (playerGridPositionBig == foodGridPositionBig)
            {
                EatFood();
            }
        }
    }

    internal void CheckFoodAmount()
    {
        if (!foodExists)
        {
            waitingFood += Progress.CheckFoodProgress();
            if (waitingFood > 0)
            {
                NewFood();
            }
        }
    }
}
