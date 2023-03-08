using System;
using UnityEngine;

public class Grid<TGridObject> {

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        
        gridArray = new TGridObject[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = createGridObject(this, x, y);
                GridBuildingSystem.Instance.openTile.Add(new Vector2(x,y));
                //Pathfinding.Instance.GetNode(x, y).SetIsWalkable(Pathfinding.Instance.GetNode(x, y).isWalkable);
            }
        }
       
        bool showDebug = true;
        if (showDebug) {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int y = 0; y < gridArray.GetLength(1); y++) {
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 20, Color.black, TextAnchor.MiddleCenter, TextAlignment.Center);
                   
                }
            }

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
                for (int i = 0; i < GridBuildingSystem.Instance.openTile.Count; i++) {
                    Vector2 tile = GridBuildingSystem.Instance.openTile[i];
                    if (tile == new Vector2(eventArgs.x, eventArgs.y)) {
                        GridBuildingSystem.Instance.openTile.RemoveAt(i);
                        GridBuildingSystem.Instance.closedTile.Add(tile);
                        Pathfinding.Instance.GetNode(eventArgs.x, eventArgs.y).SetIsWalkable(!Pathfinding.Instance.GetNode(eventArgs.x, eventArgs.y).isWalkable);
                        break;
                    }
                }
            };
        }
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            TriggerGridObjectChanged(x, y);
        }
    }

    public void TriggerGridObjectChanged(int x, int y) {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        GetXY(worldPosition, out int x, out int y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height) 
            return gridArray[x, y];
        return default;
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }
    
    public Vector2Int ValidateGridPosition(Vector2Int gridPosition ,PlacedObjectTypeSO placedObjectTypeSO) {
        if (gridPosition.x < 0)
        {
            return new Vector2Int(
                Mathf.Clamp(gridPosition.x, 0, width - 1),
                Mathf.Clamp(gridPosition.y, 0, height - 1)
            ); 
        }else if (gridPosition.x >= GridBuildingSystem.Instance.gridWidth )
        {
            return new Vector2Int(
                Mathf.Clamp(gridPosition.x, (int)(GridBuildingSystem.Instance.gridWidth-placedObjectTypeSO.width), (int)( GridBuildingSystem.Instance.gridWidth-placedObjectTypeSO.width)),
                Mathf.Clamp(gridPosition.y, (int)(gridPosition.y-1), (int)( gridPosition.y))
            );
            
        }
        else
        {
            return new Vector2Int(gridPosition.x,gridPosition.y);
        }
        
    }

}
