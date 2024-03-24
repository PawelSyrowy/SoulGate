using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConnectPointsAlgorithm
{
    Vector3Int FirstCell;
    Vector3Int LastCell;
    readonly List<Vector3Int> TilesPositionsOther;
    readonly Vector2Int TileWorldSize;
    readonly List<Vector3Int> TilemapPositionsBorder;

    internal ConnectPointsAlgorithm(Vector3Int firstCell, Vector3Int lastCell, List<Vector3Int> tileWorldPositions, Vector2Int tileWorldSize, List<Vector3Int> tilemapPositions)
    {
        FirstCell = firstCell;
        LastCell = lastCell;
        TilesPositionsOther = tileWorldPositions;
        TileWorldSize = tileWorldSize;
        TilemapPositionsBorder = tilemapPositions;
    }

    internal List<Vector3Int> FinishBorder()
    {
        if (1==2) //TilemapPositionsBorder.Count > 0
        {
            List<Vector3Int> shortestWayToBorder = FindShortestWayToBorder();
            List<Vector3Int> wayBetweenPoints = FindShortestWayBetweenPoints();
            List<Vector3Int> finishBorder = wayBetweenPoints.Count <= shortestWayToBorder.Count ? wayBetweenPoints : shortestWayToBorder;

            return finishBorder;
        }
        else
        {
            return FindShortestWayBetweenPoints();
        }
    }

    private List<Vector3Int> FindShortestWayToBorder()
    {
        List<Vector3Int> shortestWayToBorder = new();
        shortestWayToBorder.AddRange(FindWayToBorder(FirstCell));
        shortestWayToBorder.AddRange(FindWayToBorder(LastCell));

        return shortestWayToBorder;
    }

    internal List<Vector3Int> FindWayToBorder(Vector3Int onlyCell)
    {
        List<Vector3Int> borderTiles = new();
        foreach (var tile in TilemapPositionsBorder)
        {
            if (tile.x == TilesPositionsOther[1].x || tile.x == TilesPositionsOther[3].x || tile.y == TilesPositionsOther[3].y || tile.y == TilesPositionsOther[1].y)
            {
                borderTiles.Add(tile);
            }
        }

        List<Vector3Int> tilesToDrawClockwise = MoveAndCheckForBorder(onlyCell, 1, borderTiles);
        List<Vector3Int> tilesToDrawCounterClockwise = MoveAndCheckForBorder(onlyCell, -1, borderTiles);

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

            if (currentPosition.x == TilesPositionsOther[3].x)
            {
                directionY = -clockwise;
            }
            else if (currentPosition.x == TilesPositionsOther[1].x)
            {
                directionY = clockwise;
            }

            if (currentPosition.y == TilesPositionsOther[3].y)
            {
                directionX = clockwise;
            }
            else if (currentPosition.y == TilesPositionsOther[1].y)
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

    internal List<Vector3Int> FindShortestWayBetweenPoints()
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

        if (Mathf.Abs(FirstCell.x - LastCell.x) == TileWorldSize.x)
        {
            checkX = false;
        }
        else if (Mathf.Abs(FirstCell.y - LastCell.y) == TileWorldSize.y)
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
}