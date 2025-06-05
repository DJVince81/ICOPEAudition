using Assets.Scripts.Managers;
using Assets.Scripts.PatientData.AlgoData;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.GameData;

namespace Assets.Scripts.UI
{
    [System.Serializable]
    public enum FieldsName { 
        PatientName, 
        StepSuccess, 
        StepFailed, 
        DiagnoticsSuccess, 
        DiagnosticsFailed, 
        ActionSuccess, 
        ActionFailed, 
        Scores,
        SuccessRate,
        StepName,
        DiangnoticsDetails,
        ActionsDetails
    }

    
    [System.Serializable]
    public class FieldsTable
    {
        public FieldsName fieldsName;
        public TextMeshProUGUI fields;
    }

    [System.Serializable]
    public class DetailsFields
    {
        public FieldsName fieldsName;
        public TextMeshProUGUI fields;
        public Image image;
    }


    public class PatientScoreManager : MonoBehaviour
    {
        [Header("Section synthese patient")]
        [SerializeField] private List<FieldsTable> fieldsList;

        [Header("Section details patient")]
        [SerializeField] private GameObject detailsGameObject;
        [SerializeField] private List<DetailsFields> answerFieldsList;

        [Header("Score calcul")]
        [SerializeField] private static int _stepMulticateur = 10;
        [SerializeField] private static int _diagActionMulticateur = 5;

        [Header("Details image sprite")]
        [SerializeField] private Sprite _correctSprite;
        [SerializeField] private Sprite _wrongSprite;


        // ---- PRIVATES VARIABLES ----
        private string _currentPatientName;
        private int _currentStepToDisplay;
        private bool _isDetailsActive = false;
        private List<Step> stepsList;

        private static int CalculateTotalScore(int nbStepSucc, int nbStepFailed, int nbDiagSucc, int nbDiagFailed, int nbActionSucc, int nbActionFailed) 
        {  
            return nbStepSucc * _stepMulticateur - nbStepFailed * _stepMulticateur + nbDiagSucc * _diagActionMulticateur - nbDiagFailed * _diagActionMulticateur + nbActionSucc * _diagActionMulticateur + nbActionFailed * _diagActionMulticateur;
        }

        private void SetSyntheseScore(string patientName, int nbStepSucc, int nbStepFailed, int nbDiagSucc, int nbDiagFailed, int nbActionSucc, int nbActionFailed, float succesRate)
        {
            foreach (FieldsTable fieldsTable in fieldsList)
            {
                switch (fieldsTable.fieldsName)
                {
                    case FieldsName.PatientName:
                        fieldsTable.fields.text = patientName;
                        break;
                    case FieldsName.StepSuccess:
                        fieldsTable.fields.text = nbStepSucc.ToString();
                        break;
                    case FieldsName.StepFailed:
                        fieldsTable.fields.text = nbStepFailed.ToString();
                        break; 
                    case FieldsName.DiagnoticsSuccess:
                        fieldsTable.fields.text = nbDiagSucc.ToString();
                        break;
                    case FieldsName.DiagnosticsFailed:
                        fieldsTable.fields.text = nbDiagFailed.ToString();
                        break;
                    case FieldsName.ActionSuccess: 
                        fieldsTable.fields.text = nbActionSucc.ToString();
                        break;
                    case FieldsName.ActionFailed: 
                        fieldsTable.fields.text = nbActionFailed.ToString();
                        break;
                    case FieldsName.Scores:
                        fieldsTable.fields.text = CalculateTotalScore(nbStepSucc, nbStepFailed, nbDiagSucc, nbDiagFailed, nbActionSucc, nbActionFailed).ToString();
                        break;
                    case FieldsName.SuccessRate:
                        fieldsTable.fields.text = succesRate.ToString();
                        break;
                }
            }
        }

        public void GetSetDisplayScore(string patientName)
        {
            stepsList = new List<Step>();
            // Get Patient data form patient score
            var pRecords = GameManager.Instance.GameData.GetPatientCaseRecords(patientName);
            _currentPatientName = patientName;
            _currentStepToDisplay = 0;

            var keys = pRecords.stepRecords.Keys;
            foreach (var key in keys)
            {
                stepsList.Add(key);
            }

            SetSyntheseScore(patientName, pRecords.numberStepSucceed, pRecords.numberStepFailed, pRecords.numberDiagCorrect, pRecords.numberDiagIncorrect, pRecords.numberActionCorrect, pRecords.numberActionIncorrect, pRecords.successRate);
            SetStepDetailsText(stepsList[_currentStepToDisplay].ToString(), pRecords.stepRecords[stepsList[_currentStepToDisplay]].diagnosticAnswer, pRecords.stepRecords[stepsList[_currentStepToDisplay]].actionAnswer);
        }

