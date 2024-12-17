using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    public GameObject swat;
    bool isTriggered = false;
    GameObject others;
    public static float beesdie;
    public AudioSource smack; 

    public void Start()
    {
        swat.SetActive(false);
        beesdie = 0;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (isTriggered && other.CompareTag("Bee"))
        {
            other.gameObject.SetActive(false);
            beesdie++;
        }

    }
    void OnTriggerStay(Collider other)
    {
        //not needed
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !(Pause.isPaused))
        {
            StartCoroutine(Swatt());
            StartCoroutine(Kill());
        }
    }

    void OnTriggerExit(Collider other)
    {
    }
    public IEnumerator Kill()
    {
        isTriggered = true;
        yield return new WaitForSeconds(.2f);
        isTriggered = false;
    }
    public IEnumerator Swatt()
    {
        swat.SetActive(true);
        smack.Play();
        yield return new WaitForSeconds(.2f);
        swat.SetActive(false);
    }
}
