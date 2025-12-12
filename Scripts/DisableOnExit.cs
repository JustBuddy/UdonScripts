using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DisableOnExit : UdonSharp.UdonSharpBehaviour
{
    [Header("Objects to Toggle")]
    [Tooltip("An array of GameObjects to turn OFF when a player leaves THIS GameObject's collider.")]
    [SerializeField]
    private GameObject[] objectsToToggle; // An array of GameObjects to turn off

    [Tooltip("If true, THIS GameObject (the one with the collider and script) will also be disabled on exit.")]
    [SerializeField]
    private bool disableSelf = true; // If true, disables the GameObject this script is on

    // A reference to this GameObject's collider. We don't need a specificTriggerCollider field
    // if the script is attached to the same GO as the collider, as OnPlayerTriggerExit will
    // inherently use that collider.
    private Collider _thisGameObjectCollider;

    void Start()
    {
        // Get the collider attached to this GameObject.
        // OnPlayerTriggerExit will only fire if *this* GameObject has a collider.
        _thisGameObjectCollider = GetComponent<Collider>();

        if (_thisGameObjectCollider == null)
        {
            Debug.LogError("DisableOnExit: No Collider found on this GameObject! This script requires a Collider (set to Is Trigger) to function.", this);
            return;
        }

        // Ensure the collider is a trigger, as OnPlayerTriggerExit only works with triggers.
        if (!_thisGameObjectCollider.isTrigger)
        {
            Debug.LogError("DisableOnExit: The Collider on this GameObject is not set to 'Is Trigger'. Please enable 'Is Trigger' for the collider for this script to work.", _thisGameObjectCollider);
        }

        // Initialize the objectsToToggle array if it's null (Unity usually handles this in Inspector)
        if (objectsToToggle == null)
        {
            objectsToToggle = new GameObject[0]; // Ensure it's not null to prevent NullReferenceExceptions
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        // Only trigger if the player exiting is the local player
        if (player.isLocal)
        {
            // Turn off all specified GameObjects
            foreach (GameObject obj in objectsToToggle)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }

            // Disable the GameObject this script is on if specified
            if (disableSelf)
            {
                this.gameObject.SetActive(false);
            }

            // Optional: If you want to log when it happens
            // Debug.Log($"Player {player.displayName} exited trigger. Objects disabled.");
        }
    }

    /*
    // Optional: Add an OnPlayerTriggerEnter if you want to enable objects when entering
    // This example is for turning objects OFF on EXIT, so this is commented out.
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            // Example:
            // foreach (GameObject obj in objectsToToggle)
            // {
            //     if (obj != null)
            //     {
            //         obj.SetActive(true);
            //     }
            // }
            // if (disableSelf)
            // {
            //     this.gameObject.SetActive(true);
            // }
        }
    }
    */
}