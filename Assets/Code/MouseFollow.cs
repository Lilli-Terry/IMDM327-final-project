using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    public Rigidbody rigidbody;

    void Update()
    {
                if (rigidbody == null)
        {
            Debug.LogError("Rigidbody is not assigned!");
            return;
        }

        // Get the mouse position in screen coordinates
        Vector3 mousePosition = Input.mousePosition;

        // Convert mouse position to world space
        float zDistance = Camera.main.WorldToScreenPoint(rigidbody.position).z;
        mousePosition.z = zDistance;

        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        // Update the Rigidbody position
        rigidbody.MovePosition(targetPosition);
    }
}
