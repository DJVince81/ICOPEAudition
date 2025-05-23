using Assets.Scripts.PatientData.AlgoData;
using Assets.Scripts.UI.TutorialContents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
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
            if (root == null || root.Name != "GameData") throw new Exception("Invalid XML format.");

            GlobalData gameData = new GlobalData
            {
                totGames = int.Parse(root["totGames"].InnerText),
                nbGameSession = int.Parse(root["nbGameSession"].InnerText),
                nbLevelsCompleted = int.Parse(root["nbLevelsCompleted"].InnerText),
                nbStepsCompleted = int.Parse(root["nbStepsCompleted"].InnerText),
                globalActionErrors = int.Parse(root["globalActionErrors"].InnerText),
                globalDiagnosticErrors = int.Parse(root["globalDiagnosticErrors"].InnerText),
                gameTime = float.Parse(root["gameTime"].InnerText),
                currentSessionTime = float.Parse(root["currentSessionTime"].InnerText),
                levelRecords = LoadLevelRecords(root.SelectSingleNode("levelRecords"))
            };

            return gameData;
        }

        public static TutorialEntry LoadTutoriaDataByID(string path, string stepName, int id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode node = doc.DocumentElement;
            if (node == null || node.Name != "Tutorial") throw new Exception("Invalid XML format.");

            node = doc.SelectSingleNode($"//Step[@Name='{stepName}']");
            if (node == null)
            {
                Debug.LogError($"Step '{stepName}' not found");
                return null;
            }

            TutorialEntry entry = new TutorialEntry();
            node = doc.SelectSingleNode($"//Entry[ID='{id}']");
            if (node != null)
            {
                entry.ID = int.Parse(node["ID"]?.InnerText);
                entry.Intitule = node["Intitule"]?.InnerText;
                entry.Text = node["Text"]?.InnerText;   
                return entry;
            }
            return null;
        }

        /// <summary>
        /// Valid if the xml document is comfort to xsd schema.
        /// To call on start of the game.
        /// </summary>
        /// <param name="pathXmlFile">Path to XML file</param>
        /// <param name="pathXsdFile">Path to XSD file</param>
        /// <returns>Boolean</returns>
        public static bool ValidateXML(string pathXmlFile, string pathXsdFile)
        {
            if (!File.Exists(pathXsdFile))
            {
                Debug.LogError("XSD file not found for validation");
                return false;
            }
            if (!File.Exists(pathXmlFile))
            {
                Debug.LogError("XML file not found for validation");
                return false;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pathXmlFile);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("", pathXsdFile);

            bool isValid = true;
            xmlDoc.Schemas = schemaSet;
            xmlDoc.Validate((sender, args) =>
            {
                if (args.Severity == XmlSeverityType.Error)
                {
                    Debug.LogError("XML validation error: " + args.Message);
                    isValid = false;
                }
            });
            return isValid;
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
                    timeSpentInLevel = float.Parse(levelNode["timeSpentInLevel"].InnerText),
                    stepRecords = LoadStepRecords(levelNode.SelectSingleNode("stepRecords"))
                };

                LevelState name = (LevelState)Enum.Parse(typeof(LevelState), levelNode.Name);
                levelRecords[name] = records;
            }
            return levelRecords;
        }

        /// <summary>
        /// Return a dictionary<AlgoState, StepRecords> (enum: AlgoState, struct: StepRecords) store in xml document recursively. Take xml node as input.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static Dictionary<Step, StepRecords> LoadStepRecords(XmlNode node)
        {
            Dictionary<Step, StepRecords> stepRecords = new Dictionary<Step, StepRecords>();
            foreach (XmlNode stepNode in node.ChildNodes)
            {
                StepRecords step = new StepRecords
                {
                    attempt = int.Parse(stepNode["attempt"].InnerText),
                    actionAnswer = LoadStringList(stepNode.SelectSingleNode("actionAnswer")),
                    diagnosticAnswer = LoadStringList(stepNode.SelectSingleNode("diagnosticAnswer"))
                };
                Step algoState = (Step)Enum.Parse(typeof(Step), stepNode.Name);
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
