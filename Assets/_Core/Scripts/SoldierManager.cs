using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SoldierManager : MonoBehaviour
{
    public static SoldierManager Instance { get; private set; }
    public GridBuildingSystem _gridBuildingSystem;
    [SerializeField] public Soldier _soldier1,_soldier2,_soldier3;
    [SerializeField] public Building barracks;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _gridBuildingSystem = GridBuildingSystem.Instance;
    }

    public void CreateSoldier(Soldier _soldier)
    {
        Vector2Int soldierPos = _gridBuildingSystem.FindClosestGrid(barracks.transform.position);
        Instantiate(_soldier.prefab, new Vector3(soldierPos.x * _gridBuildingSystem.cellSize + _gridBuildingSystem.cellSize/2 ,
            soldierPos.y * _gridBuildingSystem.cellSize + _gridBuildingSystem.cellSize/2 ,0), quaternion.identity);
        _gridBuildingSystem.openTile.Remove(soldierPos);
    }
    
}
