using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/BuildingType")]
public class BuildingTypeSO : ScriptableObject
{
   public string nameString;
   public Transform prefab;
   public Sprite sprite,selectedSprite;
   public Vector2 constructionSize;
   public int healthAmountMax;
   public string specText;
   public bool canProduceSoldier;
}
