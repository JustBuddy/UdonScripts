using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Persistence;

public class TogglePersistence : UdonSharpBehaviour
{
    public Toggle targetToggle;
    [Tooltip("A unique string to identify this toggle's state for persistence.")]
    public string persistenceKey = "MyUniqueToggleState"; // Make sure this is unique per toggle!

    private bool lastToggleState; // To track changes in Update()

    void Start()
    {
        if (targetToggle == null)
        {
            Debug.LogError("[TogglePersistence] Target Toggle is not assigned!", this);
            return;
        }

        // Initialize lastToggleState with the current toggle state
        // This ensures the first change detected is a real user interaction.
        lastToggleState = targetToggle.isOn;
    }

    void OnEnable()
    {
        // Load the toggle state when the GameObject is enabled
        LoadToggleState();
    }

    // Called when the local player's data has been loaded or restored.
    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        // Ensure this callback is for the local player and the key is valid.
        if (player == Networking.LocalPlayer && !string.IsNullOrWhiteSpace(persistenceKey))
        {
            LoadToggleState();
        }
    }

    void Update()
    {
        if (targetToggle == null) return; // Prevent errors if toggle is missing

        // Check if the toggle's state has changed since the last frame
        if (targetToggle.isOn != lastToggleState)
        {
            // State has changed, save the new state
            SaveToggleState(targetToggle.isOn);
            // Update the last known state
            lastToggleState = targetToggle.isOn;
        }
    }

    private void SaveToggleState(bool state)
    {
        if (string.IsNullOrWhiteSpace(persistenceKey))
        {
            Debug.LogWarning("[TogglePersistence] Persistence key is empty. Not saving toggle state.", this);
            return;
        }
        PlayerData.SetBool(persistenceKey, state);
        Debug.Log($"[TogglePersistence] Saved state for '{persistenceKey}': {state}");
    }

    private void LoadToggleState()
    {
        if (string.IsNullOrWhiteSpace(persistenceKey))
        {
            Debug.LogWarning("[TogglePersistence] Persistence key is empty. Not loading toggle state.", this);
            return;
        }

        bool loadedState;
        if (PlayerData.TryGetBool(Networking.LocalPlayer, persistenceKey, out loadedState))
        {
            // Set the toggle's state.
            // Update lastToggleState immediately after loading to prevent
            // Update() from detecting a "change" on the next frame and resaving.
            targetToggle.isOn = loadedState;
            lastToggleState = loadedState;
            Debug.Log($"[TogglePersistence] Loaded state for '{persistenceKey}': {loadedState}");
        }
        else
        {
            Debug.Log($"[TogglePersistence] No saved data found for '{persistenceKey}'. Setting to default (current toggle state).");
            // If no data, save its current initial state so it's consistent for future loads.
            // This also ensures the default state is persisted.
            SaveToggleState(targetToggle.isOn);
            lastToggleState = targetToggle.isOn; // Initialize lastToggleState
        }
    }
}