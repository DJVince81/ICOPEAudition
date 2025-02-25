using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

# region XML Class Declarations
public class TutorialStep
{
    [XmlElement("ID")]
    public int ID { get; set; }

    [XmlElement("Intitule")]
    public string Intitule { get; set; }
    [XmlElement("Text")]
    public string Text { get; set; }
}

public class Tutorial
{
    [XmlElement("Title")]
    public string Title { get; set; }
    [XmlElement("Step")]
    public List<TutorialStep> Steps { get; set; }
}
#endregion

public class XMLWriter : MonoBehaviour
{
    #region Private variable
    private string _xmlFilePath;
    private string _xsdFilePath;
    #endregion

    #region Private methods
    private void WriteXML()
    {
        Tutorial tutorial = new Tutorial
        {
            Title = "",
            Steps = new List<TutorialStep>
            {
                new TutorialStep {ID = 1, Intitule = "", Text = ""},
            }
        };

        XmlSerializer serializer = new XmlSerializer(typeof(Tutorial));
        using (StreamWriter writer = new StreamWriter(_xmlFilePath))
        {
            serializer.Serialize(writer, tutorial);
        }

        Debug.Log("Tutorial XML saved at: " + _xmlFilePath);
    }
    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _xmlFilePath = Path.Combine(Application.persistentDataPath, "Tutorial.xml");
        _xsdFilePath = Path.Combine(Application.streamingAssetsPath, "");

        WriteXML();
    }
}
