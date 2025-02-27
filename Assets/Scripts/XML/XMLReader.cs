using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

#region Entry class def
/// <summary>
/// Class that define the content of XML we read
/// </summary>
public class Entry
{
    public int ID;
    public string Intitule;
    public string Text;
}
#endregion

public class XMLReader : MonoBehaviour
{
    #region Private variable
    // TO change for web version full path
    private string _xmlFilePath = "Assets/Resources/XML_Text/Tutorial.xml";
    private string _xsdFilePath = "Assets/Resources/XML_Text/TutorialSchema.xsd";
    #endregion

    #region Private methods
    /// <summary>
    /// Valid if the xml we have is comfort to the xsd schema.
    /// </summary>
    /// <returns>Boolean that validate the XML file</returns>
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
                    //XmlReader.ReadToFollowing(string s)
                }
            }
        }
        else
        {
            Debug.LogError("XML file not found :" + _xmlFilePath);
        }
    }
    /// <summary>
    /// Read XML entry that contais text and return it
    /// </summary>
    /// <remarks>
    /// In this case we read node ID, Intitule and text. Return Entry.ID (int), Entry.Intiutle (string) and Entry.Text (string)
    /// </remarks>
    /// <param name="reader"></param>
    /// <returns></returns>
    private Entry ReadEntryDetails(XmlReader reader)
    {
        Entry entry = new Entry();
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Entry") break;
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "ID":
                        reader.Read();
                        entry.ID = int.Parse(reader.Value);
                        break;
                    case "Intitule":
                        reader.Read();
                        entry.Intitule = reader.Value;
                        break;
                    case "Text":
                        reader.Read();
                        entry.Text = reader.Value;
                        break;
                }
            }
        }
        return entry;
    }

    #endregion

    #region Public methods
    public Entry readXmlStream(string stepName, int id)
    {
        if (!File.Exists(_xmlFilePath))
        { 
            Debug.LogError("XML file not found");
            return null;
        }
        if (!File.Exists(_xsdFilePath))
        {
            Debug.LogError("XSD file not found");
            return null;
        }

        using (XmlReader reader = XmlReader.Create(_xmlFilePath))
        {
            bool insideStep = false;
            
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Step")
                {
                    string nameAttr = reader.GetAttribute("Name");
                    insideStep = (nameAttr == stepName);
                }
                if(insideStep && reader.NodeType == XmlNodeType.Element && reader.Name == "Entry")
                {
                    Entry entry = ReadEntryDetails(reader);
                    if (entry != null && entry.ID == id)
                    {
                        return entry;
                    }
                }
            }
        }
        return null;
    }
    #endregion


    #region Unity Initialization
    void Start()
    {
        ValidateXML();
    }
    #endregion
}

