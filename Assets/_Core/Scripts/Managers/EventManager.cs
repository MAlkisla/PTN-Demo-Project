using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent<string> SelectedBuildingForProduction = new UnityEvent<string>();
    public static UnityEvent ProductionBuildingCompleted = new UnityEvent();
    public static UnityEvent<string> SelectedBuildingForInformation = new UnityEvent<string>();
    public static UnityEvent<Soldier> SelectedSoldierForInformation = new UnityEvent<Soldier>();
    public static UnityEvent<Barracks> SelectedBuildingForSpawning = new UnityEvent<Barracks>();
    public static UnityEvent<Vector3, IHealth> ToBeAttackedObjectSelected = new UnityEvent<Vector3, IHealth>();
}