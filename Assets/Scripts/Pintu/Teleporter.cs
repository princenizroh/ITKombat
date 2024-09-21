using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform destination;
    public GameObject Objek; 
    public Button Button; 
    private bool isObjekInside = false;

    private void Start()
    {
        if (Button != null)
        {
            Button.onClick.AddListener(OnButtonClick); 
        }
    }
    public Transform GetDestination()
    {
        return destination;
    }
    private void OnButtonClick()
    {
        if (isObjekInside) 
        {
            Objek.transform.position = destination.position;
            Debug.Log("Objek ditransfer ke destinasi.");
        }
        else
        {
            Debug.Log("Objek tidak berada di dalam collider.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Objek)
        {
            isObjekInside = true;
            Debug.Log("Objek berada di dalam collider.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Objek)
        {
            isObjekInside = false;
            Debug.Log("Objek keluar dari collider.");
        }
    }
}
