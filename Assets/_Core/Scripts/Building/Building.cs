using System;
using System.Collections;
using System.Collections.Generic;
using _Core.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    private BuildingTypeSO _buildingTypeSo;
    private HealthSystem _healthSystem;
    private Transform _buildingSpecBtn;
    private void Awake()
    {
        _buildingTypeSo = GetComponent<BuildingTypeHolder>().buildingType;
        _buildingSpecBtn = transform.Find("SpecButton");
        HideBuildingBtn();
    }

    void Start()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _healthSystem.SetHealthAmountMax(_buildingTypeSo.healthAmountMax,true);
        _healthSystem.OnDied += HealthSystem_OnDied;
        BuildingManager.Instance.ShowSpecBtn += BuildingManager_ShowSpecBtn;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _healthSystem.Damage(10);
        }
    }

    private void HealthSystem_OnDied(object sender, EventArgs e)
    {
       Destroy(gameObject);
    }
    private void BuildingManager_ShowSpecBtn(object sender, EventArgs e)
    {
        ShowBuildingBtn();
    }
    private void ShowBuildingBtn()
    {
        if (_buildingSpecBtn != null)
        {
            _buildingSpecBtn.gameObject.GetComponent<Button>().interactable = true;
        }
    }
    private void HideBuildingBtn()
    {
        if (_buildingSpecBtn !=null)
        {
            _buildingSpecBtn.gameObject.GetComponent<Button>().interactable=false;
        }
    }
}
