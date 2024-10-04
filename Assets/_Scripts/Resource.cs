using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Resource", order = 1)]
public class Resource : ScriptableObject
{
    public enum ResourceType { None, Wood, Stone, Iron }
    public ResourceType resourceType; // Add this line
    public GameObject Prefab; // Assuming you have a prefab property



    public ResourceType Type { get; set; }  // Ensure this is 'Type', not 'resourceType'
    public GameObject prefab { get; set; }   // Prefab associated with this resource

    public Resource(ResourceType type, GameObject prefab)
    {
        resourceType = type;
        Prefab = prefab;
    }
}
