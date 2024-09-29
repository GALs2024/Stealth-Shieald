using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BtnPressTest : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public VibratingButton vibrationButton;
    public VibratingObject vibratingObject;
    public SceneSwitcher sceneSwitcher;

    // Start is called before the first frame update
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Button Pressed");
    }

    // Update is called once per frame
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Button Clicked");
        this.vibrationButton.StopVibration();
        this.vibratingObject.StopVibration();
        this.sceneSwitcher.delay = 2.0f;
        this.sceneSwitcher.sceneName = "5-VRMovie";
        StartCoroutine(this.sceneSwitcher._SwitchSceneAfterDelay());
    }
}
