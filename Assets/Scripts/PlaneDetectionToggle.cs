using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using TMPro;


[RequireComponent(typeof(ARPlaneManager))]
public class PlaneDetectionToggle : MonoBehaviour
{
    private ARPlaneManager planeManager;
    [SerializeField]
    private TMP_Text toggleButtonText;

    private void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
        toggleButtonText.text = "DISABLE";
    }

    //:COMMENT:27/03/2022:PIET: defines the value of SetAllPlanesActive depending on if the Plane Manager is enabled or no and updates the text on the button
    public void TogglePlaneDetection()
    {
        //:COMMENT:27/03/2022:PIET: enables or disables the Plane Manager
        planeManager.enabled = !planeManager.enabled; 
        string toggleButtonMessage = "";

        if(planeManager.enabled)
        {
            toggleButtonMessage="DISABLE";
            SetAllPlanesActive(true);
        }
        else
        {
            toggleButtonMessage="ENABLE";
            SetAllPlanesActive(false);
        }

        toggleButtonText.text = toggleButtonMessage;
    }

    //:COMMENT:27/03/2022:PIET: defines the planes' status : display if true, hide if false
    public void SetAllPlanesActive(bool value)
    {
        foreach(var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }
}
