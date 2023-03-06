using System;
using System.Collections;
using System.Collections.Generic;
using _Core.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class BuildingTypeSelectUI : MonoBehaviour
{
    private Dictionary<BuildingTypeSO, Transform> _btnTransformDictionary;
    public Transform btnTemplate;
    private Transform _activeBtn;
    private Image oldImg;
    private void Awake()
    {
        BuildingTypeListSO buildingTypeList = Resources.Load<BuildingTypeListSO>(nameof(BuildingTypeListSO));
        _btnTransformDictionary = new Dictionary<BuildingTypeSO, Transform>();
        int index = 0;
        foreach (BuildingTypeSO buildingType in buildingTypeList.list)
        {
            Transform btnTransform = Instantiate(btnTemplate, transform);
            btnTransform.gameObject.SetActive(true);
            btnTransform.GetComponent<Image>().sprite = buildingType.sprite;
            btnTransform.GetComponent<Button>().onClick.AddListener(() =>
            {
                BuildingManager.Instance.SetActiveBuildingType(buildingType);
            });
            _btnTransformDictionary[buildingType] = btnTransform;
            index++;
        }
    }

    private void Start()
    {
        BuildingManager.Instance.OnActiveBuildingTypeChanged += BuildingManager_OnActiveBuildingTypeChanged;
        UpdateActiveBuildingTypeButton();
    }

    private void BuildingManager_OnActiveBuildingTypeChanged(object sender, BuildingManager.OnActiveBuildingTypeChangedEventArgs e)
    {
        UpdateActiveBuildingTypeButton();
    }

    public void UpdateActiveBuildingTypeButton()
    {
        foreach (BuildingTypeSO buildingType in _btnTransformDictionary.Keys)
        {
            Transform btnTransform = _btnTransformDictionary[buildingType];
            btnTransform.Find("Selected").gameObject.SetActive(false);
        }

        BuildingTypeSO activeBuildingType = BuildingManager.Instance.GetActiveBuildingTypeButton();
        if (activeBuildingType != null && _btnTransformDictionary.ContainsKey(activeBuildingType))
        {
            Transform selectedBtnTransform = _btnTransformDictionary[activeBuildingType].Find("Selected");
            if (selectedBtnTransform != null)
            {
                _activeBtn = selectedBtnTransform;
                selectedBtnTransform.gameObject.SetActive(true);
                selectedBtnTransform.GetComponent<Image>().sprite = activeBuildingType.selectedSprite;
            }
        }
    }
}