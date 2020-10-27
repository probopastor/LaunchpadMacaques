/*
 * When this script is placed on an object in the screen, clicking on the object will tell all
 * corrupted or corrupting objects in the scene to start uncorrupting. The origin of the
 * un-corruption is this object's position, and the un-corruption currently spreads at the same
 * speed as the normal corruption.
 * 
 * Bug: if an object is partially corrupted when this object is clicked, it will instantly become
 * fully corrupted, but will still un-corrupt normally.
 * 
 * If you don't want things to un-corrupt over time, call UncorruptInstantly() on
 * CorruptableObject and the vaporwave effect will be instantly removed from the object.
 */
using UnityEngine;

public class RemoveCorruptionAroundObject : MonoBehaviour
{
    void Update()
    {
        // When the player clicks, check if they clicked on this object
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 100f);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    StartCorruptionRemoval();
                }
            }
        }
    }

    void StartCorruptionRemoval()
    {
        CorruptableObject[] objects = FindObjectsOfType<CorruptableObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            // If the object is already corrupted or is corrupting now
            if (objects[i].corruptionState == CorruptableObject.CorruptionState.FullyCorrupted
             || objects[i].corruptionState == CorruptableObject.CorruptionState.Corrupting)
            {
                // Tell the object to start uncorrupting from this objects position
                objects[i].UncorruptFromPoint(transform.position);
            } 
        }
    }
}
