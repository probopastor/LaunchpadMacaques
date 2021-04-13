/*
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* SwingHelper.cs
* Helps handle swinging issues
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingHelper : MonoBehaviour
{
    private GrapplingGun grapplingGun;
    private Transform orientation;


    [Header("Angle Settings")]
    [SerializeField, Tooltip("The Minimun angle the players swinging needs to hit to be considered a good loop")] private float minCheckAngle = 80;
    [SerializeField, Tooltip("The Maximum angle the players swingings needs to hit to be considerd a good loop")] private float maxCheckAngle = 100;

    [SerializeField, Tooltip("The Ammount of time without having a good loop before game will try to fix it")] private float timeBeforeFixing = 5;

    [Range(0, 1)]
    [SerializeField, Tooltip("The intensity at which the direction change will happen")] float directionChangeIntensity = .5f;

    [SerializeField] float intensityIncreaseAmount = .01f;

    [Min(1)]
    [SerializeField] private int neededAmmountOfBadLoops = 3;


    private bool inNegative = false;

    private int correctAngleInt = 0;

    private bool checking = false;
    private bool hitAngle = false;
    private int loop = 0;

    private bool isFixing = false;

    private Vector3 dirToTargert = Vector3.zero;

    private bool stuckHorizontal = false;

    private float tempIntesnity;

    private Rigidbody playerRb;



    private int currentAmmountOfBadLoops = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        Matt_PlayerMovement temp = FindObjectOfType<Matt_PlayerMovement>();
        grapplingGun = FindObjectOfType<GrapplingGun>();
        orientation = temp.GetOrientaion();
        playerRb = temp.gameObject.GetComponent<Rigidbody>();
        hitAngle = false;

    }


    // Update is called once per frame
    private void Update()
    {

        if (grapplingGun.IsGrappling())
        {
            if (!checking & !isFixing)
            {
                StartCoroutine(NeedFixed());
            }

            float angle = DegreesBetweenObjects(orientation.gameObject, grapplingGun.GetCurrentGrappledObject());

            // Makes sure the diretion the player is swinging in has not changed (Swinging up/Swinging Down)
            if ((inNegative && angle < 0) || (!inNegative && angle > 0))
            {

                // Checks to see if the players current swing angle is within bounds of the necessary angle for a good loop
                if (Mathf.Abs(angle) > minCheckAngle && Mathf.Abs(angle) < maxCheckAngle && !hitAngle)
                {
                    hitAngle = true;
                    correctAngleInt++;
                }
            }

            // Checks if the direction the player in going in has changed (Swinging up/Swinging Down)
            else if ((inNegative && angle > 0) || (!inNegative && angle < 0))
            {
                loop++;

                // Checks if the player has completed a full loop around a grapple point
                if (loop == 2)
                {
                    FinishedLoop();

                }

                hitAngle = false;
                inNegative = !inNegative;
            }

        }
    }

    /// <summary>
    /// Resets variables if player is not grappling
    /// </summary>
    public void ResetVariables()
    {
        StopAllCoroutines();
        tempIntesnity = directionChangeIntensity;
        checking = false;
        correctAngleInt = 2;
        hitAngle = false;
        loop = 0;
        dirToTargert = Vector3.zero;
        isFixing = false;
        stuckHorizontal = false;
        currentAmmountOfBadLoops = 0;

    }


    /// <summary>
    /// Checks to see if the player had a good loop
    /// </summary>
    private void FinishedLoop()
    {
        // Checks if the player hit the correct angle while swinging up and while swinging down
        if (correctAngleInt >= 2 && hitAngle)
        {
            // Resets Coroutines and variables
            dirToTargert = Vector3.zero;

            StopAllCoroutines();
            isFixing = false;
            checking = false;

            currentAmmountOfBadLoops = 0;
        }

        else
        {
            currentAmmountOfBadLoops += 1;

            if (currentAmmountOfBadLoops >= neededAmmountOfBadLoops)
            {
                StopAllCoroutines();
                isFixing = false;
                checking = false;

                StartCoroutine(FixDirection());
               // StartCoroutine(CheckForStuckHorizontal());
            }
        }


        // Reset information about the loop
        correctAngleInt = 0;
        loop = 0;
    }


    IEnumerator NeedFixed()
    {
        checking = true;
        yield return new WaitForSeconds(timeBeforeFixing);
        StartCoroutine(FixDirection());
        //StartCoroutine(CheckForStuckHorizontal());
        checking = false;
    }

    IEnumerator FixDirection()
    {
        float currentTime = 0;
        while (true)
        {
            if (grapplingGun.GetCurrentGrappledObject() != null)
            {
                var temp = (grapplingGun.GetCurrentGrappledObject().transform.position - orientation.transform.position);
                temp.y = 0;

                var x = orientation.transform.forward.x;
                var z = orientation.transform.forward.z;


                if (!stuckHorizontal)
                {
                    //if (Mathf.Abs(x) > Mathf.Abs(z))
                    //{
                    //    temp.x = 0;
                    //}

                    //else
                    //{
                    //    temp.z = 0;
                    //}

                    tempIntesnity += intensityIncreaseAmount * Time.deltaTime;
                }

                //else
                //{
                //   tempIntesnity = 1;

                //}


                if (currentTime > 5)
                {
                    currentTime = 0;
                    playerRb.velocity = Vector3.zero;
                    tempIntesnity = 1;
                }

                dirToTargert = temp;


            }

            currentTime += Time.deltaTime;
            Debug.Log("Fixing");
            yield return null;
        }


    }

    //IEnumerator CheckForStuckHorizontal()
    //{
    //    Vector3 playerPos = orientation.transform.position;
    //    while (true)
    //    {

    //        yield return new WaitForFixedUpdate();

    //        if (Mathf.Abs(playerRb.velocity.y) < 5)
    //        {
    //            stuckHorizontal = true;
    //        }

    //        playerPos = orientation.transform.position;
    //    }
    //}




    /// <summary>
    /// Returns the Y angle between two objects
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    float DegreesBetweenObjects(GameObject thisObject, GameObject target)
    {
        if (target == null)
        {
            return -1;
        }
        float sideA = Vector3.Distance(thisObject.transform.position, new Vector3(target.transform.position.x, thisObject.transform.position.y, target.transform.position.z));
        float sideB = target.transform.position.y - thisObject.transform.position.y;
        float angleRad = Mathf.Atan(sideB / sideA);
        return angleRad * Mathf.Rad2Deg;
    }


    #region Public Methods
    /// <summary>
    /// Returns the direction towards the target on the X and Z
    /// </summary>
    /// <returns></returns>
    public Vector3 GetDirectionToTarget()
    {
        return dirToTargert;
    }

    /// <summary>
    /// Get the intensity at which the players grapple should be fixed
    /// </summary>
    /// <returns></returns>
    public float GetDirectionChangeIntensity()
    {
        return tempIntesnity;
    }


    /// <summary>
    /// Will check if the changed direction will put player in correct swing angle
    /// </summary>
    public void UsedDirectionChange()
    {
        float angle = DegreesBetweenObjects(orientation.gameObject, grapplingGun.GetCurrentGrappledObject());

        if ((angle > 0) && Mathf.Abs(angle) > (minCheckAngle - 10) && Mathf.Abs(angle) < (maxCheckAngle + 10))
        {

            StopDirectionChange();
        }

    }

    private void StopDirectionChange()
    {
        StopAllCoroutines();
        dirToTargert = Vector3.zero;
        isFixing = false;
        checking = false;
        tempIntesnity = directionChangeIntensity;
        stuckHorizontal = false;
        currentAmmountOfBadLoops = neededAmmountOfBadLoops - 1;
    }

    public bool StuckHorizontal()
    {
        return stuckHorizontal;
    }
    #endregion
}
