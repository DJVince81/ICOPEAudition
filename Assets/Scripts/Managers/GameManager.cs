using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StatesManager))]
[RequireComponent(typeof(StepManager))]
[RequireComponent(typeof(TelemetryManager))]
[RequireComponent(typeof(AudioManager))]
public class GameManager : MonoBehaviour
{
    #region Event System
    public event System.Action<int, int> OnMoneyChanged;
    public event System.Action<bool, bool> OnAssistantDisabled;
    #endregion

    public static GameManager Instance;

    public StatesManager StatesManager { get; private set; }
    public StepManager StepManager { get; private set; }
    public TelemetryManager TelemetryManager { get; private set; }
    public AudioManager AudioManager { get; private set; }

    //public List<Item> items;

    #region Structures
    public int Money
    {
        get
        {
            return _money;
        }
        set
        {
            int previousMoney = _money;
            _money = value;
            PlayerPrefs.SetInt("money", _money);
            OnMoneyChanged?.Invoke(_money, _money - previousMoney);
        }
    }

    public bool EnableAssistant
    {
        get { return _enableAssistant; }
        set 
        { 
            bool _isEnable = _enableAssistant;
            _enableAssistant = value; 
            PlayerPrefs.SetInt("enableAssistant", _enableAssistant ? 1 : 0);
            OnAssistantDisabled?.Invoke(_enableAssistant, _isEnable); 
        }
    }
    #endregion

    #region Configurable Attributes
    [Header("Tutoriel")]
    [SerializeField] private bool _enableAssistant = true; // Par défault true car on suppose que le joueur y joue pour la première fois.
    [Header("Money")]
    [SerializeField] private int _money = 20;

    [Header("Menus")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _gameMenu;
    [SerializeField] private TipsPanel _tipsPanel;
    [SerializeField] private GameObject _stepMenu;
    [SerializeField] private GameObject _activeAssistant;

    [Header("Panels")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _shopPanel;
    #endregion

    #region Internal methods
    internal void LoadStep(int stepIndex)
    {
        if (stepIndex == 0)
        {
            AudioManager.PlayBGM("tense_dark");
            AudioManager.StopCurrentSfx();
            ClearScreen();
            _stepMenu.SetActive(true);
            StepManager.Initialize();
            TelemetryManager.IncrGames();
        }
        StepManager.LoadStep(stepIndex);
    }

    internal void ClearScreen()
    {
        _mainMenu.SetActive(false);
        _gameMenu.SetActive(false);
        _stepMenu.SetActive(false);
        _activeAssistant.SetActive(false);
    }

    internal void LoadMainMenu()
    {
        AudioManager.StopCurrentSfx();
        ClearScreen();
        _mainMenu.SetActive(true);
    }

    internal void LoadGameMenu()
    {
        AudioManager.PlaySFX("ambiant", "AMBIANT");
        ClearScreen();
        _gameMenu.SetActive(true);
        _tipsPanel.Display();
        if (StatesManager.paused) TogglePause();
    }
    #endregion

    #region Main methods
    public void LaunchGame()
    {
        StartCoroutine(LaunchGameAfterTime());
    }

    private IEnumerator LaunchGameAfterTime()
    {
        yield return new WaitForSeconds(0.8f);
        StatesManager.ChangeState();
    }

    public void TogglePause()
    {
        StatesManager.paused ^= true;
    }

    public void ChangeState()
    {
        if (!StatesManager.paused) StatesManager.ChangeState();
    }

    public void ClickButton()
    {
        AudioManager.PlaySFX(Random.value > 0.5 ? "ui_click2" : "ui_click2");
    }

    public void EnableTutorial()
    {
        if (GameManager.Instance.EnableAssistant)
        {
            //setActive tutorial
            _activeAssistant.SetActive(true);
        }
    } 

    public void CloseSettings()
    {
        if (StatesManager.State == StatesManager.States.MAIN_MENU)
        {
            _mainPanel.SetActive(true);
        }
        else
        {
            _pausePanel.SetActive(true);
        }
        PlayerPrefs.Save();
    }

    public void PlayBonjour()
    {
        AudioManager.PlaySFX("bonjour");
    }
    #endregion

    #region Initializing varialbles
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        StatesManager = GetComponent<StatesManager>();
        StepManager = GetComponent<StepManager>();
        TelemetryManager = GetComponent<TelemetryManager>();
        AudioManager = GetComponent<AudioManager>();

        AudioManager.LoopBgm(true);
        AudioManager.LoopSfx(true, "AMBIANT");
    }

    // Résumé :
    //      Load Main Menus
    //      Load items
    //      Load Player money (default : 20)
    //      Play sound
    //
    //      
    void Start()
    {
        StatesManager.ReturnMainMenu();
        LoadListItems();

        _money = PlayerPrefs.GetInt("money", 20);
        _enableAssistant = PlayerPrefs.GetInt("", 1) == 1 ? true : false; // Can be problem
        AudioManager.PlayBGM("skyline");
    }
    #endregion

    private void LoadListItems()
    {
        Transform items = _gameMenu.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
        string[] savedItems = PlayerPrefs.GetString("items", "").Split(";");
        for (int i = 0; i < items.childCount; i++)
        {
            foreach (string item in savedItems)
            {
                Transform loadedItem = items.GetChild(i);
                if (item.Equals(loadedItem.name)) loadedItem.gameObject.SetActive(true);
            }
        }

        Transform itemButtons = _shopPanel.transform.GetChild(0).GetChild(0).GetChild(1);
        for (int i = 0; i < itemButtons.childCount; i++)
        {
            foreach (string item in savedItems)
            {
                Transform loadedItem = itemButtons.GetChild(i);
                if (item.Equals(loadedItem.name)) loadedItem.gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }

    internal void AddBoughtItem(string name)
    {
        PlayerPrefs.SetString("items", $"{name};{PlayerPrefs.GetString("items", "")}");
        PlayerPrefs.Save();
    }
}
