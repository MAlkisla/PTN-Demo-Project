using _Core.Scripts.Managers;
using _Core.Scripts.Utils;
using UnityEngine;

namespace _Core.Scripts.Grid
{
    public static class GridObjectFactory
    {
        public static GridObject GetGridObject(string buildingName)
        {
            switch (buildingName)
            {
                case Constants.BarracksBuildingName:
                    Debug.Log("Barracks selected");
                    return GameManager.Instance.buildingsStats.GetStats(Constants.BarracksBuildingName).buildingPrefab;
                case Constants.CommandBaseBuildingName:
                    Debug.Log("CommandBase selected");
                    return GameManager.Instance.buildingsStats.GetStats(Constants.CommandBaseBuildingName).buildingPrefab;
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
}