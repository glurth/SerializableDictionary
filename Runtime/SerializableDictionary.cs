using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EyE.Collections
{

    /// <summary>
    /// Provides a runtime System.Collections.Generic.Dictionary&lt; Key,Value &gt; Structure, that can be properly serialized in unity.  This class is derived from, and has the functionality of a standard c# Dictionary&lt;K,V>
    /// In order for the SerializableDictionary to show up in the editor with the correct controls:  you will need to create two need classes and files in order to apply the appropriate attributes
    /// Create a non-generic class derived from SerializableDictionary.  This class does not need a body, but does need to be tagged with [System.Serializable].
    /// Create a property drawer class derived from SerializableDictionaryPropertyDrawerBase (stored in an Editor folder). This class does not need a body, but does need to be tagged with [[CustomPropertyDrawer(typeof(YourClassDerivedFromSerializableDictionary), true)]].
    /// </summary>
    /// <typeparam name="TKeyType">Dictionary Keys are object of this type.  It is the programmers responsibility to make sure they are unique.</typeparam>
    /// <typeparam name="TValueType">This is the type of each Value stored with a key.</typeparam>
    [System.Serializable]
    public class SerializableDictionary<TKeyType, TValueType> :IDictionary<TKeyType, TValueType>, ISerializationCallbackReceiver
    {
        private Dictionary<TKeyType, TValueType> internalDicionary;

        /// <summary>
        /// Default Constructor. Initializes the Dictionary with zero elements.
        /// </summary>
        public SerializableDictionary()
        {
            internalDicionary = new Dictionary<TKeyType, TValueType>(0, null);
        }

        /// <summary>
        /// Initializes the Dictionary with a number of elements allocated, but does not assign any elements.
        /// </summary>
        /// <param name="len">number of elements the dictionary should be initially allocated.</param>
        public SerializableDictionary(int len=0)
        {
            internalDicionary = new Dictionary<TKeyType, TValueType>(len, null);
        }
        /// <summary>
        /// Initializes the Dictionary with a number of elements allocated, but does not assign any elements.
        /// </summary>
        /// <param name="len">number of elements the dictionary should be initially allocated.</param>
        public SerializableDictionary(SerializableDictionary<TKeyType, TValueType> toCopy)
        {
            internalDicionary = new Dictionary<TKeyType, TValueType>(toCopy);
        }
        /// <summary>
        /// Initializes the Dictionary with a number of elements allocated, but does not assign any elements.
        /// </summary>
        /// <param name="len">number of elements the dictionary should be initially allocated.</param>
        public SerializableDictionary(Dictionary<TKeyType, TValueType> toCopy)
        {
            internalDicionary = new Dictionary<TKeyType, TValueType>(toCopy);
        }

        /// <summary>
        /// This field exists for use by the propertyDrawer.  It provides a place to store a new entry's Key information
        /// </summary>
        public TKeyType editorNewKeyValue;
        /// <summary>
        /// This field exists for use by the propertyDrawer.  It provides a place to store a new entry's Value information
        /// </summary>
        public TValueType editorNewValueValue;

        /*
        /// <summary>
        /// After checking IsEditorAddKeyValid, the two new entry keys editorNewKeyValue and editorNewValueValue, will be used to create a new entry in the dictionary.
        /// </summary>
        public void EditorAdd()
        {
            if (CheckEditorAddKeyValid)
            {
                pairKList.Add(editorNewKeyValue);
                pairVList.Add(editorNewValueValue);
                this.Add(editorNewKeyValue, editorNewValueValue);
            }
        }*/



        /// <summary>
        /// ReadOnly property that return True, if the new entry key editorNewKeyValue does not already exist in the dictionary.  The new key value may not be null.
        /// </summary>
        bool CheckEditorAddKeyValid()
        {
            if (editorNewKeyValue == null) return false;

            // if (ReferenceEquals(editorNewKeyValue, null)) return false;
            if (typeof(UnityEngine.Object).IsAssignableFrom(editorNewKeyValue.GetType()))
            {
                if (!(editorNewKeyValue as UnityEngine.Object)) return false;
            }
            if (this.ContainsKey(editorNewKeyValue)) return false;
            if (pairKList.Contains(editorNewKeyValue)) return false;
            return true;
        }

        [SerializeField]
        bool isEditorAddKeyValid;
        [SerializeField]
        string keyTypeName;
        [SerializeField]
        string valueTypeName;

        /// <summary>
        /// returns the all the Keys in the dictionary as a Collection
        /// </summary>
        public ICollection<TKeyType> Keys { get { return internalDicionary.Keys; } }


        /// <summary>
        /// returns the all Values in the dictionary as a Collection
        /// </summary>
        public ICollection<TValueType> Values
        {
            get
            {
                return internalDicionary.Values;
            }
        }

        /// <summary>
        /// returns the number of Keys in the dictionary
        /// </summary>
        public int Count { get { return internalDicionary.Count; } }

        /// <summary>
        /// Returns true is the dictionary is ReadOnly
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// This indexer is used to find a Particular Value in the dictionary associated with the provided key
        /// </summary>
        /// <param name="key">key used to lookup the value stored in the dictionary</param>
        /// <returns>the value found associated with the provided key</returns>
        public TValueType this[TKeyType key] { 
            get { return internalDicionary[key]; } 
            set { internalDicionary[key] = value; } }

        /// <summary>
        /// Serialized version of the Key data, synchronized with the dictionary during serialization.  This is what gets saved to disk, not the runtime dictionary.
        /// </summary>
        /// 
        [SerializeField]
        public List<TKeyType> pairKList = new List<TKeyType>();
        /// <summary>
        /// Serialized version of the Value data, synchronized with the dictionary during serialization.  This is what gets saved to disk, not the runtime dictionary.
        /// </summary>
        [SerializeField]
        public List<TValueType> pairVList = new List<TValueType>();

        /// <summary>
        /// Required to implement the unity ISerializationCallbackReceiver.  It will convert the runtime dictionary into a simple serialize list that can be stored by unity.
        /// </summary>
        public void OnBeforeSerialize()
        {
            //pairList = new List<KeyValuePairStruct>();
            pairKList = new List<TKeyType>();
            pairVList = new List<TValueType>();
          //  if (dictionary != null)
                foreach (KeyValuePair<TKeyType, TValueType> dictionaryPair in this)
                {
                    pairKList.Add(dictionaryPair.Key);
                    pairVList.Add(dictionaryPair.Value);
                }
        }
        
        /// <summary>
        /// Required to implement the unity ISerializationCallbackReceiver.  It will convert the serialized list data into a runtime dictionary.
        /// </summary>
        public void OnAfterDeserialize()
        {
            
           if (this.Count > 0)
                Clear();

            if (pairKList != null && pairVList != null)
            {
                int max = pairKList.Count;
                if (pairVList.Count != max)
                    throw new System.Exception("Problem Deserializing SerializableDictionary. Key count (" + max + ") does not match value count(" + pairVList.Count + "). Aborting deserialization.  KeyType:"+typeof(TKeyType) + "  ValueType:"+typeof(TValueType));
                for (int i = 0; i < max; i++)
                {
                    this.Add(pairKList[i], pairVList[i]);
                }
            }
            else
            {
                Debug.LogWarning("Unable to deserialize:  keylist, or value list == null");
            }
            isEditorAddKeyValid = CheckEditorAddKeyValid();
            keyTypeName = typeof(TKeyType).Name;
            valueTypeName = typeof(TValueType).Name;
        }

        /// <summary>
        /// Add the provided key/value pair to the dictionary
        /// </summary>
        /// <param name="key">key to be added.  If the key already exists an exception will be thrown.</param>
        /// <param name="value">the value to be associated with the key.</param>
        public void Add(TKeyType key, TValueType value)
        {
            internalDicionary.Add(key, value);
        }

        /// <summary>
        /// Checks to see if the specified key already exists in the dictionary
        /// </summary>
        /// <param name="key">key to check for existence</param>
        /// <returns>returns true if they is in the dictionary already, false if not</returns>
        public bool ContainsKey(TKeyType key)
        {
            return internalDicionary.ContainsKey(key);
        }

        /// <summary>
        /// Removes the specified key, and it's associated value, from the dictionary
        /// </summary>
        /// <param name="key">key to be removed.  An exception is thrown if null is passed in.</param>
        /// <returns>false if it is not found.</returns>
        public bool Remove(TKeyType key)
        {
            return internalDicionary.Remove(key);
        }

        /// <summary>
        /// Will attempt to get the value associated with the provided key.  If found the function will put the value into the out parameter, value, then return true. 
        /// If the key is not found in the dictionary, the function will return false, and the default value of TValueType will be put into the out parameter, value.
        /// </summary>
        /// <param name="key">key to lookup</param>
        /// <param name="value">out parameter that will be filled with the found value.</param>
        /// <returns>true if the key exists in the dictionary, false if not.</returns>
        public bool TryGetValue(TKeyType key, out TValueType value)
        {
            return internalDicionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// adds the provided KeyValuePair{TKeyType, TValueType} to the dictionary.  This object contains BOTH the key, and the value to be stored.
        /// if the key already exists in the dictionary an exception will be thrown.
        /// </summary>
        /// <param name="item">This object contains BOTH the key, and the value to be stored.</param>
        public void Add(KeyValuePair<TKeyType, TValueType> item)
        {
            internalDicionary.Add(item.Key,item.Value);
        }

        /// <summary>
        /// Removes all keys and values from the dictionary.
        /// </summary>
        public void Clear()
        {
            internalDicionary.Clear();
        }

        /// <summary>
        /// check if the dictionary contains the key specified, and if so, that the value associated with the key also matches the value in the provided parameter.
        /// </summary>
        /// <param name="item">KeyValuePair to check for in the collection</param>
        /// <returns>true if the exact keyValue pair is stored in the collection.</returns>
        public bool Contains(KeyValuePair<TKeyType, TValueType> item)
        {
            return ((IDictionary<TKeyType, TValueType>)internalDicionary).ContainsKey(item.Key);
        }

        /// <summary>
        /// Used to copy the collection Keys and Values into an array of KeyValuePair{TKeyType, TValueType}
        /// </summary>
        /// <param name="array">array to be populated with pairs</param>
        /// <param name="arrayIndex">index, into the array, at which to start population</param>
        public void CopyTo(KeyValuePair<TKeyType, TValueType>[] array, int arrayIndex)
        {
            ((IDictionary<TKeyType, TValueType>)internalDicionary).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified key-value pair from the dictionary.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKeyType, TValueType> item)
        {
            return internalDicionary.Remove(item.Key);
        }

        /// <summary>
        /// Used to get an IEnumerator{KeyValuePair{TKeyType, TValueType}} for use in iterating through the dictionary's KeyValuePairs
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKeyType, TValueType>> GetEnumerator()
        {
            return internalDicionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalDicionary.GetEnumerator();
        }
    }
}