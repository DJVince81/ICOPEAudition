using Assets.Scripts.Managers;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PatientAnimation : MonoBehaviour
    {
        // Sprite List
        [Header("Sprite characters list")]
        [SerializeField] private Sprite[] characterSprites;
        
        // Interaction Area Variable
        [Header("Patient selection area")]
        [SerializeField] private RectTransform interactionArea;
        [SerializeField] private GameObject imageCharacter;
        private Vector2 targetPosition;

        [Header("Doors")]
        [SerializeField] private RectTransform leftDoor;
        [SerializeField] private RectTransform rightDoor;

        // Doors variables
        public float openAngle = -90f;
        public float duration = 0.5f;
        private bool isOpen = false;


        // Animation size variable
        [SerializeField] private float scaleFactor = 1.15f;
        [SerializeField] private float animationDuration = 2f;

        [Header("Animation patient area")]
        [SerializeField] public RectTransform spawnPatientArea;

        /// <summary>
        /// Set a new target position to the sprite to stimule life in the UI.
        /// </summary>
        private void SetNewTargetPosition(RectTransform area)
        {
            float panelWidth = area.GetComponent<RectTransform>().rect.width;
            float panelHeight = area.GetComponent<RectTransform>().rect.height;

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
            if (characterSprites == null)
            {
                Debug.LogError("no sprite set in the sprites library.");
                return;
            }

            int randIndex = Random.Range(0, characterSprites.Length - 1);
            Sprite newSprite = characterSprites[randIndex];
            imageCharacter.GetComponent<Image>().sprite = newSprite;
            // Set the gameObject rectTransfor with the new sprite size
            imageCharacter.GetComponent<RectTransform>().sizeDelta = new Vector2(newSprite.rect.width, newSprite.rect.height);
        }

        /// <summary>
        /// Make a yo-yo animation on the sprite.
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
            SetNewSprite();
            SetNewTargetPosition(spawnPatientArea);
            
            //AnimationSizeImage();
        }

        // Load -> sprite -> SetPositionPersoDoor -> OpenDoor -> blackTransition -> SetPositionOnArea -> CloseDoor
        
        // OPEN CLOSE DOOR
        public void ToggleDoor()
        {
            // move door postion and mor angle (-145/145 degree)
            float targetAngle = isOpen ? 0f : openAngle;
            rightDoor.DOLocalRotate(new Vector3(0, targetAngle, 0), duration).SetEase(Ease.InOutCubic);
            leftDoor.DOLocalRotate(new Vector3(0, -targetAngle, 0), duration).SetEase (Ease.InOutCubic);
            isOpen = !isOpen;
        }

        //public void PlayAnimation()
    }
}