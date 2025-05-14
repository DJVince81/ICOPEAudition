using Assets.Scripts.PatientData;
using Assets.Scripts.PatientData.AlgoData;
using Assets.Scripts.PatientData.Steps;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Managers
{
    public class StepManagerN : MonoBehaviour
    {
        [SerializeField] public Button returnButton;
        [SerializeField] public Button confirmNextButton;

        // GameObject
        [Header("Steps gameObject")]
        [SerializeField] public GameObject patientDisplay;
        [SerializeField] public List<GameObject> displayList;

        [Header("Question gameObject")]
        [SerializeField] public GameObject questionsDisplay;
        [SerializeField] public Button[] choiceButtons;

        [Header("Correction gameObject")]
        [SerializeField] public GameObject correctionDisplay;
        [SerializeField] public TextMeshProUGUI answerText;
        [SerializeField] public TextMeshProUGUI answerSelected;
        [SerializeField] public GameObject answerJustification;
        [SerializeField] public List<GameObject> answerGameObjectSprites;

        // Script
        private PatientPresentation patientPresentation;
        private PatientWisperTest patientWisperTest;

        private enum InteractionState { ISREADING, ISANSWERING, ISCORRECTION};
        private InteractionState interactionState;

        private enum AnswerState { DIAGNOSTIC, ACTION}
        private AnswerState answerState;

        private NewPatientData patientData;
        
        private int _currentStep;
        private bool _isDiagnosticValid;
        private bool _isActionValid;

        public void Initialize(NewPatientData patient)
        {
            patientData = patient;
        }

        public void LoadStep(int currentStep)
        {
            _currentStep = currentStep;

            _isDiagnosticValid = false;
            _isActionValid = false;

            ClearAllDisplay();
 
            switch (patientData.steps[_currentStep].type)
            {
                case Step.Case_presentation:
                    interactionState = InteractionState.ISREADING;
                    // WARNING - CRIME DE GUERRE
                    //patientDisplay.SetActive(true);
                    displayList[_currentStep].SetActive(true);
                    patientPresentation.SetTexts(patientData);
                    break;
                case Step.Wisper_test:
                    displayList[_currentStep].SetActive(true);
                    patientWisperTest.PlayAnimation();
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
            // set navigation button (Buttons)
            SetButtonsNavigation();
        }

        // SET TEXT AND INTERACTION 
        private void SetButtonsNavigation()
        {
            ClearAllListerner();
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
                // returnButton.enabled = false;
                returnButton.onClick.AddListener(BackToQuestion);
                confirmNextButton.onClick.AddListener(GoToNextStep);

                TextMeshProUGUI confirmeNextText = confirmNextButton.GetComponentInChildren<TextMeshProUGUI>();
                confirmeNextText.text = "Suivant";
            }
        }

        private void SetResponses()
        {
            if (patientData.steps[_currentStep].hasDiagnosticPhase)
            {
                answerState = AnswerState.DIAGNOSTIC;
                LoadPossibleResponses(patientData.steps[_currentStep].diagnosticPhase);
            }
            else if (patientData.steps[_currentStep].hasActionPhase)
            {
                answerState = AnswerState.ACTION;
                LoadPossibleResponses(patientData.steps[_currentStep].actionPhase);
            }
        }

        private void LoadPossibleResponses(PhaseData phaseData)
        {
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
            ClearAllDisplay();
            switch (patientData.steps[_currentStep].type)
            {
                case Step.Case_presentation:
                    questionsDisplay.SetActive(true);
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
        
        private void BackToQuestion()
        {
            ClearAllDisplay();
            questionsDisplay.SetActive(true);
        }
        private void BackToDocument()
        {
            ClearAllDisplay();
            switch (patientData.steps[_currentStep].type)
            {
                case Step.Case_presentation:
                    //patientDisplay.SetActive(true);
                    displayList[_currentStep].SetActive(true);
                    break;
                case Step.Wisper_test:
                    displayList[_currentStep].SetActive(true);
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

        private void ClearAllDisplay()
        {
            
            for(int i = 0; i < displayList.Count; i++)
            {
                displayList[i].SetActive(false);
            }

            questionsDisplay.SetActive(false);
            correctionDisplay.SetActive(false);
        }

        private void OnAnswerCorrect(PhaseData phaseData, int index)
        {
            string feedBackText = "Mauvaise réponse !";
            // DIAGNOSTIC CHOICE
            if (answerState == AnswerState.DIAGNOSTIC)
            {
                if (IsAnswerCorrect(phaseData.answerData, index))
                {
                    _isDiagnosticValid = true;
                    feedBackText = "Bonne réponse";
                }
                else _isDiagnosticValid = false;
            }
            // ACTION CHOICE
            if (answerState == AnswerState.ACTION)
            {
                if (IsAnswerCorrect(phaseData.answerData, index))
                {
                    _isActionValid = true;
                    feedBackText = "Bonne réponse";
                }
                else _isActionValid = false;
            }
            ShowAnswerDetail(phaseData.answerData[index], feedBackText);
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


        private void ShowAnswerDetail(AnswerData answer, string feedBackText)
        {
            interactionState = InteractionState.ISCORRECTION;

            // Load texts
            answerText.text = feedBackText;
            answerSelected.text = answer.answerText;
            // Load correction text if not null
            if (answer.correctionText != "")
            {
                answerJustification.GetComponent<TextMeshProUGUI>().text = answer.correctionText;
            }
            // Load image if not null
            if (answer.sprites.Count > 0)
            {
                int max = Mathf.Min(answer.sprites.Count, answerGameObjectSprites.Count);
                for (int i = 0; i < max; i++)
                {
                    answerGameObjectSprites[i].GetComponent<Image>().sprite = answer.sprites[i];
                    // Set gameobject actif
                    answerGameObjectSprites[i].SetActive(true);
                }
            }
            // Set GameObject active
            questionsDisplay.SetActive(false);
            correctionDisplay.SetActive(true);

            // Set bottom buttons
            SetButtonsNavigation();
        }

        private void ClearAllListerner()
        {
            returnButton.onClick.RemoveAllListeners();
            confirmNextButton.onClick.RemoveAllListeners();
        }

        private void GoToNextStep()
        {
            // Control if dignostic & action is completed
            if (IsStepCompleted(patientData.steps[_currentStep])) GameManager.Instance.GameStateManager.GetNextStep();
        }

        private bool IsStepCompleted(AlgoStep step)
        {
            bool diagnoticOK = !step.hasDiagnosticPhase || _isDiagnosticValid;
            bool actionOk = !step.hasActionPhase || _isActionValid;
            
            return diagnoticOK && actionOk;
            
        }

        private void Awake()
        {
     
            foreach(GameObject go in displayList)
            {
                if (go.TryGetComponent<PatientPresentation>(out PatientPresentation component)) patientPresentation = component;
                if (go.TryGetComponent<PatientWisperTest>(out PatientWisperTest component1)) patientWisperTest = component1;
                
            }
            //patientPresentation = patientDisplay.GetComponent<PatientPresentation>();
            //patientDisplay.GetComponent<T>();
        }
    }
}
