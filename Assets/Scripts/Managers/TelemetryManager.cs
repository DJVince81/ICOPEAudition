using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public class TelemetryManager : MonoBehaviour
{
    [SerializeField] private string serverURL = "https://icope.rodriguez-vincent.fr/datas_manager.php";

    XmlDocument xmlDocument = null;

    public void TestXMLDocument()
    {
        object[] dataList =
        {
            (int) 42,
            (string) "hello world",
            (string) "caractères spéciaux @.",
            (bool) true,
            (DateTime) DateTime.Now,
            (float) 0.51f,
        };
        xmlDocument = ConvertToXML(dataList);
        SaveToServer();
    }

    public XmlDocument ConvertToXML(object[] dataList)
    {
        return ConvertToXML(new List<object>(dataList));
    }

    public XmlDocument ConvertToXML(List<object> dataList)
    {
        StringBuilder xmlBuilder = new();

        xmlBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        xmlBuilder.AppendLine("<Data>");

        foreach (object data in dataList)
        {
            xmlBuilder.AppendLine("\t<Item Type=\"" + data.GetType().ToString() + "\">");

            string dataString = data switch
            {
                int intValue => intValue.ToString(),
                float floatValue => floatValue.ToString(),
                string stringValue => stringValue,
                bool boolValue => boolValue.ToString(),
                DateTime dateTimeValue => dateTimeValue.ToString("o"),
                _ => null // Gérer les autres types d'objets
            };

            if (dataString != null)
            {
                xmlBuilder.AppendLine("\t\t" + dataString);
            }

            xmlBuilder.AppendLine("\t</Item>");
        }

        xmlBuilder.AppendLine("</Data>");

        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(xmlBuilder.ToString());
        return xmlDoc;
    }

    public void SaveToServer()
    {
        if (xmlDocument == null) return;
        StartCoroutine(UploadXML(xmlDocument));
    }

    private IEnumerator UploadXML(XmlDocument xmlDocument)
    {
        string xmlString = xmlDocument.OuterXml;

        using UnityWebRequest request = new(serverURL, "POST");
        byte[] xmlData = Encoding.UTF8.GetBytes(xmlString);
        request.uploadHandler = new UploadHandlerRaw(xmlData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/xml");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error sending XML file to server: " + request.error);
        }
        else
        {
            Debug.Log("XML file successfully sent to server.");
            Debug.Log("Server response : " + request.downloadHandler.text);
        }
    }
}
