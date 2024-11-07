using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform destination;
    public GameObject Objek; 
    private bool isObjekInside = false;

    public Transform GetDestination()
    {
        return destination;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Objek)
        {
            isObjekInside = true;
            Debug.Log("Objek berada di dalam collider.");
            Objek.transform.position = destination.position;
            Debug.Log("Objek ditransfer ke destinasi.");
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
