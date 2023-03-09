using System.Collections.Generic;
using _Core.Scripts.Managers;
using _Core.Scripts.Soldiers;
using UnityEngine;

namespace _Core.Scripts.Buildings
{
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
}