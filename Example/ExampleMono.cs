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
    // Override Equals
    public override bool Equals(object obj)
    {
        // Check for null and type mismatch
        if (obj == null || GetType() != obj.GetType())
            return false;

        // Cast to SampleDataClass and compare fields
        var other = (SampleDataClass)obj;
        return anInt == other.anInt &&
               aFloat == other.aFloat &&
               eNumVal == other.eNumVal;
    }

    // Override GetHashCode
    public override int GetHashCode()
    {
        unchecked // Allow arithmetic overflow, but it will not affect the result
        {
            int hash = 17;
            hash = hash * 23 + anInt.GetHashCode();
            hash = hash * 23 + aFloat.GetHashCode();
            hash = hash * 23 + eNumVal.GetHashCode();
            return hash;
        }
    }
}

/// <summary>
/// This is a concrete, non generic version of SerializableDictionary<int, SampleDataClass> and so, will display using the SerializableDictionary's CustomPropertyDrawer
/// Note: it must have the [System.Serializable] attribute
/// </summary>
[System.Serializable]
public class SampleDataByInt : SerializableDictionary<int, SampleDataClass> { }

[System.Serializable]
public class SampleIntByData : SerializableDictionary<SampleDataClass,int> { }
[System.Serializable]
public class SampleHoldingDictionary
{
    public string name;
    public SampleDataByInt dictionary;
    public string otherData;
}

public class ExampleMono : MonoBehaviour
{
    public int anInt;
    //this member will not display properly in unity editor because it is generic
    public SerializableDictionary<int, SampleDataClass> testSerializableDictionaryDic = new SerializableDictionary<int, SampleDataClass>();

    //this member will display properly in unity editor as it is a concrete, non-generic class
    public SampleDataByInt sampleDataByIntDic = new SampleDataByInt();
    public string aString;
    public List<SampleHoldingDictionary> sampleClassList = new List<SampleHoldingDictionary>() { new SampleHoldingDictionary() };

    public SerializableDictionary<SampleDataClass, int> testSerializableDictionaryClassKey = new SerializableDictionary<SampleDataClass,int>();
}
