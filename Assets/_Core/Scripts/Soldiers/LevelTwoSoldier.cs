using _Core.Scripts.Managers;
using _Core.Scripts.Utils;
using UnityEngine;

namespace _Core.Scripts.Soldiers
{
    public class LevelTwoSoldier : Soldier
    {
        private void Start()
        {
            var stats = GameManager.Instance.soldiersStats.GetStats(Constants.LevelTwoSoldierName);
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            objectName = stats.soldierName;
            healthPoints = stats.healthPoints;
            damagePoints = stats.damagePoints;
            spriteRenderer.sprite = stats.soldierSprite;
        }
    }
}