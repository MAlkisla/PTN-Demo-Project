using System;
using UnityEngine;

[Serializable]
public class BuildingStats
{
    public string buildingName;
    public int healthPoints;
    public int buildingXSize;
    public int buildingYSize;
    [TextArea] public string buildingInfo;
    public Sprite buildingSprite;
    public Building buildingPrefab;
}