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
    #endregion

    #region Public methods
    /// <summary>
    /// Check the currentState of the game and load text for tutorial.
    /// </summary>
    /// <remarks>
    /// Here a list of actual gameState and it's links name for seaching the text attach.
    /// GAME_MENU -> Waiting_room,
    /// GAME_E0 -> Step_0,
    /// GAME_E1 -> Step_1,
    /// GAME_E2 -> Step_2,
    /// GAME_E3 -> Step_3,
    /// GAME_E4 -> Step_4
    /// </remarks>
    public void checkStateTutorial()
    {
        switch (GameManager.Instance.StatesManager.currentState)
        {
            case StatesManager.States.GAME_MENU:
                nameStep = "Waiting_room";
                setTexts(nameStep, indexText);
                break;
        }
    }

    /// <summary>
    /// Change indexText value for loading next text.
    /// </summary>
    public void nextTextButton()
    {
        indexText++;
        setTexts(nameStep, indexText);
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
    private void setTexts(string nameStep, int idSteps)
    {
        Entry entry = xmlReader.readXmlStream(nameStep, idSteps);
        if (entry != null)
        {
            _intituleText.text = entry.Intitule;
            _Text.text = entry.Text;
        }
        else
        {
            GameManager.Instance.setTutorialUI();
            indexText = 0;
        }
    }

    /// <summary>
    /// Check if input button like mouse click (Fire1) and keyboard (enter) is hit and call fuction "nextTextButton".
    /// </summary>
    private void getInputs()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return))
        {
            nextTextButton();
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
        checkStateTutorial();
    }

    private void Update()
    {
        if (GameManager.Instance._tutorialPanel.activeSelf) getInputs();
    }
    #endregion
}
