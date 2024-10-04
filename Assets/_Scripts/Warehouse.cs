using System.Collections;
using UnityEngine;

public class Warehouse : MonoBehaviour, IInteractable
{
    public Transform inactiveParent; // Assign this in the Unity Inspector

    public Resource.ResourceType resourceType;                              
    public Transform resourceUnstackPoint;              
    public GameObject resourcePrefab;

    public Transform resourceStackPoint;
    public float stackHeight;
    public int currentResourceCount = 0;
    public int capacity = 20;

    private bool isInteracting = false;
    public bool HasResources(Resource.ResourceType resourceType)
    {
        // Return true if there are resources of the requested type
        return this.resourceType == resourceType;
    }


    public void CollectResources(int amount)
    {
        currentResourceCount += amount;
    }

    public void StoreResource(GameObject resource)
    {
        if (currentResourceCount < capacity)
        {
            Instantiate(resource, resourceStackPoint.position, Quaternion.identity);
            currentResourceCount++;
        }
    }

    public bool HasSpace()
    {
        return currentResourceCount < capacity;
    }

    internal void AddResourceToWarehouse(Resource.ResourceType resourceType)
    {
        Vector3 stackPosition = resourceStackPoint.position + Vector3.up * (currentResourceCount * stackHeight);
        GameObject newResource = Instantiate(resourcePrefab, stackPosition, Quaternion.identity, resourceStackPoint);
        newResource.GetComponent<Resource>().resourceType = resourceType;

        currentResourceCount++;
    }

    public void CollectResources()
    {
        if (currentResourceCount > 0)
        {
            currentResourceCount--;
            Destroy(resourceStackPoint.GetChild(resourceStackPoint.childCount - 1).gameObject);
        }
    }

    public void Interact(PlayerInventory inventory)
    {
        if (isInteracting) return; // Prevent re-entry if already interacting

        if (HasResources(resourceType) && inventory.HasSpace() && currentResourceCount > 0)
        {
            isInteracting = true; // Set the flag to indicate interaction started

            // Instantiate the resource object to transfer
            GameObject resource = Instantiate(resourcePrefab, resourceStackPoint.position, Quaternion.identity, inventory.backpackTransform);

            // Start the animation and wait for it to finish
            StartCoroutine(AnimateResourceLoad(resource, inventory));
        }
    }

    private IEnumerator AnimateResourceLoad(GameObject resource, PlayerInventory inventory)
    {
        // Start the animation
        yield return inventory.AnimateResourceLoadToBackpack(resource); // Wait for the animation to complete

        // Add the resource to the player's inventory
        inventory.AddResource(resourceType);

        // Collect the resource from the warehouse
        CollectResources();

        isInteracting = false; // Reset the flag after interaction is complete
    }

}
