using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Using internal
using GameConstants;
using GameUtils;

public class City : MonoBehaviour
{
    // Stats
    public int Cash { get; set; }
    public int Day { get; set; }
    public float CurrentPopulation { get; set; }
    public int CurrentJobs { get; set; }
    public float Food { get; set; }
    public float DeathPopulation { get; set; }
    // Max Stats
    public float MaxPopulation { get; set; }
    public int MaxJobs { get; set; }
    // Per turn calculations 
    public float PopulationPerTurn { get; set; }
    public float CashPerTurn { get; set; }
    public float FoodPerTurn { get; set; }
    public int JobsPerTurn { get; set; }

    // The buildings total list
    public Dictionary<BUILDING_TYPES, int> buildingCounts;

    [Header("Finished Food effects")]
    public float DeathRate;

    /**
     * On init handle the creation of the city
     * @return void
     */
    void Start()
    {
        // Init our building counts
        buildingCounts = new Dictionary<BUILDING_TYPES, int>();

        foreach (BUILDING_TYPES type in Enum.GetValues(typeof(BUILDING_TYPES)))
        {
            buildingCounts.Add(type, 0);
        }

        DeathPopulation = GameManager.instance.DefaultDeathPopulation;
        Cash = GameManager.instance.DefaultCash;
        Food = GameManager.instance.DefaultFood;
        CurrentJobs = GameManager.instance.DefaultCurrentJobs;
        MaxJobs = GameManager.instance.DefaultMaxJobs;
        CurrentPopulation = GameManager.instance.DefaultCurrentPopulation;
        MaxPopulation = GameManager.instance.DefaultMaxPopulation;

        EndTurn(false);
    }


    /**
     * Finish the current turn
     * @return void
     */
    public void EndTurn(bool withDayIncreament = true)
    {
        // Next day
        if (withDayIncreament)
        {
            Day++;
        }

        CalculateJobs();
        CalculateFood();
        CalculateCash();
        CalculateFood();
        CaclulatePopulation();

        // Update our UI
        GameUI.instance.UpdateCityData();
    }

    /**
     * Calculate the jobs
     * @return void
     */
    void CalculateJobs()
    {
        JobsPerTurn = DictionaryExtension.GetValueOrDefault(buildingCounts, BUILDING_TYPES.Factory, 0) * GameManager.instance.JobsPerFactory;
        // Get Total jobs ceiling of farms
        MaxJobs = JobsPerTurn;
        CurrentJobs = Mathf.Min((int)CurrentPopulation, MaxJobs);
    }

    /**
     * Calculate the cash
     * @return void
     */
    void CalculateCash()
    {
        // Cash per turn possible
        CashPerTurn = CurrentJobs * (int)GameManager.instance.JobSalary;
        // Calculate our total earnings possible
        Cash += (int)CashPerTurn;
    }

    /**
     * Calculate the food
     * @return void
     */
    void CalculateFood()
    {
        // Total food per turn
        FoodPerTurn = DictionaryExtension.GetValueOrDefault(buildingCounts, BUILDING_TYPES.Farm, 0) * GameManager.instance.FoodPerFarm;
        // Calculate our total amount of food
        Food += FoodPerTurn;
    }

    /**
     * Calculate the population
     * @return void
     */
    void CaclulatePopulation()
    {
        // Get the total sum for death people for this turn and population earned
        PopulationPerTurn = DictionaryExtension.GetValueOrDefault(buildingCounts, BUILDING_TYPES.House) * GameManager.instance.PopulationPerHouse;
        // Calculate our max population
        MaxPopulation = PopulationPerTurn;

        // If we have enough food to feed our entire population
        // If our current population is lower then max population
        if (Food >= CurrentPopulation && CurrentPopulation <= MaxPopulation)
        {
            // Remove total food taken from our total amount of people according to the rate we assigned
            // Example if current population is 50, and each per takes 1 quater of food (.25f), that measn 25 food is less.
            Food -= (CurrentPopulation * GameManager.instance.FoodPerPerson);
            // We set our current population according the following
            // 1. If we after sum our food times how much food per person to our current population
            // 2. Max population so we get if we reached full or not as integer
            CurrentPopulation = Mathf.Min(CurrentPopulation += Food * GameManager.instance.FoodPerPerson, MaxPopulation);
        } 
        // If food is lower then our current poulation, we start starve to dead
        else if (Food < CurrentPopulation)
        {
            // We must reduce our population
            // 1. Calculate current population - the food
            // 2. Apply a death rate possible to reduce people. Sort of if 100 people, and is applied by 0.5% then there 5 of people who would die.
            // Corona Virus? Later
            DeathPopulation = (CurrentPopulation - Food) * DeathRate;
            // Death effects both per turn and current poopulation * effects other resources
            CurrentPopulation -= DeathPopulation;
            PopulationPerTurn -= DeathPopulation;
        }
    }

    /**
     * Adding new building to buildings total
     * @param type The building type
     * @return void
     */
    public void AddBuildingCount(BUILDING_TYPES type)
    {
        buildingCounts[type] += 1;
    }

    /**
     * Reduce building from buildings total
     * @param type The building type
     * @return void
     */
    public void ReduceBuildingCount(BUILDING_TYPES type)
    {
        buildingCounts[type] -= 1;
    }

    /**
     * Reduce the cash of the user
     * @param int cost The total amount to reduce
     * @return void
     */
    public void ReduceCash(int cost)
    {
        Cash -= cost;

        GameUI.instance.UpdateCash();
    }

    /**
     * Increase the cash of the user
     * @param int cost The total amount to increase
     * @return void
     */
    public void IncreaseCash(int amount)
    {
        Cash += amount;

        GameUI.instance.UpdateCash();
    }

    /**
     * Can you sell current property?
     * @param BUILDING_TYPES type The type for building
     * @return void
     */
    public bool CanSell(BUILDING_TYPES type)
    {
        switch(type)
        {
            case BUILDING_TYPES.Factory:
                if (CurrentJobs >= MaxJobs)
                {
                    return false;
                }
                break;
            case BUILDING_TYPES.House:
                if (CurrentPopulation >= MaxPopulation)
                {
                    return false;
                }
                break;
        }

        return true;
    }
}
