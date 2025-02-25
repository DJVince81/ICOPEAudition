using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class XMLReader : MonoBehaviour
{
    #region Private variable
    private string _xmlFilePath;
    private string _xsdFilePath;
    #endregion

    #region Private methods
    private bool ValidateXML()
    {
        if (!File.Exists(_xmlFilePath))
        {
            Debug.LogError("XML file not found for validation");
            return false;
        }
        if (!File.Exists(_xsdFilePath))
        {
            Debug.LogError("XSD file not found for validation");
            return false;
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(_xmlFilePath);

        XmlSchemaSet schemaSet = new XmlSchemaSet();
        schemaSet.Add("", _xsdFilePath);

        bool isValid = true;
        xmlDoc.Schemas = schemaSet;
        xmlDoc.Validate((sender, args) =>
        {
            if (args.Severity == XmlSeverityType.Error)
            {
                Debug.LogError("XML Validation Error: " + args.Message);
                isValid = false;
            }
        });
        return isValid;
    }

    private void ReadXML()
    {
        if (File.Exists(_xmlFilePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Tutorial));
            using (StreamReader reader = new StreamReader(_xmlFilePath))
            {
                Tutorial tutorial = (Tutorial)serializer.Deserialize(reader);
                foreach (var step in tutorial.Steps)
                {
                    //Set in TMP_Pro (pass in param container TMP_pro)
                }
            }
        }
        else
        {
            Debug.LogError("XML file not found :" + _xmlFilePath);
        }
    }
    #endregion

    #region Unity Initialization
    void Start()
    {
        _xmlFilePath = Path.Combine(Application.persistentDataPath, "Resources/XML Text/Tutorial.xml");
        _xsdFilePath = Path.Combine(Application.streamingAssetsPath, "Resources/XML Text/");

        if (ValidateXML())
        {
            ReadXML();
        }
    }
    #endregion
}