        private void SetStepDetailsText(string stepName, List<string> diagAnswer, List<string> actionAnswer)
        {
            ClearAnswersDetails();

            int indexDiagnotics = 0;
            int indexAction = 0;

            // List of answer to enable or disable
            foreach(DetailsFields fieldsTable in answerFieldsList)
            {
                if (fieldsTable.fieldsName == FieldsName.StepName)
                {
                    fieldsTable.fields.text = stepName;
                }
                else if (fieldsTable.fieldsName == FieldsName.DiangnoticsDetails && indexDiagnotics < diagAnswer.Count)
                {
                    fieldsTable.fields.text = diagAnswer[indexDiagnotics];
                    LayoutRebuilder.ForceRebuildLayoutImmediate(fieldsTable.fields.rectTransform);
                    // Set image (error / correct) last anwser = correct, other false
                    if (indexDiagnotics == diagAnswer.Count - 1) fieldsTable.image.sprite = _correctSprite; // Set correct image
                    else fieldsTable.image.sprite = _wrongSprite;
                    fieldsTable.fields.transform.parent.gameObject.SetActive(true);
                    indexDiagnotics++;
                }
                else if (fieldsTable.fieldsName == FieldsName.ActionsDetails && indexAction < actionAnswer.Count)
                {
                    fieldsTable.fields.text = actionAnswer[indexAction];
                    // Set image (error / correct) last anwser = correct, other false
                    if (indexAction == actionAnswer.Count - 1) fieldsTable.image.sprite = _correctSprite;
                    else fieldsTable.image.sprite = _wrongSprite;
                    fieldsTable.fields.transform.parent.gameObject.SetActive(true);
                    indexAction++;
                }
            }
        }

        private void ClearAnswersDetails()
        {
            // Clear anwser details ...
            foreach (DetailsFields fieldsTable in answerFieldsList)
            {
                if (fieldsTable.fieldsName == FieldsName.DiangnoticsDetails || fieldsTable.fieldsName == FieldsName.ActionsDetails)
                    fieldsTable.fields.transform.parent.gameObject.SetActive(false);
            }
        }

        //CALL BY 'DETAILS' BUTTON FROM SCORE CONTENT
        public void ShowCloseDetailsSection()
        {
            _isDetailsActive = !_isDetailsActive;
            // Always hide by default but show it when player it details button
            detailsGameObject.SetActive(_isDetailsActive);
            // Play animation
        }

        //CALL BY 'NEXT' BUTTON FROM DETAILS CONTENTS
        public void ShowNextStepDetails()
        {
            _currentStepToDisplay++;
            var pRecords = GameManager.Instance.GameData.GetPatientCaseRecords(_currentPatientName);
            if (_currentStepToDisplay < pRecords.stepRecords.Count)
                SetStepDetailsText(stepsList[_currentStepToDisplay].ToString(), pRecords.stepRecords[stepsList[_currentStepToDisplay]].diagnosticAnswer, pRecords.stepRecords[stepsList[_currentStepToDisplay]].actionAnswer);
            else
                _currentStepToDisplay = pRecords.stepRecords.Count - 1;
        }

        //CALL BY 'PREVIOUS' BUTTON FROM DETAILS CONTENTS
        public void ShowPreviousStepDetails()
        {
            _currentStepToDisplay--;
            var pRecords = GameManager.Instance.GameData.GetPatientCaseRecords(_currentPatientName);
            if (_currentStepToDisplay >= 0 )
                SetStepDetailsText(stepsList[_currentStepToDisplay].ToString(), pRecords.stepRecords[stepsList[_currentStepToDisplay]].diagnosticAnswer, pRecords.stepRecords[stepsList[_currentStepToDisplay]].actionAnswer);
            else
                _currentStepToDisplay = 0;
        }

        //CALL BY 'RETURN TO WAITING ROOM' BUTTON FROM SCORE CONTENT
        public static void GoToMenu()
        {
            GameManager.Instance.GameStateManager.NextPatientCase();
        }
    }
}