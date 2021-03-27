using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingHelper : MonoBehaviour
{
    private GrapplingGun grapplingGun;
    private Transform orientation;


    private float minCheckAngle = 80;
    private float maxCheckAngle = 100;

    private float timeBeforeFixing = 5;


    private bool inNegative = false;

    private int correctAngleInt = 0;

    private bool checking = false;
    private bool hitAngle = false;
    private int loop = 0;

    private bool isFixing = false;

    private Vector3 dirToTargert = Vector3.zero;
    // Start is called before the first frame update

    private void Awake()
    {
        grapplingGun = FindObjectOfType<GrapplingGun>();
        orientation = FindObjectOfType<Matt_PlayerMovement>().GetOrientaion();
        hitAngle = false;

    }


    // Update is called once per frame
    private void Update()
    {

        if (grapplingGun.IsGrappling())
        {
            if (!checking &!isFixing)
            {
                StartCoroutine(CheckingForAngle());
            }


            float angle = DegreesBetweenObjects(orientation.gameObject, grapplingGun.GetCurrentGrappledObject());

            if ((inNegative && angle < 0) || (!inNegative && angle > 0))
            {

                if (Mathf.Abs(angle) > minCheckAngle && Mathf.Abs(angle) < maxCheckAngle && !hitAngle)
                {
                    hitAngle = true;
                    correctAngleInt++;
   
                }

           
            }

            else if ((inNegative && angle > 0) || (!inNegative && angle < 0))
            {
                loop++;
   

                if (loop == 2)
                {
                    if (correctAngleInt >= 2 && hitAngle)
                    {
                        Debug.Log("Good Loop");
                        dirToTargert = Vector3.zero;

                        StopAllCoroutines();
                        isFixing = false;
                        checking = false;
                    }

                    else
                    {
                        Debug.Log("Bad Loop:");
                    }

                    correctAngleInt = 0;
                    loop = 0;
                }

                hitAngle = false;

                inNegative = !inNegative;

            }

        }

        else
        {
            checking = false;
            correctAngleInt = 2;
            hitAngle = false;
            loop = 0;
            dirToTargert = Vector3.zero;
            isFixing = false;
        }
    }



    IEnumerator CheckingForAngle()
    {
        checking = true;
        yield return new WaitForSeconds(timeBeforeFixing);
        StartCoroutine(FixDirection());
        checking = false;
    }

    IEnumerator FixDirection()
    {
        while (true)
        {
            if (grapplingGun.GetCurrentGrappledObject() != null)
            {
                dirToTargert = (grapplingGun.GetCurrentGrappledObject().transform.position - orientation.transform.position).normalized;
            }

            yield return null;
        }


    }


    float DegreesBetweenObjects(GameObject thisObject, GameObject target)
    {
        if(target == null)
        {
            return -1;
        }
        float sideA = Vector3.Distance(thisObject.transform.position, new Vector3(target.transform.position.x, thisObject.transform.position.y, target.transform.position.z));
        float sideB = target.transform.position.y - thisObject.transform.position.y;
        float angleRad = Mathf.Atan(sideB / sideA);
        return angleRad * Mathf.Rad2Deg;
    }

    public Vector3 GetDirectionToTarget()
    {
        return dirToTargert;
    }
}
