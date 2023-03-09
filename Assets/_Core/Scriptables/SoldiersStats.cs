using System.Collections.Generic;
using _Core.Scripts.Soldiers;
using UnityEngine;

namespace _Core.Scriptables
{
    [CreateAssetMenu(fileName = "SoldierStats", menuName = "ScriptableObjects/SoldierStats", order = 1)]
    public class SoldiersStats : ScriptableObject
    {
        [SerializeField] private List<SoldierStats> Soldiers;

        public SoldierStats GetStats(object soldierName)
        {
            foreach (var soldierStats in Soldiers)
            {
                if (soldierStats.soldierName.Equals(soldierName))
                {
                    return soldierStats;
                }
            }

            return null;
        }
    }
}