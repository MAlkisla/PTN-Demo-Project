using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Core.Scripts
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }
        private Camera mainCamera;
        [SerializeField] private BuildingTypeListSO buildingTypeList;
        [SerializeField] private BuildingTypeSO buildingType;
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
                build = Instantiate(activeBuildingType.prefab, transform.position, transform.rotation);
                float hitX = Mathf.Round(GetMouseWorldPosition().x / 32) * 32;
                float hitY = Mathf.Round(GetMouseWorldPosition().y / 32) * 32;
                build.transform.position = new Vector2(hitX, hitY);
            }

            if (build != null)
            {
               
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            var mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            return mouseWorldPosition;
        }

        public void SetActiveBuildingType(BuildingTypeSO buildingTypeSo)
        {
            activeBuildingType = buildingTypeSo;
        }
    }
}