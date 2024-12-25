using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ITKombat
{
    [Serializable]
    public class PageElement
    {
        public int id;                  // ID halaman
        public GameObject pageObject;  // Objek terkait halaman
    }

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

        [SerializeField] private List<PageElement> pageElements; // Daftar elemen halaman

        private void Awake()
        {
            currentPage = 1;
            targetPos = levelPageRect.localPosition;
            dragThreshold = Screen.width / 15f; // Threshold untuk mendeteksi swipe
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

        public void Selected()
        {
            // Cari elemen berdasarkan ID halaman saat ini
            PageElement selectedPage = pageElements.Find(page => page.id == currentPage);

            if (selectedPage != null && selectedPage.pageObject != null)
            {
                // Lakukan sesuatu dengan objek terkait halaman
                Debug.Log($"Selected Page ID: {selectedPage.id}");
                Debug.Log($"Associated Object: {selectedPage.pageObject.name}");

                // Simpan ID halaman yang dipilih
                PlayerPrefs.SetInt("SelectedPageID", selectedPage.id);

                // Pindah ke scene (contoh)
                SceneManager.LoadScene("TargetSceneName");
            }
            else
            {
                Debug.LogWarning("Page or Object not found for the selected ID!");
            }
        }

        private void MovePage()
        {
            LeanTween.moveLocal(levelPageRect.gameObject, targetPos, tweenTime).setEase(tweenType);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
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
