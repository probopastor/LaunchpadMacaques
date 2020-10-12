using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class StartCorruptionOnClick : MonoBehaviour
{
    private StudioEventEmitter emitter;

    private void Start()
    {
        emitter = GetComponent<StudioEventEmitter>();
    }

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
                emitter.Play();
                StartCoroutine(FallOff());
                Vector3 localPos = hit.collider.transform.InverseTransformPoint(hit.point);
                pBlock.SetVector("CorruptionStartPos", localPos);

                r.SetPropertyBlock(pBlock);
            }
        }
    }

    IEnumerator FallOff()
    {
        byte fall = 0;
        emitter.SetParameter("Fall", fall);
        while (emitter.IsPlaying())
        {
            fall++;
            emitter.SetParameter("Fall", (float)fall/100);
            yield return new WaitForFixedUpdate();
            if (fall >= 100) emitter.Stop();
        }
    }
}
