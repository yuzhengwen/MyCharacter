using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SamplePlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float xInput = 0;
    public float speed = 5;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxis("Horizontal");
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
    }
}
