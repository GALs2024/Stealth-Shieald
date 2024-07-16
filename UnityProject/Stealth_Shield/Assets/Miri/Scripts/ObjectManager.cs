using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject AvatarObject;
    public GameObject[] PictureObjects;
    public GameObject SmartPhone;
    public GameObject PowerOffButton;
    public GameObject ContinueButton;

    private float timer = 0f;
    private bool picturesShown = false;
    private bool avatarDestroyed = false;
    private bool phoneShown = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject picture in PictureObjects)
        {
            picture.SetActive(false);
        }
        SmartPhone.SetActive(false);
        PowerOffButton.SetActive(false);
        ContinueButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 30f && timer < 35f)
        {
            ShowPicture1();
        }
        else if(timer >= 35f && timer < 40f)
        {
            ShowPicture2();
        }
        else if (timer >= 40f && timer < 45f)
        {
            ShowPicture3();
        }
        else if (timer >= 45f && timer < 50f)
        {
            ShowPicture4();
        }

        else if (timer >= 50f && timer < 55f)
        {
            HidePicturesAndAvatar();
            ShowSmartPhone();
        }
        
    }

    void ShowPicture1()
    {
        PictureObjects[0].SetActive(true);
    }
    void ShowPicture2()
    {
        PictureObjects[1].SetActive(true);
    }
    void ShowPicture3()
    {
        PictureObjects[2].SetActive(true);
    }
    void ShowPicture4()
    {
        PictureObjects[3].SetActive(true);
    }

    void HidePicturesAndAvatar()
    {
        if (!avatarDestroyed)
        {
            AvatarObject.SetActive(false);
            foreach (GameObject picture in PictureObjects)
            {
                picture.SetActive(false);
            }
            avatarDestroyed = true;
        }
    }

    void ShowSmartPhone()
    {
        if (!phoneShown)
        {
            SmartPhone.SetActive(true);
            phoneShown = true;
        }
    }

    public void OnSmartPhoneInteraction()
    {
        // This function should be called when the user interacts with the smartphone
        PowerOffButton.SetActive(true);
        ContinueButton.SetActive(true);
    }

    public void OnPowerOffButtonPressed()
    {
        // Handle power off button press
        Debug.Log("Power off button pressed");
        // Add your logic for powering off the smartphone
    }

    public void OnContinueButtonPressed()
    {
        // Handle continue button press
        Debug.Log("Continue button pressed");
        // Add your logic for continuing the experience
    }
}
