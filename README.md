# Unity Serializable Dictionary

This repository provides an implementation of a serializable dictionary for Unity, enabling the serialization of dictionary data types so they can be viewed and edited within the Unity Editor. This package includes a serializable dictionary class along with a custom property drawer to display it in the editor.

## Features

- **Serializable Dictionary**: A dictionary that can be serialized by Unity, supporting types specified by the user.
- **Custom Property Drawer**: Enhances the Unity Editor interface to allow for intuitive manipulation of dictionaries.

## Installation

### Using Git in Unity Package Manager

To install this package through Unity's Package Manager with Git, follow these steps:

1. Open your Unity project.
2. Navigate to `Window` -> `Package Manager`.
3. In the Package Manager window, click the `+` (plus) button at the top left.
4. Select `Add package from git URL...`.
5. Enter the following URL and press 'Add':
   ```
   https://github.com/glurth/SerializableDictionary.git
   ```

Unity will clone the repository and the package will appear in your list of packages. Unity might take a few moments to download and import the package.

## Components

### 1. SerializableDictionary

`SerializableDictionary<TKeyType, TValueType>` acts like a standard C# dictionary but includes serialization capabilities for Unity editor and runtime.

#### How to Use

- **Define a Serializable Dictionary**:
  Create a subclass of `SerializableDictionary`, specify key and value types, and mark it with `[System.Serializable]` to ensure it is visible in the Unity Editor.

  ```csharp
  [System.Serializable]
  public class MyDictionary : SerializableDictionary<string, int> { }
  ```

#### Features

- **Editor Integration**: Fields are provided to assist with the addition of new entries directly within the Unity Editor.
- **Serialization Support**: Implements custom serialization logic to handle Unity's serialization system.

### 2. SerializableDictionaryPropertyDrawerBase

`SerializableDictionaryPropertyDrawerBase` is designed to provide a custom property drawer for `SerializableDictionary` in Unity, enabling easy modification of dictionary entries through the Unity Inspector. For this to work, you will need to derive a concrete variant of SerializableDictionary, with the [System.Serializable] attribute applied to it.

#### Features

- **Layout Customization**: Supports both horizontal and vertical layouts for displaying key-value pairs.
- **Interactive Editing**: Facilitates adding and removing entries directly via the Inspector, enhancing usability.

## Usage Example

Below is an example showing how to define and use a serializable dictionary in your Unity scripts:

```csharp
[System.Serializable]
public class StringToIntDictionary : SerializableDictionary<string, int> { }

public class ExampleUsage : MonoBehaviour {
    public StringToIntDictionary myDictionary;
}
```

With this setup, `myDictionary` will be visible in the Unity Inspector with an interface for adding, editing, and removing dictionary entries.

## Contributions

Contributions, issues, and feature requests are welcome! Please submit them via the GitHub repository. Note: Due to licensing, contributions can only be included with explicit written permission from the copyright holder.

## License

This package is licensed under the EyE Dual-Licensing Agreement.

It provides free, perpetual use for indie developers and non-commercial projects whose teams had Total Gross Receipts under $100,000 USD in the previous fiscal year.

Organizations exceeding this threshold must obtain a Perpetual Commercial License (PCL) for each named commercial project.

Please review the full terms in [LICENSE.md](LICENSE.md) before commercial use.