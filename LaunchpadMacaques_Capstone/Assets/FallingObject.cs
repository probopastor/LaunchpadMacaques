/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Unknown) 
* (FallingObject.cs) 
* (Will handle falling platforms based on player distance) 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    [Header("Falling Settings")]
    [SerializeField, Tooltip("The Distance the player has to be within for this object to begin falling")] float distanceToStartFalling;
    [SerializeField, Tooltip("The speed at which this object falls")] float fallSpeed;
    [SerializeField, Tooltip("The Delay time before this object actually begins to fall")] float delayBeforeFalling;

    [Header("Death Settings")]
    [SerializeField, Tooltip("The Tags that will kill this object")] List<string> deathTags;

    [Header("Shake Settings")]
    [SerializeField] float shakeSpeed = 40;
    [SerializeField] float shakeAmmount = 5;

    [Header("Other Settings")]
    [SerializeField] bool playerCanStandOn = true;

    private Vector3 orgPos;
    private bool falling = false;

    private Matt_PlayerMovement player;

    List<GameObject> objectsOnPlatform = new List<GameObject>();


    private void Start()
    {
        orgPos = this.transform.position;
        player = FindObjectOfType<Matt_PlayerMovement>();
    }

    private void Update()
    {
        CheckPlayerDistance();
    }

    /// <summary>
    /// Will handle the starting of the falling routine
    /// </summary>
    private void CheckPlayerDistance()
    {
        if (!falling && Vector3.Distance(this.transform.position, player.transform.position) < distanceToStartFalling)
        {
            StartCoroutine(Falling());
        }
    }


    /// <summary>
    /// Turns off this object
    /// </summary>
    private void KillThisObject()
    {
        objectsOnPlatform.Clear();
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// Respawns the object at this objects orriginal position
    /// </summary>
    public void RespawnObject()
    {
        falling = false;
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        this.GetComponent<BoxCollider>().enabled = true;
        this.transform.position = orgPos;
    }

    /// <summary>
    /// Will handle the falling of this object
    /// </summary>
    /// <returns></returns>
    IEnumerator Falling()
    {
        falling = true;
        float currentTime = 0;


        // Will loop through and shake the platform
        // Will end once the delay before falling time has been reached
        while(currentTime < delayBeforeFalling)
        {
            Vector3 newPos = this.transform.position;
            newPos.x += Mathf.Sin(Time.time * shakeSpeed) * shakeAmmount * Time.deltaTime;
            UpdateObjectsOnPlatform(new Vector3(Mathf.Sin(Time.time * shakeSpeed) * shakeAmmount * Time.deltaTime, 0, 0));

            this.transform.position = newPos;

            currentTime += Time.deltaTime;

            yield return null;
        }

        // Will keep pushing the platform down until something kills the platform
        while (true)
        {
            Vector3 newPos = this.transform.position;
         
            newPos.y -= fallSpeed * Time.deltaTime;
            UpdateObjectsOnPlatform(new Vector3(0, -(fallSpeed * Time.deltaTime), 0));


            this.transform.position = newPos;

            yield return null;
        }
    }

    /// <summary>
    /// Will update the players position
    /// </summary>
    /// <param name="movementToAdd"></param>
    private void UpdateObjectsOnPlatform(Vector3 movementToAdd)
    {
        foreach (GameObject x in objectsOnPlatform)
        {
            x.transform.position += movementToAdd;
        }
    }

    #region Collision Settings

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (deathTags.Contains(other.gameObject.tag))
        {
            StopAllCoroutines();
            KillThisObject();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player") && playerCanStandOn)
        {
            objectsOnPlatform.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            objectsOnPlatform.Remove(collision.gameObject);
        }
    }
    #endregion
}
