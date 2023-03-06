using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationMenu : MonoBehaviour
{
    public static InformationMenu Instance { get; private set; }
    public GameObject informationMenu;
    public Image buildingImage;
    public TextMeshProUGUI buildingName, buildingSpecText;
    public GameObject soldierSpawnObj;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        informationMenu = transform.GetChild(0).gameObject;
    }
}
