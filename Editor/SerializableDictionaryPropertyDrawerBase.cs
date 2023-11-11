using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using EyE.Unity.Collections;
using EyE.EditorUnity.Extensions;

namespace EyE.EditorUnity.Collections

{
    /// \ingroup UnityEyETools
    /// \ingroup UnityEyEToolsEditor
    /// <summary>
    /// property drawer used to display SerializableDictionry.
    /// </summary>
    public class SerializableDictionaryPropertyDrawerBase : PropertyDrawer
    {


        /// <summary>
        /// Static bool that stores the configured option for drawing keyValue pairs horizontally or vertically.  When true, the will be drawn horizontally.
        /// </summary>
        public static bool drawKeyValueHorizontaly=true;

        /// <summary>
        /// stores the current foldout state of the SerializableDictionaryPropertyDrawer.  Changeable by user.
        /// </summary>
        protected bool expand = true;
        /// <summary>
        /// Simple static function used to draw a box at the given rect, factoring in the current indent level.
        /// </summary>
        /// <param name="position"></param>
        public void DrawBox(Rect position)
        {
        //    if (EditorToolsOptions.DrawBoxInFoldoutArea.Value)
          //      EditorUtil.BeginDarkenedBox(position);
                //   GUI.enabled = false;
                position = EditorGUI.IndentedRect(position);
            //EditorGUI.TextArea(position, "");
            //EditorGUI.DrawTextureTransparent(position, EditorUtil.FrameTex());// EditorGUIUtility.whiteTexture);
            GUI.Box(position, "");
            //  GUI.enabled = true;
        }
        bool deleteClick = false;
        int deleteIndex = 0;
        SerializedProperty lastProperty =null;
        bool DrawKeyValueHorizontaly = true;
        bool PutFoldoutInBox = false;
        /// <summary>
        /// Function performs the actual drawing of the SerializableDictionary object
        /// </summary>
        /// <param name="position">location to draw the SerializableDictionary</param>
        /// <param name="property">SerializedProperty that references the SerializableDictionary</param>
        /// <param name="label">Label of the SerializableDictionary.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Debug.Log("SD prop");
            try
            {
           //     lastTarget = property.serializedObject;

                SerializedProperty pairKListProperty = property.FindPropertyRelative("pairKList");
                SerializedProperty pairVListProperty = property.FindPropertyRelative("pairVList");

                int listSize = pairKListProperty.arraySize;
                Rect foloutRect = position;
                foloutRect.height = EditorGUIUtility.singleLineHeight;
                label = new GUIContent(label);
                label.text += "  " + listSize + " Entries";
                expand = property.isExpanded;//test
                expand = EditorGUI.Foldout(foloutRect, expand, label, true);
                property.isExpanded = expand;
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (expand)
                {
                    Rect outerBox = position;
                    position.width -= 4;
                    EditorGUI.indentLevel++;
                    position.height = EditorGUIUtility.singleLineHeight;

                    for (int i = 0; i < listSize; i++)
                    {
                        SerializedProperty keyProperty = pairKListProperty.GetArrayElementAtIndex(i);
                        SerializedProperty valueProperty = pairVListProperty.GetArrayElementAtIndex(i);
                        float keyHeight = EditorGUI.GetPropertyHeight(keyProperty);
                        float valueHeight = EditorGUI.GetPropertyHeight(valueProperty);
                        if (PutFoldoutInBox)//EditorToolsOptions.SerializedDictionaryPropertyDrawerPutFoldoutInBox.Value)
                        {
                            Rect box = position;
                            box.xMin -= 16;
                            box.xMax += 2;
                            if (DrawKeyValueHorizontaly)//EditorToolsOptions.SerializedDictionaryPropertyDrawerDrawKeyValueHorizontaly.Value)
                                box.height = Mathf.Max(keyHeight, valueHeight) + (2 * EditorGUIUtility.standardVerticalSpacing);
                            else
                                box.height = keyHeight + valueHeight + 3 * EditorGUIUtility.standardVerticalSpacing;
                            EditorGUI.DrawRect(box,Color.gray);
                          //  EditorUtil.BeginDarkenedBox(box);
                        }
                        Rect buttonPos = position;
                        buttonPos.xMin -= 14;
                        buttonPos.width = 14;
                        buttonPos.yMin += 4;
                        buttonPos.yMax -= 2;
                        bool click = GUI.Button(buttonPos, "-");
                        if (click)
                        {
                            deleteClick = true;
                            deleteIndex = i;
                        }
                        
                        //                    position.width -= 4;
                        //                  position.width += 4;
                        position.y += EditorGUIUtility.standardVerticalSpacing;
                        float fullWidth = position.width;
                        float initialLabelWidth = EditorGUIUtility.labelWidth;
                        float initialX = position.x;
                        if (DrawKeyValueHorizontaly)//EditorToolsOptions.SerializedDictionaryPropertyDrawerDrawKeyValueHorizontaly.Value)
                        {
                            position.width *= 0.5f;
                            position.width -= 2;
                            EditorGUIUtility.labelWidth = position.width * 0.5f;

                        }
                        EditorGUI.PropertyField(position, keyProperty, KeyLabel(), true);// + keyProperty.type));
                         
                        float currentKeyHeight= EditorGUI.GetPropertyHeight(keyProperty, KeyLabel(), true) + EditorGUIUtility.standardVerticalSpacing;
                        if (DrawKeyValueHorizontaly)//EditorToolsOptions.SerializedDictionaryPropertyDrawerDrawKeyValueHorizontaly.Value)
                        {
                            position.x += position.width + 2;
                        }
                        else
                            position.y += currentKeyHeight;// EditorGUI.GetPropertyHeight(keyProperty, KeyLabel(),true) + EditorGUIUtility.standardVerticalSpacing;
                        

                        EditorGUI.PropertyField(position, valueProperty, ValueLabel(), true);// + valueProperty.type));
                        float currentValueHeight = EditorGUI.GetPropertyHeight(valueProperty, ValueLabel(), true) + EditorGUIUtility.standardVerticalSpacing;
                        if (DrawKeyValueHorizontaly)
                            position.y += Mathf.Max(currentKeyHeight, currentValueHeight);
                        else
                            position.y += currentValueHeight;

                        position.width = fullWidth;
                        position.x = initialX;
                        EditorGUIUtility.labelWidth = initialLabelWidth;
                        //  position.y += EditorGUIUtility.standardVerticalSpacing;
                      //  if (EditorToolsOptions.SerializedDictionaryPropertyDrawerPutFoldoutInBox.Value)
                       //     EditorUtil.EndDarkenedBox();

                        // position.y += EditorGUIUtility.standardVerticalSpacing;

                    }
                    // position.y += EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.indentLevel--;


                    if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
                    {
                        if (deleteClick)
                        {
                            Debug.Log("delete clicked, opening ConfirmDelete dialog window. current event: " + Event.current + "   Event type: " + Event.current.type);
                            lastProperty = property;
                            //  ConfirmPopupWindow.PopUp(ConfirmPopupCallback, "Confirm", "Confirm you would like to delete this entry",new Rect(position.center,new Vector2(300,100)));
                            ConfirmPopupCallback(true);
                            deleteClick = false;
                        }

                    }
                    DrawNewElementSection(position, property);

                }//end property expanded
                 //  if (EditorToolsOptions.SerializedDictionaryPropertyDrawerPutFoldoutInBox.Value)
                 //     EditorUtil.EndDarkendBox();
            }
            catch(System.Exception e)
            {
                Debug.Log("SerializableDictionaryPropertyDrawerBase CaughtAnException: " + e.Message);
            }
        }
        /// <summary>
        /// Used to get the height of the dictionary, when drawn.
        /// </summary>
        /// <param name="property">SerializedProperty that references the tree</param>
        /// <param name="label">Label of the Tree.</param>
        /// <returns>The total height required to draw the tree</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty iterator = property.Copy();
            // MyObject myObj = ScriptableObject.CreateInstance<MyObject>();
            // SerializedObject mySerializedObject = new UnityEditor.SerializedObject(myObj);
            // SerializedProperty iterator = mySerializedObject.FindProperty("PropertyName");
            /*Debug.Log("StartingProperty:  " + property.name);
            while (iterator.Next(true))
            {
                Debug.Log(iterator.name);
            }
            Debug.Log("DoneProperty:  " + property.name);*/
            SerializedProperty pairKListProperty = property.FindPropertyRelative("pairKList");
            SerializedProperty pairVListProperty = property.FindPropertyRelative("pairVList");
          
