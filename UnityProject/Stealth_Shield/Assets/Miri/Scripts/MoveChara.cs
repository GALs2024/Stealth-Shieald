using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveChara : MonoBehaviour
{
    //カメラオブジェクト
    public GameObject mainCamera;

    //カメラとプレイヤー間の固定距離の設定
    public Vector3 offset = new Vector3(0, 0, 5);

    
    void LateUpdate()
    {
        // カメラがオブジェクトを追従する
        mainCamera.transform.position = transform.position + offset;   
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Translate(0, 0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.Translate(1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.Translate(0, 0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.Translate(-1, 0, 0);
        }
    }

}