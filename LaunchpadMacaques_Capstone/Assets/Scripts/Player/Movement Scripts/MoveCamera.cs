using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform player;
    bool shaking;
    private Vector3 shakeAmount;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(StartScreenShake(.5f, 20));
        }
    }
    private void FixedUpdate()
    {
        if (shaking)
        {
            transform.position = player.transform.position + shakeAmount;
        }

        else
        {
            transform.position = player.transform.position;
        }

    }


    public IEnumerator StartScreenShake(float duriation, float intensity)
    {
        shaking = true;
        float elapsedTime = 0;
  
        float increaseSpeed = intensity / (duriation / 4);

        float currentIntensity = intensity;

        while (elapsedTime < duriation)
        {

            if (elapsedTime <= duriation / 4)
            {
                currentIntensity += increaseSpeed * Time.fixedDeltaTime;
            }

            if (elapsedTime >= duriation * .75)
            {
                currentIntensity -= increaseSpeed * Time.fixedDeltaTime;
            }
            shakeAmount = (Random.insideUnitSphere * currentIntensity * Time.fixedDeltaTime);
            elapsedTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        shaking = false;
    }
}