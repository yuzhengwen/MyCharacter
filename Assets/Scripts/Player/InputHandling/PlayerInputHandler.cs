using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using YuzuValen;
using YuzuValen.AbilitySystem;
using static PlayerInputActions;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInputHandler : MonoBehaviour, IGameplayActions
{
    public PlayerInputActions controls;
    private PlayerMovement playerMovement;
    public AbilityController abilityController;

    public float xInput = 0;

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
        controls.Gameplay.SetCallbacks(this);
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<float>();
        if (context.phase == InputActionPhase.Canceled)
        {
            playerMovement.fsm.TriggerEvent("XMoveInputReleased", new MoveEventArgs(xInput));
            return;
        }
        playerMovement.fsm.TriggerEvent("XMoveInput", new MoveEventArgs(xInput));
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            playerMovement.fsm.TriggerEvent("JumpInput");
        }
        if (context.canceled)
        {
            playerMovement.fsm.TriggerEvent("JumpInputReleased");
        }
    }

    public void OnSwordAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            abilityController.UseAbility(0);
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            abilityController.UseAbility(1);
        }
    }
    public void OnAttack1(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            abilityController.UseAbility(2);
        }
    }
    public void OnAttack2(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            abilityController.UseAbility(3);
        }
    }
    public void OnAttack3(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            abilityController.UseAbility(4);
        }
    }
    public class MoveEventArgs : EventArgs
    {
        public float xInput;
        public MoveEventArgs(float xInput)
        {
            this.xInput = xInput;
        }
    }
}
