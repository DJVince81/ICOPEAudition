using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UITutorialControler : MonoBehaviour
{
    [SerializeField] private TMP_Text _intituleText;
    [SerializeField] private TMP_Text _Text;

    private XMLReader xmlReader;
    private int indexText = 0;
    private string nameStep;

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

    public void nextTextButton()
    {
        indexText++;
        setTexts(nameStep, indexText);
    }

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

    private void getInputs()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return))
        {
            nextTextButton();
        }
    }

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
}
