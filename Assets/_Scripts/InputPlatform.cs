using UnityEngine;

public class InputPlatform : MonoBehaviour, IInteractable
{
    public Building building;

    public void Interact(PlayerInventory inventory)
    {
        if (building.HasSpace())
        {
            building.LoadResources(inventory);
        }
    }
}
