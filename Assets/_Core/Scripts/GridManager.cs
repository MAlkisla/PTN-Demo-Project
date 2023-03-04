using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Core.Scripts
{
    // public class GridManager : MonoBehaviour
    // {
    //     public GameObject tilePrefab;   // The tile object to use in the grid
    //     public int rows;                // The number of rows in the grid
    //     public int columns;             // The number of columns in the grid
    //     public float cellSize = 32f;    // The size of each cell in the grid
    //     public Color tileColor1, tileColor2; // The second color to assign to the tiles
    //
    //     private GameObject[,] _grid;     // The 2D array to hold the tiles in the grid
    //     private Transform _gridParent;   // The parent object for the grid
    //
    //     private void Start()
    //     {
    //         CreateGrid();
    //         AdjustCamera();
    //     }
    //
    //     private void CreateGrid()
    //     {
    //         // Create the grid parent object
    //         _gridParent = new GameObject("Grid").transform;
    //
    //         _grid = new GameObject[rows, columns];
    //
    //         for (int row = 0; row < rows; row++)
    //         {
    //             for (int col = 0; col < columns; col++)
    //             {
    //                 GameObject tile = Instantiate(tilePrefab, _gridParent);  // Create a new tile object as a child of the grid parent object
    //                 tile.transform.position = new Vector3(col * cellSize, row * cellSize, 0); // Position the tile in the grid
    //
    //                 // Set the color of the tile based on its position
    //                 if ((row + col) % 2 == 0)
    //                 {
    //                     tile.GetComponent<Renderer>().material.color = tileColor1;
    //                 }
    //                 else
    //                 {
    //                     tile.GetComponent<Renderer>().material.color = tileColor2;
    //                 }
    //
    //                 _grid[row, col] = tile;  // Add the tile to the grid array
    //             }
    //         }
    //     }
    //
    //     private void AdjustCamera()
    //     {
    //         if (Camera.main == null) return;
    //         Camera.main.orthographic = true; // Set the camera to use orthographic projection
    //
    //         // Get the screen resolution and calculate the aspect ratio
    //         float screenAspect = (float)Screen.width / (float)Screen.height;
    //
    //         // Calculate the size of the camera's view based on the size of the grid and the screen resolution
    //         float gridWidth = columns * cellSize - cellSize / 2;
    //         float gridHeight = rows * cellSize - cellSize / 2;
    //         float cameraHeight = Mathf.Max(gridWidth / screenAspect, gridHeight);
    //         float cameraSize = cameraHeight / 2f;
    //         Camera.main.orthographicSize = cameraSize;
    //
    //         // Position the camera to look at the center of the grid
    //         Vector3 gridCenter = new Vector3(gridWidth / 2f, gridHeight / 2f, 0);
    //         Camera.main.transform.position = gridCenter + new Vector3(0, 0, -10f);
    //     }
    // }


    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Tile tilePrefab;
        public float cellSize;
        [SerializeField] private int rows;
        [SerializeField] private int columns;
        public int Row => rows;
        public int Column => columns;

        [SerializeField] private Color tileColor1, tileColor2;

        public Tile[,] _gridTiles;

        private Transform _gridParent;


        // #region Singleton
        //
        // private static GridManager m_Instance;
        //
        // public static GridManager Instance => m_Instance;
        //
        //
        // private void Awake()
        // {
        //     m_Instance = this;
        // }
        //
        // #endregion

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

        public Tile GetClosestTile(Vector2 pos)
        {
            var tile = GetTile(pos);
            if (tile != null && tile.TileEmpty) return tile;
            var tempCounterX = 0;
            var tempCounterY = 0;
            var tempCounterMinusX = 0;
            var tempCounterMinusY = 0;
            var squareOffset = 1;
            var tempPos = pos - Vector2.right;
            tile = GetTile(tempPos);
            var maxSearchDepth = rows * columns;
            var counter = 0;
            while (tile == null || !tile.TileEmpty)
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
                    tempPos = pos - Vector2.right * squareOffset;
                }

                tile = GetTile(tempPos);
            }

            return tile;
        }

        private Vector2 GetLastTileCoordinates()
        {
            int lastRow = rows - 1;
            int lastCol = columns - 1;
            return new Vector2(lastRow * cellSize, lastCol * cellSize);
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