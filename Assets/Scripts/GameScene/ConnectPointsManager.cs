using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConnectPointsManager : MonoBehaviour
{
    Vector3Int FirstCell;
    Vector3Int LastCell;
    readonly List<Vector3Int> TilesPositionsOther;
    readonly List<Vector3Int> TilemapPositions;

    internal ConnectPointsManager(Vector3Int firstCell, Vector3Int lastCell, List<Vector3Int> tileWorldPositions)
    {
        FirstCell = firstCell;
        LastCell = lastCell;
        TilesPositionsOther = tileWorldPositions;
    }

    internal List<Vector3Int> FindWayBetweenPoints()
    {
        List<Vector3Int> tilesToDraw = new();
        List<Vector3Int> tilesOnLine;
        tilesOnLine = CheckForSharedPoints(false);
        if (tilesOnLine.Count == 2)
        {
            tilesToDraw = DrawStraighLineBetweenPoints(FirstCell, LastCell);
        }
        else if (tilesOnLine.Count == 3)
        {
            tilesOnLine = CheckForSharedPoints(true);
            tilesToDraw.AddRange(DrawStraighLineBetweenPoints(FirstCell, tilesOnLine[0]));
            tilesToDraw.AddRange(DrawStraighLineBetweenPoints(tilesOnLine[0], LastCell));
        }
        else if (tilesOnLine.Count == 4)
        {
            tilesOnLine = CheckForShortestWay();
            tilesToDraw.AddRange(DrawStraighLineBetweenPoints(FirstCell, tilesOnLine[0]));
            tilesToDraw.AddRange(DrawStraighLineBetweenPoints(tilesOnLine[0], tilesOnLine[1]));
            tilesToDraw.AddRange(DrawStraighLineBetweenPoints(tilesOnLine[1], LastCell));
        }
        return tilesToDraw;
    }

    List<Vector3Int> CheckForShortestWay()
    {
        List<Vector3Int> tilesShortestWay = new();
        bool checkX;

        if (Mathf.Abs(FirstCell.x - LastCell.x) == 71)
        {
            checkX = false;
        }
        else if (Mathf.Abs(FirstCell.y - LastCell.y) == 35)
        {
            checkX = true;
        }
        else
        {
            return tilesShortestWay;
        }

        float shortestWay = 1000;
        if (checkX)
        {
            foreach (var cell1 in TilesPositionsOther)
            {
                if (cell1.y == FirstCell.y)
                {
                    foreach (var cell2 in TilesPositionsOther)
                    {
                        if (cell2.y == LastCell.y)
                        {
                            if (cell2.x == cell1.x)
                            {
                                float newShortestWay = Mathf.Abs(cell1.x - FirstCell.x) + Mathf.Abs(cell2.x - LastCell.x);
                                if (newShortestWay < shortestWay)
                                {
                                    shortestWay = newShortestWay;
                                    List<Vector3Int> newTilesShortestWay = new()
                                    {
                                        cell1,
                                        cell2
                                    };
                                    tilesShortestWay = newTilesShortestWay;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            foreach (var cell1 in TilesPositionsOther)
            {
                if (cell1.x == FirstCell.x)
                {
                    foreach (var cell2 in TilesPositionsOther)
                    {
                        if (cell2.x == LastCell.x)
                        {
                            if (cell2.y == cell1.y)
                            {
                                float newShortestWay = Mathf.Abs(cell1.y - FirstCell.y) + Mathf.Abs(cell2.y - LastCell.y);
                                if (newShortestWay < shortestWay)
                                {
                                    shortestWay = newShortestWay;
                                    List<Vector3Int> newTilesShortestWay = new()
                                    {
                                        cell1,
                                        cell2
                                    };
                                    tilesShortestWay = newTilesShortestWay;
                                }
                            }
                        }
                    }
                }
            }
        }
        return tilesShortestWay;
    }

    List<Vector3Int> CheckForSharedPoints(bool findOne)
    {
        List<Vector3Int> tilesOnLine = new();

        if (findOne == true)
        {
            foreach (var cell in TilesPositionsOther)
            {
                if ((cell.x == FirstCell.x && cell.y == LastCell.y) || (cell.y == FirstCell.y && cell.x == LastCell.x))
                {
                    tilesOnLine.Add(cell);
                    return tilesOnLine;
                }
            }
        }

        foreach (var cell in TilesPositionsOther)
        {
            if (cell.x == FirstCell.x || cell.x == LastCell.x)
            {
                tilesOnLine.Add(cell);
            }
            else if (cell.y == FirstCell.y || cell.y == LastCell.y)
            {
                tilesOnLine.Add(cell);
            }
        }

        return tilesOnLine;
    }

    List<Vector3Int> DrawStraighLineBetweenPoints(Vector3Int firstCell, Vector3Int lastCell)
    {
        List<Vector3Int> tilesToDraw = new();

        if (firstCell.x == lastCell.x)
        {
            int direction = (firstCell.y > lastCell.y) ? -1 : 1;

            for (int i = firstCell.y * direction; i <= lastCell.y * direction; i++)
            {
                tilesToDraw.Add(new Vector3Int(firstCell.x, (i * direction), 0));
            }
        }
        if (firstCell.y == lastCell.y)
        {
            int direction = (firstCell.x > lastCell.x) ? -1 : 1;

            for (int i = firstCell.x * direction; i <= lastCell.x * direction; i++)
            {
                tilesToDraw.Add(new Vector3Int((i * direction), firstCell.y, 0));
            }
        }

        return tilesToDraw;
    }

    internal ConnectPointsManager(Vector3Int firstCell, List<Vector3Int> tileWorldPositions, List<Vector3Int> tilemapPositions)
    {
        FirstCell = firstCell;
        TilesPositionsOther = tileWorldPositions;
        TilemapPositions = tilemapPositions;
    }

    internal List<Vector3Int> FindWayToBorder()
    {
        List<Vector3Int> borderTiles = new();
        foreach (var tile in TilemapPositions)
        {
            if (tile.x == 0 || tile.x == 71 || tile.y == 35 || tile.y == 0)
            {
                borderTiles.Add(tile);
            }
        }

        List<Vector3Int> tilesToDrawClockwise = MoveAndCheckForBorder(FirstCell, 1, borderTiles);
        List<Vector3Int> tilesToDrawCounterClockwise = MoveAndCheckForBorder(FirstCell, -1, borderTiles);

        List<Vector3Int> tilesToDraw = tilesToDrawClockwise.Count <= tilesToDrawCounterClockwise.Count ? tilesToDrawClockwise : tilesToDrawCounterClockwise;

        return tilesToDraw;
    }

    List<Vector3Int> MoveAndCheckForBorder(Vector3Int currentPosition, int clockwise, List<Vector3Int> borderTiles)
    {
        List<Vector3Int> tilesFromFirstCellToBorder = new();
        bool borderFound = false;

        int safeCount = 0;
        while (borderFound == false)
        {
            int directionX = 0;
            int directionY = 0;

            if (currentPosition.x == 71)
            {
                directionY = -clockwise;
            }
            else if (currentPosition.x == 0)
            {
                directionY = clockwise;
            }

            if (currentPosition.y == 35)
            {
                directionX = clockwise;
            }
            else if (currentPosition.y == 0)
            {
                directionX = -clockwise;
            }

            foreach (var tile in TilesPositionsOther)
            {
                if (tile.x == currentPosition.x && tile.y == currentPosition.y)
                {
                    float smart = currentPosition.x * currentPosition.y * clockwise;
                    if (clockwise == 1)
                    {
                        if (smart > 0)
                        {
                            directionX = 0;
                        }
                        else
                        {
                            directionY = 0;
                        }
                    }
                    else
                    {
                        if (smart < 0)
                        {
                            directionY = 0;
                        }
                        else
                        {
                            directionX = 0;
                        }
                    }
                }
            }

            currentPosition = new Vector3Int(currentPosition.x + directionX, currentPosition.y + directionY);
            tilesFromFirstCellToBorder.Add(currentPosition);

            safeCount++;

            foreach (var tile in borderTiles)
            {
                if (tile == currentPosition || safeCount == 500)
                {
                    borderFound = true;
                }
            }
        }

        return tilesFromFirstCellToBorder;
    }
}
