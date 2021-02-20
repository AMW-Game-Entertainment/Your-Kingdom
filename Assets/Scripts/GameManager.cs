using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool HasSelectedBuilding { get; set; }

    [Header("Default Resources")]
    public float DefaultMaxPopulation;
    public float DefaultCurrentPopulation;
    public float DefaultFood;
    public int DefaultCurrentJobs;
    public int DefaultMaxJobs;
    public float DefaultDeathPopulation;
    public int DefaultCash;

    [Header("Per Turn Cost")]
    public int PopulationPerHouse;
    public float FoodPerFarm;
    public int JobsPerFactory;
    public float JobSalary;
    public float FoodPerPerson;

    public bool isMenuActive;

    // Set the current instance as singleton
    public static GameManager instance;


    private void Awake()
    {
        instance = this;
    }
}
