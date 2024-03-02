using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform gunPoint;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private float weaponRange = 10f;

    private Vector3 shootDirection;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        shootDirection = (mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        var hit = Physics2D.Raycast(gunPoint.position, shootDirection, weaponRange);
        
        var trailObj = Instantiate(bulletTrail, gunPoint.position, transform.rotation);
        var trail = trailObj.GetComponent<BulletTrail>();

        if (!hit.collider)
        {
            trail.SetTarget(gunPoint.position + shootDirection * weaponRange);
        }
        else
        {
            trail.SetTarget(hit.point);
        }
    }
}
