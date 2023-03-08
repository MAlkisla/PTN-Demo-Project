using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Barracks : Building
{
    public List<Spawnable> spawnables;
    public Transform spawnPoint;

    public bool spawnPointSet { get; set; }

    protected override void OnMouseDown()
    {
        if (GameManager.Instance.CurrentState == GameState.Idle && !InputManager.Instance.MouseOverUI)
        {
            base.OnMouseDown();
            EventManager.SelectedBuildingForSpawning.Invoke(this);
        }
    }
}