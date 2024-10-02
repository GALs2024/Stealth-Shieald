using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VibrationControlButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TimelineController timelineController;
    public VibratingObject VibratingObject;

    public AudioSource audioSource;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Button Pressed");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Button Clicked");

        VibratingObject.StopVibration();
        audioSource.Stop();

        timelineController.PlayTimeline();
    }
}
