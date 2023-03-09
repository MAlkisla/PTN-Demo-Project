using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    public List<Spawnable> spawnables;
    public Transform spawnPoint;

    protected override void OnMouseDown()
    {
        if (GameManager.Instance.CurrentState == GameState.Idle && !InputManager.Instance.MouseOverUI)
        {
            base.OnMouseDown();
            EventManager.SelectedBuildingForSpawning.Invoke(this);
        }
    }
}