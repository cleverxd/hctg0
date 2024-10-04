using System.Collections;
using UnityEngine;

public class ResourceLoader : IResourceLoader
{
    private Warehouse warehouse;
    private PlayerInventory inventory;
    private Resource.ResourceType resourceType;
    private Transform inputPosition;
    private int currentResourceCount = 0;
    private int capacity;

    public ResourceLoader(Warehouse warehouse, PlayerInventory inventory, Resource.ResourceType resourceType, Transform inputPosition, int capacity)
    {
        this.warehouse = warehouse;
        this.inventory = inventory;
        this.resourceType = resourceType;
        this.inputPosition = inputPosition;
        this.capacity = capacity;
    }

    public void LoadResources()
    {
        if (currentResourceCount < capacity)
        {
            Resource.ResourceType resourceToLoad = inventory.HasResource(resourceType);
            if (resourceToLoad != Resource.ResourceType.None)
            {
                inventory.RemoveResource(resourceToLoad);
                currentResourceCount++;

                // Instantiate the resource at the input position
                Vector3 position = inputPosition.position + Vector3.up * (currentResourceCount * warehouse.stackHeight);
                GameObject prefab = inventory.GetPrefab(resourceToLoad);
                Resource newResource = new Resource(resourceToLoad, prefab);
                GameObject resource = Object.Instantiate(newResource.Prefab, position, Quaternion.identity);

                // Optionally, set the parent if it's not persistent
                resource.transform.SetParent(inputPosition);

                if (inputPosition.childCount > 0)
                {
                    Transform lastResource = inputPosition.GetChild(inputPosition.childCount - 1);

                    // Define the target position for animation (e.g., below the loader)
                    Vector3 targetPosition = lastResource.position;

                    // Start the animation coroutine
                    warehouse.StartCoroutine(AnimateResourceLoad(targetPosition, lastResource.gameObject));
                }
            }
        }
    }
    public void UnloadResources()
    {
        if (currentResourceCount > 0 && inputPosition.childCount > 0)
        {
            currentResourceCount--;

            Transform lastResource = inputPosition.GetChild(inputPosition.childCount - 1);
            Vector3 targetPosition = warehouse.transform.GetChild(0).position;

            // Start the animation coroutine to move and destroy the resource
            warehouse.StartCoroutine(AnimateResourceUnload(targetPosition, lastResource.gameObject));
        }
    }

    public bool HasSpace() => currentResourceCount < capacity;
    internal bool IsLoaded() => currentResourceCount > 0;

    private IEnumerator AnimateResourceLoad(Vector3 targetPosition, GameObject resource)
    {
        Vector3 startPosition = inventory.backpackTransform.position;
        float duration = 0.2f; // Duration of the animation
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (resource != null)
            {
                // Animate movement from start to target
                resource.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        // Ensure the resource ends exactly at the target position
        resource.transform.position = targetPosition;
    }

    private IEnumerator AnimateResourceUnload(Vector3 targetPosition, GameObject resource)
    {
        Vector3 startPosition = resource.transform.position;
        float duration = 0.2f; // Duration of the animation
        float elapsed = 0f;

        while (elapsed < duration)
        {
            resource.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the resource ends exactly at the target position
        resource.transform.position = targetPosition;

        // Destroy the resource after the animation ends
        Object.Destroy(resource);
    }
}
