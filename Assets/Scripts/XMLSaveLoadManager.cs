using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using static Assets.Scripts.PlayerData;
using static Assets.Scripts.Managers.GameStateManager;



namespace Assets.Scripts
{

    [XmlRoot("IUD")]
    public class SaveData
    {
        [XmlElement("id")]
        public string UID { get; set; }
        [XmlElement("DataPlayer")]
        public Data Data{ get; set; }
    }

    public class Data
    {
        [XmlElement("GlobalData")]
        public GlobalData globalData { get; set; }

        [XmlElement("LevelData"), XmlArrayItem("Level")]
        public List<LevelEntry> LevelEntries { get; set; } = new List<LevelEntry>();

        [XmlElement("StepData"), XmlArrayItem("Step")]
        public List<StepEntry> StepEntries { get; set; } = new List<StepEntry>();
    }

    public class StepEntry
    {
        [XmlAttribute("id")]
        public AlgoState Step { get; set; }
        [XmlAttribute("Records")]
        public StepRecord Records { get; set; }
    }

    public class LevelEntry
    {
        [XmlAttribute("id")]
        public int Level { get; set; }
        [XmlAttribute("Records")]
        public LevelRecords Records { get; set; }
    }

    public class StepRecord
    {
        public int attempt;
        [XmlArray("actionError"), XmlArrayItem("Error")]
        public List<int> actionError = new List<int>();
        [XmlArray("diagnosticError"), XmlArrayItem("Error")]
        public List<int> diagnosticError = new List<int>();
    }

    public static class XmlSaveLoadManager
    {        
        public static void SaveToXml(SaveData data, string fileName = "saveData")
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(stream, data);
            }
        }

        public static SaveData LoadFromXml(string fileName = "saveData")
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            if (!File.Exists(filePath)) return null;
            
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return serializer.Deserialize(stream) as SaveData;
            }
        }
    }
}
