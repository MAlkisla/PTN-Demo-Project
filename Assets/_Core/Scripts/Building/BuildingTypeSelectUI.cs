using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingTypeSelectUI : MonoBehaviour
{
    private Dictionary<PlacedObjectTypeSO, Transform> _btnTransformDictionary;
    public Transform btnTemplate;
    private Transform _activeBtn;
    private Image oldImg;
    private void Awake()
    {
        PlacedObjectTypeListSO placedObjectTypeList = Resources.Load<PlacedObjectTypeListSO>(nameof(PlacedObjectTypeListSO));
        _btnTransformDictionary = new Dictionary<PlacedObjectTypeSO, Transform>();
        int index = 0;
        foreach (PlacedObjectTypeSO placedObjectType in placedObjectTypeList.list)
        {
            Transform btnTransform = Instantiate(btnTemplate, transform);
            btnTransform.gameObject.SetActive(true);
            btnTransform.GetComponent<Image>().sprite = placedObjectType.sprite;
            btnTransform.GetComponent<Button>().onClick.AddListener(() =>
            {
                GridBuildingSystem.Instance.SetActiveBuildingType(placedObjectType);
            });
            _btnTransformDictionary[placedObjectType] = btnTransform;
            index++;
        }
    }

    private void Start()
    {
        GridBuildingSystem.Instance.OnActiveBuildingTypeChanged += GridBuildingSystem_OnActiveBuildingTypeChanged;
        UpdateActiveBuildingTypeButton();
    }

    private void GridBuildingSystem_OnActiveBuildingTypeChanged(object sender, GridBuildingSystem.OnActiveBuildingTypeChangedEventArgs e)
    {
        UpdateActiveBuildingTypeButton();
    }

    public void UpdateActiveBuildingTypeButton()
    {
        foreach (PlacedObjectTypeSO buildingType in _btnTransformDictionary.Keys)
        {
            Transform btnTransform = _btnTransformDictionary[buildingType];
            btnTransform.Find("Selected").gameObject.SetActive(false);
        }

        PlacedObjectTypeSO activeBuildingType = GridBuildingSystem.Instance.GetActiveBuildingTypeButton();
          if (activeBuildingType != null && _btnTransformDictionary.ContainsKey(activeBuildingType))
          {
              Transform selectedBtnTransform = _btnTransformDictionary[activeBuildingType].Find("Selected");
              if (selectedBtnTransform != null)
              {
                  _activeBtn = selectedBtnTransform;
                  selectedBtnTransform.gameObject.SetActive(true);
                  selectedBtnTransform.GetComponent<Image>().sprite = activeBuildingType.selectedSprite;
                  GridBuildingSystem.Instance.placedObjectTypeSO = activeBuildingType;
                  GridBuildingSystem.Instance.RefreshSelectedObjectType();
              }
          }
    }
}