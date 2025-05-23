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
        [SerializeField] private Button returnButton;
        [SerializeField] private Button confirmNextButton;

        // GameObject
        [Header("Steps gameObject")]
        [SerializeField] private GameObject patientDisplay;
        [SerializeField] private List<GameObject> displayList;

        [Header("Question gameObject")]
        [SerializeField] private GameObject questionsDisplay;
        [SerializeField] private Button[] choiceButtons;

        [Header("Correction gameObject")]
        [SerializeField] private GameObject correctionDisplay;
        [SerializeField] private TextMeshProUGUI answerText;
        [SerializeField] private TextMeshProUGUI answerSelected;
        [SerializeField] private GameObject answerJustification;
        [SerializeField] private List<GameObject> answerGameObjectSprites;

        [Header("Colors answer")]
        [SerializeField] private Color correctColor;
        [SerializeField] private Color incorrectColor;
        [SerializeField] private Image backgroudAnswer;

        // Sprite Doctor (1st position happy expression, 2nd position sad expression, 3rd position talking)
        // For the future to change to allow player to choose his character.
        [Header("Sprite doctor")]
        [SerializeField] private Sprite[] doctorSprite;

        [Header("GameObject Image correction")]
        [SerializeField] private Image doctorExpressionsImages;
        
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
        private bool _isDiagnosticValid;
        private bool _isActionValid;

        private Dictionary<Step, int> mappingDisplays;

        public void Initialize(NewPatientData newPatient)
        {
            _currentStep = 0; // reset current step to 0
            patientData = newPatient;
        }


        public void LoadStep(Step currentStep)
        {
            ClearAllDisplay();
            
            interactionState = InteractionState.ISREADING;

            // Set bools to fasle each step
            _isDiagnosticValid = false;
            _isActionValid = false;

            switch (currentStep)
            {
                case Step.Case_presentation:
                    // Load patient sprite & patient text
                    step1PresentationPatient.SetSprites(patientData.characterSprites[0]);
                    step1PresentationPatient.SetPresentationTexts(patientData);
                    // Display current step
                    displayList[0].SetActive(true);
                    break;
                case Step.Wisper_test:
                    // Load wisper text (animation with dotween)
                    step2WisperTest.PlayFirstText(patientData.steps[_currentStep]);
                    // Display current step
                    displayList[1].SetActive(true);
                    break;
                case Step.Questionnary:
                    // Load questionary & answer
                    List<QuestionData> questions = patientData.steps[_currentStep].questionnaireData.questions;
                    List<PatientQuestionAnswer> answers = patientData.steps[_currentStep].predefinedAnwser;
                    // Set texts
                    step4And5Questionnary.SetQuestionayText(questions, answers);
                    //Set Patient Sprite
                    step4And5Questionnary.SetPatientSprite(patientData.characterSprites[0]);
                    // Display current step
                    displayList[2].SetActive(true);
                    break;
                case Step.Additional_questionnaire:
                    // Load questionary & answer
                    List<QuestionData> questions2 = patientData.steps[_currentStep].questionnaireData.questions;
                    List<PatientQuestionAnswer> answers2 = patientData.steps[_currentStep].predefinedAnwser;
                    step4And5Questionnary.SetQuestionayText(questions2, answers2);
                    // Display current step
                    displayList[2].SetActive(true);
                    break;
                case Step.Otoscopy:
                    // Load patient ear image
                    Step5Otoscopie.SetImageOtoscopiePatient(patientData.steps[_currentStep].spriteEarExams);
                    // Display current step
                    displayList[3].SetActive(true);
                    break;
                case Step.Weber_test:
                    // Load texts dialogue
                    step6HhiesTest.SetTextDialogue(patientData.steps[_currentStep].contextDescription);
                    // Display current step
                    displayList[4].SetActive(true);
                    break;
                case Step.HHIES_test:
                    // Load patient ear image
                    step7HhiesTest.SetImageHHIES(patientData.steps[_currentStep].spriteEarExams);
                    // Display current step
                    displayList[5].SetActive(true);
                    break;
                case Step.Audiometry:
                    // Load patient ear image
                    Step8Audiometrie.SetImageAudiometrie(patientData.steps[_currentStep].spriteEarExams);
                    // Display current step
                    displayList[6].SetActive(true);
                    break;
            }

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
                choiceButtons[i].GetComponent<AnswerButton>().Reset();
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
            bool isCorrect = false;
            // DIAGNOSTIC CHOICE
            if (answerState == AnswerState.DIAGNOSTIC)
            {
                if (IsAnswerCorrect(phaseData.answerData, index))
                {
                    _isDiagnosticValid = true;
                    isCorrect = true;
                    feedBackText = "Bonne réponse";
                }
                else
                {
                    choiceButtons[index].GetComponent<AnswerButton>().SetIncorrect();
                    _isDiagnosticValid = false;
                }
            }
            // ACTION CHOICE
            if (answerState == AnswerState.ACTION)
            {
                if (IsAnswerCorrect(phaseData.answerData, index))
                {
                    _isActionValid = true;
                    isCorrect = true;
                    feedBackText = "Bonne réponse";
                }
                else
                {
                    _isActionValid = false;
                    choiceButtons[index].GetComponent<AnswerButton>().SetIncorrect();
                }
            }
            ShowAnswerDetail(phaseData.answerData[index], feedBackText, isCorrect);
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


        private void ShowAnswerDetail(AnswerData answer, string feedBackText, bool anwserCorrect)
        {
            interactionState = InteractionState.ISCORRECTION;

            // Clear images
            foreach(GameObject go in answerGameObjectSprites)
            {
                go.SetActive(false);
            }

            // Load texts
            answerText.text = feedBackText;
            answerSelected.text = answer.answerText;

            // Set background color
            if (anwserCorrect)
            {
                backgroudAnswer.color = correctColor;
                doctorExpressionsImages.sprite = doctorSprite[0]; // Happy expression
            }
            else
            { 
                backgroudAnswer.color = incorrectColor;
                doctorExpressionsImages.sprite = doctorSprite[1]; // Sad expression
            }

            // Load correction text if not null
            if (answer.correctionText != "")
            {
                answerJustification.GetComponent<TextMeshProUGUI>().text = "<u><b>Justification :</b></u> " + answer.correctionText;
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
            if (IsStepCompleted(patientData.steps[_currentStep]))
            {
                _currentStep++;
                GameManager.Instance.GameStateManager.NextStep(_currentStep);
            }
            else if (_isDiagnosticValid && answerState == AnswerState.DIAGNOSTIC)
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
