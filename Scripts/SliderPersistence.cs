using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Persistence;

public class SliderPersistence : UdonSharpBehaviour
{
    public Slider targetSlider;
    [Tooltip("A unique string to identify this slider's state for persistence.")]
    public string persistenceKey = "MyUniqueSliderValue"; // Make sure this is unique per slider!

    private float lastSliderValue; // To track changes in Update()

    void Start()
    {
        if (targetSlider == null)
        {
            Debug.LogError("[SliderPersistence] Target Slider is not assigned!", this);
            return;
        }

        // Initialize lastSliderValue with the current slider value
        lastSliderValue = targetSlider.value;
    }

    // Called when the local player's data has been loaded or restored.
    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        // Ensure this callback is for the local player and the key is valid.
        if (player == Networking.LocalPlayer && !string.IsNullOrWhiteSpace(persistenceKey))
        {
            LoadSliderValue();
        }
    }

    void Update()
    {
        if (targetSlider == null) return; // Prevent errors if slider is missing

        // Check if the slider's value has changed significantly
        // Using a small epsilon to avoid saving on tiny floating-point inaccuracies
        if (Mathf.Abs(targetSlider.value - lastSliderValue) > 0.001f) // 0.001f is an arbitrary small value
        {
            // Value has changed, save the new value
            SaveSliderValue(targetSlider.value);
            // Update the last known value
            lastSliderValue = targetSlider.value;
        }
    }

    private void SaveSliderValue(float value)
    {
        if (string.IsNullOrWhiteSpace(persistenceKey))
        {
            Debug.LogWarning("[SliderPersistence] Persistence key is empty. Not saving slider value.", this);
            return;
        }
        PlayerData.SetFloat(persistenceKey, value);
        Debug.Log($"[SliderPersistence] Saved value for '{persistenceKey}': {value}");
    }

    private void LoadSliderValue()
    {
        if (string.IsNullOrWhiteSpace(persistenceKey))
        {
            Debug.LogWarning("[SliderPersistence] Persistence key is empty. Not loading slider value.", this);
            return;
        }

        float loadedValue;
        // Try to get the float value associated with the persistenceKey for the local player.
        if (PlayerData.TryGetFloat(Networking.LocalPlayer, persistenceKey, out loadedValue))
        {
            // Set the slider's value.
            // Update lastSliderValue immediately after loading to prevent
            // Update() from detecting a "change" on the next frame and resaving.
            targetSlider.value = loadedValue;
            lastSliderValue = loadedValue;
            Debug.Log($"[SliderPersistence] Loaded value for '{persistenceKey}': {loadedValue}");
        }
        else
        {
            Debug.Log($"[SliderPersistence] No saved data found for '{persistenceKey}'. Setting to default (current slider value).");
            // If no data, save its current initial state so it's consistent for future loads.
            // This also ensures the default state is persisted.
            SaveSliderValue(targetSlider.value);
            lastSliderValue = targetSlider.value; // Initialize lastSliderValue
        }
    }
}