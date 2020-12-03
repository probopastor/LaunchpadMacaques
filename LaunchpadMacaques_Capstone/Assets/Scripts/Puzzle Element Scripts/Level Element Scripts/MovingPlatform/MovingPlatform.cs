/*
 * Launchpad Macaques - Neon Oblivion
 * Zackary Seiple
 * MovingPlatform.cs
 * This script controls the behaviour and options of the moving platform modular puzzle element
 */

using FMOD;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
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
    private Collider col;
    [SerializeField, HideInInspector, Tooltip("The Line Renderer that visually represents the path the moving platform will take between the different points")]
    private LineRenderer lineRenderer;
    [Tooltip("A set that actively contains the Transforms of the objects currently on this platform")]
    private HashSet<Transform> objectsOnPlatform;

    private void Start()
    {
        InitializeLineRenderer();
        col = GetComponent<Collider>();
        objectsOnPlatform = new HashSet<Transform>();

        if (points.Length > 1)
        {
            transform.position = points[currentPointIndex];
            StartCoroutine(Move());
        }

    }

    /// <summary>
    /// Handles the movement of the platform (surprise)
    /// </summary>
    /// <returns></returns>
    IEnumerator Move()
    {
        //ABM (Always. Be. Movin.)
        while(true)
        { 
            //The point the platform is currently at when this iteration starts
            Vector3 startPoint = points[currentPointIndex];
            //The next point in the 
            Vector3 targetPoint = Vector3.zero;

            //Check if the point is the last point, if so, follow the loop pattern to continue
            if (currentPointIndex + 1 >= points.Length)
            {
                switch ((int)loopPattern)
                {
                    //Reverse, flip the array around and continue (go back the way it came)
                    case (int)LoopPattern.Reverse:
                        System.Array.Reverse(points);
                        currentPointIndex = 1;
                        targetPoint = points[currentPointIndex];
                        InitializeLineRenderer();
                        break;
                    //Keep order, go straight back to the starting point after end point
                    case (int)LoopPattern.Cycle:
                        currentPointIndex = 0;
                        targetPoint = points[currentPointIndex];
                        break;
                }
            }
            //Point is not last point, proceed as normal
            else
            {
                currentPointIndex++;
                targetPoint = points[currentPointIndex];
            }

            //Loop to move platform from startPoint to targetPoint
            while (transform.position != targetPoint)
            {
                Vector3 oldPoint = transform.position;
                Vector3 newPoint;

                //Slow down toward beginning and end
                if (easeInAndOut)
                {
                    //New point moves toward target point while lerping to slow down as it approaches or leaves the start or target point respectively
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

                //Physics-based operations are used to detect and move objects on platforms, must wait for fixed update to do so
                yield return new WaitForFixedUpdate();
                CheckForObjectsOnPlatform();
                UpdateObjectsOnPlatform(newPoint - oldPoint);
            }

            //Wait for hold time before continuing
            yield return new WaitForSeconds(holdTime);
        }
    }

    /// <summary>
    /// Checks for objects (Player and PlayerCube) on Moving Platform, assigns results to the HashSet 'objectsOnPlatform'
    /// </summary>
    private void CheckForObjectsOnPlatform()
    {
        Vector3 center = transform.position;
        center.y += 0.5f;

        Vector3 halfExtents = col.bounds.size / 2;

        Collider[] hit;
        hit = Physics.OverlapBox(center, halfExtents, transform.rotation);

        objectsOnPlatform.Clear();
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].tag == "Player" || hit[i].tag == "PlayerCube")
            {
                UnityEngine.Debug.Log("Hit Player");
                objectsOnPlatform.Add(hit[i].transform);
            }
        }

    }

    /// <summary>
    /// Moves every object on this platform (every Transform in objectsOnPlatform) accordingly by the Vector3 movementToAdd
    /// </summary>
    /// <param name="movementToAdd">The Vector3 movement that each object needs to follow</param>
    private void UpdateObjectsOnPlatform(Vector3 movementToAdd)
    {
        foreach(Transform x in objectsOnPlatform)
        {
            UnityEngine.Debug.Log("Adding Movement");
            x.position += movementToAdd;
        }
    }

    private void InitializeLineRenderer()
    {

        lineRenderer = GetComponent<LineRenderer>();

        if (points.Length < 2)
            return;

        //Line Renderer (Fill backwards for texture
        lineRenderer.positionCount = points.Length;
        for (int i = points.Length -1; i >= 0; i--)
        {
            lineRenderer.SetPosition(points.Length - 1 - i, points[i]);
        }

        lineRenderer.loop = loopPattern == LoopPattern.Cycle;
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

    public LineRenderer GetLineRenderer()
    {
        return GetComponent<LineRenderer>();
    }
    #endregion
}
