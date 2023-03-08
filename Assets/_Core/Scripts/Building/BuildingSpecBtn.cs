using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BuildingSpecBtn : MonoBehaviour
{
   [SerializeField] private Building building;
   private InformationMenu _informationMenu;
   private void Awake()
   {
       building = gameObject.transform.parent.GetComponent<Building>();
       PlacedObjectTypeSO buildingType = building.GetComponent<BuildingTypeHolder>().buildingType;
      _informationMenu = InformationMenu.Instance;
      transform.GetChild(0).GetComponent<Button>().onClick.AddListener((() =>
      {
            _informationMenu.informationMenu.SetActive(true);
            _informationMenu.buildingImage.sprite = buildingType.sprite;
            _informationMenu.buildingName.text = buildingType.nameString;
            _informationMenu.buildingSpecText.text = buildingType.specText;
            _informationMenu.soldierSpawnObj.SetActive(buildingType.canProduceSoldier);

            SoldierManager.Instance.barracks = building;
      }));
   }
}
