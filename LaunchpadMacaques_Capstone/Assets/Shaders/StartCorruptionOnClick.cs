/*
 * Shows how to start corrupting an object from a specific point (such as grapple point)
 * In this example, that point is where the object was clicked.
 */
using UnityEngine;

public class StartCorruptionOnClick : MonoBehaviour
{
    void Update()
    {
        // If mouse clicked, check if it was clicked on this object
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 100f);

            if (hit.collider != null)
            {
                CorruptableObject co = hit.collider.GetComponent<CorruptableObject>();
                if (co != null)
                {
                    // Tell the object to start corrupting from click point
                    co.StartCorrupting(hit.point);
                }
            }
        }
    }
}
