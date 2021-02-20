using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameConstants;

public class Building : MonoBehaviour
{
    [Header("Required")]
    public int id;
    public int cost;
    public BUILDING_TYPES type;

    [Header("Optional")]
    public string buildingName;

    private void Awake()
    {
        if (buildingName == null)
        {
            buildingName = gameObject.name;
        }
    }

    public int GetSellPrice()
    {
        return cost / 2;
    }
}
