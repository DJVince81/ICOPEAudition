using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

[RequireComponent(typeof(Collider2D))]
public class Clickable : MonoBehaviour
{
    #region Inspector
    #if UNITY_EDITOR

    [CustomEditor(typeof(Clickable))]
    public class ArgsEditor : Editor
    {
        private Args newItem = new Args();
        private bool argsVisible = true;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Clickable clickable = (Clickable)target;
            clickable.objectName = EditorGUILayout.TextField("Name", clickable.objectName);
            clickable.type = EditorGUILayout.TextField("Type", clickable.type);

            // Adding function field
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Function", EditorStyles.boldLabel);
            clickable.functionScript = EditorGUILayout.ObjectField("", clickable.functionScript, typeof(MonoScript), false) as MonoScript;

            EditorGUILayout.Space();
            argsVisible = EditorGUILayout.BeginFoldoutHeaderGroup(argsVisible, "Properties");

            if (argsVisible)
            {
                for (int i = clickable.args.Count - 1; i >= 0; i--)
                {
                    Args arg = clickable.args[i];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(10));
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        clickable.args.RemoveAt(i);
                    }
                    EditorGUILayout.LabelField("" + i, GUILayout.Width(20));
                    arg.type = (Args.DataType)EditorGUILayout.EnumPopup("", arg.type, GUILayout.Width(80));
                    EditorGUILayout.LabelField("", GUILayout.Width(20));
                    switch (arg.type)
                    {
                        case Args.DataType.Int:
                            arg.intValue = EditorGUILayout.IntField(new GUIContent("Value"), arg.GetInt());
                            break;
                        case Args.DataType.String:
                            arg.stringValue = EditorGUILayout.TextField(new GUIContent("Value"), arg.GetString());
                            break;
                        case Args.DataType.Float:
                            arg.floatValue = EditorGUILayout.FloatField(new GUIContent("Value"), arg.GetFloat());
                            break;
                        case Args.DataType.Double:
                            arg.doubleValue = EditorGUILayout.DoubleField(new GUIContent("Value"), arg.GetFloat());
                            break;
                        case Args.DataType.Bool:
                            arg.boolValue = EditorGUILayout.Toggle(new GUIContent("Value"), arg.GetBool());
                            break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                // Ajouter un nouvel élément
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(10));
                if (GUILayout.Button("+", GUILayout.Width(43)))
                {
                    clickable.args.Add(newItem);
                    newItem = new Args();
                }
                newItem.type = (Args.DataType)EditorGUILayout.EnumPopup("", newItem.type, GUILayout.Width(80));
                EditorGUILayout.LabelField("Value", GUILayout.Width(40));
                switch (newItem.type)
                {
                    case Args.DataType.Int:
                        newItem.intValue = EditorGUILayout.IntField("", newItem.GetInt());
                        break;
                    case Args.DataType.String:
                        newItem.stringValue = EditorGUILayout.TextField("", newItem.GetString());
                        break;
                    case Args.DataType.Float:
                        newItem.floatValue = EditorGUILayout.FloatField("", newItem.GetFloat());
                        break;
                    case Args.DataType.Double:
                        newItem.doubleValue = EditorGUILayout.DoubleField("", newItem.GetDouble());
                        break;
                    case Args.DataType.Bool:
                        newItem.boolValue = EditorGUILayout.Toggle(newItem.GetBool());
                        break;
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            serializedObject.ApplyModifiedProperties();

            // Marquer l'objet comme modifié pour que les modifications soient sauvegardées
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
    #endif
    #endregion

    [Header("Properties")]
    [SerializeField] private string objectName;
    [SerializeField] private string type;

    [SerializeField] private MonoScript functionScript;
    [SerializeField] private BaseFunction function;

    [System.Serializable]
    public class Args
    {
        public enum DataType
        {
            Int,
            String,
            Float,
            Double,
            Bool
        }

        [SerializeField] internal DataType type;
        [SerializeField] internal string stringValue;
        [SerializeField] internal int intValue;
        [SerializeField] internal float floatValue;
        [SerializeField] internal double doubleValue;
        [SerializeField] internal bool boolValue;

        public string GetString() { return stringValue; }
        public int GetInt() { return intValue; }
        public float GetFloat() { return floatValue; }
        public double GetDouble() { return doubleValue; }
        public bool GetBool() { return boolValue; }
        public T GetValue<T>()
        {
            switch (type)
            {
                case DataType.Int:
                    if (typeof(T) == typeof(int))
                        return (T)(object)GetInt();
                    else if (typeof(T) == typeof(float))
                        return (T)(object)Convert.ToSingle(GetInt());
                    else if (typeof(T) == typeof(double))
                        return (T)(object)Convert.ToDouble(GetInt());
                    else if (typeof(T) == typeof(bool))
                        return (T)(object)Convert.ToBoolean(GetInt());
                    else if (typeof(T) == typeof(string))
                        return (T)(object)GetInt().ToString();
                    break;
                case DataType.Float:
                    if (typeof(T) == typeof(float))
                        return (T)(object)GetFloat();
                    else if (typeof(T) == typeof(double))
                        return (T)(object)Convert.ToDouble(GetFloat());
                    else if (typeof(T) == typeof(string))
                        return (T)(object)GetFloat().ToString();
                    break;
                case DataType.Double:
                    if (typeof(T) == typeof(double))
                        return (T)(object)GetDouble();
                    else if (typeof(T) == typeof(float))
                        return (T)(object)Convert.ToSingle(GetDouble());
                    else if (typeof(T) == typeof(string))
                        return (T)(object)GetDouble().ToString();
                    break;
                case DataType.String:
                    if (typeof(T) == typeof(string))
                        return (T)(object)GetString();
                    break;
                case DataType.Bool:
                    if (typeof(T) == typeof(bool))
                        return (T)(object)GetBool();
                    else if (typeof(T) == typeof(string))
                        return (T)(object)GetBool().ToString();
                    break;
                default:
                    return default(T);
            }
            return default(T);
        }
    }

    [SerializeField] private List<Args> args;

    private void Awake()
    {
        Type functionType = functionScript.GetClass();
        if (functionType != null && functionType.IsSubclassOf(typeof(BaseFunction))) function = (BaseFunction)gameObject.AddComponent(functionType);
    }

    void OnMouseDown()
    {
        Debug.Log("Nom : " + objectName);
        Debug.Log("Type : " + type);
        if (function != null)
        {
            function.Execute(args.ConvertAll<object>(x => x));
        }
    }
}
