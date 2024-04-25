using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrenadeLauncher : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private float timeBetweenPoints = 0.1f;
    private int noOfPoints = 25;
    [SerializeField] private float startThrowStrength = 50f;
    [SerializeField] private float maxThrowStrength = 90f;
    [SerializeField] private float grenadeMass = 5f;
    [SerializeField] private float gravity = 9.81f;
    private Vector3 acceleration;
    private Vector3 shootDirection;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        acceleration = new Vector3(0, -gravity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.G))
            DrawProjection();
        if (Input.GetKeyUp(KeyCode.G))
        {
            startThrowStrength = 50f;
            lineRenderer.enabled = false;
        }
    }

    private void DrawProjection()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = Mathf.CeilToInt(noOfPoints / timeBetweenPoints) + 1;

        shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        Vector3 startVelocity = startThrowStrength * shootDirection / grenadeMass;
        int i = 0;
        lineRenderer.SetPosition(i, transform.position);
        for (float t = 0; t < noOfPoints; t += timeBetweenPoints)
        {
            i++;
            Vector3 pos = transform.position + t * startVelocity + 0.5f * acceleration * t * t;
            lineRenderer.SetPosition(i, pos);
        }
        if (startThrowStrength > maxThrowStrength) return;
        startThrowStrength += 0.05f;
    }
}
