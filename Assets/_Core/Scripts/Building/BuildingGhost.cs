using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    public static BuildingGhost Instance { get; private set; }
    public Transform visual;
    private PlacedObjectTypeSO placedObjectTypeSO;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start() {
        RefreshVisual();
        GridBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
        RefreshVisual();
    }

    private void LateUpdate() {
        Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

    }

    private void RefreshVisual() {
        if (visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }

        PlacedObjectTypeSO placedObjectTypeSO = GridBuildingSystem.Instance.GetPlacedObjectTypeSO();

        if (placedObjectTypeSO != null) {
            visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            // visual.localPosition = new Vector3((placedObjectTypeSO.width * GridBuildingSystem2D.Instance.cellSize)/2,(placedObjectTypeSO.height * GridBuildingSystem2D.Instance.cellSize)/2);
            // visual.localScale =new Vector3((placedObjectTypeSO.width * GridBuildingSystem2D.Instance.cellSize),(placedObjectTypeSO.height * GridBuildingSystem2D.Instance.cellSize));
        }
    }

}
