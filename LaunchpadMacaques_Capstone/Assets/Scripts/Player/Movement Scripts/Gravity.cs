using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gravity : MonoBehaviour
{
    public float gravity = 1100f;
    private bool useGravity = true;
    private float orgGravity = 0;
    Rigidbody rb = null;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        orgGravity = gravity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (useGravity)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * gravity);
        }

    }
    public float GetGravityAmmount()
    {
        return gravity;
    }

    public void SetGravityAmmount(float grav)
    {
        gravity = grav;
    }

    public void ResetGravityAmmount()
    {
        gravity = orgGravity;
    }


    public void UseGravity(bool use)
    {
        useGravity = use;
    }

    public float GetOrgGravity()
    {
        return orgGravity;
    }
}
