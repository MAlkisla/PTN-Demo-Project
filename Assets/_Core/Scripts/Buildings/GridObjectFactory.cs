using UnityEngine;

public static class GridObjectFactory
{
    public static GridObject GetGridObject(string buildingName)
    {
        switch (buildingName)
        {
            case Constants.BarracksBuildingName:
                Debug.Log("Barracks selected");
                return GameManager.Instance.buildingsStats.GetStats(Constants.BarracksBuildingName).buildingPrefab;
            case Constants.TownCenterBuildingName:
                Debug.Log("TownCenter selected");
                return GameManager.Instance.buildingsStats.GetStats(Constants.TownCenterBuildingName).buildingPrefab;
            case Constants.PowerPlantBuildingName:
                Debug.Log("PowerPlant selected");
                return GameManager.Instance.buildingsStats.GetStats(Constants.PowerPlantBuildingName).buildingPrefab;
            case Constants.LevelOneSoldierName:
                Debug.Log("LevelOneSoldier selected");
                return GameManager.Instance.soldiersStats.GetStats(Constants.LevelOneSoldierName).soldierPrefab;
            case Constants.LevelTwoSoldierName:
                Debug.Log("LevelTwoSoldier selected");
                return GameManager.Instance.soldiersStats.GetStats(Constants.LevelTwoSoldierName).soldierPrefab;
            case Constants.LevelThreeSoldierName:
                Debug.Log("LevelThreeSoldier selected");
                return GameManager.Instance.soldiersStats.GetStats(Constants.LevelThreeSoldierName).soldierPrefab;
            default:
                return null;
        }
    }
}