            if (pairKListProperty == null || pairVListProperty == null)
            {
                Debug.Log("pairKListProperty == null: " + (pairKListProperty == null).ToString() + "\npairVListProperty == null" + (pairVListProperty == null).ToString());
                return 100;
            }
            
            int listSize = pairKListProperty.arraySize;
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // foldout line
           
           // height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (property.isExpanded)
            {
              //  height += EditorGUIUtility.standardVerticalSpacing;// space between top of outer box and top of first inner/element box OR space between inner boxes
                for (int i = 0; i < listSize; i++)
                {
                   // height += EditorGUIUtility.standardVerticalSpacing;// space between top of inner box and key field
                SerializedProperty keyProperty = pairKListProperty.GetArrayElementAtIndex(i);
                    SerializedProperty valueProperty = pairVListProperty.GetArrayElementAtIndex(i);
                    height += 2 * EditorGUIUtility.standardVerticalSpacing;//space before between and after key and value fields
                    float keyHeight = EditorGUI.GetPropertyHeight(keyProperty,true);
                    float valueHeight = EditorGUI.GetPropertyHeight(valueProperty,true);
                    if (DrawKeyValueHorizontaly)//EditorToolsOptions.SerializedDictionaryPropertyDrawerDrawKeyValueHorizontaly.Value)
                        height += Mathf.Max(keyHeight, valueHeight);
                    else
                        height += keyHeight + valueHeight + EditorGUIUtility.standardVerticalSpacing;
                    //  height += EditorGUIUtility.singleLineHeight / 2;
                }
                height += EditorGUIUtility.standardVerticalSpacing;// space between bottom of last inner box and new element section
                height += GetNewElementSectionHeight(property);
            }
            //height += 4* EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            return height;
        }

        // bool showAddNewEntrySection;
        /// <summary>
        /// Draws section providing controls to add a new element to the Dictionary
        /// </summary>
        /// <param name="pos">location to place these controls</param>
        /// <param name="property">property that references the SerializableDictionary</param>
        void DrawNewElementSection(Rect pos, SerializedProperty property)
        {

            SerializedProperty newKeyProp = property.FindPropertyRelative("editorNewKeyValue");
            SerializedProperty newvalueProp = property.FindPropertyRelative("editorNewValueValue");
            float keyHeight = EditorGUI.GetPropertyHeight(newKeyProp);
            float valueHeight = EditorGUI.GetPropertyHeight(newvalueProp);
            if (true)//EditorToolsOptions.SerializedDictionaryPropertyDrawerPutFoldoutInBox.Value)
            {
                Rect box = pos;
                box.xMax += 2;
                box.height = GetNewElementSectionHeight(property);
              //  EditorUtil.BeginDarkenedBox(box);
            }
            EditorGUI.indentLevel++;
            //showAddNewEntrySection = newKeyProp.isExpanded;
            newKeyProp.isExpanded = EditorGUI.Foldout(pos, newKeyProp.isExpanded, new GUIContent("Add new entry"), true);
            
            if (newKeyProp.isExpanded)
            {
                object o = property.GetValue();
                System.Type t = o.GetType();
                System.Reflection.PropertyInfo isValidProp=t.GetProperty("IsEditorAddKeyValid", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance);
                bool isValidKey=  (bool)isValidProp.GetValue(o,null);

                EditorGUI.indentLevel++;
                pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.PropertyField(pos, newKeyProp,new GUIContent("New entry key ("+ newKeyProp.GetFieldType() +")"),true);
                pos.y += keyHeight + EditorGUIUtility.standardVerticalSpacing; 
                
                EditorGUI.PropertyField(pos, newvalueProp, new GUIContent("New entry value(" + newvalueProp.GetFieldType() + ")"),true);
                pos.y += valueHeight + EditorGUIUtility.standardVerticalSpacing;
                Rect buttonPos = EditorGUI.IndentedRect(pos);
                bool add=false;
                if (isValidKey)
                    add = GUI.Button(buttonPos, "Add new entry");
                else
                {
                    GUIStyle  helpStyle= GUI.skin.GetStyle("HelpBox");
                    TextAnchor orig = helpStyle.alignment;
                    helpStyle.alignment = TextAnchor.MiddleCenter;
                    EditorGUI.HelpBox(buttonPos, "Add Disabled: New key is Invalid", MessageType.Warning);
                    helpStyle.alignment = orig;
                }
                if (add)
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "add to dictionary");
                    
                    t.BaseType.InvokeMember("EditorAdd", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, o, new object[0]);
                    
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
          //  if (EditorToolsOptions.SerializedDictionaryPropertyDrawerPutFoldoutInBox.Value)
          //      EditorUtil.EndDarkenedBox();
        }
        /// <summary>
        /// Computes how tall the section to that allows new elements to be added to th dictionary will be.
        /// </summary>
        /// <param name="property">property that references the SerializableDictionary</param>
        /// <returns>the computed height of the section</returns>
        float GetNewElementSectionHeight(SerializedProperty property)
        {
            SerializedProperty newKeyProp = property.FindPropertyRelative("editorNewKeyValue");
            SerializedProperty newValueProp = property.FindPropertyRelative("editorNewValueValue");
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (!newKeyProp.isExpanded) return  height;  
           

            height += EditorGUI.GetPropertyHeight(newKeyProp) + EditorGUIUtility.standardVerticalSpacing;// EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUI.GetPropertyHeight(newValueProp) + EditorGUIUtility.standardVerticalSpacing; //EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; //button
            return height;
        }

        /// <summary>
        /// Internally stores the label to be used for keys
        /// </summary>
        static GUIContent keyLabel=null;
        /// <summary>
        /// label to be used for keys.  Users may override this function to change the label.
        /// </summary>
        /// <returns>the newly generated, or previously cashed keyLabel </returns>
        protected virtual GUIContent KeyLabel()
        {
            if(keyLabel==null)
                keyLabel= new GUIContent("Key", "Each Key in the dictionary is a unique value.");
            return keyLabel;
        }

        /// <summary>
        /// Internally stores the label to be used for Value Sections
        /// </summary>
        static GUIContent valueLabel = null;
        /// <summary>
        /// label to be used for value sections.  Users may override this function to change the label.
        /// </summary>
        /// <returns>the newly generated, or previously cashed keyLabel </returns>
        protected virtual GUIContent ValueLabel()
        {
            if (valueLabel == null)
                valueLabel= new GUIContent("Value", "Value that is associated with the Key.");
            return valueLabel;
        }
        /// <summary>
        /// pups up a unity dialog to confirm deletion of an element
        /// </summary>
        /// <returns>true if deletion is confirmed by the user</returns>
        bool ConfirmDelete()
        {
            return EditorUtility.DisplayDialog("Confirm Delete",
                "Confirm you would like to delete this entry.", "Delete", "Cancel");
        }
        void ConfirmPopupCallback(bool result)
        {
            if (result && lastProperty != null)
            {
                DeleteAtIndex(lastProperty, deleteIndex);
            }
        }
        private static System.Reflection.MethodInfo m_RepaintInspectors = null;
        public static void RepaintAllInspectors()
        {
            if (m_RepaintInspectors == null)
            {
                var inspWin = typeof(EditorApplication).Assembly.GetType("UnityEditor.InspectorWindow");
                m_RepaintInspectors = inspWin.GetMethod("RepaintAllInspectors", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            }
            m_RepaintInspectors.Invoke(null, null);
        }


        static void DeleteAtIndex(SerializedProperty property, int deleteIndex)
        {
            SerializedProperty pairKListProperty = property.FindPropertyRelative("pairKList");
            SerializedProperty pairVListProperty = property.FindPropertyRelative("pairVList");
            if (pairKListProperty.arraySize > deleteIndex)
            {
                SerializedProperty keyToDelete = pairKListProperty.GetArrayElementAtIndex(deleteIndex);
                if (keyToDelete.propertyType == SerializedPropertyType.ObjectReference)
                    if (keyToDelete.objectReferenceValue != null)
                        pairKListProperty.DeleteArrayElementAtIndex(deleteIndex);
                pairKListProperty.DeleteArrayElementAtIndex(deleteIndex);

                SerializedProperty ValueToDelete = pairVListProperty.GetArrayElementAtIndex(deleteIndex);
                if (ValueToDelete.propertyType == SerializedPropertyType.ObjectReference)
                    if (ValueToDelete.objectReferenceValue != null)
                        pairVListProperty.DeleteArrayElementAtIndex(deleteIndex);
                pairVListProperty.DeleteArrayElementAtIndex(deleteIndex);

                Debug.Log("post remove: pairKListProperty count " + pairKListProperty.arraySize);
                Debug.Log("post remove: pairVListProperty count " + pairVListProperty.arraySize);
                property.serializedObject.ApplyModifiedProperties();
                Debug.Log("post Apply: pairKListProperty count " + pairKListProperty.arraySize);
                Debug.Log("post Apply: pairVListProperty count " + pairVListProperty.arraySize);
            }
        }
    }
}