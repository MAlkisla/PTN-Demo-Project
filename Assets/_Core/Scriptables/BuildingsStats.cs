using System.Collections.Generic;
using _Core.Scripts.Buildings;
using UnityEngine;

namespace _Core.Scriptables
{
    [CreateAssetMenu(fileName = "BuildingStats", menuName = "ScriptableObjects/BuildingStats", order = 1)]
    public class BuildingsStats : ScriptableObject
    {
        [SerializeField] private List<BuildingStats> Buildings;

        public BuildingStats GetStats(object buildingName)
        {
            foreach (var buildingStats in Buildings)
            {
                if (buildingStats.buildingName.Equals(buildingName))
                {
                    return buildingStats;
                }
            }

            return null;
        }
    }
}