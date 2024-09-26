using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VibrationControlButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public VibratingObject VibratingObject;

    // public SceneSwitcher SceneSwitcher;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Button Pressed");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Button Clicked");

        VibratingObject.StopVibration();

        // SceneSwitcher.delay = 2.0f;
        // SceneSwitcher.sceneName = "5-VRMovie";
        // StartCoroutine(SceneSwitcher._SwitchSceneAfterDelay());
    }
}
