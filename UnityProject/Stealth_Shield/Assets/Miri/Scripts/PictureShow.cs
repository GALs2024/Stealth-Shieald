using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureShow : MonoBehaviour
{

    private string url = "https://img.rurubu.jp/img_srw/andmore/images/0000562373/h6r2XETqBsJp9Fjm2CijcW3ltelkgLgaJ5MSoIDV.jpg";

    IEnumerator Start()
    {

        WWW www = new WWW(url);
        yield return www;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = www.texture;

    }

}