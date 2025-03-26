using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EyE.Collections;
namespace EyE.Collections.UnitTests
{

    /// <summary>
    /// This class contains comprehensive tests for the SerializableDictionary class. 
    /// Run tests in Unity via [MenuItem("Unit Tests/SerializableDictionaryTest")]
    /// It includes tests for:
    /// - Default constructor initialization.
    /// - Constructor with a specified initial size.
    /// - Constructor with another SerializableDictionary.
    /// - Constructor with a standard Dictionary.
    /// - Adding elements.
    /// - Checking for the existence of keys.
    /// - Removing elements.
    /// - Trying to get values.
    /// - Using the indexer.
    /// - Clearing the dictionary.
    /// - Testing serialization and deserialization.
    /// - Handling corrupt data during deserialization.
    /// - Actual Unity serialization/deserialization using JsonUtility.
    /// - Adding duplicate keys.
    /// - Looking up non-existent keys.
    /// Each test logs errors with detailed messages if they fail, or logs success if they pass.
    /// The class also includes counters for the total number of tests, the number of tests that passed, and the number of tests that failed.
    /// </summary>
    public class SerializableDictionaryTest
    {
        private int totalTests = 0;
        private int totalPassed = 0;
        private int totalFailed = 0;

        [MenuItem("Unit Tests/SerializableDictionaryTest")]
        static public void LogTestResults()
        {
            SerializableDictionaryTest tester = new SerializableDictionaryTest();
            tester.RunTests();
            tester.LogFinalResults();
        }

        public void RunTests()
        {
            TestDefaultConstructor();
            TestConstructorWithSize();
            TestConstructorWithSerializableDictionary();
            TestConstructorWithDictionary();
            TestAdd();
            TestContainsKey();
            TestRemove();
            TestTryGetValue();
            TestIndexer();
            TestClear();
            TestSerialization();
            TestDeserializeCorruptData();
            TestSerializationInUnity();
            TestAddDuplicateKeys();
            TestLookupNonExistentKey();
        }

        private void LogError(string testName, string message)
        {
            Debug.LogError($"Test {testName} failed: {message}");
            totalFailed++;
        }

        private void LogSuccess(string testName)
        {
            Debug.Log($"Test {testName} passed.");
            totalPassed++;
        }

        private void TestDefaultConstructor()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            if (dict.Count != 0)
            {
                LogError(nameof(TestDefaultConstructor), "Count is not zero.");
            }
            else
            {
                LogSuccess(nameof(TestDefaultConstructor));
            }
        }

