using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(StatesManager))]
[RequireComponent(typeof(StepManager))]
[RequireComponent(typeof(TelemetryManager))]
[RequireComponent(typeof(AudioManager))]
public class GameManager : MonoBehaviour
{
    public event System.Action<int, int> OnMoneyChanged;

    public static GameManager Instance;

    public StatesManager StatesManager { get; private set; }
    public StepManager StepManager { get; private set; }
    public TelemetryManager TelemetryManager { get; private set; }
    public AudioManager AudioManager { get; private set; }

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
            OnMoneyChanged?.Invoke(_money, _money - previousMoney);
        }
    }

    [Header("Money")]
    [SerializeField] private int _money = 20;

    [Header("Menus")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _gameMenu;
    [SerializeField] private TipsPanel _tipsPanel;
    [SerializeField] private GameObject _stepMenu;

    [Header("Panels")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _mainPanel;

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
    }

    internal void LoadMainMenu()
    {
        AudioManager.PlayBGM("skyline");
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

    public void CloseSettings()
    {
        if ( StatesManager.State == StatesManager.States.MAIN_MENU)
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

        _moneyText.text = _money.ToString();
        AudioManager.LoopBgm(true);
        AudioManager.LoopSfx(true, "AMBIANT");
    }

    void Start()
    {
        StatesManager.ReturnMainMenu();
    }
}
