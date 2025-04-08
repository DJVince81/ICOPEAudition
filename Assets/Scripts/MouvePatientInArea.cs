using Assets.Scripts.Managers;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MouvePatientInArea : MonoBehaviour
    {
        // Interaction Area Variable
        [SerializeField] private RectTransform interactionArea;
        [SerializeField] private GameObject imageCharacter;
        private Vector2 targetPosition;

        // Sprite List
        [SerializeField] private Sprite[] sprites;

        // Animation size variable
        [SerializeField] private float scaleFactor = 1.15f;
        [SerializeField] private float animationDuration = 2f;


        /// <summary>
        /// Set a new target position to the sprite to stimule life in the UI.
        /// </summary>
        private void SetNewTargetPosition()
        {
            float panelWidth = interactionArea.GetComponent<RectTransform>().rect.width;
            float panelHeight = interactionArea.GetComponent<RectTransform>().rect.height;

            float imageWidth = imageCharacter.GetComponent<RectTransform>().rect.width;
            float imageHeight = imageCharacter.GetComponent<RectTransform>().rect.height;

            float randomX = Random.Range(-panelWidth / 2 + imageWidth / 2, panelWidth / 2 - imageWidth / 2);
            float randomY = Random.Range(-panelHeight / 2 + imageHeight / 2, panelHeight / 2 - imageHeight / 2);

            targetPosition = new Vector2(randomX, randomY);

            imageCharacter.GetComponent<RectTransform>().anchoredPosition = targetPosition;
        }

        /// <summary>
        /// Set new sprite to stimule life.
        /// </summary>
        private void SetNewSprite()
        {
            if (sprites == null)
            {
                Debug.LogError("no sprite set in the sprites library.");
                return;
            }

            int randIndex = Random.Range(0, sprites.Length - 1);
            Sprite newSprite = sprites[randIndex];
            imageCharacter.GetComponent<Image>().sprite = newSprite;
            // Set the gameObject rectTransfor with the new sprite size
            imageCharacter.GetComponent<RectTransform>().sizeDelta = new Vector2(newSprite.rect.width, newSprite.rect.height);
        }

        /// <summary>
        /// Make a yoyo animation on the sprite.
        /// </summary>
        private void AnimationSizeImage()
        {
            RectTransform rectTransform = imageCharacter.GetComponent<RectTransform>();
            rectTransform.DOSizeDelta(rectTransform.sizeDelta * scaleFactor, animationDuration / 2).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetId("sizeAnim");
        }

        public static void StopAnimation()
        {
            DOTween.Kill("sizeAnim");
        }

        /// <summary>
        /// Call when loading the waiting_room, call private function :
        /// SetNewTargetPosition,AttachFunction, SetNewSprite.
        /// </summary>
        public void SetNewCharacterInArea()
        {
            SetNewTargetPosition();
            SetNewSprite();
            AnimationSizeImage();
        }
    }

}