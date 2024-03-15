using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class ClosedShapesAlgorithm : MonoBehaviour
{
    readonly List<Vector3Int> Positions;
    readonly int OffsetX;
    readonly int OffsetY;
    readonly Vector2Int TileWorldSize;

    bool[,] grid;
    int width;
    int height;

    //public List<Vector3> EmptyPositionsTest = new List<Vector3>();
    //public List<Vector3> ForbiddenPositionsTest = new List<Vector3>();
    //public List<Vector3> BusyPositionsTest = new List<Vector3>();
    //public List<Vector3> BorderPositionsTest = new List<Vector3>();

    internal ClosedShapesAlgorithm(List<Vector3Int> positions, int offsetX, int offsetY, Vector2Int tileWorldSize)
    {
        Positions = positions;
        OffsetX = offsetX + 1;
        OffsetY = offsetY + 1;
        TileWorldSize = tileWorldSize;
    }

    internal List<List<Vector3Int>> GetEmptyPositions(Vector3 enemyPoint)
    {
        width = TileWorldSize.x + 3;
        height = TileWorldSize.y + 3;

        grid = new bool[width, height];
        foreach (Vector3Int position in Positions)
        {
            grid[position.x + OffsetX, position.y + OffsetY] = true;
            //BorderPositionsTest.Add(new Vector3((position.x + 100) * GlobalScale, (position.y) * GlobalScale, 0));
        }
        FloodFill(Point.Empty);
        List<Vector3Int> EmptyPositions = PrintGrid()[0];

        grid = new bool[width, height];
        foreach (Vector3Int position in Positions)
        {
            grid[position.x + OffsetX, position.y + OffsetY] = true;
        }
        Vector3Int enemyPointConverted = Vector3Int.RoundToInt(enemyPoint);
        FloodFill(new Point(enemyPointConverted.x + OffsetX, enemyPointConverted.y + OffsetY));
        List<Vector3Int> NegativePositions = PrintGrid()[1];

        List<List<Vector3Int>> positions = new()
        {
            new(),
            new()
        };
        positions[0] = EmptyPositions;
        positions[1] = NegativePositions;

        return positions;
    }

    List<List<Vector3Int>> PrintGrid()
    {
        List<List<Vector3Int>> positions = new()
        {
            new(),
            new()
        };

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                char c;

                if (row == 0 || row == height - 1 || col == 0 || col == width - 1)
                {
                    c = 'X';
                    //if (c == 'X')
                    //{
                    //    ForbiddenPositionsTest.Add(new Vector3((col - OffsetX + 100) * GlobalScale, (row - OffsetY) * GlobalScale, 0));
                    //}
                }
                else
                {
                    c = grid[col, row] ? '#' : '.';
                    if (c == '.')
                    {
                        positions[0].Add(new Vector3Int((col - OffsetX), (row - OffsetY), 0));
                        //EmptyPositionsTest.Add(new Vector3((col - OffsetX+100) * GlobalScale, (row - OffsetY) * GlobalScale, 0));
                    }
                    else if (c == '#')
                    {
                        //BusyPositionsTest.Add(new Vector3((col - OffsetX + 100) * GlobalScale, (row - OffsetY) * GlobalScale, 0));
                        positions[1].Add(new Vector3Int((col - OffsetX), (row - OffsetY), 0));
                    }
                }
            }
        }
        return positions;
    }

    void FloodFill(Point point)
    {
        Queue<Point> q = new Queue<Point>();
        q.Enqueue(point);
        while (q.Count > 0)
        {
            Point n = q.Dequeue();
            if (grid[n.X, n.Y])
                continue;
            Point w = n, e = new Point(n.X + 1, n.Y);
            while ((w.X >= 0) && !grid[w.X, w.Y])
            {
                grid[w.X, w.Y] = true;

                if ((w.Y > 0) && !grid[w.X, w.Y - 1])
                    q.Enqueue(new Point(w.X, w.Y - 1));
                if ((w.Y < height - 1) && !grid[w.X, w.Y + 1])
                    q.Enqueue(new Point(w.X, w.Y + 1));
                w.X--;
            }
            while ((e.X <= width - 1) && !grid[e.X, e.Y])
            {
                grid[e.X, e.Y] = true;

                if ((e.Y > 0) && !grid[e.X, e.Y - 1])
                    q.Enqueue(new Point(e.X, e.Y - 1));
                if ((e.Y < height - 1) && !grid[e.X, e.Y + 1])
                    q.Enqueue(new Point(e.X, e.Y + 1));
                e.X++;
            }
        }
    }
}
