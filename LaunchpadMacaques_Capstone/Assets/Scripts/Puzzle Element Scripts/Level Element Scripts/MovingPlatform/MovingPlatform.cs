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

    //Inspector:
    [SerializeField, Tooltip("The array Vector3 points that the platform will go to (in order)")]
    private Vector3[] points;

    public enum MovementStyle { ConstantSpeed , ConstantTime };
    [SerializeField, Tooltip("The way the platform will move from point to point " +
        "\nConstant Speed: Will move at the same speed regardless of the distance between the two points" +
        "\nConstant Time: Will always take the same amount of time to get from point to point, but may alter speed to do so")]
    private MovementStyle movementStyle = MovementStyle.ConstantSpeed;

    [SerializeField, Tooltip("The float speed that the platform will go when moving to the next point. Only taken into account when the movement style is set to \"Constant Speed\"")]
    private float movementSpeed = 1;
    [SerializeField, Tooltip("The time it takes for the platform to get from point to point. Only taken into account when the movement style is set to \"Constant Time\"")]
    private float movementTime = 5f;

    [SerializeField, Tooltip("The time (in seconds) that the platform will wait at each point before continuing to the next")]
    private float holdTime = 0;

    private enum LoopPattern { Reverse, Cycle };
    [SerializeField, Tooltip("How the platform will continue once it reaches the end of its set points")]
    private LoopPattern loopPattern = LoopPattern.Reverse;

    [SerializeField, Tooltip("Should the platform accelerate or deaccelerate around each point?")]
    private bool easeInAndOut = false;
    [SerializeField, Tooltip("The distance from the start and end positions where the easing will start")]
    private float easeDistance = 2;

    [SerializeField, Tooltip("The prefab for the object that will appear at the endpoints of the moving platform path")]
    private GameObject endpointPrefab;
    [SerializeField, Tooltip("The scale of the endpoint model")]
    private float endpointScale = 0;

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
        InitializeEndPoints();
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
            float distance = 0;
            float currentTime = 0;

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

            distance = Vector3.Distance(startPoint, targetPoint);

            //Loop to move platform from startPoint to targetPoint
            while (transform.position != targetPoint)
            {
                Vector3 oldPoint = transform.position;
                Vector3 newPoint;

                //Slow down toward beginning and end and move at a constant speed
                if (easeInAndOut && movementStyle == MovementStyle.ConstantSpeed)
                {
                    //New point moves toward target point while lerping to slow down as it approaches or leaves the start or target point respectively
                    newPoint = Vector3.MoveTowards(transform.position, 
                                                   targetPoint,
                                                   Mathf.Lerp(0, 
                                                              movementSpeed,
                                                              Mathf.Clamp(Mathf.Min(Vector3.Distance(transform.position, startPoint), Vector3.Distance(transform.position, targetPoint)) / easeDistance, 
                                                                          0.05f, 
                                                                          1))
                                                   * Time.deltaTime);
                }
                //Don't slow down at the beginning and end and move at a constant speed
                else if(!easeInAndOut && movementStyle == MovementStyle.ConstantSpeed)
                {
                    newPoint = Vector3.MoveTowards(transform.position, targetPoint, movementSpeed * Time.deltaTime);
                }
                //Slow down at the beginning and end while arriving over a set time period
                else if(easeInAndOut && movementStyle == MovementStyle.ConstantTime)
                {
                    //You were working on this, have to figure out how to combine easing in with a consistent movement time
                    newPoint = Vector3.Lerp(startPoint, targetPoint, (currentTime / movementTime) * Time.deltaTime);
                    //newPoint = Vector3.Lerp(startPoint, targetPoint, (currentTime / movementTime) ) * Time.deltaTime);
                }
                //Don't slow down at the beginning and end and arrive to destination over a set time period
                else
                {
                    newPoint = Vector3.Lerp(startPoint, targetPoint, (currentTime / movementTime) * Time.deltaTime);
                }

                transform.position = newPoint;
                currentTime += Time.deltaTime;

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
        //Check slightly above the platform to see if player is on it
        Vector3 center = transform.position;
        center.y += 0.5f;

        Vector3 halfExtents = col.bounds.size / 2;

        Collider[] hit;
        hit = Physics.OverlapBox(center, halfExtents, transform.rotation);

        objectsOnPlatform.Clear();
        for (int i = 0; i < hit.Length; i++)
        {   
            //Only move players and cubes on the platform
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

    /// <summary>
    /// Generate the line render that connects the points of the moving platform's path
    /// </summary>
    private void InitializeLineRenderer()
    {

        lineRenderer = GetComponent<LineRenderer>();

        if (points.Length < 2)
            return;

        //Line Renderer (Fill backwards for texture)
        lineRenderer.positionCount = points.Length;
        for (int i = points.Length -1; i >= 0; i--)
        {
            lineRenderer.SetPosition(points.Length - 1 - i, points[i]);
        }

        lineRenderer.loop = loopPattern == LoopPattern.Cycle;
    }

    /// <summary>
    /// Generate the end points at the stopping points along the moving platform's path
    /// </summary>
    private void InitializeEndPoints()
    {
        GameObject endpoint;
        for (int i = 0; i < points.Length; i++)
        {
            endpoint = Instantiate<GameObject>(endpointPrefab, points[i], Quaternion.identity);
            endpoint.GetComponent<Collider>().isTrigger = true;
            endpoint.transform.localScale = new Vector3(endpointScale, endpointScale, endpointScale);

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

    public LineRenderer GetLineRenderer()
    {
        return GetComponent<LineRenderer>();
    }
    #endregion
}
