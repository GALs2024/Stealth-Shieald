using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public AudioClip bgmClip; // Reference to the audio clip in the Inspector
    public AudioClip[] audioClips;

    public float bgmVolume = 0.1f;  // bgmClipの音量を0.5に設定（最大値は1.0）
    public float audioClipsVolume = 0.5f;

    private AudioSource audioSource;

    public RandomAudioPlayer randomAudioPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
        play();
    }

    public void play()
    {
        audioSource.PlayOneShot(bgmClip, bgmVolume);
        Invoke("DelayMethod", 5.0f);
    }

    private void DelayMethod()
    {
        StartCoroutine(PlayClipAndInit());
    }

    private IEnumerator PlayClipAndInit()
    {
        // Play the second audio clip
        audioSource.PlayOneShot(audioClips[0], audioClipsVolume);

        // Wait for the length of the audio clip
        yield return new WaitForSeconds(audioClips[0].length);

        // Now call Init() on randomAudioPlayer
        randomAudioPlayer.Init();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
