using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Core.Scripts
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }
        [SerializeField] private Tile tilePrefab;
        public float cellSize;
        [SerializeField] private int rows;
        [SerializeField] private int columns;
        public int Row => rows;
        public int Column => columns;

        [SerializeField] private Color tileColor1, tileColor2;

        public Tile[,] _gridTiles;

        private Transform _gridParent;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _gridTiles = new Tile[rows, columns];
            _gridParent = new GameObject("Grid").transform;
            GenerateGrid();
            AdjustCamera();
        }

        private void GenerateGrid()
        {
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    var newTile = Instantiate(tilePrefab, new Vector3(x * cellSize, y * cellSize, 1),
                        Quaternion.identity, _gridParent);
                    newTile.Init(x, y, (x + y) % 2 == 0 ? tileColor1 : tileColor2);
                    _gridTiles[x, y] = newTile;
                }
            }
        }

        public Tile GetTile(int x, int y)
        {
            if (x >= rows || x < 0 || y >= columns || y < 0) return null;
            return _gridTiles[x, y];
        }

        public Tile GetTile(Vector2 posVector)
        {
            var xIndex = (int)(posVector.x / cellSize);
            var yIndex = (int)(posVector.y / cellSize);
            return GetTile(xIndex, yIndex);
        }

        private Vector2 GetLastTileCoordinates()
        {
            int lastRow = rows - 1;
            int lastCol = columns - 1;
            return new Vector2(lastRow * cellSize, lastCol * cellSize);
        }
        public List<Tile> GetAdjacentTiles(int row, int column)
        {
            List<Tile> adjacentTiles = new List<Tile>();
            row = (int)(BuildingManager.Instance.activeBuildingType.constructionSize.x * row);
            column = (int)(BuildingManager.Instance.activeBuildingType.constructionSize.y * column);
            // Check top tile
            if (row > 0)
            {
                adjacentTiles.Add(_gridTiles[row - 1, column]);
            }

            // Check bottom tile
            if (row < rows - 1)
            {
                adjacentTiles.Add(_gridTiles[row + 1, column]);
            }

            // Check left tile
            if (column > 0)
            {
                adjacentTiles.Add(_gridTiles[row, column - 1]);
            }

            // Check right tile
            if (column < columns - 1)
            {
                adjacentTiles.Add(_gridTiles[row, column + 1]);
            }

            // Check top left tile
            if (row > 0 && column > 0)
            {
                adjacentTiles.Add(_gridTiles[row - 1, column - 1]);
            }

            // Check top right tile
            if (row > 0 && column < columns - 1)
            {
                adjacentTiles.Add(_gridTiles[row - 1, column + 1]);
            }

            // Check bottom left tile
            if (row < rows - 1 && column > 0)
            {
                adjacentTiles.Add(_gridTiles[row + 1, column - 1]);
            }

            // Check bottom right tile
            if (row < rows - 1 && column < columns - 1)
            {
                adjacentTiles.Add(_gridTiles[row + 1, column + 1]);
            }

            return adjacentTiles;
        }



        private void AdjustCamera()
        {
            if (Camera.main == null) return;
            Camera.main.orthographic = true; // Set the camera to use orthographic projection

            // Get the screen resolution and calculate the aspect ratio
            float screenAspect = (float)Screen.width / (float)Screen.height;

            // Calculate the size of the camera's view based on the size of the grid and the screen resolution
            float gridWidth = columns * cellSize;
            float gridHeight = rows * cellSize;
            float cameraHeight = Mathf.Max(gridWidth / screenAspect, gridHeight);
            float cameraSize = cameraHeight / 2f;
            Camera.main.orthographicSize = cameraSize;

            // Position the camera to look at the center of the grid
            Vector2 gridCenter = GetLastTileCoordinates() / 2;
            Camera.main.transform.position = (Vector3)gridCenter + new Vector3(0, 0, -10f);
        }
    }
}