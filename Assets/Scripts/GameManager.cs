using TMPro;
using UnityEngine;

[RequireComponent (typeof(StatesManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextMeshProUGUI DebugTMP;

    public StatesManager StatesManager { get; private set; }

    internal void LoadStep(int e)
    {
        DebugTMP.text = DebugTMP.text + "\n\rStep " + e;
    }

    internal void LoadMainMenu()
    {
        DebugTMP.text = DebugTMP.text + "\n\rMain Menu";
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
    }

    void Start()
    {
        StatesManager.ReturnMainMenu();
    }

    void Update()
    {
        
    }
}
