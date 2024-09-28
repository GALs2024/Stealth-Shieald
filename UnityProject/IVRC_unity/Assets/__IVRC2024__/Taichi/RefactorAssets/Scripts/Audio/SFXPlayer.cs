using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public float volume;

    public void SetVolume(float volume)
    {
        this.volume = volume;
    }

    public void PlaySFX(AudioClip audioClip)
    {
        AudioUtils.PlaySFX(audioSource, audioClip, this.volume);
    }
}
