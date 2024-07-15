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

        if (timer >= 30f && timer < 50f)
        {
            ShowPictures();
        }
        else if (timer >= 50f && timer < 55f)
        {
            HidePicturesAndAvatar();
        }
        else if (timer >= 55f && timer < 60f)
        {
            ShowSmartPhone();
        }
    }

    void ShowPictures()
    {
        int index = (int)((timer - 30f) / 5f);
        if (index < PictureObjects.Length && !picturesShown)
        {
            for (int i = 0; i <= index; i++)
            {
                PictureObjects[i].SetActive(true);
            }
            picturesShown = true;
        }
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
