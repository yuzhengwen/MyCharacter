using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrenadeLauncher : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private float timeBetweenPoints = 0.1f;
    private int noOfPoints = 25;
    private float throwStrength = 50f;
    private float grenadeMass = 5f;
    private float gravity = 9.81f;
    private Vector3 acceleration;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        lineRenderer = GetComponent<LineRenderer>();
        acceleration = new Vector3(0, -gravity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
            DrawProjection();
    }

    private void DrawProjection()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = Mathf.CeilToInt(noOfPoints/timeBetweenPoints) + 1;
        Vector3 shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        Vector3 startVelocity = throwStrength * shootDirection / grenadeMass;
        int i = 0;
        lineRenderer.SetPosition(i, initialPosition);
        for (float t = 0; t < noOfPoints; t += timeBetweenPoints)
        {
            i++;
            Vector3 pos = initialPosition + t * startVelocity + 0.5f * acceleration * t * t;
            lineRenderer.SetPosition(i, pos);
        }
    }
}
