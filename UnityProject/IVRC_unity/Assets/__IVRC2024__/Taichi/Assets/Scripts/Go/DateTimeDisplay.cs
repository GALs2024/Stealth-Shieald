using System;
using UnityEngine;
using TMPro;
using System.Collections;

// Displays the current date and time in a TextMeshProUGUI element.
public class DateTimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI DateTimeText;

    private string _dateTimeFormat = "yyyy/MM/dd HH:mm:ss";

    // Called when the script instance is being loaded.
    private void Start()
    {
        if (DateTimeText == null)
        {
            Debug.LogError("TextMeshProUGUIオブジェクトが設定されていません。");
            return;
        }

        // Start coroutine to update the date and time every second
        StartCoroutine(UpdateDateTime());
    }

    // Coroutine that updates the displayed date and time every second.
    private IEnumerator UpdateDateTime()
    {
        while (true)
        {
            // Update the displayed date and time
            DateTimeText.text = DateTime.Now.ToString(_dateTimeFormat);

            // Wait for 1 second before updating again
            yield return new WaitForSeconds(1f);
        }
    }
}
