using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Resource inputOneResource;
    public Resource inputTwoResource;
    public Resource outputResource;
    public Warehouse warehouse;

    public Transform inputPos1;
    public Transform inputPos2;

    public int capacity = 10;

    private ResourceLoader inputOneLoader;
    private ResourceLoader inputTwoLoader;

    public float productionInterval = 1f;
    private float timer = 0f;

    [HideInInspector] public ProductionStatus status;

    public enum ProductionStatus
    {
        Running,
        Stopped_No_Resources,
        Stopped_No_Storage
    }

    private void Start()
    {
        PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();

        if (inputOneResource != null)
        {
            inputOneLoader = new ResourceLoader(warehouse, inventory, inputOneResource.resourceType, inputPos1, capacity);
        }

        if (inputTwoResource != null)
        {
            inputTwoLoader = new ResourceLoader(warehouse, inventory, inputTwoResource.resourceType, inputPos2, capacity);
        }


        
        
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= productionInterval)
        {
            ProduceResource();
            timer = 0f;
        }
    }

    private void ProduceResource()
    {
        // Case 1: No input resources required; produce output directly
        if (warehouse.HasSpace() && inputOneLoader == null && inputTwoLoader == null)
        {
            if (status != ProductionStatus.Running)
                status = ProductionStatus.Running;

            Vector3 stackPosition = warehouse.resourceStackPoint.position + Vector3.up * (warehouse.currentResourceCount * warehouse.stackHeight);
            GameObject newResource = Object.Instantiate(outputResource.Prefab, stackPosition, Quaternion.identity, warehouse.resourceStackPoint);
            warehouse.CollectResources(1);
            return; // Exit after production
        }

        // Check if input loaders have space and if resources are available in the warehouse
        bool hasInputOne = inputOneLoader != null && inputOneLoader.IsLoaded();
        bool hasInputTwo = inputTwoLoader != null && inputTwoLoader.IsLoaded();

        // Case 2: One input resource is loaded and produces output
        if (warehouse.HasSpace() && hasInputOne && inputTwoLoader == null)
        {
            if (status != ProductionStatus.Running)
                status = ProductionStatus.Running;

            Vector3 stackPosition = warehouse.resourceStackPoint.position + Vector3.up * (warehouse.currentResourceCount * warehouse.stackHeight);
            GameObject newResource = Object.Instantiate(outputResource.Prefab, stackPosition, Quaternion.identity, warehouse.resourceStackPoint);
            warehouse.CollectResources(1);

            inputOneLoader.UnloadResources();
            return; // Exit after production
        }

        // Case 3: Two input resources are loaded and produce output
        if (warehouse.HasSpace() && hasInputOne && hasInputTwo)
        {
            if (status != ProductionStatus.Running)
                status = ProductionStatus.Running;

            Vector3 stackPosition = warehouse.resourceStackPoint.position + Vector3.up * (warehouse.currentResourceCount * warehouse.stackHeight);
            GameObject newResource = Object.Instantiate(outputResource.Prefab, stackPosition, Quaternion.identity, warehouse.resourceStackPoint);
            warehouse.CollectResources(1);

            inputOneLoader.UnloadResources();
            inputTwoLoader.UnloadResources();
            return; // Exit after production
        }

        if (!warehouse.HasSpace() && (hasInputOne || hasInputTwo) || !warehouse.HasSpace() && (!hasInputOne && !hasInputTwo))
        {
            if (status != ProductionStatus.Stopped_No_Storage)
                status = ProductionStatus.Stopped_No_Storage;
        }

        if (inputOneLoader != null && !inputOneLoader.IsLoaded() || inputTwoLoader != null && !inputTwoLoader.IsLoaded())
        {
            if (status != ProductionStatus.Stopped_No_Resources)
                status = ProductionStatus.Stopped_No_Resources;
        }

    }

    public void LoadResources(PlayerInventory inventory)
    {
        if (inputOneLoader != null && inputOneLoader.HasSpace())
        {
            Resource.ResourceType resourceToLoad = inventory.HasResource(inputOneResource.resourceType);
            if (resourceToLoad != Resource.ResourceType.None)
            {
                inputOneLoader.LoadResources();
                return;
            }
        }

        if (inputTwoLoader != null && inputTwoLoader.HasSpace())
        {
            Resource.ResourceType resourceToLoad = inventory.HasResource(inputTwoResource.resourceType);
            if (resourceToLoad != Resource.ResourceType.None)
            {
                inputTwoLoader.LoadResources();
                return;
            }
        }
    }

    public bool HasSpace() => inputOneLoader.HasSpace() || (inputTwoLoader != null && inputTwoLoader.HasSpace());
}
