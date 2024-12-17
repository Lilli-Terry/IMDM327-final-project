using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beehive : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("bee");
    }
}
