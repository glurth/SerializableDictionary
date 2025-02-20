using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EyE.Collections;
//using EyE.EditorUnity.Extensions;

namespace EyE.EditorUnity.Collections

{
    /// \ingroup UnityEyETools
    /// \ingroup UnityEyEToolsEditor
    /// <summary>
    /// property drawer used to display SerializableDictionry.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>),true)]
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
 
        Vector2 scrollPos;
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
                foloutRect.width /= 2;
                label = new GUIContent(label);
                label.text += "  " + listSize + " Entries";
                expand = property.isExpanded;//test
                expand = EditorGUI.Foldout(foloutRect, expand, label, true);
                property.isExpanded = expand;
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (expand)
                {
                    
                    foloutRect.xMin += foloutRect.width + 5;
                    foloutRect.width = 19;
                    if (GUI.Button(foloutRect, new GUIContent("O", "Toggle outline")))
                        PutFoldoutInBox = !PutFoldoutInBox;
                   
                    Rect outerBox = position;
                    position.width -= 4;
                    EditorGUI.indentLevel++;
                    position.height = EditorGUIUtility.singleLineHeight;
                    float elementsHeight = GetAllEntriesHeight(property, null);

                    Rect contentRect = new Rect(0, 0, position.width - GUI.skin.verticalScrollbar.fixedWidth, elementsHeight);
                    Rect scrollRect = position;
                    bool usingScroll = false;
                    if (contentRect.height > 300)
                    {
                        usingScroll = true;
                        scrollRect.height = 300;

                        scrollRect=DrawRectOutline(scrollRect, Color.black);
                        contentRect.xMax -= 1;
                        contentRect.xMin += 1;

                        scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, contentRect);
                    }
                    else
                    {
                        scrollRect.height = elementsHeight;
                        if (PutFoldoutInBox)
                        {
                            scrollRect.height += 2;
                            scrollRect = DrawRectOutline(scrollRect, Color.black);
                        }
                        contentRect = scrollRect;
                    }
                    contentRect.height = position.height;

                    for (int i = 0; i < listSize; i++)
                    {
                        SerializedProperty keyProperty = pairKListProperty.GetArrayElementAtIndex(i);
                        SerializedProperty valueProperty = pairVListProperty.GetArrayElementAtIndex(i);
                        float keyHeight = EditorGUI.GetPropertyHeight(keyProperty);
                        float valueHeight = EditorGUI.GetPropertyHeight(valueProperty);
                        if (i%2==0)//PutFoldoutInBox)//EditorToolsOptions.SerializedDictionaryPropertyDrawerPutFoldoutInBox.Value)
                        {
                            Rect box = contentRect;
                           // box.xMin += 2;// 16;
                            //box.xMax += 2;
                            if (DrawKeyValueHorizontaly)//EditorToolsOptions.SerializedDictionaryPropertyDrawerDrawKeyValueHorizontaly.Value)
                                box.height = Mathf.Max(keyHeight, valueHeight) + (2 * EditorGUIUtility.standardVerticalSpacing);
                            else
                                box.height = keyHeight + valueHeight + 3 * EditorGUIUtility.standardVerticalSpacing;
                            //DrawRectOutline(box, Color.black);
                            EditorGUI.DrawRect(box, Color.gray* 1.4f);
                            //  EditorUtil.BeginDarkenedBox(box);
                      
                        }
                        Rect buttonPos = contentRect;
                        //buttonPos.xMin -= 14;
                        buttonPos.width = 16;
                        buttonPos.xMin += 2;
                        buttonPos.yMin += 2;
                        buttonPos.yMax -= 2;
                        Color oldBG = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(1,.75f,.76f);
                        bool click = GUI.Button(buttonPos, new GUIContent("-","Delete Key/Value entry"));
                        GUI.backgroundColor = oldBG;
                        if (click)
                        {
                            if (EditorUtility.DisplayDialog(
                                    "Confirm Action",
                                    "Are you sure you want to delete this entry?",
                                    "Yes",
                                    "No"))
                            {
                                deleteClick = true;
                                deleteIndex = i;
                            }
                        }

                        //                    position.width -= 4;
                        //                  position.width += 4;
                        contentRect.y += EditorGUIUtility.standardVerticalSpacing;
                        float fullWidth = contentRect.width;// -1;
                        float initialLabelWidth = EditorGUIUtility.labelWidth;
                        float initialX = contentRect.x;
                        contentRect.width -= 2;

                        if (DrawKeyValueHorizontaly)//EditorToolsOptions.SerializedDictionaryPropertyDrawerDrawKeyValueHorizontaly.Value)
                        {
                            contentRect.width *= 0.5f;
                            EditorGUIUtility.labelWidth = contentRect.width * 0.5f;

                        }
                        Rect keyPos = contentRect;
                        keyPos.xMin += buttonPos.width;
                        EditorGUI.PropertyField(keyPos, keyProperty, KeyLabel(), true);// + keyProperty.type));
                         
                        float currentKeyHeight= EditorGUI.GetPropertyHeight(keyProperty, KeyLabel(), true) + EditorGUIUtility.standardVerticalSpacing;
                        if (DrawKeyValueHorizontaly)//EditorToolsOptions.SerializedDictionaryPropertyDrawerDrawKeyValueHorizontaly.Value)
                        {
                            contentRect.x += contentRect.width + 2;
                            contentRect.xMax -= 2;
                        }
                        else
                        {
                            contentRect.y += currentKeyHeight;// EditorGUI.GetPropertyHeight(keyProperty, KeyLabel(),true) + EditorGUIUtility.standardVerticalSpacing;
                            contentRect.xMin += buttonPos.width;
                        }
                        

                        EditorGUI.PropertyField(contentRect, valueProperty, ValueLabel(), true);// + valueProperty.type));
                        float currentValueHeight = EditorGUI.GetPropertyHeight(valueProperty, ValueLabel(), true) + EditorGUIUtility.standardVerticalSpacing;
                        if (DrawKeyValueHorizontaly)
                            contentRect.y += Mathf.Max(currentKeyHeight, currentValueHeight);
                        else
                            contentRect.y += currentValueHeight;

                        contentRect.width = fullWidth;
                        contentRect.x = initialX;
                        EditorGUIUtility.labelWidth = initialLabelWidth;
                    }
                    if (usingScroll)
                        GUI.EndScrollView();
                    position.y += scrollRect.height;
                    // position.y += EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.indentLevel--;


