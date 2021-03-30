/*****************************************************************************
// File Name: SlowHover.cs
// Author: Connor Wolf
// Creation Date: 2/16/20
//
// Brief Description: This script causes collectibles to slowly hover up and down in place
*****************************************************************************/

using UnityEngine;

public class SlowHover : MonoBehaviour
{
    public float hoverVary;
    [SerializeField]
    private GameObject _childMesh = null;
    private float offset;

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y + (Random.Range(-hoverVary, hoverVary)), transform.position.z);

        if(_childMesh != null)
            _childMesh.transform.rotation = transform.rotation;
        offset = Random.Range(-1, 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.timeScale != 0)
        {
            transform.Translate(Vector3.up * Mathf.Cos(Time.time+offset) * hoverVary /** Time.fixedDeltaTime*/, Space.World);
        }
    }
}
