using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameConstants;

public class BuildingHandler : MonoBehaviour
{
    public Building[] buildings;
    public City city;
    public World world; // The build handler is attached to the ground but it can be used for other matters. Therefore we will access the World as reference
    private Building selectedBuilding; // We need this to save the reference of the selected building, therefore we could get the information from

    private float lastCheckTime;

    // On start
    void Start()
    {
        selectedBuilding = GetComponent<Building>();
    }

    // Update is called once per frame
    void Update()
    {
        // Assign last time check at
        lastCheckTime += Time.deltaTime;

        // 1. If mouse click on left and some building was selected, it means we are about to add a building
        // 2. If Dragging on the ground using shift left key, then just spam the building all over the place
        if (Input.GetMouseButtonDown(0) && selectedBuilding || Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift) && selectedBuilding)
        {
            InteractBuilding(BUILDING_SELECT_TYPE.Add);
        }
        // If mouse click on right and some building was selected, it means we remove the building
        else if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift) && selectedBuilding || 
            Input.GetButtonDown("Cancel") && !Input.GetKey(KeyCode.LeftShift) && selectedBuilding)
        {
            OnRemoveBuilding();
        }
        // If on click of building 
        else if (Input.GetMouseButtonDown(1) && !selectedBuilding)
        {
            InteractBuilding(BUILDING_SELECT_TYPE.Remove);
        }
        // If selected some model from gui panel, it means the player trying to drag a building. So we do dragging effect
        // We do this every 100ms. This way is smooth when dragging and not lagging, but also doesn't run 60fps of updating all the time
        else if (selectedBuilding && lastCheckTime >= 0.1f)
        {
            lastCheckTime = lastCheckTime % 0.1f;
            OnMoveBuilding();
        } 
    }


    /**
     * On removing building interaction
     * @return void
     */
    void OnRemoveBuilding()
    {
        if (selectedBuilding)
        {
            GameManager.instance.HasSelectedBuilding = false;
            // Clear building instance
            selectedBuilding = GetComponent<Building>();
            // Clear it from the world too
            world.ClearDraggableBuilding();
        }
    }


    /**
     * On moving building interaction
     * @return void
     */
    void OnMoveBuilding()
    {
        // Be sure is still selected that has building
        GameManager.instance.HasSelectedBuilding = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // Reference same as C++ pointer. Hit* is gonna be used as our reference
        // With out means, the paramter that will be edited it will reflect outside. In this case 'RaycastHit hit' will have the changes done inside Raycast
        // Same like in C++ which is well known as pointer
        // Raycast(ray, &hit)
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 gridPosition = world.CalculateGridPoisition(hit.point);
            world.UpdateDraggableBuilding(selectedBuilding, gridPosition);
        }
    }

    /**
     * Interact with the world according the changes received 
     * @return void
     */
    void InteractBuilding(BUILDING_SELECT_TYPE type)
    {
        // Be sure is still selected that has building
        GameManager.instance.HasSelectedBuilding = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // Reference same as C++ pointer. Hit* is gonna be used as our reference

        // With out means, the paramter that will be edited it will reflect outside. In this case 'RaycastHit hit' will have the changes done inside Raycast
        // Same like in C++ which is well known as pointer
        // Raycast(ray, &hit)
        if (Physics.Raycast(ray, out hit))
        {
            // Get our grid position by getting it from world
            Vector3 gridPosition = world.CalculateGridPoisition(hit.point);

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                bool hasBuilding = world.HasBuildingAtPosition(gridPosition);

                // Does this point has a building? Remove
                if (hasBuilding && type == BUILDING_SELECT_TYPE.Remove && !selectedBuilding)
                {
                    Building foundBuilding = world.GetBuildingByCoords(gridPosition);

                    if (city.CanSell(foundBuilding.type))
                    {

                        city.IncreaseCash(foundBuilding.GetSellPrice());
                        city.ReduceBuildingCount(foundBuilding.type);
                        world.RemoveBuilding(gridPosition);

                        GameUI.instance.AddNotification(NOTIFICATION_TYPES.SUCCESS, string.Format("You sold a {0} for ${1}", foundBuilding.buildingName, foundBuilding.GetSellPrice()));
                    } else
                    {
                        GameUI.instance.AddNotification(NOTIFICATION_TYPES.ERROR, string.Format("You cannot sell {0}, one or more of your resources will exceed the upper limit.", foundBuilding.buildingName));
                    }
                }
                // Doesnt have? Create
                else if (!hasBuilding && type == BUILDING_SELECT_TYPE.Add && selectedBuilding)
                {
                    if (city.Cash >= selectedBuilding.cost)
                    {
                        city.ReduceCash(selectedBuilding.cost);
                        city.AddBuildingCount(selectedBuilding.type);
                        world.AddBuilding(selectedBuilding, gridPosition);
                    }
                    else
                    {
                        GameUI.instance.AddNotification(NOTIFICATION_TYPES.ERROR, "Not enough cash");
                    }
                }
            }
        }
    }

    /**
     * Enable the builder
     * @param id The building id
     * @return void
     */
    public void EnableBuildier(int id)
    {
        GameManager.instance.HasSelectedBuilding = true;
        // Remove any previous draggable object
        OnRemoveBuilding();
        // Select the new building
        selectedBuilding = buildings[id];
    }
}
