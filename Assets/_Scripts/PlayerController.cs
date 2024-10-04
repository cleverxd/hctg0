using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInventory inventory;      // Reference to the player's inventory

    private float actionTimer = 0;
    [SerializeField] private float actionSpeed = 0.25f;

    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody rb;

    private IInteractable currentInteractable;  // Interface reference to any interactable object

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        actionTimer += Time.deltaTime;
        // Handle player movement
        MovePlayer();
    }

    // Method to move the player
    void MovePlayer()
    {
        Vector2 moveDir = moveAction.action.ReadValue<Vector2>();

        Vector3 movement = new Vector3(moveDir.x, 0.0f, moveDir.y);
        rb.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);
    }

    // Handle interactions with objects
    private void OnTriggerStay(Collider other)
    {
        // Get the interactable component
        if (currentInteractable == null)
        {
            currentInteractable = other.GetComponent<IInteractable>();
        }

        // Perform the interaction if the player is allowed to interact
        if (actionTimer > actionSpeed && currentInteractable != null)
        {
            currentInteractable.Interact(inventory);
            actionTimer = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear interactable reference when exiting the trigger zone
        if (currentInteractable != null && other.GetComponent<IInteractable>() == currentInteractable)
        {
            currentInteractable = null;
        }
    }

}
