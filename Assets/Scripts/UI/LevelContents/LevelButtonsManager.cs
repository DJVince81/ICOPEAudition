using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.LevelContents
{
    public class LevelButtonsManagers : MonoBehaviour
    {
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
            }
            else if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0)
            {
                currentButtonIndex = (currentButtonIndex - 1 + buttons.Length) % buttons.Length;
                EventSystem.current.SetSelectedGameObject(buttons[currentButtonIndex].gameObject);
            }
        }
        // BUTTONS NAVIGATION DEFAULT
        /// <summary>
        /// Force the default selected button if the current selected button is null.
        /// </summary>
        private void ForceDefaultOnNull()
        {
            if (EventSystem.current.currentSelectedGameObject == null) EventSystem.current.SetSelectedGameObject(defaultSelectedButton.gameObject);
        }

        /// <summary>
        /// Start the script.
        /// </summary>
        void Start()
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton.gameObject);
        }

        /// <summary>
        /// Update the script.
        /// </summary>
        void Update()
        {
            ForceDefaultOnNull();
            ButtonNavigator();
            ScrollNavigation();
        }
    }
}