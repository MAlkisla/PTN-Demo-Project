using System;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    #region Singleton

    private static InputManager m_Instance;

    public static InputManager Instance => m_Instance;

    private void Awake()
    {
        m_Instance = this;
    }

    #endregion

    private int _buildingXSize;
    private int _buildingYSize;

    [SerializeField] private SpriteRenderer _buildingSpriteRenderer;
    [SerializeField] private SpriteRenderer _availableBGSpriteRenderer;

    private float _cellOffset;

    private BuildingStats _selectedBuildingStats;

    private bool _dragging;
    private bool _startedDragging;
    private Vector3 _camDragDiff;
    private Vector3 _camOrigin;

    public bool MouseOverUI => UIManager.Instance.MouseOverUI;

    private bool _spawnerBuildingSelected;
    private Transform _spawnerBuildingsSpawnPoint;

    private bool _soldierSelected;
    private Soldier _selectedSoldier;

    private void Start()
    {
        _cellOffset = GridManager.Instance.cellSize / 2f;
        EventManager.SelectedBuildingForProduction.AddListener(SelectBuilding);
        EventManager.SelectedBuildingForSpawning.AddListener(SpawnerBuildingSelected);
        EventManager.SelectedSoldierForInformation.AddListener(SoldierSelected);
        EventManager.ToBeAttackedObjectSelected.AddListener(AttackWithSoldier);
    }

    private void Update()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (MouseOverUI)
        {
            if (Input.GetMouseButtonUp(0))
            {
                _dragging = false;
                _startedDragging = false;
            }
            return;
        }

        #region CameraControl

        if (Input.GetMouseButtonDown(0))
        {
            if (!_startedDragging)
            {
                _startedDragging = true;
                _camOrigin = mouseWorldPos;
            }
        }

        if (Input.GetMouseButton(0))
        {
            _camDragDiff = mouseWorldPos - Camera.main.transform.position;
        }

        if (_startedDragging)
        {
            if (!_dragging && Camera.main.transform.position != _camOrigin - _camDragDiff) _dragging = true;
            if (_dragging) Camera.main.transform.position = _camOrigin - _camDragDiff;

            var cameraPos = Camera.main.transform.position;
            cameraPos.x = Mathf.Clamp(cameraPos.x, 0, GridManager.Instance.width);
            cameraPos.y = Mathf.Clamp(cameraPos.y, 0, GridManager.Instance.Height);
            Camera.main.transform.position = cameraPos;
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize+1f, 5f, 10f);
            
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize-1f, 5f, 10f);
        }

        //mouseUp check after building so no accidentally buildings created

        #endregion

        #region BuildingInputHandle

        if (GameManager.Instance.CurrentState == GameState.Building)
        {
            var intXPos = Mathf.RoundToInt(mouseWorldPos.x);
            var intYPos = Mathf.RoundToInt(mouseWorldPos.y);

            HighlightTiles(out var isTilesEmpty, intXPos, intYPos);


            if (Input.GetMouseButtonUp(0) && isTilesEmpty && !_dragging)
            {
                BuildBuilding(intXPos, intYPos);
            }

            if (Input.GetMouseButtonDown(1))
            {
                DeselectBuilding();
            }
        }

        #endregion
        
        #region IdleInputHandle

        if (GameManager.Instance.CurrentState == GameState.Idle)
        {
            if (Input.GetMouseButtonUp(0) && !_dragging)
            {
                var intXPos = Mathf.RoundToInt(mouseWorldPos.x);
                var intYPos = Mathf.RoundToInt(mouseWorldPos.y);

                var tile = GridManager.Instance.GetTile(intXPos, intYPos); 
                if (tile && tile.tileEmpty)
                {
                    UIManager.Instance.CloseInformationTab();
                    if (_spawnerBuildingSelected)
                    {
                        SpawnerBuildingDeselect();
                    }

                    if (_soldierSelected)
                    {
                        SoldierDeselect();
                    }
                }
            }

            if (_spawnerBuildingSelected && Input.GetMouseButtonDown(1))
            {
                var intXPos = Mathf.RoundToInt(mouseWorldPos.x);
                var intYPos = Mathf.RoundToInt(mouseWorldPos.y);

                var tile = GridManager.Instance.GetTile(intXPos, intYPos); 
                if (tile && tile.tileEmpty)
                {
                    _spawnerBuildingsSpawnPoint.position = tile.transform.position;
                    _spawnerBuildingsSpawnPoint.gameObject.SetActive(true);
                }
            }

            if (_soldierSelected && Input.GetMouseButtonDown(1))
            {
                var intXPos = Mathf.RoundToInt(mouseWorldPos.x);
                var intYPos = Mathf.RoundToInt(mouseWorldPos.y);

                var tile = GridManager.Instance.GetTile(intXPos, intYPos);
                if (tile.tileEmpty)
                {
                    _selectedSoldier.StopAttacking();
                    _selectedSoldier.Move(tile);
                }
            }
        }
        
        #endregion

        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
            _startedDragging = false;
        }
    }

    public void SelectBuilding(string buildingName)
    {
        _selectedBuildingStats = GameManager.Instance.buildingsStats.GetStats(buildingName);
        _buildingXSize = _selectedBuildingStats.buildingXSize;
        _buildingYSize = _selectedBuildingStats.buildingYSize;
        _buildingSpriteRenderer.transform.localScale = new Vector3(_buildingXSize, _buildingYSize, 1);
        _buildingSpriteRenderer.sprite = _selectedBuildingStats.buildingSprite;
        SpawnerBuildingDeselect();
        SoldierDeselect();
    }

    public void DeselectBuilding()
    {
        _buildingSpriteRenderer.sprite = null;
        _availableBGSpriteRenderer.color = Color.clear;
        _buildingSpriteRenderer.gameObject.SetActive(false);
        GameManager.Instance.CurrentState = GameState.Idle;
    }

    public void BuildBuilding(int intXPos, int intYPos)
    {
        // using factory pattern here is unnecessary because of the scriptable system created, but here you go
        // its simplified version of factory pattern where factory is static and objects are still
        //on their scriptable stats object but in correct usage of factory pattern, you can store your
        //factory objects on factory. 
        var gridObject = GridObjectFactory.GetGridObject(_selectedBuildingStats.buildingName);
        Instantiate(gridObject, new Vector2(intXPos, intYPos), Quaternion.identity);
        _buildingSpriteRenderer.sprite = null;
        _availableBGSpriteRenderer.color = Color.clear;
        _buildingSpriteRenderer.gameObject.SetActive(false);
        
        for (int x = 0; x < _buildingXSize; x++)
        {
            for (int y = 0; y < _buildingYSize; y++)
            {
                var tile = GridManager.Instance.GetTile(intXPos + x, intYPos + y);
                tile.SetEmpty(false);
            }
        }
        
        EventManager.ProductionBuildingCompleted.Invoke();
    }

    private void HighlightTiles(out bool isTilesEmpty, int intXPos, int intYPos)
    {
        if(!_buildingSpriteRenderer.gameObject.activeSelf) _buildingSpriteRenderer.gameObject.SetActive(true);
            isTilesEmpty = true;
        for (int x = 0; x < _buildingXSize; x++)
        {
            for (int y = 0; y < _buildingYSize; y++)
            {
                var tile = GridManager.Instance.GetTile(intXPos + x, intYPos + y);
                if (!tile)
                {
                    isTilesEmpty = false;
                    break;
                }
                if (tile.tileEmpty) continue;
                isTilesEmpty = false;
                break;
            }

            if (!isTilesEmpty) break;
        }

        _buildingSpriteRenderer.transform.position = new Vector2(intXPos + _buildingXSize / 2f - _cellOffset,
            intYPos + _buildingYSize / 2f - _cellOffset);
        var availableCol = isTilesEmpty ? Color.green : Color.red;
        availableCol.a = 0.35f;
        _availableBGSpriteRenderer.color = availableCol;
    }

    public void SpawnerBuildingSelected(Barracks selectedBarracks)
    {
        _spawnerBuildingsSpawnPoint = selectedBarracks.spawnPoint;
        _spawnerBuildingSelected = true;
        SoldierDeselect();
    }

    public void SoldierSelected(Soldier soldier)
    {
        _selectedSoldier = soldier;
        _soldierSelected = true;
        SpawnerBuildingDeselect();
    }

    private void SoldierDeselect()
    {
        if (_selectedSoldier != null)
        {
            _soldierSelected = false;
            _selectedSoldier = null;
        }
    }

    private void SpawnerBuildingDeselect()
    {
        if (_spawnerBuildingsSpawnPoint != null)
        {
            _spawnerBuildingSelected = false;
            _spawnerBuildingsSpawnPoint.gameObject.SetActive(false);
            _spawnerBuildingsSpawnPoint = null;
        }
    }

    public void AttackWithSoldier(Vector3 toBeAttackedUnitPos, IHealth toBeAttackedUnit)
    {
        if (_soldierSelected && !ReferenceEquals(toBeAttackedUnit, _selectedSoldier))
        {
            _selectedSoldier.Attack(toBeAttackedUnitPos, toBeAttackedUnit);
        }
    }
}