using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SourceCodeCompression : MonoBehaviour {

    public Text _tOrigin;
    public Text _tResult;
    public Text _tLength;

    public string TextOrigin { get { return _tOrigin.text; } }
    public string TextResult { set { _tResult.text=value; } }

    public void  ConvertText()
    {
        string origin = TextOrigin;
        string result = origin;
        //replace empty space;
        result = result.Replace("  ", "");
        //replace private by space
        result = result.Replace("private", " ");

        TextResult = result;
        _tLength.text = _tOrigin.cachedTextGenerator.characterCount + "->" + result.Length;
       

    }

}


public static class StringExtensions
{
    public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
    {
        int startIndex = 0;
        while (true)
        {
            startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
            if (startIndex == -1)
                break;

            originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

            startIndex += newValue.Length;
        }

        return originalString;
    }

}