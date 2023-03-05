using System;
using System.Collections;
using System.Collections.Generic;
using _Core.Scripts;
using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
    public static BuildingGhost Instance { get; private set; }
    public GameObject _spriteGameObject;
  
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        _spriteGameObject = transform.Find("sprite").gameObject;
        Hide();
    }

    private void Start()
    {
        BuildingManager.Instance.OnActiveBuildingTypeChanged += BuildingManager_OnActiveBuildingTypeChanged;
    }

    private void BuildingManager_OnActiveBuildingTypeChanged(object sender, BuildingManager.OnActiveBuildingTypeChangedEventArgs e)
    {
        if ( e.ActiveBuildingType == null)
        {
            Hide();
        }
        else
        {
            Show(e.ActiveBuildingType.sprite);
        }
       
    }

    private void Update()
    {
        transform.position = UtilsClass.GetMouseWorldPosition();
    }

    private void Show(Sprite ghostSprite)
    {
        _spriteGameObject.SetActive(true);
        _spriteGameObject.GetComponent<SpriteRenderer>().sprite = ghostSprite;
    }
    private void Hide()
    {
        _spriteGameObject.SetActive(false);
    }
   
}
