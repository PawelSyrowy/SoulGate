using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{    
    [SerializeField] internal EnemyControl[] enemyArray;

    internal void AllEnemyStart()
    {
        foreach (EnemyControl enemy in enemyArray)
        {
            enemy.StartMovement();
        }
    }

    internal void AllEnemyStop()
    {
        foreach (EnemyControl enemy in enemyArray)
        {
            enemy.EnemyStopped();
        }
    }

    internal void AllEnemyMove()
    {
        foreach (EnemyControl enemy in enemyArray)
        {
            enemy.EnemyMoving();
        }
    }
}
