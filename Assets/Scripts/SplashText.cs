using TMPro;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;

public class SplashText : MonoBehaviour
{

    [SerializeField] private TMP_Text splashText;
    [SerializeField] private string fileName;

    private List<string> rawSplashTexts;
    private List<string> splashTexts;
    private string rawSplash;

    void Start()
    {
        rawSplashTexts = new List<string>();
        splashTexts = new List<string>();

        string line;
        StreamReader sr = new StreamReader(fileName); // Pass the file path and file name to the StreamReader constructor
            
        line = sr.ReadLine(); // Read the first line of text
        while (line != null) // Continue to read until you reach end of file
        {
            rawSplashTexts.Add(line);
            line = sr.ReadLine(); // Read the next line
        }
        sr.Close(); // close the file

        GetRandomSplash();
    }

    void GetRandomSplash()
    {
        int randomindex = UnityEngine.Random.Range(0, splashTexts.Count);

        try
        {
            rawSplash = splashTexts[randomindex];
        }
        catch
        {
            splashTexts = rawSplashTexts;
            randomindex = UnityEngine.Random.Range(0, splashTexts.Count);
            rawSplash = splashTexts[randomindex];
        }

        splashTexts.RemoveAt(randomindex);

        splashText.text = rawSplash;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            GetRandomSplash();
        }
    }
}
