using Game.Runtime.Entities.Player.Components;
using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Utilities.Patterns.StateMachines;
using Game.Runtime.Entities.Player.States;
using KBCore.Refs;
using Game.Runtime.Components.Inputs;
using UnityEngine;
using System;
using Game.Runtime.Utilities.Helpers.Timers;

namespace Game.Runtime.Entities.Player.Controllers
{
    [RequireComponent(typeof(PlayerMover), typeof(CeilingDetector))]
    public class PlayerMovementController : StatefulEntity
    {
        [SerializeField] InputReader input;
        [SerializeField, Self] Transform tr;
        [SerializeField, Self] PlayerMover mover;
        [SerializeField, Self] CeilingDetector ceilingDetector;
        [SerializeField] bool useLocalMomentum = false;
        [SerializeField] float movementSpeed = 7f;
        [SerializeField] float airControlRate = 2f;
        [SerializeField] float jumpSpeed = 10f;
        [SerializeField] float jumpDuration = 0.2f;
        [SerializeField] float airFriction = 0.5f;
        [SerializeField] float groundFriction = 100f;
        [SerializeField] float gravity = 30f;
        [SerializeField] float slideGravity = 5f;
        [SerializeField] float slopeLimit = 30f;
        bool jumpKeyIsPressed, jumpKeyWasPressed, jumpKeyWasLetGo, jumpInputIsLocked, isMoving;
        float friction;
        Vector3 momentum, savedVelocity, savedMovementVelocity, currentVelocity, horizontalMomentum, verticalMomentum;
        Transform cameraTransform;
        CountdownTimer jumpTimer;

        public Vector3 GetVelocity => savedVelocity;
        public Vector3 GetMomentum => useLocalMomentum ? tr.localToWorldMatrix * momentum : momentum;
        public Vector3 GetMovementVelocity => savedMovementVelocity;
        bool IsGrounded => StateMachine.CurrentState is PlayerGroundedState or PlayerSlidingState;
        bool IsRising => VectorMath.GetDotProduct(GetMomentum, tr.up) > 0f;
        bool IsFalling => VectorMath.GetDotProduct(GetMomentum, tr.up) < 0f;
        bool IsGroundTooSteep => !mover.IsGrounded || Vector3.Angle(mover.GroundNormal, tr.up) > slopeLimit;

        public event Action OnIdle = delegate { };
        public event Action OnMove = delegate { };
        public event Action<Vector3> OnJump = delegate { };
        public event Action<Vector3> OnLand = delegate { };

        protected override void Awake()
        {
            base.Awake();
            cameraTransform = Camera.main.transform;
            jumpTimer = new CountdownTimer(jumpDuration);
            SetupStateMachine();
        }

        void SetupStateMachine()
        {
            var grounded = new PlayerGroundedState(this);
            var falling = new PlayerFallingState(this); //BUG: Cant fix transistion to ground
            var sliding = new PlayerSlidingState(this);
            var rising = new PlayerRisingState(this);
            var jumping = new PlayerJumpingState(this);

            At(grounded, rising, IsRising);
            At(grounded, sliding, mover.IsGrounded && IsGroundTooSteep);
            At(grounded, falling, !mover.IsGrounded);
            At(grounded, jumping, (jumpKeyIsPressed || jumpKeyWasPressed) && !jumpInputIsLocked);

            At(falling, rising, IsRising);
            At(falling, grounded, mover.IsGrounded && !IsGroundTooSteep);
            At(falling, sliding, mover.IsGrounded && IsGroundTooSteep);

            At(sliding, rising, IsRising);
            At(sliding, falling, !mover.IsGrounded);
            At(sliding, grounded, mover.IsGrounded && !IsGroundTooSteep);

            At(rising, grounded, mover.IsGrounded && !IsGroundTooSteep);
            At(rising, sliding, mover.IsGrounded && IsGroundTooSteep);
            At(rising, falling, IsFalling);
            At(rising, falling, ceilingDetector.HitCeiling);

            At(jumping, rising, jumpTimer.IsFinished || jumpKeyWasLetGo);
            At(jumping, falling, ceilingDetector.HitCeiling);

            StateMachine.SetState(falling);
        }

        void OnEnable()
        {
            input.EnablePlayerMap();
            input.Jump += HandleJumpKeyInput;
        }

        void OnDisable() => input.Jump -= HandleJumpKeyInput;

        void HandleJumpKeyInput(bool isButtonPressed)
        {
            if (!jumpKeyIsPressed && isButtonPressed)
                jumpKeyWasPressed = true;

            if (jumpKeyIsPressed && !isButtonPressed)
            {
                jumpKeyWasLetGo = true;
                jumpInputIsLocked = false;
            }

            jumpKeyIsPressed = isButtonPressed;
        }

        protected override void Update() => base.Update();

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            ResetJumpKeys();

            mover.CheckForGround();
            ceilingDetector.Reset();

            HandleMomentum();
            HandleVelocity();
            SetMover();
        }

        void HandleVelocity()
        {
            Debug.Log(StateMachine.CurrentState);
            Debug.Log(mover.IsGrounded);
            Debug.Log(!IsGroundTooSteep);

            if (StateMachine.CurrentState is not PlayerGroundedState)
                currentVelocity = Vector3.zero;
            currentVelocity += useLocalMomentum ? tr.localToWorldMatrix * momentum : momentum;

            savedVelocity = currentVelocity;
            savedMovementVelocity = CalculateMovementVelocity();

            var wasMoving = isMoving;
            isMoving = GetMovementVelocity != Vector3.zero;
            if (isMoving != wasMoving)
                (isMoving ? OnMove : OnIdle).Invoke();
        }

