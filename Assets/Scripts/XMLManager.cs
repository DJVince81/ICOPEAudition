using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public static class XmlManager
    {

        public static void SaveToXml(object data, string path, string rootName = "Root")
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement(rootName);
            xmlDoc.AppendChild(root);

            SerializedObject(xmlDoc, root, data);

            xmlDoc.Save(path);
            Debug.Log("File saved");
        }

        private static void SerializedObject(XmlDocument xmlDocument, XmlElement parentNode, object data)
        {
            if (data == null) return;

            Type objetType = data.GetType();
            if (objetType.IsPrimitive || objetType == typeof(string))
            {
                parentNode.InnerText = data.ToString();
            }
            else if (data is IDictionary dictionary)
            {
                foreach (var key in dictionary.Keys)
                {
                    XmlElement itemNode = xmlDocument.CreateElement(key.ToString());
                    SerializedObject(xmlDocument, itemNode, dictionary[key]);
                    parentNode.AppendChild(itemNode);
                }
            }
            else if (data is IEnumerable list)
            {
                foreach (var item in list)
                {
                    XmlElement itemNode = xmlDocument.CreateElement("Item");
                    SerializedObject(xmlDocument, itemNode, item);
                    parentNode.AppendChild(itemNode);
                }
            }
            else
            {
                foreach (FieldInfo field in objetType.GetFields())
                {
                    XmlElement fieldNode = xmlDocument.CreateElement(field.Name);
                    SerializedObject(xmlDocument, fieldNode, field.GetValue(data));
                    parentNode.AppendChild(fieldNode);
                }

                foreach (PropertyInfo property in objetType.GetProperties())
                {
                    if (property.CanRead)
                    {
                        XmlElement propNode = xmlDocument.CreateElement(property.Name);
                        SerializedObject(xmlDocument, propNode, property.GetValue(data));
                        parentNode.AppendChild(propNode);
                    }
                }
            }
        }














    }
}
