using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MonoBehaviour targetScript;
    [HideInInspector] public string selectedMethod;

    [SerializeField] private KeyCode activationKey = KeyCode.Space;

    // targetScriptを取得するためのプロパティ
    public MonoBehaviour TargetScript => targetScript;

    private bool _hasExperienceStarted = false;

    private void Update()
    {
        if (Input.GetKeyDown(activationKey) && !_hasExperienceStarted)
        {
            StartExperience();
        }
    }

    private void StartExperience()
    {
        _hasExperienceStarted = true;
        Debug.Log("Experience Started!");

        ExecuteTargetMethod();
    }

    private void ExecuteTargetMethod()
    {
        if (targetScript != null && !string.IsNullOrEmpty(selectedMethod))
        {
            var method = targetScript.GetType().GetMethod(selectedMethod);
            if (method != null)
            {
                method.Invoke(targetScript, null);
                Debug.Log($"Invoked {selectedMethod} on {targetScript.GetType().Name}");
            }
            else
            {
                Debug.LogError($"Method '{selectedMethod}' not found on script '{targetScript.GetType().Name}'");
            }
        }
        else
        {
            Debug.LogError("Please ensure the target script and method are properly set in the Inspector.");
        }
    }
}