        void SetMover()
        {
            mover.SetExtendSensorRange(IsGrounded);
            mover.SetVelocity(currentVelocity);
        }

        Vector3 CalculateMovementVelocity() => CalculateMovementDirection() * movementSpeed;

        Vector3 CalculateMovementDirection()
        {
            var direction = cameraTransform == null
                ? tr.right * input.Direction.x + tr.forward * input.Direction.y
                : Vector3.ProjectOnPlane(cameraTransform.right, tr.up).normalized * input.Direction.x +
                  Vector3.ProjectOnPlane(cameraTransform.forward, tr.up).normalized * input.Direction.y;
            return direction.magnitude > 1f ? direction.normalized : direction;
        }

        void HandleMomentum()
        {
            momentum = useLocalMomentum ? tr.localToWorldMatrix * momentum : tr.worldToLocalMatrix * momentum;
            verticalMomentum = VectorMath.ExtractDotVector(momentum, tr.up);
            verticalMomentum -= tr.up * (gravity * Time.deltaTime);
            horizontalMomentum = momentum - verticalMomentum;
            horizontalMomentum = Vector3.MoveTowards(horizontalMomentum, Vector3.zero, friction * Time.deltaTime);
            momentum = horizontalMomentum + verticalMomentum;
        }

        public void GroundMomentum()
        {
            friction = groundFriction;

            currentVelocity = CalculateMovementVelocity();

            AdjustHorizontalMomentum(CalculateMovementVelocity());

            if (VectorMath.GetDotProduct(verticalMomentum, tr.up) < 0f)
                verticalMomentum = Vector3.zero;
        }

        public void SlidingMomentum()
        {
            HandleSliding();

            AdjustHorizontalMomentum(CalculateMovementVelocity());

            momentum = Vector3.ProjectOnPlane(momentum, mover.GroundNormal);
            if (VectorMath.GetDotProduct(momentum, tr.up) > 0f)
                momentum = VectorMath.RemoveDotVector(momentum, tr.up);

            var slideDirection = Vector3.ProjectOnPlane(-tr.up, mover.GroundNormal).normalized;
            momentum += slideDirection * (slideGravity * Time.deltaTime);
        }

        void AdjustHorizontalMomentum(Vector3 movementVelocity)
        {
            if (horizontalMomentum.magnitude > movementSpeed)
            {
                if (VectorMath.GetDotProduct(movementVelocity, horizontalMomentum.normalized) > 0f)
                    movementVelocity = VectorMath.RemoveDotVector(movementVelocity, horizontalMomentum.normalized);

                horizontalMomentum += movementVelocity * (Time.deltaTime * airControlRate * 0.25f);
            }
            else
            {
                horizontalMomentum += movementVelocity * (Time.deltaTime * airControlRate);
                horizontalMomentum = Vector3.ClampMagnitude(horizontalMomentum, movementSpeed);
            }
        }

        void HandleSliding()
        {
            var pointDownVector = Vector3.ProjectOnPlane(mover.GroundNormal, tr.up).normalized;
            var movementVelocity = CalculateMovementVelocity();
            movementVelocity = VectorMath.RemoveDotVector(movementVelocity, pointDownVector);
            horizontalMomentum += movementVelocity * Time.fixedDeltaTime;
        }

        public void OnJumpStart()
        {
            momentum = useLocalMomentum ? tr.localToWorldMatrix * momentum : tr.worldToLocalMatrix * momentum;
            momentum += tr.up * jumpSpeed;

            jumpTimer.Start();

            jumpInputIsLocked = true;

            OnJump.Invoke(momentum);
        }

        public void HandleJumping()
        {
            momentum = VectorMath.RemoveDotVector(momentum, tr.up);
            momentum += tr.up * jumpSpeed;
        }

        void ResetJumpKeys()
        {
            jumpKeyWasLetGo = false;
            jumpKeyWasPressed = false;
        }

        public void OnGroundContactLost()
        {
            friction = airFriction;

            momentum = useLocalMomentum ? tr.localToWorldMatrix * momentum : tr.worldToLocalMatrix * momentum;

            var velocity = GetMovementVelocity;
            if (velocity.sqrMagnitude >= 0f && momentum.sqrMagnitude > 0f)
            {
                var projectedMomentum = Vector3.Project(momentum, velocity.normalized);
                var dot = VectorMath.GetDotProduct(projectedMomentum.normalized, velocity.normalized);

                if (projectedMomentum.sqrMagnitude >= velocity.sqrMagnitude && dot > 0f)
                    velocity = Vector3.zero;
                else if (dot > 0f)
                    velocity -= projectedMomentum;
            }
            momentum += velocity;
        }

        public void OnFallStart()
        {
            var currentUpMomemtum = VectorMath.ExtractDotVector(momentum, tr.up);
            momentum = VectorMath.RemoveDotVector(momentum, tr.up);
            momentum -= tr.up * currentUpMomemtum.magnitude;
        }

        public void OnGroundContactRegained()
        => OnLand.Invoke(useLocalMomentum ? tr.localToWorldMatrix * momentum : momentum);
    }
}