        private void TestConstructorWithSize()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>(10);
            if (dict.Count != 0)
            {
                LogError(nameof(TestConstructorWithSize), "Count is not zero.");
            }
            else
            {
                LogSuccess(nameof(TestConstructorWithSize));
            }
        }

        private void TestConstructorWithSerializableDictionary()
        {
            totalTests++;
            var originalDict = new SerializableDictionary<int, string>();
            originalDict.Add(1, "one");
            originalDict.Add(2, "two");

            var dict = new SerializableDictionary<int, string>(originalDict);
            if (dict.Count != 2 || dict[1] != "one" || dict[2] != "two")
            {
                LogError(nameof(TestConstructorWithSerializableDictionary), "Dictionary contents do not match.");
            }
            else
            {
                LogSuccess(nameof(TestConstructorWithSerializableDictionary));
            }
        }

        private void TestConstructorWithDictionary()
        {
            totalTests++;
            var originalDict = new Dictionary<int, string>();
            originalDict.Add(1, "one");
            originalDict.Add(2, "two");

            var dict = new SerializableDictionary<int, string>(originalDict);
            if (dict.Count != 2 || dict[1] != "one" || dict[2] != "two")
            {
                LogError(nameof(TestConstructorWithDictionary), "Dictionary contents do not match.");
            }
            else
            {
                LogSuccess(nameof(TestConstructorWithDictionary));
            }
        }

        private void TestAdd()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            dict.Add(1, "one");
            if (dict.Count != 1 || dict[1] != "one")
            {
                LogError(nameof(TestAdd), "Failed to add item.");
            }
            else
            {
                LogSuccess(nameof(TestAdd));
            }
        }

        private void TestContainsKey()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            dict.Add(1, "one");

            if (!dict.ContainsKey(1))
            {
                LogError(nameof(TestContainsKey), "Key 1 should exist.");
            }
            else
            {
                LogSuccess(nameof(TestContainsKey));
            }
            if (dict.ContainsKey(2))
            {
                LogError(nameof(TestContainsKey), "Key 2 should not exist.");
            }
            else
            {
                LogSuccess(nameof(TestContainsKey)+"!Not");
            }
        }

        private void TestRemove()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            dict.Add(1, "one");
            dict.Remove(1);

            if (dict.ContainsKey(1))
            {
                LogError(nameof(TestRemove), "Key 1 should not exist.");
            }
            else
            {
                LogSuccess(nameof(TestRemove));
            }
        }

        private void TestTryGetValue()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            dict.Add(1, "one");

            if (dict.TryGetValue(1, out string value) && value == "one")
            {
                LogSuccess(nameof(TestTryGetValue));
            }
            else
            {
                LogError(nameof(TestTryGetValue), "Failed to retrieve value for key 1.");
            }
        }

        private void TestIndexer()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            dict.Add(1, "one");

            if (dict[1] != "one")
            {
                LogError(nameof(TestIndexer), "Indexer get failed.");
            }
            else
            {
                dict[1] = "uno";
                if (dict[1] != "uno")
                {
                    LogError(nameof(TestIndexer), "Indexer set failed.");
                }
                else
                {
                    LogSuccess(nameof(TestIndexer));
                }
            }
        }

        private void TestClear()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            dict.Add(1, "one");
            dict.Clear();

            if (dict.Count != 0)
            {
                LogError(nameof(TestClear), "Failed to clear dictionary.");
            }
            else
            {
                LogSuccess(nameof(TestClear));
            }
        }

        private void TestSerialization()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");

            dict.OnBeforeSerialize();
            dict.OnAfterDeserialize();

            if (dict.Count != 2 || dict[1] != "one" || dict[2] != "two")
            {
                LogError(nameof(TestSerialization), "Serialization or deserialization failed.");
            }
            else
            {
                LogSuccess(nameof(TestSerialization));
            }
        }
        private void TestDeserializeCorruptData()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            dict.pairKList = new List<int> { 1, 2 };
            dict.pairVList = new List<string> { "one" };

            try
            {
                dict.OnAfterDeserialize();
                LogError(nameof(TestDeserializeCorruptData), "Deserialization should have thrown an exception due to mismatched key/value counts.");
            }
            catch (Exception)
            {
                LogSuccess(nameof(TestDeserializeCorruptData));
            }
        }

        private void TestSerializationInUnity()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");

            string serializedData = JsonUtility.ToJson(dict);
            var deserializedDict = JsonUtility.FromJson<SerializableDictionary<int, string>>(serializedData);

            if (deserializedDict.Count != 2 || deserializedDict[1] != "one" || deserializedDict[2] != "two")
            {
                LogError(nameof(TestSerializationInUnity), "Unity serialization or deserialization failed.");
            }
            else
            {
                LogSuccess(nameof(TestSerializationInUnity));
            }
        }

        private void TestAddDuplicateKeys()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            dict.Add(1, "one");

            try
            {
                dict.Add(1, "uno");
                LogError(nameof(TestAddDuplicateKeys), "Adding duplicate key should have thrown an exception.");
            }
            catch (Exception)
            {
                LogSuccess(nameof(TestAddDuplicateKeys));
            }
        }

        private void TestLookupNonExistentKey()
        {
            totalTests++;
            var dict = new SerializableDictionary<int, string>();
            dict.Add(1, "one");

            if (dict.ContainsKey(2))
            {
                LogError(nameof(TestLookupNonExistentKey), "Key 2 should not exist.");
            }
            else if (dict.TryGetValue(2, out string value))
            {
                LogError(nameof(TestLookupNonExistentKey), "TryGetValue should have returned false for non-existent key.");
            }
            else
            {
                LogSuccess(nameof(TestLookupNonExistentKey));
            }
        }
        private void LogFinalResults()
        {
            Debug.Log($"Total tests: {totalTests}, Total passed: {totalPassed}, Total failed: {totalFailed}");
        }
    }
}