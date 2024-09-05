using System;
using System.IO;
using UnityEngine;

public static class ApiKeyLoader
{
    private static string apiKeyFile = "Assets/__IVRC2024__/Taichi/Assets/API/openai_api_key.txt";

    public static string LoadApiKey()
    {
        try
        {
            return File.ReadAllText(apiKeyFile).Trim();
        }
        catch (Exception e)
        {
            Debug.LogError("Error reading API Key file: " + e.Message);
            return null;
        }
    }
}
