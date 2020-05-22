using UnityEngine;
using System.Collections;

public class CSVReader
{
    public static void Read(string infoFile)
    {
        TextAsset textData = Resources.Load(infoFile) as TextAsset;
    }
}
