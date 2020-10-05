using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCorruptionOnClick : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 100f);

            if (hit.collider != null)
            {
                Renderer r = hit.collider.GetComponent<Renderer>();
                MaterialPropertyBlock pBlock = new MaterialPropertyBlock();

                r.GetPropertyBlock(pBlock);

                pBlock.SetFloat("CorruptionStartTime", Time.time);
                Vector3 localPos = hit.collider.transform.InverseTransformPoint(hit.point);
                pBlock.SetVector("CorruptionStartPos", localPos);

                r.SetPropertyBlock(pBlock);
            }
        }
    }
}
