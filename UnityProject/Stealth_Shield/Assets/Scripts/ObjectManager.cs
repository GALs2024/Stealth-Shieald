using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject AvatarObject;
    public GameObject PictureObject1;
    public GameObject PictureObject2;
    public GameObject PictureObject3;
    public GameObject PictureObject4;

    private float timer = 0f;
    private bool avatarDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        PictureObject1.SetActive(false);
        PictureObject2.SetActive(false);
        PictureObject3.SetActive(false);
        PictureObject4.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer>=30f && !avatarDestroyed)
        {
            AvatarObject.SetActive(false);
            PictureObject1.SetActive(true);
            PictureObject2.SetActive(true);
            PictureObject3.SetActive(true);
            PictureObject4.SetActive(true);
            avatarDestroyed=true;
        }
    }
}
