/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Unknown) 
* (FallingObject.cs) 
* (Will handle falling platforms based on player distance) 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FallingObject : MonoBehaviour
{
    [Header("Falling Settings")]
    float fallSpeed;
    float delayBeforeFalling;

    [Header("Death Settings")]
    protected List<string> deathTags;

    [Header("Shake Settings")]
    float shakeSpeed = 40;
    float shakeAmmount = 5;

    [Header("Other Settings")]
   protected bool playerCanStandOn = true;

    private Vector3 orgPos;
    protected bool falling = false;

    protected Matt_PlayerMovement player;

   protected List<GameObject> objectsOnPlatform = new List<GameObject>();

    private bool respawnOnDeath;


    private void Start()
    {
        orgPos = this.transform.position;
        player = FindObjectOfType<Matt_PlayerMovement>();
    }

    public virtual void Update() { }

    /// <summary>
    /// Will handle the starting of the falling routine
    /// </summary>


    public virtual void SetVariables(float fallSpeed, float delayBeforeFalling, List<string> deathTags, float shakeSpeed, float shakeAmmount, bool playerCanStandOn, bool respawOnDeath)
    {
        this.fallSpeed = fallSpeed;
        this.delayBeforeFalling = delayBeforeFalling;
        this.deathTags = deathTags;
        this.shakeSpeed = shakeSpeed;
        this.shakeAmmount = shakeAmmount;
        this.playerCanStandOn = playerCanStandOn;
        this.respawnOnDeath = respawOnDeath;
    }

    /// <summary>
    /// Turns off this object
    /// </summary>
  protected void KillThisObject()
    {
        objectsOnPlatform.Clear();
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<BoxCollider>().enabled = false;
        Destroy(this.gameObject.GetComponent<Rigidbody>());
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
   public IEnumerator Falling()
    {
        falling = true;
        float currentTime = 0;
        var temp = this.gameObject.AddComponent<Rigidbody>();
        temp.useGravity = false;
        temp.constraints = RigidbodyConstraints.FreezeRotation;


        // Will loop through and shake the platform
        // Will end once the delay before falling time has been reached
        while (currentTime < delayBeforeFalling)
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
    protected void OnTriggerEnter(Collider other)
    {
        if (deathTags.Contains(other.gameObject.tag))
        {
            StopAllCoroutines();
            KillThisObject();
        }
    }



    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerCanStandOn)
        {
            objectsOnPlatform.Add(collision.gameObject);
        }

        if (deathTags.Contains(collision.gameObject.tag))
        {
            StopAllCoroutines();
            KillThisObject();
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            objectsOnPlatform.Remove(collision.gameObject);
        }
    }

    #endregion
}
