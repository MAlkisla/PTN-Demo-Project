using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectTypeList")]
public class PlacedObjectTypeListSO : ScriptableObject
{
   public List<PlacedObjectTypeSO> list;
}
