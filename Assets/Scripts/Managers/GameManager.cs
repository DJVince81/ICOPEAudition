using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
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

        public bool IsTutorialEnable
        {
            get { return _isTutorialEnable; }
            set
            {
                bool _isEnable = _isTutorialEnable;
                _isTutorialEnable = value;
                PlayerPrefs.SetInt("enableAssistant", _isTutorialEnable ? 1 : 0);
                OnAssistantDisabled?.Invoke(_isTutorialEnable, _isEnable);
            }
        }
        #endregion

        #region Configurable Attributes
        [Header("Tutoriel")]
        [SerializeField] private bool _isTutorialEnable = true; // Par défault true car on suppose que le joueur y joue pour la première fois.
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
        [SerializeField] private GameObject _settingsPanelCheckbox;
        [SerializeField] public GameObject _tutorialPanel;

        [Header("Character blinking")]
        [SerializeField] private float speedColorChange = 1.0f;
        [SerializeField] private GameObject _elderPerson; // maybe change to list
        private Component _outlineCharacter;
        #endregion

        #region Private variables
        private bool isTutorialUIEnable = false;
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

        // Change tipsPanel is disable -> tips will be a glossaire
        // Now ask player for they fisrt time (in the current session, todo) if they want activate the assistant.
        internal void LoadGameMenu()
        {
            AudioManager.PlaySFX("ambiant", "AMBIANT");
            ClearScreen();
            _gameMenu.SetActive(true);
            //_tipsPanel.Display();
            if (TelemetryManager.GetNbGames() == 0) _activeAssistant.SetActive(true);
            if (StatesManager.isPaused) TogglePause();
        }
        
        public static void AddBoughtItem(string name)
        {
            PlayerPrefs.SetString("items", $"{name};{PlayerPrefs.GetString("items", "")}");
            PlayerPrefs.Save();
        }
        #endregion

        #region Main methods
        public void LaunchGame()
        {
            StartCoroutine(LaunchGameAfterTime());
        }

        private IEnumerator LaunchGameAfterTime()
        {
            yield return new WaitForSeconds(0.5f);
            StatesManager.ChangeState();
        }

        public void TogglePause()
        {
            StatesManager.isPaused ^= true;
        }

        public void ChangeState()
        {
            if (!StatesManager.isPaused) StatesManager.ChangeState();
        }

        public void ClickButton()
        {
            AudioManager.PlaySFX("ui_click2");
        }

        public void CloseSettings()
        {
            if (StatesManager.currentState == StatesManager.States.MAIN_MENU)
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

        public void SetCheckBoxSettings()
        {
            _settingsPanelCheckbox.GetComponent<Toggle>().isOn = IsTutorialEnable;
        }

        /// <summary>
        /// Resume
        /// Function that allows making a blinking outline on image.
        /// </summary>
        public void CharacterOutlineBliking()
        {
            if (_gameMenu.activeInHierarchy)
            {
                // Ping Pong oscillation between 0 and 1
                float pingPong = Mathf.PingPong(Time.time * speedColorChange, 1.0f);
                // Ping Pon between white and black
                Color newColor = Color.Lerp(Color.black, Color.white, pingPong);
                // Feed to outline character component
                _elderPerson.GetComponent<Outline>().effectColor = newColor;
            }
        }

        public void SetTutorialUI()
        {
            isTutorialUIEnable = !isTutorialUIEnable;
            if (IsTutorialEnable)
            {
                _tutorialPanel.SetActive(isTutorialUIEnable);
            }
        }


        #endregion

        #region Initializing methods
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
                return;
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


        void Start()
        {
            StatesManager.ReturnMainMenu();
            LoadListItems();

            _money = PlayerPrefs.GetInt("money", 20);
            _isTutorialEnable = PlayerPrefs.GetInt("", 1) == 1 ? true : false; // Can be problem
            AudioManager.PlayBGM("skyline");

            // Get component
            if (_elderPerson.GetComponent<Outline>() != null) _outlineCharacter = _elderPerson.GetComponent<Outline>();
        }

        private void FixedUpdate()
        {
            CharacterOutlineBliking();
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
    }
}