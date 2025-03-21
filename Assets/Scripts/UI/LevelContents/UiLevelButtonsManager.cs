using Assets.Scripts.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.LevelContents
{
    public class UiLevelButtonsManagers : MonoBehaviour
    {
        // LAUNCH GAME BUTTON   
        [Header("Launch game button")]
        public Button launchGameButton; // The launch game button

        // BUTTONS NAVIGATION
        [Header("Navigation button")]
        public Button defaultSelectedButton; // The default selected button
        public Button[] buttons; // The buttons to navigate
        public int currentButtonIndex = 0; // The current button index

        // SCROLL VIEW NAVIGATION
        [Header("Scroll view navigation")]
        public ScrollRect scrollRect; // The scroll rect to navigate
        public RectTransform contentPanel; // The content panel of the scroll rect
        public float scrollSpeed = 10f; // The scroll speed

        [Header("Level Display")]
        [SerializeField] private TMP_Text attemptText;
        [SerializeField] private TMP_Text totalActionErrText;
        [SerializeField] private TMP_Text totalDiagErrText;
        [SerializeField] private TMP_Text timeText;
        [Header("Step Display")]
        [SerializeField] private GameObject stepPrefab;
        [SerializeField] private Transform contentLevelPanel;


        // PRIVATE VARIABLES
        private float targetScrollPosition = 1f; // The target scroll position
        private bool isScrolling = false; // Is the scroll rect scrolling

        // SCROLL VIEW NAVIGATION TO SELECTED BUTTON
        /// <summary>
        /// Scroll to the selected button in the scroll view.
        /// </summary>
        /// <param name="selected"></param>
        private void ScrollToSelected(RectTransform selected)
        {
            if (selected == null) return;

            RectTransform viewport = scrollRect.viewport;
            float selectedY = -selected.anchoredPosition.y;
            float viewPortHeight = viewport.rect.height;
            float contentHeight = contentPanel.rect.height;

            targetScrollPosition = 1 - Mathf.Clamp01((selectedY - viewPortHeight / 2) / (contentHeight - viewPortHeight));
            isScrolling = true;
        }

        // SCROLL VIEW NAVIGATION
        /// <summary>
        /// Scroll the scroll view to the selected button.
        /// </summary>
        private void ScrollNavigation()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null && selected.transform.IsChildOf(contentPanel)) ScrollToSelected(selected.GetComponent<RectTransform>());

            if (isScrolling)
            {
                scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, targetScrollPosition, Time.deltaTime * scrollSpeed);
                if (Mathf.Abs(scrollRect.verticalNormalizedPosition - targetScrollPosition) < 0.01f)
                {
                    scrollRect.verticalNormalizedPosition = targetScrollPosition;
                    isScrolling = false;
                }
            }
        }

        // BUTTONS NAVIGATION
        /// <summary>
        /// Navigate the buttons up & down.
        /// </summary>
        private void ButtonNavigator()
        {
            if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") < 0)
            {
                currentButtonIndex = (currentButtonIndex + 1) % buttons.Length;
                EventSystem.current.SetSelectedGameObject(buttons[currentButtonIndex].gameObject);
                OnButtonClicked(currentButtonIndex);
            }
            else if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0)
            {
                currentButtonIndex = (currentButtonIndex - 1 + buttons.Length) % buttons.Length;
                EventSystem.current.SetSelectedGameObject(buttons[currentButtonIndex].gameObject);
                OnButtonClicked(currentButtonIndex);
            }
        }
        // BUTTONS NAVIGATION DEFAULT
        /// <summary>
        /// Force the default selected button if the current selected button is null.
        /// </summary>
        private void ForceDefaultOnNull()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(defaultSelectedButton.gameObject);
                Debug.Log("Plop");
            }
        }

        // START GAME
        /// <summary>
        /// Start the game with the current level selected.
        /// </summary>
        private void StartGame()
        {
            GameManager.Instance.GameStateManager.LoadLevelFromPanel(currentButtonIndex);
        }

        // ON BUTTON CLICKED
        /// <summary>
        /// When a button is clicked.
        /// </summary>
        /// <param name="index"></param>
        private void OnButtonClicked(int index)
        {
            currentButtonIndex = index;
            EventSystem.current.SetSelectedGameObject(buttons[currentButtonIndex].gameObject);
            SetTextsDisplay();
        }

        private void SetTextsDisplay()
        {
            string[] strings = GameManager.Instance.GameData.LevelRecordsToString(currentButtonIndex);
            DestroyUiItemClone();

            if (strings == null)
            {
                attemptText.text = "Nombre d'essais: N/A";
                totalActionErrText.text = "Total d'errreur d'action: N/A";
                totalDiagErrText.text = "Total d'erreurs de diagnostiques: N/A";
                timeText.text = "Temps: N/A";
            }
            else
            {
                attemptText.text = $"Nombre d'essais: {strings[0]}";
                totalActionErrText.text = $"Total d'errreur d'action: {strings[1]}";
                totalDiagErrText.text = $"Total d'erreurs de diagnostiques: {strings[2]}";
                timeText.text = $"Temps: {strings[3]}";
                CreateUiItem();
            }
        }

        private void CreateUiItem()
        {
            Dictionary<string, string[]> data = GameManager.Instance.GameData.GetStepRecordsInfo(currentButtonIndex);

            

            foreach (var var in data)
            {
                GameObject newUiItem = Instantiate(stepPrefab, contentLevelPanel);
                TMP_Text[] itemTexts = newUiItem.GetComponentsInChildren<TMP_Text>();
                itemTexts[0].text =$"Etape: {var.Key}";

                for (int i = 1; i < itemTexts.Length; i++)
                {
                    itemTexts[i].text = itemTexts[i].text.Substring(0, itemTexts[i].text.IndexOf(":")+1) +" "+ var.Value[i - 1];
                }
            }
        }

        private void DestroyUiItemClone()
        {
            foreach (Transform child in contentLevelPanel)
            {
                if (child.gameObject.name.EndsWith("(Clone)")) Destroy(child.gameObject);
            }
        }

        // SET LISTENERS ON BUTTONS (ON START)
        /// <summary>
        /// Set listeners on the buttons when the game start.
        /// </summary>
        private void SetListenersOnButtons()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int index = i;
                buttons[i].onClick.AddListener(() => OnButtonClicked(index));
            }
        }

        /// <summary>
        /// Start the script.
        /// </summary>
        void Start()
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton.gameObject);
            SetTextsDisplay();
            // Set the listeners (launchGameButton and LevelsButtons)
            launchGameButton.onClick.AddListener(StartGame);
            SetListenersOnButtons();
        }

        /// <summary>
        /// Update the script.
        /// </summary>
        void Update()
        {
            //ForceDefaultOnNull();
            ButtonNavigator();
            ScrollNavigation();
        }
    }
}