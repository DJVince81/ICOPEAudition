using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using static Assets.Scripts.PlayerData;



namespace Assets.Scripts
{

    [XmlRoot("IUD")]
    public class SaveData
    {
        [XmlElement("id")]
        public string UID { get; set; }

        [XmlElement("GlobalData")]
        public GlobalData globalData { get; set; }

        [XmlElement("LevelData")]
        public Dictionary<int, LevelRecord> levelRecords { get; set; }

        [XmlElement("StepData")]
        public Dictionary<int, StepRecords> stepRecords { get; set; }
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
