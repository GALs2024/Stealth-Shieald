using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFomatter : MonoBehaviour
{
    public static string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // 改行コードやその他のエスケープシーケンスを空白に置き換える
        return input.Replace("\n", " ")
                    .Replace("\r", " ")
                    .Replace("\\", " ")
                    .Replace("\"", " ")
                    .Replace("\t", " ");
    }
}
