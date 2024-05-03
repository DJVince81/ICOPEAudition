using TMPro;
using UnityEngine;

[RequireComponent(typeof(StatesManager))]
[RequireComponent(typeof(StepManager))]
[RequireComponent(typeof(TelemetryManager))]
public class GameManager : MonoBehaviour
{
    public event System.Action<int, int> OnMoneyChanged;

    public static GameManager Instance;

    public StatesManager StatesManager { get; private set; }
    public StepManager StepManager { get; private set; }
    public TelemetryManager TelemetryManager { get; private set; }

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
    [SerializeField] private GameObject _stepMenu;

    internal void LoadStep(int stepIndex)
    {
        if (stepIndex == 0)
        {
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
        ClearScreen();
        _mainMenu.SetActive(true);
    }

    internal void LoadGameMenu()
    {
        ClearScreen();
        _gameMenu.SetActive(true);
        if (StatesManager.paused) TogglePause();
    }

    public void LaunchGame()
    {
        Debug.Log("Launch Game ");
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
    }

    void Start()
    {
        StatesManager.ReturnMainMenu();
    }
}
