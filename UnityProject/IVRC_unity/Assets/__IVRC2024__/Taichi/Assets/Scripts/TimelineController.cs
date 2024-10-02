using UnityEngine;
using UnityEngine.Playables; // PlayableDirectorにアクセスするために必要

public class TimelineController : MonoBehaviour
{
    // PlayableDirectorを参照するための変数
    public PlayableDirector director;

    // Timelineを再生する関数
    public void PlayTimeline()
    {
        if (director != null)
        {
            director.Play(); // Timelineを再生
        }
        else
        {
            Debug.LogError("PlayableDirectorが指定されていません");
        }
    }

    // Timelineを停止する関数
    public void StopTimeline()
    {
        if (director != null)
        {
            director.Stop(); // Timelineを停止
        }
        else
        {
            Debug.LogError("PlayableDirectorが指定されていません");
        }
    }
}
