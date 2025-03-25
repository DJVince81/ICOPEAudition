using System.Collections;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(GameStateManager))]
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
        //public StatesManager StatesManager { get; private set; }

        public GameStateManager GameStateManager { get; private set; }
        public GameData GameData { get; set; }
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
                PlayerPrefs.SetInt("enableTutorial", _isTutorialEnable ? 1 : 0);
                OnAssistantDisabled?.Invoke(_isTutorialEnable, _isEnable);
            }
        }
        #endregion

        #region Configurable Attributes
        [Header("Tutoriel")]
        [SerializeField] private bool _isTutorialEnable = true; // Par défault true car on suppose que le joueur y joue pour la première fois.
        public readonly string _pathXmlFile = "Assets/Resources/XML_Text/Tutorial.xml";
        private readonly string _pathXsdFile = "Assets/Resources/XML_Text/TutorialSchema.xsd";
        [Header("Money")]
        [SerializeField] private int _money = 20;

        [Header("Menus")]
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _gameMenu;
        [SerializeField] private TipsPanel _tipsPanel;
        [SerializeField] private GameObject _stepMenu;
        [SerializeField] private GameObject _isTutoriaActive;

        [Header("Panels")]
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private GameObject _shopPanel;
        [SerializeField] private GameObject _settingsPanelCheckbox;
        [SerializeField] public GameObject _tutorialPanel;

        [Header("Character pluse")]
        [SerializeField] private GameObject _elderPerson; // maybe change to list
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
            _isTutoriaActive.SetActive(false);
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
            _isTutoriaActive.SetActive(true);
            // Play pulse animation
            _elderPerson.GetComponent<Animation>().Play();
            //_tipsPanel.Display();
            //if (StatesManager.isPaused) TogglePause();
        }
        
        public static void AddBoughtItem(string name)
        {
            PlayerPrefs.SetString("items", $"{name};{PlayerPrefs.GetString("items", "")}");
            PlayerPrefs.Save();
        }
        #endregion

        #region Main methods

        //LOAD GAME ON GRANDMA CLICK -> CALL LaunchGameAfterTime()
        public void LaunchGame()
        {
            StartCoroutine(LaunchGameAfterTime());
        }

        //LOAD GAME ON GRANDMA CLICK -> CHANGE STATE.MANAGER -> LOAD LEVEL 1    
        private IEnumerator LaunchGameAfterTime()
        {
            yield return new WaitForSeconds(0.5f);
            //StatesManager.ChangeState();
            GameData.InitializeRecords();
            GameData.GlobalRecordsOnLevelStart();
            GameStateManager.LoadLevelState();
        }

        // TOGGLE PAUSE
        public void TogglePause()
        {
            //StatesManager.isPaused ^= true;
        }

        // PLAY AUDIO
        public void ClickButton()
        {
            AudioManager.PlaySFX("ui_click2");
        }


        // PLAY AUDIO ON GRANDPA CLICK
        public void PlayBonjour()
        {
            AudioManager.PlaySFX("bonjour");
        }

        public void SetCheckBoxSettings()
        {
            _settingsPanelCheckbox.GetComponent<Toggle>().isOn = IsTutorialEnable;
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

            //StatesManager = GetComponent<StatesManager>();
            GameStateManager = GetComponent<GameStateManager>();
            GameData = GetComponent<GameData>();
            StepManager = GetComponent<StepManager>();
            TelemetryManager = GetComponent<TelemetryManager>();
            AudioManager = GetComponent<AudioManager>();

            AudioManager.LoopBgm(true);
            AudioManager.LoopSfx(true, "AMBIANT");
        }


        void Start()
        {
            // Check validity of tutorial XML
            XmlManager.ValidateXML(_pathXmlFile, _pathXsdFile);
            
            //StatesManager.ReturnMainMenu();
            LoadListItems();

            _money = PlayerPrefs.GetInt("money", 20);
            _isTutorialEnable = PlayerPrefs.GetInt("enableTutorial") == 1;
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
    }
}