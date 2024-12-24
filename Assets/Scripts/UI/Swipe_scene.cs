using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class SwipeScene : MonoBehaviour
    {
        public GameObject scrollBar;
        private float scrollPos = 0;
        private float[] pos;

        void Start()
        {
            InitializePositions();
        }

        void Update()
        {
            // Update nilai scrollBar
            scrollPos = scrollBar.GetComponent<Scrollbar>().value;

            // Tentukan posisi yang paling dekat dengan scrollPos
            int closestIndex = GetClosestIndex(scrollPos);

            // Atur skala item untuk menyorot elemen tengah
            for (int i = 0; i < pos.Length; i++)
            {
                if (i == closestIndex)
                {
                    // Besarkan elemen yang dipilih
                    transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                }
                else
                {
                    // Kecilkan elemen lainnya
                    transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                }
            }

            // Snap scrollBar ke posisi terdekat
            scrollBar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollBar.GetComponent<Scrollbar>().value, pos[closestIndex], 0.1f);
        }

        private void InitializePositions()
        {
            // Inisialisasi posisi berdasarkan jumlah anak dalam GameObject
            int childCount = transform.childCount;
            pos = new float[childCount];
            float distance = 1f / (childCount - 1);

            for (int i = 0; i < childCount; i++)
            {
                pos[i] = distance * i;
            }
        }

        private int GetClosestIndex(float value)
        {
            // Cari indeks elemen dengan posisi terdekat ke scrollPos
            int closestIndex = 0;
            float closestDistance = Mathf.Abs(pos[0] - value);

            for (int i = 1; i < pos.Length; i++)
            {
                float distance = Mathf.Abs(pos[i] - value);
                if (distance < closestDistance)
                {
                    closestIndex = i;
                    closestDistance = distance;
                }
            }

            return closestIndex;
        }
    }
}
