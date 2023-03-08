using UnityEngine;

public class LevelThreeSoldier : Soldier
{
    private void Start()
    {
        var stats = GameManager.Instance.soldiersStats.GetStats(Constants.LevelThreeSoldierName);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        objectName = stats.soldierName;
        healthPoints = stats.healthPoints;
        damagePoints = stats.damagePoints;
        spriteRenderer.sprite = stats.soldierSprite;
    }
}