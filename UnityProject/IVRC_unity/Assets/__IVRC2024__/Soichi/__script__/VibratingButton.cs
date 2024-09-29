using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VibratingButton : MonoBehaviour
{
    public float vibrationSpeed = 1.0f;
    public float vibrationAmplitude = 0.1f;
    private bool isVibrating = true;
    private Vector3 originalPosition;
    private AudioSource audioSource;

    void Start()
    {
        originalPosition = transform.position;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing from the GameObject.");
            return;
        }

        //button.onClick.AddListener(ToggleVibration); // Buttonがクリックされたときに振動を切り替える
    }

    void Update()
    {
        if (isVibrating)
        {
            float vibration = Mathf.Sin(Time.time * vibrationSpeed) * vibrationAmplitude;
            transform.position = originalPosition + new Vector3(vibration, 0, 0);

            if (!audioSource.isPlaying)
            {
                audioSource.Play(); // 振動中に音を鳴らす
            }
        }
        else
        {
            transform.position = originalPosition; // 振動が停止したら元の位置に戻す

            /*if (audioSource.isPlaying)
            {
                audioSource.Stop(); // 振動が停止したら音を止める
            }*/
        }
    }

    public void StopVibration()
    {
        if (isVibrating) // 一度だけ停止処理を行う
        {
            isVibrating = false;
            transform.position = originalPosition;
            
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // 振動が停止したら音を止める
            }
        }
    }
}
