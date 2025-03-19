using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using UnityEngine;
using static Assets.Scripts.GameData;
using static Assets.Scripts.Managers.GameStateManager;

namespace Assets.Scripts
{
    /// <summary>
    /// XML Manager allow to save the data form the game and load-it.
    /// </summary>
    public static class XmlManager
    {
        /// <summary>
        /// Allow to save the data from the game. Take as input a object 'data' witch is globalData form GameData class, 
        /// a string path (where we want to save le file) and a string rootName (xml root).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="rootName"></param>
        public static void SaveToXml(object data, string path, string rootName = "Root")
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement(rootName);
            xmlDoc.AppendChild(root);

            SerializedObject(xmlDoc, root, data);

            xmlDoc.Save(path);
            Debug.Log("File saved");
        }

        /// <summary>
        /// Serialized and object to xml recursively.
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="parentNode"></param>
        /// <param name="data"></param>
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

        /// <summary>
        /// Deserialize a xml file. Return GlobalData struct. Take as input a path location of the file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>struct GlobalData</returns>
        /// <exception cref="Exception"></exception>
        public static GlobalData LoadGameData(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNode root = doc.DocumentElement;
            if (root == null || root.Name != "GameData") throw new Exception("Invalid XML format");

            GlobalData gameData = new GlobalData
            {
                nbGames = int.Parse(root["nbGames"].InnerText),
                nbLevelsCompleted = int.Parse(root["nbLevelsCompleted"].InnerText),
                nbStepsCompleted = int.Parse(root["nbStepsCompleted"].InnerText),
                globalActionErrors = int.Parse(root["globalActionErrors"].InnerText),
                globalDiagnosticErrors = int.Parse(root["globalDiagnosticErrors"].InnerText),
                gameTime = LoadTimerData(root.SelectSingleNode("gameTime")),
                currentSessionTime = LoadTimerData(root.SelectSingleNode("currentSessionTime")),
                levelRecords = LoadLevelRecords(root.SelectSingleNode("levelRecords"))
            };

            return gameData;
        }

        /// <summary>
        /// Return timer struct (startTime & elapsedTime). Take xml node as input.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static TimerData LoadTimerData(XmlNode node)
        {
            return new TimerData
            {
                startTime = float.Parse(node["startTime"].InnerText),
                elapsedTime = float.Parse(node["elapsedTime"].InnerText),
            };
        }

        /// <summary>
        /// Return a dictionary<LevelState, LevelRecords> (enum: LevelState, struct:LevelRecords) store in xml document recursively. Take xml node as input.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static Dictionary<LevelState, LevelRecords> LoadLevelRecords(XmlNode node)
        {
            Dictionary<LevelState, LevelRecords> levelRecords = new Dictionary<LevelState, LevelRecords>();
            foreach (XmlNode levelNode in node.ChildNodes)
            {
                LevelRecords records = new LevelRecords
                {
                    levelAttempt = int.Parse(levelNode["levelAttempt"].InnerText),
                    totActionError = int.Parse(levelNode["totActionError"].InnerText),
                    totDiagnosticError = int.Parse(levelNode["totDiagnosticError"].InnerText),
                    nbStepSucced = int.Parse(levelNode["nbStepSucced"].InnerText),
                    nbStepFailed = int.Parse(levelNode["nbStepFailed"].InnerText),
                    timeSpentInLevel = LoadTimerData(levelNode["levelTime"]),
                    stepRecords = LoadStepRecords(levelNode.SelectSingleNode("stepRecords"))
                };

                LevelState name = (LevelState) Enum.Parse(typeof(LevelState), levelNode.Name);
                levelRecords[name] = records;
            }
            return levelRecords;
        }

        /// <summary>
        /// Return a dictionary<AlgoState, StepRecords> (enum: AlgoState, struct: StepRecords) store in xml document recursively. Take xml node as input.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static Dictionary<AlgoState, StepRecords> LoadStepRecords(XmlNode node)
        {
            Dictionary<AlgoState, StepRecords> stepRecords = new Dictionary<AlgoState, StepRecords>();
            foreach (XmlNode stepNode in node.ChildNodes)
            {
                StepRecords step = new StepRecords
                {
                    attempt = int.Parse(stepNode["attempt"].InnerText),
                    actionError = LoadStringList(stepNode.SelectSingleNode("actionError")),
                    diagnosticError = LoadStringList(stepNode.SelectSingleNode("diagnosticError"))
                };
                AlgoState algoState = (AlgoState)Enum.Parse(typeof(AlgoState), stepNode.Name); 
                stepRecords[algoState] = step;
            }
            return stepRecords;
        }

        /// <summary>
        /// Return List<string> store in xml document in the "item" section. Take xml node as input.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static List<String> LoadStringList(XmlNode node)
        {
            List<String> list = new List<String>();
            foreach (XmlNode item in node.ChildNodes)
            {
                list.Add(item.InnerText);
            }
            return list;
        }
    }
}
