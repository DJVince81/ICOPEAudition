using UnityEngine;

[RequireComponent(typeof(StatesManager))]
[RequireComponent(typeof(StepManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public StatesManager StatesManager { get; private set; }

    public StepManager StepManager { get; private set; }

    internal void LoadStep(int e)
    {
        if (e == 0)
        {
            StepManager.Initialize();
        }
        StepManager.LoadStep(e);
    }

    internal void LoadMainMenu()
    {
        Debug.Log("Main Menu");
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
    }

    void Start()
    {
        StatesManager.ReturnMainMenu();
        TogglePause(); //TODO Remove after Main Menu Implementation
    }
}
