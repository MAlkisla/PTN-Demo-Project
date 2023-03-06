using System;
using System.Collections;
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
        
        private SpriteRenderer ghostSprite;
        private float flashDuration = 0.25f;
        private Color originalColor;
        private Coroutine currentCoroutine;
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
                if (activeBuildingType != null && CanSpawnBuilding(activeBuildingType,UtilsClass.GetMouseWorldPosition()))
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

        private bool CanSpawnBuilding(BuildingTypeSO buildingType, Vector3 position)
        {
            BoxCollider2D boxCollider2D = buildingType.prefab.GetComponent<BoxCollider2D>();
            
            Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(position + (Vector3)boxCollider2D.offset, boxCollider2D.size, 0);
            ghostSprite = BuildingGhost.Instance._spriteGameObject.GetComponent<SpriteRenderer>();
            originalColor = ghostSprite.color;
            bool isAreaColor = collider2DArray.Length == 0;
            if (!isAreaColor)
            {
                Flash();
                return false;
            }
            
            //collider2DArray = Physics2D.OverlapCircleAll(position, buildingType.minConstructionXY);
            Vector2 buildingSize = buildingType.prefab.GetComponent<Transform>().localScale;
            collider2DArray= Physics2D.OverlapBoxAll(position ,buildingSize*(Vector3)buildingType.constructionSize / 8 , 0);
            
            foreach (Collider2D collider2D in collider2DArray)
            {
                BuildingTypeHolder buildingTypeHolder = collider2D.GetComponent<BuildingTypeHolder>();
                if (buildingType != null)
                {
                    if (buildingTypeHolder.buildingType == buildingType)
                    {
                        return false;
                    }
                }
            }
            return collider2DArray.Length == 0;
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
                ghostSprite.color =Color.red ;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            ghostSprite.color = originalColor;
        }
    }
}