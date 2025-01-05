using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EyE.Collections;
[System.Serializable]
public enum Sel { One, Two }
[System.Serializable]
public class SampleDataClass
{
    public int anInt;
    public float aFloat;
    public Sel eNumVal;
}

public class ExampleMono : MonoBehaviour
{
    public int anInt;
    public SerializableDictionary<int, SampleDataClass> testDic = new SerializableDictionary<int, SampleDataClass>();
    public string aString;
}
