using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    #region Singleton

    private static UIManager _instance;

    public static UIManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    #endregion

    [SerializeField] private InformationTab _informationTab;
    [SerializeField] private SpawnableTab _spawnableTab;
    
    public bool MouseOverUI => IsPointerOverUI();

    private void Start()
    {
        EventManager.SelectedBuildingForInformation.AddListener(OpenBuildingInformationTab);
        EventManager.SelectedSoldierForInformation.AddListener(OpenSoldierInformationTab);
        EventManager.SelectedBuildingForSpawning.AddListener(OpenBuildingSpawnTab);
    }

    public void SelectBuilding(string buildingName)
    {
        EventManager.SelectedBuildingForProduction.Invoke(buildingName);
        GameManager.Instance.StartBuilding(buildingName);
    }

    private void OpenBuildingInformationTab(string buildingName)
    {
        CloseInformationTab();
        var stats = GameManager.Instance.buildingsStats.GetStats(buildingName);
        _informationTab.SetObjectInformation(stats.buildingName, stats.buildingSprite, stats.buildingInfo);
    }

    private void OpenBuildingSpawnTab(Barracks spawningBarracks)
    {
        var spawnables = spawningBarracks.spawnables;
        _spawnableTab.SetSpawnableInformation(spawnables, spawningBarracks);
    }
    
    private void OpenSoldierInformationTab(Soldier soldier)
    {
        CloseInformationTab();
        var stats = GameManager.Instance.soldiersStats.GetStats(soldier.objectName);
        _informationTab.SetObjectInformation(stats.soldierName, stats.soldierSprite, stats.soldierInfo);
    }

    public void CloseInformationTab()
    {
        _informationTab.CloseInformationTab();
        _spawnableTab.CloseSpawnableTab();
    }
    
    private bool IsPointerOverUI() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return true;
        } 
        PointerEventData pe = new PointerEventData(EventSystem.current);
        pe.position = Input.mousePosition;
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pe, hits);
        return hits.Count > 0;
    }
}