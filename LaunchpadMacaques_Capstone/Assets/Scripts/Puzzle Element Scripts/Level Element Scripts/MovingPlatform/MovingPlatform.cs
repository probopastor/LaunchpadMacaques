using FMOD;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    //PRIVATE:
    //Inspector:
    [SerializeField, Tooltip("The array Vector3 points that the platform will go to (in order)")]
    private Vector3[] points;

    [SerializeField, Tooltip("The float speed that the platform will go when moving to the next point")]
    private float movementSpeed = 1;

    [SerializeField, Tooltip("The time (in seconds) that the platform will wait at each point before continuing to the next")]
    private float holdTime = 0;

    private enum LoopPattern { Reverse, Cycle };
    [SerializeField, Tooltip("How the platform will continue once it reaches the end of its set points")]
    private LoopPattern loopPattern = LoopPattern.Reverse;

    [SerializeField, Tooltip("Should the platform accelerate or deaccelerate around each point?")]
    private bool easeInAndOut;
    [SerializeField, Tooltip("The distance from the start and end positions where the easing will start")]
    private float easeDistance = 2;
    //Non-Inspector:
    private int currentPointIndex = 0;

    private void Start()
    {
        if (points.Length > 1)
        {
            transform.position = points[currentPointIndex];
            StartCoroutine(Move());
        }

    }

    IEnumerator Move()
    {
        while(true)
        { 
            Vector3 startPoint = points[currentPointIndex];
            Vector3 targetPoint = Vector3.zero;

            if (currentPointIndex + 1 >= points.Length)
            {
                switch ((int)loopPattern)
                {
                    case (int)LoopPattern.Reverse:
                        System.Array.Reverse(points);
                        currentPointIndex = 1;
                        targetPoint = points[currentPointIndex];
                        break;
                    case (int)LoopPattern.Cycle:
                        currentPointIndex = 0;
                        targetPoint = points[currentPointIndex];
                        break;
                }
            }
            else
            {
                currentPointIndex++;
                targetPoint = points[currentPointIndex];
            }

            while (transform.position != targetPoint)
            {
                Vector3 newPoint;

                if (easeInAndOut)
                {
                    newPoint = Vector3.MoveTowards(transform.position, targetPoint,
                        Mathf.Lerp(0, movementSpeed,
                                   Mathf.Clamp((Mathf.Min(Vector3.Distance(transform.position, startPoint), Vector3.Distance(transform.position, targetPoint)) / easeDistance), 
                                               0.05f, 
                                               Mathf.Infinity))
                                   * Time.deltaTime);
                }
                else
                {
                    newPoint = Vector3.MoveTowards(transform.position, targetPoint, movementSpeed * Time.deltaTime);
                }

                transform.position = newPoint;

                yield return null;
            }

            yield return new WaitForSeconds(holdTime);
        }
    }

    #region Getters & Setters
    public void SetPoint(int index, Vector3 value)
    {
        points[index] = value;
    }

    public Vector3 GetPoint(int index)
    {
        return points[index];
    }
    #endregion
}
