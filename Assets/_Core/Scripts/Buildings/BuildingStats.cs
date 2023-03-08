using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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