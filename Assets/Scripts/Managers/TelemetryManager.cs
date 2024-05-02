using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class TelemetryManager : MonoBehaviour
{
    [SerializeField] private string serverURL = "https://icope.rodriguez-vincent.fr/";
    [SerializeField] private string datasManagerPHP = "datas_manager.php";
    [SerializeField] private string uuidPHP = "uuid.php";

    XmlDocument xmlDocument = null;
    private string uuid;
    private int nbWins, nbGames;
    private List<int> nbShowSteps = new();
    private List<int> nbLosesStepsDiag = new();
    private List<int> nbLosesStepsAction = new();
    private double gameTime;

    IEnumerator Start()
    {
        uuid = PlayerPrefs.GetString("UUID", string.Empty);
        if (uuid == string.Empty)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get($"{serverURL}{uuidPHP}");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                uuid = webRequest.downloadHandler.text;
                PlayerPrefs.SetString("UUID", uuid);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.LogError("Erreur lors de la récupération de l'UUID : " + webRequest.error);
            }
        }
        nbGames = 7;
        nbWins = 4;
        for (int i = 0; i < 5; i++)
        {
            nbShowSteps.Add(5);
            nbLosesStepsDiag.Add(i);
            nbLosesStepsAction.Add(i + 10);
        }
        gameTime = 724.82;
    }

    public void SaveDatas()
    {
        if (nbGames > 0)
        {
            /*
             * Nombre de patients traités
             * Pourcentage réussite
             * Nombre erreurs étape 1
             * Nombre erreurs étape 2
             * Nombre erreurs étape 3
             * Nombre erreurs étape 4
             * Nombre erreurs étape 5
             * Temps passé sur le jeu
             * Questionnaire de satisfaction (Q1)
             * Questionnaire de satisfaction (Q2)
             * Questionnaire de satisfaction (Q3)
             * Questionnaire de satisfaction (Q4)
             */
            object[] dataList =
            {
                nbGames,
                (double) nbWins/nbGames * 100,
                nbLosesStepsDiag[0],
                nbLosesStepsAction[0],
                nbLosesStepsDiag[1],
                nbLosesStepsAction[1],
                nbLosesStepsDiag[2],
                nbLosesStepsAction[2],
                nbLosesStepsDiag[3],
                nbLosesStepsAction[3],
                nbLosesStepsDiag[4],
                nbLosesStepsAction[4],
                gameTime,
                "Answer 1",
                "Answer 2",
                "Answer 3",
                "Answer 4",
            };
            Debug.Log(dataList);
            xmlDocument = ConvertToXML(dataList);
            SaveToServer();
        }
    }

    public XmlDocument ConvertToXML(object[] dataList)
    {
        return ConvertToXML(new List<object>(dataList));
    }

    public XmlDocument ConvertToXML(List<object> dataList)
    {
        StringBuilder xmlBuilder = new();

        xmlBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        xmlBuilder.AppendLine("<Root>");
        xmlBuilder.AppendLine($"\t<Data uuid=\"{uuid}\">");

        foreach (object data in dataList)
        {
            xmlBuilder.AppendLine($"\t\t<Item Type=\"{data.GetType()}\">");

            string dataString = data switch
            {
                int intValue => intValue.ToString(),
                float floatValue => floatValue.ToString(),
                double doubleValue => doubleValue.ToString(),
                string stringValue => stringValue,
                bool boolValue => boolValue.ToString(),
                DateTime dateTimeValue => dateTimeValue.ToString("o"),
                _ => null // Gérer les autres types d'objets
            };

            if (dataString != null)
            {
                xmlBuilder.AppendLine($"\t\t\t{dataString}");
            }

            xmlBuilder.AppendLine("\t\t</Item>");
        }

        xmlBuilder.AppendLine("\t</Data>");
        xmlBuilder.AppendLine("</Root>");

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

        using UnityWebRequest request = new($"{serverURL}{datasManagerPHP}", "POST");
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
