using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleGameObjectsOnTrigger : UdonSharpBehaviour
{
    [SerializeField] private GameObject[] objectsToToggle; // Renamed from objectsToTogglePrimary
    [SerializeField] private GameObject[] objectsToToggleSecondary;
    [SerializeField] private float initialCheckDelay = 5f;
    [SerializeField] private float secondaryToggleDelay = 1f;

    private bool playerInTrigger = false;

    private void Start()
    {
        // Always start with all primary objects turned ON.
        foreach (GameObject obj in objectsToToggle) // Using original field name
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
        // Always start with all secondary objects turned ON.
        foreach (GameObject obj in objectsToToggleSecondary)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        // Schedule the initial check after a delay.
        SendCustomEventDelayedSeconds(nameof(_PerformInitialCheck), initialCheckDelay);
    }

    public void _PerformInitialCheck()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer != null)
        {
            Collider triggerCollider = GetComponent<Collider>();
            if (triggerCollider != null)
            {
                Vector3 playerPos = localPlayer.GetPosition();
                Bounds triggerBounds = triggerCollider.bounds;

                if (triggerBounds.Contains(playerPos))
                {
                    // Player is inside the trigger, ensure objects are ON.
                    playerInTrigger = true;
                    _TogglePrimaryObjects(true);
                    SendCustomEventDelayedSeconds(
                        nameof(_ToggleSecondaryObjectsTrue),
                        secondaryToggleDelay
                    );
                }
                else
                {
                    // Player is outside the trigger, turn objects OFF.
                    playerInTrigger = false;
                    _TogglePrimaryObjects(false);
                    SendCustomEventDelayedSeconds(
                        nameof(_ToggleSecondaryObjectsFalse),
                        secondaryToggleDelay
                    );
                }
            }
            else
            {
                Debug.LogError(
                    "ToggleGameObjectsOnTrigger: Collider component not found!"
                );
            }
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            playerInTrigger = true;
            _TogglePrimaryObjects(true);
            SendCustomEventDelayedSeconds(
                nameof(_ToggleSecondaryObjectsTrue),
                secondaryToggleDelay
            );
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            playerInTrigger = false;
            _TogglePrimaryObjects(false);
            SendCustomEventDelayedSeconds(
                nameof(_ToggleSecondaryObjectsFalse),
                secondaryToggleDelay
            );
        }
    }

    private void _TogglePrimaryObjects(bool state)
    {
        foreach (GameObject obj in objectsToToggle) // Using original field name
        {
            if (obj != null)
            {
                obj.SetActive(state);
            }
        }
    }

    public void _ToggleSecondaryObjectsTrue()
    {
        if (playerInTrigger)
        {
            foreach (GameObject obj in objectsToToggleSecondary)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }
    }

    public void _ToggleSecondaryObjectsFalse()
    {
        if (!playerInTrigger)
        {
            foreach (GameObject obj in objectsToToggleSecondary)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}