using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIMovieManager : MonoBehaviour
{
    public string sceneName;
    
    public void StartMovie()
    {
        Debug.Log("StartMovie");
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(this.sceneName);
    }
}
