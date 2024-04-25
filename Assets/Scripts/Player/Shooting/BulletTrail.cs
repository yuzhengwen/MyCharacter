using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private Vector3 startingPos;
    private Vector3 targetPos;
    private float progress;
    [SerializeField] private float speed = 40f;
    public void SetTarget(Vector3 point)
    {
        targetPos = point.WithAxis(z: -1);
    }

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position.WithAxis(z: -1);
    }

    // Update is called once per frame
    void Update()
    { 
        progress += speed * Time.deltaTime;
        transform.position = Vector3.Lerp(startingPos, targetPos, progress);
    }
}