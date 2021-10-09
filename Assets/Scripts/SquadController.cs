using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int unitsCount = 10;

    [Header("References")]
    [SerializeField] private GameObject unitPrefab;

    private List<UnitController> _unitsList = new List<UnitController>();

    private void Start()
    {
        CreateFirstUnits();
    }

    private void CreateFirstUnits()
    {
        for (int i = 0; i < unitsCount; i++)
        {
            UnitController unit = Instantiate(unitPrefab);
        }
    }
}
