using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ObjectToggleMimic : UdonSharpBehaviour
{
    public GameObject[] referenceObjects;
    public GameObject[] targetObjects;
    public bool invertToggle = false;

    void Update()
    {
        if (referenceObjects == null || targetObjects == null) return;
        if (referenceObjects.Length == 0 || targetObjects.Length == 0) return;

        bool anyReferenceActive = false;
        foreach (GameObject obj in referenceObjects)
        {
            if (obj != null && obj.activeSelf)
            {
                anyReferenceActive = true;
                break;
            }
        }

        bool newTargetState = invertToggle ? !anyReferenceActive : anyReferenceActive;

        foreach (GameObject obj in targetObjects)
        {
            if (obj != null && obj.activeSelf != newTargetState)
            {
                obj.SetActive(newTargetState);
            }
        }
    }
}