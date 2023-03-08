using UnityEngine;

public class LevelOneSoldier : Soldier
{
    private void Start()
    {
        var stats = GameManager.Instance.soldiersStats.GetStats(Constants.LevelOneSoldierName);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        objectName = stats.soldierName;
        healthPoints = stats.healthPoints;
        damagePoints = stats.damagePoints;
        spriteRenderer.sprite = stats.soldierSprite;
    }
}
