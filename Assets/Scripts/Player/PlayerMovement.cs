using System;
using UnityEngine;
using UnityEngine.InputSystem;
using YuzuValen.HFSM;
using static PlayerInputActions;

namespace YuzuValen
{
    public class PlayerMovement : MonoBehaviour
    {
        private Collider2D cl;
        public Rigidbody2D rb;
        public LayerMask terrain;

        public StateMachine<string> fsm;
        public StateMachine<string> airborneFSM, groundedFSM;
        private PlayerInputHandler inputHandler;

        public float moveSpeed = 5f;
        void Start()
        {
            cl = GetComponent<BoxCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            inputHandler = GetComponent<PlayerInputHandler>();

            SetUpFSM();
        }

        private void SetUpFSM()
        {
            fsm = new PlayerSM("MainSM", this);
            airborneFSM = new PlayerSM(PlayerState.Airborne, this);
            groundedFSM = new GroundSM(PlayerState.Grounded, this);

            SetupStates();
            SetupTransitions();
            fsm.OnEnter();
        }
        private void SetupStates()
        {
            airborneFSM.AddState(new JumpState(PlayerState.AirborneState.Jumping, this));
            airborneFSM.AddState(PlayerState.AirborneState.Falling);
            airborneFSM.AddState(PlayerState.AirborneState.Coyote);

            groundedFSM.AddState(PlayerState.GroundedState.Idle);
            groundedFSM.AddState(PlayerState.GroundedState.Walking);

            fsm.AddState(groundedFSM);
            fsm.AddState(airborneFSM);
            fsm.AddState(PlayerState.Wall);

            groundedFSM.SetInitialState(PlayerState.GroundedState.Idle);
            airborneFSM.SetInitialState(PlayerState.AirborneState.Falling);
            fsm.SetInitialState(PlayerState.Airborne);
        }
        private void SetupTransitions()
        {
            // jump sequence
            airborneFSM.AddTransition(PlayerState.AirborneState.Falling, PlayerState.Grounded, () => IsGrounded());

            // ground movement sequence
            groundedFSM.AddTriggerTransition("XMoveInput", PlayerState.GroundedState.Idle, PlayerState.GroundedState.Walking);
            groundedFSM.AddTriggerTransition("XMoveInputReleased", PlayerState.GroundedState.Walking, PlayerState.GroundedState.Idle);

        }

        void Update()
        {
            fsm.Update();
            Debug.Log(string.Join(",", fsm.GetAllCurrentStates()));
        }
        private void FixedUpdate()
        {
            fsm.FixedUpdate();
            rb.velocity = new Vector2(inputHandler.xInput * moveSpeed, rb.velocity.y);
        }

        private bool IsGrounded()
        {
            var hit = Physics2D.BoxCast(cl.bounds.center, cl.bounds.size, 0f, Vector2.down, 0.1f, terrain);
            return hit;
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
