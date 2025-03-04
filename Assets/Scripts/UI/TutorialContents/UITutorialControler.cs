using Assets.Scripts.Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controler that manage text placement for the Tutorial.
/// </summary>
public class UITutorialControler : MonoBehaviour
{
    #region SerializedField TMP_text
    [SerializeField] private TMP_Text _intituleText;
    [SerializeField] private TMP_Text _Text;
    #endregion

    #region Local variable
    private XMLReader xmlReader;
    private int indexText = 0;
    private string nameStep;
    private bool[] isStepDoneTab = { false, false, false, false, false }; //To set in the gameManager maybe
    #endregion

    #region Public methods
    /// <summary>
    /// Check the currentState of the game and load text for tutorial.
    /// </summary>
    /// <remarks>
    /// Here a list of actual gameState and it's links name for seaching the text attach.
    /// GAME_MENU -> Waiting_room,
    /// GAME_E0 -> Test_chuchotement,
    /// GAME_E1 -> Questionnaire_go,
    /// GAME_E2 -> Video_ostocopie,
    /// GAME_E3 -> Test_Weber,
    /// GAME_E4 -> Test_Audiometrie
    /// </remarks>
    public void CheckStateTutorial()
    {
        //Debug.Log(GameManager.Instance.StatesManager.currentState);
        switch (GameManager.Instance.StatesManager.currentState)
        {
            case StatesManager.States.GAME_MENU:
                nameStep = "Waiting_room";
                SetTexts(nameStep, indexText);
                break;
            case StatesManager.States.GAME_E0:
                nameStep = "Test_chuchotement";
                Debug.Log("Here");
                if (!isStepDoneTab[0])
                {
                    SetTexts(nameStep, indexText);
                    isStepDoneTab[0] = true;
                }
                break;
            case StatesManager.States.GAME_E1:
                nameStep = "Questionnaire_go";
                if (!isStepDoneTab[1])
                {
                    SetTexts(nameStep, indexText);
                    isStepDoneTab[1] = true;
                }
                break;
            case StatesManager.States.GAME_E2:
                nameStep = "Video_ostocopie";
                if (!isStepDoneTab[2])
                {
                    SetTexts(nameStep, indexText);
                    isStepDoneTab[2] = true;
                }
                break;
            case StatesManager.States.GAME_E3:
                nameStep = "Test_Weber";
                if (!isStepDoneTab[3])
                {
                    SetTexts(nameStep, indexText);
                    isStepDoneTab[3] = true;
                }
                break;
            case StatesManager.States.GAME_E4:
                nameStep = "Test_Audiometrie";
                if (!isStepDoneTab[4])
                {
                    SetTexts(nameStep, indexText);
                    isStepDoneTab[4] = true;
                }
                break;
        }
    }

    /// <summary>
    /// Change indexText value for loading next text.
    /// </summary>
    public void NextTextButton()
    {
        indexText++;
        SetTexts(nameStep, indexText);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Set text with the xml information.
    /// </summary>
    /// <remarks>
    /// Set text for an intitule and Text (dialogue).
    /// </remarks>
    /// <param name="nameStep">The name of the current state for the XML file.</param>
    /// <param name="idSteps">The ids of the current text to load.</param>
    private void SetTexts(string nameStep, int idSteps)
    {
        Entry entry = xmlReader.readXmlStream(nameStep, idSteps);
        if (entry != null)
        {
            _intituleText.text = entry.Intitule;
            _Text.text = entry.Text;
        }
        else
        {
            GameManager.Instance.SetTutorialUI();
            indexText = 0;
        }
    }

    /// <summary>
    /// Check if input button like mouse click (Fire1) and keyboard (enter) is hit and call fuction "nextTextButton".
    /// </summary>
    private void GetInputs()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return))
        {
            NextTextButton();
        }
    }
    #endregion

    #region Unity method
    private void Awake()
    {
        xmlReader = GetComponent<XMLReader>();
    }

    private void OnEnable()
    {
        CheckStateTutorial();
    }

    private void Update()
    {
        if (GameManager.Instance._tutorialPanel.activeSelf) GetInputs();
    }
    #endregion
}
