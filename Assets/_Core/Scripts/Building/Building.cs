using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private BuildingTypeSO _buildingTypeSo;
    private HealthSystem _healthSystem;
    void Start()
    {
        _buildingTypeSo = GetComponent<BuildingTypeHolder>().buildingType;
        _healthSystem = GetComponent<HealthSystem>();
        _healthSystem.SetHealthAmountMax(_buildingTypeSo.healthAmountMax,true);
        _healthSystem.OnDied += HealthSystem_OnDied;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _healthSystem.Damage(10);
        }
    }

    private void HealthSystem_OnDied(object sender, EventArgs e)
    {
       Destroy(gameObject);
    }
}
