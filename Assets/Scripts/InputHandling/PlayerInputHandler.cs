using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuzuValen;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInputHandler : MonoBehaviour
{
    PlayerInputActions controls;
    PlayerMovement playerMovement;
    private void Awake()
    {
         controls = new PlayerInputActions();
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        controls.Gameplay.SetCallbacks(playerMovement);
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}
