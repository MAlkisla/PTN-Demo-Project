using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tile _tilePrefab;
    public float cellSize;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    public int width => _width;
    public int Height => _height;

    [SerializeField] private Color _indexZeroColor;
    [SerializeField] private Color _indexOneColor;

    private Tile[,] _gridTiles;

    private Transform _gridParent;

    private List<Tile> openList;
    private List<Tile> closedList;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    #region Singleton

    private static GridManager m_Instance;

    public static GridManager Instance => m_Instance;


    private void Awake()
    {
        m_Instance = this;
    }

    #endregion

    private void Start()
    {
        _gridTiles = new Tile[_width, _height];

        _gridParent = new GameObject("Grid").transform;

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var newTile = Instantiate(_tilePrefab, new Vector3(x * cellSize, y * cellSize, 1),
                    Quaternion.identity, _gridParent);
                newTile.Init(x, y, (x + y) % 2 == 0 ? _indexZeroColor : _indexOneColor);
                _gridTiles[x, y] = newTile;
            }
        }

        if (Camera.main != null)
            Camera.main.transform.position =
                new Vector3((_width * cellSize - cellSize) / 2f, (_height * cellSize - cellSize) / 2f,
                    -10f); // set cam pos to middle of grid.
    }

    public Tile GetTile(int x, int y)
    {
        var xIndex = (int)(x / cellSize);
        var yIndex = (int)(y / cellSize);
        if (xIndex >= _width || xIndex < 0 || yIndex >= _height || yIndex < 0) return null;
        return _gridTiles[xIndex, yIndex];
    }

    public Tile GetTile(Vector2 posVector)
    {
        var xIndex = (int)posVector.x;
        var yIndex = (int)posVector.y;
        return GetTile(xIndex, yIndex);
    }

    public Tile GetClosestTile(Vector2 pos)
    {
        var tile = GetTile(pos);
        if (tile != null && tile.tileEmpty) return tile;
        var tempCounterX = 0;
        var tempCounterY = 0;
        var tempCounterMinusX = 0;
        var tempCounterMinusY = 0;
        var squareOffset = 1;
        var tempPos = pos - Vector2.right;
        tile = GetTile(tempPos);
        var maxSearchDepth = _width*_height;
        var counter = 0;
        while (tile == null || !tile.tileEmpty)
        {
            if (counter == maxSearchDepth)
            {
                Debug.Log("path not found in range of search depth.");
                return null;
            }
            counter++;
            
            if (tempCounterY < squareOffset)
            {
                tempPos += Vector2.up;
                tempCounterY++;
            }
            else if (tempCounterX < squareOffset + 1)
            {
                tempPos += Vector2.right;
                tempCounterX++;
            }
            else if (tempCounterMinusY < squareOffset + 1)
            {
                tempPos -= Vector2.up;
                tempCounterMinusY++;
            }
            else if (tempCounterMinusX < squareOffset + 2)
            {
                tempPos -= Vector2.right;
                tempCounterMinusX++;
            }
            else
            {
                squareOffset += 2;
                tempCounterX = 0;
                tempCounterY = 0;
                tempCounterMinusY = 0;
                tempCounterMinusX = 0;
            }
            tile = GetTile(tempPos);
            
        }
        return tile;
    }

    #region A* Pathfinding

    public List<Tile> FindPath(int startX, int startY, int endX, int endY)
    {
        Tile startTile = GetTile(startX, startY);
        Tile endTile = GetTile(endX, endY);
        openList = new List<Tile> { startTile };
        closedList = new List<Tile>();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var tile = GetTile(x, y);
                tile.gCost = Int32.MaxValue;
                tile.CalculateFCost();
                tile.cameFromTile = null;
            }
        }

        startTile.gCost = 0;
        startTile.hCost = CalculateDistanceCost(startTile, endTile);
        startTile.CalculateFCost();

        while (openList.Count > 0)
        {
            var currentTile = GetLowestFCostTile(openList);
            if (currentTile == endTile)
            {
                return CalculatePath(endTile);
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach (var neighbourTile in GetNeighbourList(currentTile))
            {
                if(closedList.Contains(neighbourTile))continue;
                if (!neighbourTile.tileEmpty)
                {
                    closedList.Add(neighbourTile);
                    continue;
                }

                int tentativeGCost = currentTile.gCost + CalculateDistanceCost(currentTile, neighbourTile);
                if (tentativeGCost < neighbourTile.gCost)
                {
                    neighbourTile.cameFromTile = currentTile;
                    neighbourTile.gCost = tentativeGCost;
                    neighbourTile.hCost = CalculateDistanceCost(neighbourTile, endTile);
                    neighbourTile.CalculateFCost();

                    if (!openList.Contains(neighbourTile))
                    {
                        openList.Add(neighbourTile);
                    }
                }
            }
        }
        
        //out of nodes on openlist
        return null;
    }

    private int CalculateDistanceCost(Tile tileA, Tile tileB)
    {
        int xDistance = Mathf.Abs(tileA.x - tileB.x);
        int yDistance = Mathf.Abs(tileA.y - tileB.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private Tile GetLowestFCostTile(List<Tile> tiles)
    {
        var lowestFCostTile = tiles[0];
        for (int i = 1; i < tiles.Count; i++)
        {
            if (tiles[i].fCost < lowestFCostTile.fCost)
            {
                lowestFCostTile = tiles[i];
            }
        }

        return lowestFCostTile;
    }

    private List<Tile> CalculatePath(Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        path.Add(endTile);
        var currentTile = endTile;
        while (currentTile.cameFromTile != null)
        {
            path.Add(currentTile.cameFromTile);
            currentTile = currentTile.cameFromTile;
        }
        
        path.Reverse();
        return path;
    }

    private List<Tile> GetNeighbourList(Tile tile)
    {
        List<Tile> neighbourList = new List<Tile>();
        if (tile.x - 1 >= 0)
        {
            neighbourList.Add(GetTile(tile.x - 1, tile.y));
            if (tile.y - 1 >= 0)
            {
                neighbourList.Add(GetTile(tile.x - 1, tile.y - 1));
            }

            if (tile.y + 1 < _height)
            {
                neighbourList.Add(GetTile(tile.x - 1, tile.y + 1));
            }
        }

        if (tile.x + 1 < _width)
        {
            neighbourList.Add(GetTile(tile.x + 1, tile.y));
            if (tile.y - 1 >= 0)
            {
                neighbourList.Add(GetTile(tile.x + 1, tile.y - 1));
            }

            if (tile.y + 1 < _height)
            {
                neighbourList.Add(GetTile(tile.x + 1, tile.y + 1));
            }
        }

        if (tile.y - 1 >= 0) neighbourList.Add(GetTile(tile.x, tile.y - 1));
        if (tile.y + 1 < _height) neighbourList.Add(GetTile(tile.x, tile.y + 1));
        return neighbourList;
    }

    #endregion
    
}