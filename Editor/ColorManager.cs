using UnityEngine;
 

public class LightColorChanger : MonoBehaviour
{
    public Light[] lights;
    public Color color = Color.white;
 

    public void ApplyColor()
    {
        if (lights != null)
        {
            foreach (Light light in lights)
            {
                if (light != null)
                {
                    light.color = color;
                }
            }
        }
        else
        {
            Debug.LogWarning("No lights assigned to the array!");
        }
    }
}
 

#if UNITY_EDITOR
namespace UnityEditor
{
    using UnityEngine;
    using UnityEditor;
 

    [CustomEditor(typeof(LightColorChanger))]
    public class LightColorChangerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LightColorChanger lightColorChanger = (LightColorChanger)target;
 

            // Add a button to the inspector at the TOP
            if (GUILayout.Button("Apply Color"))
            {
                lightColorChanger.ApplyColor();
            }
 

            // Show the color field
            lightColorChanger.color = EditorGUILayout.ColorField("Color",
                lightColorChanger.color);
 

            // Show the default inspector fields *except* for the color
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty lightsProperty = serializedObject.FindProperty("lights");
            EditorGUILayout.PropertyField(lightsProperty, true); // true for foldout
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif