using System;
using UnityEngine;

namespace _Core.Scripts.Soldiers
{
    [Serializable]
    public class SoldierStats
    {
        public string soldierName;
        public int healthPoints;
        public int damagePoints;
        public float attackRate;
        public string soldierInfo;
        public Sprite soldierSprite;
        public Soldier soldierPrefab;
    }
}