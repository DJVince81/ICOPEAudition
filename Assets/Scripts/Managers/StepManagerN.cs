using Assets.Script.PatientData.Steps;
using Assets.Scripts.PatientData;
using Assets.Scripts.PatientData.AlgoData;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Managers
{
    public class StepManagerN : MonoBehaviour
    {
        [SerializeField] public Button[] choiceButtons;
        [SerializeField] public Button returnButton;
        [SerializeField] public Button confirmNextButton;

        // GameObject
        [Header("Step gameObject")]
        [SerializeField] public GameObject patientDisplay;


        [Header("Question gameObject")]
        [SerializeField] public GameObject questionDisplay;

        // Script
        private PatientPresentation patientPresentation;

        private enum InteractionState { ISREADING, ISANSWERING, ISCORRECTION};
        private InteractionState interactionState;

        private enum AnswerState { DIAGNOTIC, ACTION}
        private AnswerState answerState;

        private NewPatientData patientData;
        
        private int _currentStep;
        private bool _isDiagnoticValid;
        private bool _isActionValid;

        public void Initialize(NewPatientData patient)
        {
            patientData = patient;
        }

        public void LoadStep(int currentStep)
        {
            _currentStep = currentStep;

            _isDiagnoticValid = false;
            _isActionValid = false;

            // Set Text
            switch (patientData.steps[_currentStep].type)
            {
                case Step.Case_presentation:
                    interactionState = InteractionState.ISREADING;
                    patientPresentation.SetTexts(patientData);
                    SetButtonsNavigation();
                    // set navigation button (Buttons)
                    break;
                case Step.Wisper_test:
                    // call wisper test scritp
                    break;
                case Step.Questionnary:
                    // call Questionnary
                    break;
                case Step.Additional_questionnaire:
                    break;
                case Step.Otoscopy:
                    break;
                case Step.Weber_test:
                    break;
                case Step.HHIES_test:
                    break;
                case Step.Audiometry:
                    break;
            }
        }

        // SET TEXT AND INTERACTION 
        private void SetButtonsNavigation()
        {
            if (interactionState == InteractionState.ISREADING)
            {
                // update text
                TextMeshProUGUI confirmeNextText = confirmNextButton.GetComponentInChildren<TextMeshProUGUI>();
                confirmeNextText.text = "Répondre";
                // Add listerner
                confirmNextButton.onClick.AddListener(GoToQuestionDisplay);
                // enabled buttons
                confirmNextButton.enabled = true;
                returnButton.enabled = false;
            }
            if (interactionState == InteractionState.ISANSWERING)
            {
                // Update text
                TextMeshProUGUI confirmeNextText = confirmNextButton.GetComponentInChildren<TextMeshProUGUI>();
                confirmeNextText.text = "Confirmer";
                // add listerner
                returnButton.onClick.AddListener(BackToDocument);
                // enbled buttons
                returnButton.enabled = true;
            }
            if (interactionState == InteractionState.ISCORRECTION)
            {
                returnButton.enabled = false;
                TextMeshProUGUI confirmeNextText = confirmNextButton.GetComponentInChildren<TextMeshProUGUI>();
                confirmeNextText.text = "Suivant";
            }
        }

        private void SetResponses()
        {
            if (patientData.steps[_currentStep].hasDiagnosticPhase)
            {
                LoadPossibleResponses(patientData.steps[_currentStep].diagnosticPhase);
                answerState = AnswerState.DIAGNOTIC;
            }
            if (patientData.steps[_currentStep].hasActionPhase)
            {
                LoadPossibleResponses(patientData.steps[_currentStep].actionPhase);
                answerState = AnswerState.ACTION;
            }
        }

        private void LoadPossibleResponses(PhaseData phaseData)
        {
            //Debug.Log(phaseData.answerData.Count + " COUNT " + choiceButtons.Length);
            int max = Mathf.Min(phaseData.answerData.Count, choiceButtons.Length);

            for (int i = 0; i < max; i++)
            {
                int index = i;
                TextMeshProUGUI text = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();                
                text.text = phaseData.answerData[i].answerText;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnAnswerCorrect(phaseData, index));
                choiceButtons[i].interactable = true;
                choiceButtons[i].enabled = true;
            }
        }

        private void GoToQuestionDisplay()
        {
            switch (patientData.steps[_currentStep].type)
            {
                case Step.Case_presentation:
                    patientDisplay.SetActive(false);
                    questionDisplay.SetActive(true);
                    SetResponses();
                    break;
                case Step.Wisper_test:
                    break;
                case Step.Questionnary:
                    break;
                case Step.Additional_questionnaire:
                    break;
                case Step.Otoscopy:
                    break;
                case Step.Weber_test:
                    break;
                case Step.HHIES_test:
                    break;
                case Step.Audiometry:
                    break;
            }
            interactionState = InteractionState.ISANSWERING;
            SetButtonsNavigation();
        }

        private void BackToDocument()
        {
            //Debug.Log(patientData.steps[_currentStep].type);
            switch (patientData.steps[_currentStep].type)
            {
                case Step.Case_presentation:
                    patientDisplay.SetActive(true);
                    questionDisplay.SetActive(false);
                    break;
                case Step.Wisper_test:
                    break;
                case Step.Questionnary:
                    break;
                case Step.Additional_questionnaire:
                    break;
                case Step.Otoscopy:
                    break;
                case Step.Weber_test:
                    break;
                case Step.HHIES_test:
                    break;
                case Step.Audiometry:
                    break;
            }
            interactionState = InteractionState.ISREADING;
            SetButtonsNavigation();
        }

        private static void OnAnswerCorrect(PhaseData phaseData, int index)
        {
            if (IsAnswerCorrect(phaseData.answerData, index))                
                Debug.Log("Correct !");
            else
                Debug.Log("Faux");
        }

        private static bool IsAnswerCorrect(List<AnswerData> answerData, int index)
        {
            if (index < 0 || index >= answerData.Count)
            {
                Debug.LogError("index out of bound");
                return false;
            }
            return answerData[index].isCorrect;
        }


        private void ShowAnswerDetail()
        {
            //ToDo
        }


        private void GoToNextStep()
        {
            // Change status in GameStateManager
        }




        private void Awake()
        {
            patientPresentation = patientDisplay.GetComponent<PatientPresentation>();
        }
    }

}
