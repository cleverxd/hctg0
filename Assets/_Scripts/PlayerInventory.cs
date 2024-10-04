using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerInventory : MonoBehaviour
{
    public int capacity = 10;                  // Maximum inventory capacity
    public float stackHeight = 0.15f;
    public List<Resource.ResourceType> storedItems;      // List of stored items
    public Transform backpackTransform;        // The transform representing the backpack for visual items
    public GameObject woodPrefab, stonePrefab, ironPrefab;  // Prefabs for visual representation

    private Dictionary<int, GameObject> itemVisuals;      // Dictionary to track item GameObjects by index

    // Define a list or array to hold the resource prefabs
    private GameObject[] resourcePrefabs;

    void Start()
    {
        storedItems = new List<Resource.ResourceType>();
        itemVisuals = new Dictionary<int, GameObject>();

        // Initialize the resource prefabs array
        resourcePrefabs = new GameObject[]
        {
            null,       // None (index 0)
            woodPrefab, // Wood (index 1)
            stonePrefab,// Stone (index 2)
            ironPrefab  // Iron (index 3)
        };
    }

    // Method to check if there is space in the inventory
    public bool HasSpace()
    {
        return storedItems.Count < capacity;
    }

    // Method to check if a certain resource exists in the inventory
    public Resource.ResourceType HasResource(Resource.ResourceType resource)
    {
        return storedItems.Contains(resource) ? resource : Resource.ResourceType.None;
    }

    // Method to add a resource to the inventory
    public void AddResource(Resource.ResourceType resource)
    {
        if (HasSpace())
        {
            storedItems.Add(resource);
            AddVisualRepresentation(resource);  // Add the visual representation of the item in the backpack
        }
    }

    // Method to remove a resource from the inventory
    public void RemoveResource(Resource.ResourceType resource)
    {
        if (storedItems.Contains(resource))
        {
            int index = storedItems.IndexOf(resource);
            storedItems.RemoveAt(index);
            RemoveVisualRepresentation(index);  // Remove the corresponding visual item from the backpack
        }
    }

    private void AddVisualRepresentation(Resource.ResourceType resource)
    {
        Vector3 stackPosition = backpackTransform.position + Vector3.up * (storedItems.Count * stackHeight);
        GameObject resourcePrefab = GetResourcePrefab(resource);  // Retrieve the correct prefab based on the resource type

        if (resourcePrefab != null)
        {
            GameObject visualItem = Instantiate(resourcePrefab, stackPosition, Quaternion.identity, backpackTransform);
            itemVisuals[storedItems.Count - 1] = visualItem;  // Store the visual item in the dictionary for tracking
        }
    }

    // Method to remove the visual representation of a resource from the backpack
    private void RemoveVisualRepresentation(int index)
    {
        if (itemVisuals.ContainsKey(index))
        {
            Destroy(itemVisuals[index]);  // Destroy the visual item GameObject
            itemVisuals.Remove(index);

            // Shift the remaining items down to fill the gap
            for (int i = index; i < storedItems.Count; i++)
            {
                if (itemVisuals.ContainsKey(i + 1))
                {
                    itemVisuals[i] = itemVisuals[i + 1];
                    itemVisuals.Remove(i + 1);
                    itemVisuals[i].transform.position = backpackTransform.position + Vector3.up * (i * stackHeight);
                }
            }
        }
    }
    public IEnumerator AnimateResourceLoadToBackpack(GameObject resource)
    {
        Vector3 startPosition = resource.transform.position; // Start from the resource's current position
        Vector3 targetPosition = backpackTransform.position + Vector3.up * (storedItems.Count * stackHeight); // Target position in the backpack
        float duration = 0.2f; // Duration of the animation
        float elapsed = 0f;

        while (elapsed < duration)
        {
            resource.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the resource ends exactly at the target position
        Destroy(resource);
    }


    // Helper method to get the correct prefab for the given resource type
    private GameObject GetResourcePrefab(Resource.ResourceType resource)
    {
        return resourcePrefabs[(int)resource]; // Get the prefab using the resource type as index
    }

    public GameObject GetPrefab(Resource.ResourceType resourceType)
    {
        // Return the prefab based on the resource type using the resourcePrefabs array
        return resourcePrefabs[(int)resourceType];
    }
}
