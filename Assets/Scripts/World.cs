using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;

public class World : MonoBehaviour
{
    // Create our building multi dimensional array
    private Dictionary<string, Building> buildings = new Dictionary<string, Building>();
    private GameObject draggedBuilding;

    /**
     * Create instance of dragggable, each point where the mouse is at it will crease building then remove it on mouse move
     * @return void
     */
    public void UpdateDraggableBuilding(Building building, Vector3 position)
    {
        // If we have this we just update the object position
        if (draggedBuilding)
        {
            draggedBuilding.gameObject.transform.position = position;
        } 
        // If we don't, then we just create the instance model
        else
        {
            draggedBuilding = Instantiate(building, position, Quaternion.identity).gameObject;
        }
    }

    /**
     * Clear the building begin dragged
     * @return void
     */
    public void ClearDraggableBuilding()
    {
        Destroy(draggedBuilding, 0.2f);
    }

    /**
     * Adding new building on the map
     * @return void
     */
    public void AddBuilding(Building building, Vector3 position)
    {
        buildings.Add(string.Format("X{0}:Y{1}", (int)position.x, (int)position.z), Instantiate(building, position, Quaternion.identity));
    }

    /**
     * Check if the specific position has buildings already on it
     * @param Vector3 the position where we are going to add the building at
     * @return void
     */
    public bool HasBuildingAtPosition(Vector3 position)
    {
        return buildings.ContainsKey(string.Format("X{0}:Y{1}", (int)position.x, (int)position.z));
    }

    public Building GetBuildingByCoords(Vector3 position)
    {
        // Get the key
        string key = string.Format("X{0}:Y{1}", (int)position.x, (int)position.z);
        // Get the value from the list
        return DictionaryExtension.GetValueOrDefault(buildings, key);
    }

    public void RemoveBuilding(Vector3 position)
    {
        // Get the key
        string key = string.Format("X{0}:Y{1}", (int)position.x, (int)position.z);
        // Get the value from the list
        Building obj = DictionaryExtension.GetValueOrDefault(buildings, key);
        // Remove it from the list
        buildings.Remove(key);
        // Destroy it
        Destroy(obj.gameObject, 0.2f);
    }

    /**
     * Calculate the grid position where it will be adding this building
     * @param Vector3 the position where we are going to add the building at
     * @return void
     */
    public Vector3 CalculateGridPoisition(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), .5f, Mathf.Round(position.z));
    }
}
