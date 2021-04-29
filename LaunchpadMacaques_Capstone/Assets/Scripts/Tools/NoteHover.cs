using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHover : MonoBehaviour
{
    public GameObject player;
    public float speed;
    float height = 0.5f;
    Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            //find player
            player = GameObject.FindObjectOfType<Matt_PlayerMovement>().gameObject;
        }
        catch
        {
            //
        }

        pos = transform.position;
        print(pos);
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        Bob();
        Look();
    }

    //bob up and down
    void Bob()
    {
        
        //calculate what the new Y position will be
        float newY = Mathf.Sin(Time.time * speed)/4 + pos.y;
        //set the object's Y to the new calculated Y
        transform.position = new Vector3(pos.x, newY, pos.z);
    }

    void Look()
    {

        Vector3 target = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        transform.LookAt(target);

    }
}
