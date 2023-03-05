using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Core.Scripts
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }
        public event EventHandler<OnActiveBuildingTypeChangedEventArgs> OnActiveBuildingTypeChanged;

        public class OnActiveBuildingTypeChangedEventArgs : EventArgs
        { 
            public BuildingTypeSO ActiveBuildingType;
        }
        private Camera mainCamera;
        [SerializeField] private BuildingTypeListSO buildingTypeList;
        [SerializeField] private BuildingTypeSO activeBuildingType;
        Transform build = null;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            mainCamera = Camera.main;
            buildingTypeList = Resources.Load<BuildingTypeListSO>(nameof(BuildingTypeListSO));
            //buildingType = buildingTypeList.list[0];
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) &&!EventSystem.current.IsPointerOverGameObject())
            {
                if (activeBuildingType != null)
                {
                    build = Instantiate(activeBuildingType.prefab, UtilsClass.GetMouseWorldPosition(), Quaternion.identity);
                    float hitX = Mathf.Round(UtilsClass.GetMouseWorldPosition().x / 32) * 32;
                    float hitY = Mathf.Round(UtilsClass.GetMouseWorldPosition().y / 32) * 32;
                    build.transform.position = new Vector2(hitX, hitY);
                    activeBuildingType = null;
                }
            }
        }

        public void SetActiveBuildingType(BuildingTypeSO buildingType)
        {
            activeBuildingType = buildingType;
            OnActiveBuildingTypeChanged?.Invoke(this, new OnActiveBuildingTypeChangedEventArgs{ ActiveBuildingType = activeBuildingType});
        }

        public BuildingTypeSO GetActiveBuildingTypeButton()
        {
            return activeBuildingType;
        }
    }
}