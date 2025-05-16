using Assets.Scripts.PatientData;
using Assets.Scripts.PatientData.AlgoData;
using Assets.Scripts.PatientData.Steps;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
        private Step1PresentationPatient step1PresentationPatient;
        private Step2WisperTest step2WisperTest;
        private Step3And4Questionnary step4And5Questionnary;
        private Step5Otoscopie Step5Otoscopie;
        private Step6WeberTest step6HhiesTest;
        private Step7HhiesTest step7HhiesTest;
        private Step8Audiometrie Step8Audiometrie;

        private enum InteractionState { ISREADING, ISANSWERING, ISCORRECTION};
        private InteractionState interactionState;

        private enum AnswerState { DIAGNOSTIC, ACTION} // TODO : Change to boolean
        private AnswerState answerState;

        private NewPatientData patientData;
        
        private int _currentStep;
        private int _currentDisplay;
        private bool _isDiagnosticValid;
        private bool _isActionValid;

        private Dictionary<Step, int> mappingDisplays;

        public void Initialize(NewPatientData patient)
        {
            patientData = patient;

            List<Step> lSteps = new List<Step>();
            foreach (var step in patientData.steps)
            {
                lSteps.Add(step.type);
            }

            mappingDisplays = MappingDisplay(lSteps);
        }

        private static Dictionary<Step, int> MappingDisplay<Step>(List<Step> filteredSteps) where Step : System.Enum
        {
            var dict = new Dictionary<Step, int>();
            for (int i = 0; i < filteredSteps.Count; i++)
            {
                dict[filteredSteps[i]] = i;
            }


            Debug.Log(dict);
            return dict;
        }


        public void LoadStep(int currentStep)
        {
            _currentStep = mappingDisplays[(Step) currentStep];

            Debug.Log("CurrentStep: " + _currentStep);

            interactionState = InteractionState.ISREADING;

            _isDiagnosticValid = false;
            _isActionValid = false;

            ClearAllDisplay();

            switch (patientData.steps[_currentStep].type)
            {
                case Step.Case_presentation:
                    // WARNING - CRIME DE GUERRE
                    step1PresentationPatient.SetPresentationTexts(patientData);
                    break;
                case Step.Wisper_test:
                    step2WisperTest.PlayFirstText(patientData.steps[_currentStep]);
                    break;
                case Step.Questionnary:
                    // Load questionary & answer data
                    List<QuestionData> questions = patientData.steps[_currentStep].questionnaireData.questions;
                    List<PatientQuestionAnswer> answers = patientData.steps[_currentStep].predefinedAnwser;
                    // Set texts
                    step4And5Questionnary.SetQuestionayText(questions, answers);
                    break;
                case Step.Additional_questionnaire:
                    break;
                case Step.Otoscopy:
                    Step5Otoscopie.SetImageOtoscopiePatient(patientData.steps[_currentStep].spriteEarExams);
                    break;
                case Step.Weber_test:
                    step6HhiesTest.SetTextDialogue(patientData.steps[_currentStep].contextDescription); 
                    break;
                case Step.HHIES_test:
                    step7HhiesTest.SetImageHHIES(patientData.steps[_currentStep].spriteEarExams);
                    break;
                case Step.Audiometry:
                    Step8Audiometrie.SetImageAudiometrie(patientData.steps[_currentStep].spriteEarExams);
                    break;
            }

            displayList[_currentStep].SetActive(true);
            // set navigation button (Buttons)
            SetTextButtonsNavigation();
        }

        // SET TEXT AND INTERACTION 
        private void SetTextButtonsNavigation()
        {
            TextMeshProUGUI confirmeNextText = confirmNextButton.GetComponentInChildren<TextMeshProUGUI>();
               
            if (interactionState == InteractionState.ISREADING)
            {
                // update text
                returnButton.interactable = false;
                confirmeNextText.text = "Répondre";
            }
            if (interactionState == InteractionState.ISANSWERING)
            {
                // Update text
                returnButton.interactable = true;
                confirmNextButton.interactable = true;
                confirmeNextText.text = "Confirmer";
            }            
            if ( interactionState == InteractionState.ISCORRECTION)
            {
                bool isValid = false;
                switch (answerState)
                {
                    case AnswerState.DIAGNOSTIC:
                        isValid = _isDiagnosticValid;
                        break;
                    case AnswerState.ACTION:  
                        isValid = _isActionValid;
                        break;
                }

                if (isValid)
                {
                    confirmeNextText.text = "Suivant";
                    confirmNextButton.interactable = true;
                    returnButton.interactable = false;
                }
                else
                {
                    confirmNextButton.interactable = false;
                    returnButton.interactable = true;
                }
            }
        }

        private void SetResponses()
        {
            if (patientData.steps[_currentStep].hasDiagnosticPhase && !_isDiagnosticValid)
            {
                answerState = AnswerState.DIAGNOSTIC;
                LoadPossibleResponses(patientData.steps[_currentStep].diagnosticPhase);
            } 
            else
            {
                _isDiagnosticValid = true;
            }

            if (patientData.steps[_currentStep].hasActionPhase && _isDiagnosticValid)
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
            interactionState = InteractionState.ISANSWERING;
            ClearAllDisplay();
            questionsDisplay.SetActive(true);
            SetResponses();
            SetTextButtonsNavigation();
        }

        private void BackToQuestion()
        {
            if (interactionState == InteractionState.ISCORRECTION)
            {
                ClearAllDisplay();
                
                interactionState = InteractionState.ISANSWERING;
                questionsDisplay.SetActive(true);
                SetTextButtonsNavigation();
            }
        }

        private void BackToDocument()
        {
            if (interactionState == InteractionState.ISANSWERING)
            {
                ClearAllDisplay();

                interactionState = InteractionState.ISREADING;
                displayList[_currentStep].SetActive(true);
                SetTextButtonsNavigation();
            }
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
                answerJustification.GetComponent<TextMeshProUGUI>().text = "<u>Justification :</u> " + answer.correctionText;
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
            SetTextButtonsNavigation();
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
            if (_isDiagnosticValid && answerState == AnswerState.DIAGNOSTIC)
            {
                GoToQuestionDisplay();
            }
            
        }

        private bool IsStepCompleted(AlgoStep step)
        {
            bool diagnoticOK = !step.hasDiagnosticPhase || _isDiagnosticValid;
           
            return diagnoticOK && _isActionValid;
        }

        private void Awake()
        {

            // WARNING : don't trigger error if component not found. 
            foreach(GameObject go in displayList)
            {
                if (go.TryGetComponent<Step1PresentationPatient>(out Step1PresentationPatient component)) step1PresentationPatient = component;
                if (go.TryGetComponent<Step2WisperTest>(out Step2WisperTest component1)) step2WisperTest = component1;
                if (go.TryGetComponent<Step3And4Questionnary>(out Step3And4Questionnary component2)) step4And5Questionnary = component2;
                if (go.TryGetComponent<Step5Otoscopie>(out Step5Otoscopie component3)) Step5Otoscopie = component3;
                if (go.TryGetComponent<Step6WeberTest>(out Step6WeberTest component4)) step6HhiesTest = component4;
                if (go.TryGetComponent<Step7HhiesTest>(out Step7HhiesTest component5)) step7HhiesTest = component5;
                if (go.TryGetComponent<Step8Audiometrie>(out Step8Audiometrie component6)) Step8Audiometrie = component6;
            }


            //SET LISTENER
            confirmNextButton.onClick.AddListener(GoToQuestionDisplay);
            confirmNextButton.onClick.AddListener(GoToNextStep);
            returnButton.onClick.AddListener(BackToDocument);
            returnButton.onClick.AddListener(BackToQuestion);

            //patientPresentation = patientDisplay.GetComponent<PatientPresentation>();
            //patientDisplay.GetComponent<T>();
        }

        /*
        private static T FindRequiredComponent<T>(GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
                return component;
            else
                Debug.LogError($"Error: Component of type {typeof(T).Name} not found, does component is attach to the GameObject {gameObject}");
        }
        */
    }
}
