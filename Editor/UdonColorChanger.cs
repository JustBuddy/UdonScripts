using UnityEngine;
using UnityEditor;
using UdonSharp; // Needed to reference UdonSharpBehaviour
using VRSL; // Added to reference the namespace of your UdonSharpBehaviour

#if UNITY_EDITOR
public class UdonColorChanger : MonoBehaviour
{
    // Ensure all target UdonSharpBehaviours are prefab instances in the scene,
    // not direct references to prefab assets.
    public VRStageLighting_AudioLink_Static[] targetUdonBehaviours;

    // Set ColorUsage(true, true) for HDR color selection
    [ColorUsage(true, true)]
    [SerializeField]
    private Color targetColor = Color.white;

    public void ApplyColor()
    {
        if (targetUdonBehaviours != null)
        {
            foreach (VRStageLighting_AudioLink_Static udonBehaviour in targetUdonBehaviours)
            {
                if (udonBehaviour != null)
                {
                    // Ensure we are dealing with a component in the scene hierarchy (which might be a prefab instance)
                    // and not trying to modify a prefab asset directly.
                    if (PrefabUtility.IsPartOfAnyPrefab(udonBehaviour.gameObject) && !PrefabUtility.IsPartOfPrefabAsset(udonBehaviour.gameObject))
                    {
                        // Record the object for Undo/Redo purposes before modification
                        Undo.RecordObject(udonBehaviour, "Change Light Color Tint");

                        // Apply the color change
                        udonBehaviour.LightColorTint = targetColor;

                        // Explicitly mark this specific component's property as an override on the prefab instance.
                        PrefabUtility.RecordPrefabInstancePropertyModifications(udonBehaviour);

                        // Mark the component as dirty to ensure changes are saved in the scene.
                        EditorUtility.SetDirty(udonBehaviour);
                    }
                    else if (!PrefabUtility.IsPartOfAnyPrefab(udonBehaviour.gameObject))
                    {
                        // If it's not a prefab at all, just a regular scene object
                        Undo.RecordObject(udonBehaviour, "Change Light Color Tint");
                        udonBehaviour.LightColorTint = targetColor;
                        EditorUtility.SetDirty(udonBehaviour);
                    }
                    else if (PrefabUtility.IsPartOfPrefabAsset(udonBehaviour.gameObject))
                    {
                        // This case means a direct prefab asset was accidentally dragged into the array.
                        // We should warn the user as we are not editing prefab assets here.
                        Debug.LogWarning($"Skipping modification for prefab asset '{udonBehaviour.name}'. " +
                                         $"Drag a prefab instance from the Hierarchy into the 'Target Udon Behaviours' array instead.");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No VRStageLighting_AudioLink_Static Behaviours assigned to the array!");
        }

        // Mark this UdonColorChanger component dirty itself, ensuring its own serialized data (like targetColor) is saved.
        EditorUtility.SetDirty(this);
    }
}


namespace UnityEditor
{
    [CustomEditor(typeof(UdonColorChanger))]
    public class UdonColorChangerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update(); // Always synchronize at the beginning

            UdonColorChanger udonColorChanger = (UdonColorChanger)target;

            if (GUILayout.Button("Apply Color"))
            {
                udonColorChanger.ApplyColor();
            }

            SerializedProperty targetColorProperty =
                serializedObject.FindProperty("targetColor");
            EditorGUILayout.PropertyField(targetColorProperty);

            SerializedProperty targetUdonBehavioursProperty =
                serializedObject.FindProperty("targetUdonBehaviours");
            EditorGUILayout.PropertyField(targetUdonBehavioursProperty, true);

            // Apply all modified properties to the SerializedObject.
            // This is crucial for drag-and-drop and other direct Inspector edits to be saved.
            if (serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(udonColorChanger);
            }
        }
    }
}
#endif