                    if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
                    {
                        if (deleteClick)
                        {
                            lastProperty = property;
                            DeleteAtIndex(lastProperty, deleteIndex);
                            deleteClick = false;
                        }

                    }
                    DrawNewElementSection(position, property);

                }//end property expanded

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
                height += GetAllEntriesHeight(property, label);

                height += EditorGUIUtility.standardVerticalSpacing;// space between bottom of last inner box and new element section
                height += GetNewElementSectionHeight(property);
            }
            //height += 4* EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            return height;
        }

        public float GetAllEntriesHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty iterator = property.Copy();

            SerializedProperty pairKListProperty = property.FindPropertyRelative("pairKList");
            SerializedProperty pairVListProperty = property.FindPropertyRelative("pairVList");

            if (pairKListProperty == null || pairVListProperty == null)
            {
                Debug.Log("pairKListProperty == null: " + (pairKListProperty == null).ToString() + "\npairVListProperty == null" + (pairVListProperty == null).ToString());
                return 100;
            }

            int listSize = pairKListProperty.arraySize;
            float height = 0;


            if (property.isExpanded)
            {
                for (int i = 0; i < listSize; i++)
                {
                    SerializedProperty keyProperty = pairKListProperty.GetArrayElementAtIndex(i);
                    SerializedProperty valueProperty = pairVListProperty.GetArrayElementAtIndex(i);
                    height += 2 * EditorGUIUtility.standardVerticalSpacing;
                    float keyHeight = EditorGUI.GetPropertyHeight(keyProperty, true);
                    float valueHeight = EditorGUI.GetPropertyHeight(valueProperty, true);
                    if (DrawKeyValueHorizontaly)
                        height += Mathf.Max(keyHeight, valueHeight);
                    else
                        height += keyHeight + valueHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }

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
            if (PutFoldoutInBox)
            {
                Rect box = pos;
                //box.xMin -= 2;
                //box.xMax += 2;
                box.height = GetNewElementSectionHeight(property);
                box=DrawRectOutline(box, Color.black);
                box.height = pos.height;
                pos = box;
                pos.xMax -= 2;
            }
            EditorGUI.indentLevel++;
            
            newKeyProp.isExpanded = EditorGUI.Foldout(pos, newKeyProp.isExpanded, new GUIContent("Add new entry"), true);
            
            if (newKeyProp.isExpanded)
            {

                /*object o = property.GetValue();
                System.Type t = o.GetType();
                System.Reflection.PropertyInfo isValidProp=t.GetProperty("IsEditorAddKeyValid", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance);
                bool isValidKey=  (bool)isValidProp.GetValue(o,null);
                */
                bool isValidKey = property.FindPropertyRelative("isEditorAddKeyValid").boolValue;
                string keyType = property.FindPropertyRelative("keyTypeName").stringValue;
                string valueType = property.FindPropertyRelative("valueTypeName").stringValue;
                EditorGUI.indentLevel++;
                pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.PropertyField(pos, newKeyProp,new GUIContent("New entry key ("+ keyType + ")"),true);
                pos.y += keyHeight + EditorGUIUtility.standardVerticalSpacing; 
                
                EditorGUI.PropertyField(pos, newvalueProp, new GUIContent("New entry value(" + valueType + ")"),true);
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
                    SerializedProperty pairKListProperty = property.FindPropertyRelative("pairKList");
                    SerializedProperty pairVListProperty = property.FindPropertyRelative("pairVList");
                    int newIndex = pairKListProperty.arraySize++;
                    SerializedProperty newListElementKeyProperty = pairKListProperty.GetArrayElementAtIndex(newIndex);
                    TryCopyFromTo(newKeyProp, newListElementKeyProperty);
                    pairVListProperty.arraySize++;
                    SerializedProperty newListElementValueProperty = pairVListProperty.GetArrayElementAtIndex(newIndex);
                    TryCopyFromTo(newvalueProp, newListElementValueProperty);
                    property.serializedObject.ApplyModifiedProperties();
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

                //Debug.Log("post remove: pairKListProperty count " + pairKListProperty.arraySize);
                //Debug.Log("post remove: pairVListProperty count " + pairVListProperty.arraySize);
                property.serializedObject.ApplyModifiedProperties();
                //Debug.Log("post Apply: pairKListProperty count " + pairKListProperty.arraySize);
                //Debug.Log("post Apply: pairVListProperty count " + pairVListProperty.arraySize);
            }
        }

        /// <summary>
        /// draws a rect outline around the specified rect (inside rect only).  returns a rect that specifies the interior, non-outline rect.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        static Rect DrawRectOutline(Rect rect, Color color, float thickness = 1f)
        {
            // Top Edge
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            // Bottom Edge
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            // Left Edge
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            // Right Edge
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
            rect.xMin += thickness;
            rect.xMax -= thickness;
            rect.yMin += thickness;
            rect.yMax -= thickness;
            return rect;
        }

        /// <summary>
        /// Copies the value from one SerializedProperty (a) to another (b), ensuring the types match.
        /// </summary>
        /// <param name="a">The source SerializedProperty.</param>
        /// <param name="b">The destination SerializedProperty.</param>
        /// <returns>false if unable to copy for some reason</returns>
        public bool TryCopyFromTo(SerializedProperty a, SerializedProperty b)
        {
            if (a.propertyType != b.propertyType)// || a.isArray)
            {
//                Debug.LogWarning("SerializedProperty types do not match. Cannot copy values.");
                return false;
            }

            // Copy value based on type
            switch (a.propertyType)
            {
                case SerializedPropertyType.Integer:
                    b.intValue = a.intValue;
                    break;
                case SerializedPropertyType.Boolean:
                    b.boolValue = a.boolValue;
                    break;
                case SerializedPropertyType.Float:
                    b.floatValue = a.floatValue;
                    break;
                case SerializedPropertyType.String:
                    b.stringValue = a.stringValue;
                    break;
                case SerializedPropertyType.ObjectReference:
                    b.objectReferenceValue = a.objectReferenceValue;
                    break;
                case SerializedPropertyType.Enum:
                    b.enumValueIndex = a.enumValueIndex;
                    break;
                case SerializedPropertyType.Vector2:
                    b.vector2Value = a.vector2Value;
                    break;
                case SerializedPropertyType.Vector3:
                    b.vector3Value = a.vector3Value;
                    break;
                case SerializedPropertyType.Vector4:
                    b.vector4Value = a.vector4Value;
                    break;
                case SerializedPropertyType.Color:
                    b.colorValue = a.colorValue;
                    break;
                case SerializedPropertyType.Bounds:
                    b.boundsValue = a.boundsValue;
                    break;
                case SerializedPropertyType.Rect:
                    b.rectValue = a.rectValue;
                    break;
                case SerializedPropertyType.ArraySize:
                    b.arraySize = a.arraySize;
                    break;
                case SerializedPropertyType.Character:
                    b.stringValue = a.stringValue;
                    break;
                case SerializedPropertyType.LayerMask:
                    b.intValue = a.intValue;
                    break;
                case SerializedPropertyType.ManagedReference:
                    b.managedReferenceValue = a.managedReferenceValue;
                    break;
                case SerializedPropertyType.Vector2Int:
                    b.vector2IntValue = a.vector2IntValue;
                    break;
                case SerializedPropertyType.Vector3Int:
                    b.vector3IntValue = a.vector3IntValue;
                    break;
                case SerializedPropertyType.Quaternion:
                    b.quaternionValue = a.quaternionValue;
                    break;
                default:
                    //                  Debug.LogWarning("Unsupported property type: " + a.propertyType);
                    return false;
                    break;
            }
            return true;
        }
        
    }
}