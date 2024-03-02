using System;
using UnityEngine;
using UnityEngine.InputSystem;
using YuzuValen.HFSM;
using static PlayerInputActions;

namespace YuzuValen
{
    public class PlayerMovement : MonoBehaviour, IGameplayActions
    {
        private Collider2D cl;
        public Rigidbody2D rb;
        public LayerMask terrain;
        StateMachine<string> fsm;
        private StateMachine<string> airborneFSM, groundedFSM;

        public float moveSpeed = 5f;
        private float xInput = 0;
        void Start()
        {
            cl = GetComponent<BoxCollider2D>();
            rb = GetComponent<Rigidbody2D>();

            SetUpFSM();
        }

        private void SetUpFSM()
        {
            fsm = new StateMachine<string>();
            airborneFSM = new StateMachine<string>();
            groundedFSM = new StateMachine<string>();

            SetupStates();
            SetupTransitions();
            fsm.Init();
            fsm.OnEnter();
        }
        private void SetupStates()
        {
            airborneFSM.AddState(PlayerState.AirborneState.Jumping, new JumpState<string>(this));
            airborneFSM.AddState(PlayerState.AirborneState.Falling);
            airborneFSM.AddState(PlayerState.AirborneState.Coyote);

            groundedFSM.AddState(PlayerState.GroundedState.Idle);
            groundedFSM.AddState(PlayerState.GroundedState.Walking, new MoveState<string>(this));

            fsm.AddState(PlayerState.Grounded, groundedFSM);
            fsm.AddState(PlayerState.Airborne, airborneFSM);
            fsm.AddState(PlayerState.Wall);

            groundedFSM.SetInitialState(PlayerState.GroundedState.Idle);
            airborneFSM.SetInitialState(PlayerState.AirborneState.Falling);
            fsm.SetInitialState(PlayerState.Airborne);
        }
        private void SetupTransitions()
        {
            // jump sequence
            fsm.AddTriggerTransition("JumpInput", PlayerState.Grounded, PlayerState.Airborne);
            airborneFSM.AddTransition(PlayerState.AirborneState.Jumping, PlayerState.AirborneState.Falling, () => rb.velocity.y < 0);
            airborneFSM.AddTransition(PlayerState.AirborneState.Falling, PlayerState.Grounded, () => IsGrounded());

            // ground movement sequence
            groundedFSM.AddTriggerTransition("XMoveInput", PlayerState.GroundedState.Idle, PlayerState.GroundedState.Walking);
            groundedFSM.AddTransition(PlayerState.GroundedState.Walking, PlayerState.GroundedState.Idle, () => MathF.Abs(rb.velocity.x) < 0.001f);

        }

        void Update()
        {
            fsm.Update();
            Debug.Log(string.Join(",", fsm.GetAllCurrentStates()));
        }
        private void FixedUpdate()
        {
            fsm.FixedUpdate();
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }

        private bool IsGrounded()
        {
            var hit = Physics2D.BoxCast(cl.bounds.center, cl.bounds.size, 0f, Vector2.down, 0.1f, terrain);
            return hit;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            xInput = context.ReadValue<float>();
            fsm.TriggerEvent("XMoveInput", context.ReadValue<float>(), moveSpeed);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                airborneFSM.SetInitialState(PlayerState.AirborneState.Jumping);
                fsm.TriggerEvent("JumpInput");
            }
        }

        public void OnAttack1(InputAction.CallbackContext context)
        {
            Debug.Log("Input: Attack1");
        }
    }
    /// <summary>
    /// Supports nested states
    /// </summary>
    public static class PlayerState
    {
        public const string Grounded = "GROUNDED";
        public const string Airborne = "AIRBORNE";
        public const string Wall = "WALL";
        public static class AirborneState
        {
            public const string Falling = "FALLING";
            public const string Jumping = "JUMPING";
            public const string Coyote = "COYOTE";
        }
        public static class GroundedState
        {
            public const string Idle = "IDLE";
            public const string Walking = "WALKING";
            public const string Running = "RUNNING";
            public const string Crouching = "CROUCHING";
        }
    }
}
