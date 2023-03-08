using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Soldier")]
public class Soldier : ScriptableObject
{
    public string nameString;
    public Transform prefab;
}