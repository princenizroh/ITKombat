using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ITKombat
{
    public class SwipeController : MonoBehaviour, IEndDragHandler
    {
        [SerializeField] private int maxPage;
        private int currentPage;
        [SerializeField] private Vector3 pageStop;
        [SerializeField] private RectTransform levelPageRect;

        [SerializeField] private float tweenTime;
        [SerializeField] private LeanTweenType tweenType;
        private Vector3 targetPos;
        private float dragThreshold;

        private void Awake()
        {
            currentPage = 1;
            targetPos = levelPageRect.localPosition;
            dragThreshold = Screen.width / 15f; // Perbaikan typo "dragTreshould"
        }

        public void Next()
        {
            if (currentPage < maxPage)
            {
                currentPage++;
                targetPos += pageStop;
                MovePage();
            }
        }

        public void Previous()
        {
            if (currentPage > 1)
            {
                currentPage--;
                targetPos -= pageStop;
                MovePage();
            }
        }

        private void MovePage()
        {
            LeanTween.moveLocal(levelPageRect.gameObject, targetPos, tweenTime).setEase(tweenType); // Menggunakan tweenType dari serialized field
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Perbaikan typo "Event.postion" menjadi "eventData.position"
            if (Math.Abs(eventData.position.x - eventData.pressPosition.x) > dragThreshold)
            {
                if (eventData.position.x > eventData.pressPosition.x) 
                    Previous();
                else 
                    Next();
            }
            else
            {
                MovePage();
            }
        }
    }
}
