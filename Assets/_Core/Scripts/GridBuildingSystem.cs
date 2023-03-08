using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridBuildingSystem : MonoBehaviour {

    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;
    public event EventHandler<OnActiveBuildingTypeChangedEventArgs> OnActiveBuildingTypeChanged;
    public class OnActiveBuildingTypeChangedEventArgs : EventArgs
    {
        public PlacedObjectTypeSO ActiveBuildingType;
    }
    private Grid<GridObject> grid;
    [SerializeField] public List<PlacedObjectTypeSO> placedObjectTypeSOList = null;
    public PlacedObjectTypeSO placedObjectTypeSO;
    private PlacedObjectTypeSO.Dir dir;
    
    [SerializeField] private Tile tilePrefab;
    public Tile[,] _gridTiles;
    [SerializeField] public int gridWidth = 10;
    [SerializeField] public int gridHeight = 10;
    [SerializeField] public float cellSize = 32f;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private Color tileColor1, tileColor2;
    
    [SerializeField] public PlacedObjectTypeSO activeBuildingType;
    [SerializeField] private BuildingTypeSelectUI _buildingTypeSelectUI;
    
    private SpriteRenderer ghostSprite;
    private float flashDuration = 0.5f;
    private Color originalColor;
    private Coroutine currentCoroutine;

    public Testing _testing;
    public List<Vector2> openTile;
    public List<Vector2> closedTile;
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        placedObjectTypeSO = null;
    }

    public class GridObject {

        private Grid<GridObject> grid;
        private int x;
        private int y;
        public PlacedObject_Done placedObject;
        
        public GridObject(Grid<GridObject> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
            placedObject = null;
        }

        public override string ToString() {
            return x + ", " + y + "\n" + placedObject;
        }

        public void SetPlacedObject(PlacedObject_Done placedObject) {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject() {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObject_Done GetPlacedObject() {
            return placedObject;
        }

        public bool CanBuild() {
            return placedObject == null;
        }

    }

    private void Start()
    {
        _gridTiles = new Tile[gridWidth, gridHeight];
        GenerateGrid();
        AdjustCamera();
    }
    private void Update() {
        
        if (Input.GetMouseButtonDown(0) && placedObjectTypeSO != null) {
            
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            grid.GetXY(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            placedObjectOrigin = grid.ValidateGridPosition(placedObjectOrigin,placedObjectTypeSO);
            
            // Test Can Build
            List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList) {
                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild() ) {
                    canBuild = false;
                    break;
                }
            }

            if (canBuild) {
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y);

                PlacedObject_Done placedObject = PlacedObject_Done.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                }
                OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                
                DeselectObjectType();
            } else {
                // Cannot build here
                UtilsClass.CreateWorldTextPopup("Cannot Build Here!", mousePosition);

                ghostSprite = BuildingGhost.Instance.visual.transform.Find("Sprite").GetComponent<SpriteRenderer>();
                originalColor = ghostSprite.color;
                Flash();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }


        if (Input.GetMouseButtonDown(1)) {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            PlacedObject_Done placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
            if (placedObject != null) {
                // Demolish
                placedObject.DestroySelf();
        
                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                foreach (Vector2Int gridPosition in gridPositionList) {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }
            }
        }
    }
    
    private void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var newTile = Instantiate(tilePrefab, new Vector3((x * cellSize) + cellSize/2, (y * cellSize) + cellSize/2, 1),
                    Quaternion.identity, _gridParent);
                newTile.Init(x, y, (x + y) % 2 == 0 ? tileColor1 : tileColor2);
                _gridTiles[x, y] = newTile;
            }
        }
    }

    private void DeselectObjectType() {
        placedObjectTypeSO = null; RefreshSelectedObjectType();
        activeBuildingType = placedObjectTypeSO;
        OnActiveBuildingTypeChanged?.Invoke(this,
            new OnActiveBuildingTypeChangedEventArgs { ActiveBuildingType = activeBuildingType });
    }

    public void RefreshSelectedObjectType() {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }


    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        grid.GetXY(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetMouseWorldSnappedPosition() {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        grid.GetXY(mousePosition, out int x, out int y);

        if (placedObjectTypeSO != null) {
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y);
            return placedObjectWorldPosition;
        } else {
            return mousePosition;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() {
        return placedObjectTypeSO;
    }
    public Vector2 GetLastTileCoordinates()
    {
        int lastRow = gridWidth - 1;
        int lastCol = gridHeight - 1;
        return new Vector2(lastRow * cellSize, lastCol * cellSize);
    }
    private void AdjustCamera()
    {
        if (Camera.main == null) return;
        Camera.main.orthographic = true; // Set the camera to use orthographic projection

        // Get the screen resolution and calculate the aspect ratio
        float screenAspect = (float)Screen.width / (float)Screen.height;

        // Calculate the size of the camera's view based on the size of the grid and the screen resolution
        float row = gridWidth * cellSize;
        float columns = gridHeight * cellSize;
        float cameraHeight = Mathf.Max(row / screenAspect, columns);
        float cameraSize = cameraHeight / 2f;
        Camera.main.orthographicSize = cameraSize;

        // Position the camera to look at the center of the grid
        Vector2 gridCenter = GetLastTileCoordinates() / 2;
        Camera.main.transform.position = (Vector3)gridCenter + new Vector3(cellSize/2, cellSize/2, -10f);
    }
 
    public void SetActiveBuildingType(PlacedObjectTypeSO placedObjectType)
    {
        activeBuildingType = placedObjectType;
        OnActiveBuildingTypeChanged?.Invoke(this,
            new OnActiveBuildingTypeChangedEventArgs { ActiveBuildingType = activeBuildingType });
    }
    public PlacedObjectTypeSO GetActiveBuildingTypeButton()
    {
        return activeBuildingType;
    }
    
    public void Flash()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            ghostSprite.color = Color.red;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ghostSprite.color = originalColor;
    }
    
    public List<Vector2Int> GetAttackAbleGridPositions(PlacedObject_Done placedObject)
    {
        List<Vector2Int> adjacentPositions = new List<Vector2Int>();

        foreach (Vector2Int gridPosition in placedObject.GetGridPositionList())
        {
            for (int x = gridPosition.x - 1; x <= gridPosition.x + 1; x++)
            {
                for (int y = gridPosition.y - 1; y <= gridPosition.y + 1; y++)
                {
                    if (x == gridPosition.x && y == gridPosition.y) continue; // Skip the center position
                    if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) continue; // Skip positions outside the grid

                    Vector2Int adjacentPosition = new Vector2Int(x, y);
                    if (!adjacentPositions.Contains(adjacentPosition))
                    {
                        adjacentPositions.Add(adjacentPosition);
                    }
                }
            }
        }

        return adjacentPositions;
    }
    
    public Vector2Int FindClosestGrid(Vector3 placedObjectPosition)
    {
        float closestDistance = Mathf.Infinity;
        Vector2Int closestGrid = Vector2Int.zero;

        foreach (Vector2 openTile in openTile)
        {
            Vector3 openTilePosition = grid.GetWorldPosition((int)openTile.x, (int)openTile.y);
            float distance = Vector3.Distance(placedObjectPosition, openTilePosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGrid = new Vector2Int((int)openTile.x, (int)openTile.y);
            }
        }

        return closestGrid;
    }



}
