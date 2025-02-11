using UnityEngine;
using System;
using Game.Runtime.Entities.Player.Components;
using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Utilities.Patterns.StateMachines;
using Game.Runtime.Entities.Player.States;
using Game.Runtime.Systems.Inputs;
using Game.Runtime.Systems.Interaction;
using KBCore.Refs;

namespace Game.Runtime.Entities.Player.Controllers
{
    public class MovementController : StatefulEntity, IImpactable //BUG: Dont Move
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Self] PlayerMover _playerMover;
        [SerializeField, Self] CeilingDetector _ceilingDetector;
        [SerializeField] Transform _cameraTransform;
        [SerializeField] InputReader _input;
        [SerializeField] float _movementSpeed = 7f;
        [SerializeField] float _airControlRate = 2f;
        [SerializeField] float _airFriction = 0.5f;
        [SerializeField] float _groundFriction = 100f;
        [SerializeField] float _gravity = 30f;
        [SerializeField] float _slideGravity = 5f;
        [SerializeField] float _slopeLimit = 30f;
        [SerializeField] bool _useLocalMomentum = false;
        [SerializeField] bool isShowDebug = true;
        [SerializeField] Rect stateDebugText = new(10f, 10f, 200f, 20f);
        bool _isRunning;
        Vector3 _momentum;
        Vector3 _savedMovementVelocity;
        Vector3 savedVelocity;

        public Vector3 GetVelocity => savedVelocity;
        public Vector3 GetMomentum => _useLocalMomentum ? tr.localToWorldMatrix * _momentum : _momentum;
        public Vector3 GetMovementVelocity => _savedMovementVelocity;
        bool IsGrounded => StateMachine.CurrentState is GroundedState or SlidingState;
        bool IsRising => VectorMath.GetDotProduct(GetMomentum, tr.up) > 0f;
        bool IsFalling => VectorMath.GetDotProduct(GetMomentum, tr.up) < 0f;
        bool IsGroundTooSteep => !_playerMover.IsGrounded || Vector3.Angle(_playerMover.GetGroundNormal, tr.up) > _slopeLimit;

        public event Action<Vector3> OnLand = delegate { };
        public event Action OnRun = delegate { };
        public event Action OnIdle = delegate { };

        public void ApplyForce(Vector3 direction, float force)
        {
            _playerMover.SetVelocity(direction * force);
            Debug.Log("Player Get Force!");
        }

        protected override void Awake()
        {
            base.Awake();
            SetupStateMachine();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            _playerMover.CheckForGround();

            HandleMomentum();

            var velocity = StateMachine.CurrentState is GroundedState ? CalculateMovementVelocity() : Vector3.zero;
            velocity += _useLocalMomentum ? tr.localToWorldMatrix * _momentum : _momentum;

            if (_input.Direction != Vector2.zero)
            {
                if (!_isRunning)
                {
                    _isRunning = true;
                    OnRun?.Invoke();
                }
            }
            else
            {
                if (_isRunning)
                {
                    _isRunning = false;
                    OnIdle?.Invoke();
                }
            }

            _playerMover.SetExtendSensorRange(IsGrounded);
            _playerMover.SetVelocity(velocity);

            savedVelocity = velocity;
            _savedMovementVelocity = CalculateMovementVelocity();

            _ceilingDetector.ResetCeilingDetection();
        }

        protected override void Update() => base.Update();

        void OnGUI()
        {
            if (!isShowDebug) return;
            GUI.Label(stateDebugText, $"Current Player State: {StateMachine.CurrentState}");
        }

        void SetupStateMachine()
        {
            var grounded = new GroundedState(this);
            var falling = new FallingState(this);
            var sliding = new SlidingState(this);
            var rising = new RisingState(this);

            At(grounded, rising, IsRising);
            At(grounded, sliding, _playerMover.IsGrounded && IsGroundTooSteep);
            At(grounded, falling, !_playerMover.IsGrounded);

            At(falling, rising, IsRising);
            At(falling, grounded, _playerMover.IsGrounded && !IsGroundTooSteep);
            At(falling, sliding, _playerMover.IsGrounded && IsGroundTooSteep);

            At(sliding, rising, IsRising);
            At(sliding, falling, !_playerMover.IsGrounded);
            At(sliding, grounded, _playerMover.IsGrounded && !IsGroundTooSteep);

            At(rising, grounded, _playerMover.IsGrounded && !IsGroundTooSteep);
            At(rising, sliding, _playerMover.IsGrounded && IsGroundTooSteep);
            At(rising, falling, IsFalling);
            At(rising, falling, _ceilingDetector && _ceilingDetector.HitCeiling);

            StateMachine.SetState(falling);
        }

        Vector3 CalculateMovementVelocity() => CalculateMovementDirection() * _movementSpeed;

        Vector3 CalculateMovementDirection()
        {
            var direction = _cameraTransform == null
                ? tr.right * _input.Direction.x + tr.forward * _input.Direction.y
                : Vector3.ProjectOnPlane(_cameraTransform.right, tr.up).normalized * _input.Direction.x +
                  Vector3.ProjectOnPlane(_cameraTransform.forward, tr.up).normalized * _input.Direction.y;

            return direction.magnitude > 1f ? direction.normalized : direction;
        }

        void HandleMomentum()
        {
            var localToWorldMatrix = tr.localToWorldMatrix;
            var worldToLocalMatrix = tr.worldToLocalMatrix;

            if (_useLocalMomentum)
                _momentum = localToWorldMatrix * _momentum;

            var verticalMomentum = VectorMath.ExtractDotVector(_momentum, tr.up);
            var horizontalMomentum = _momentum - verticalMomentum;

            verticalMomentum -= tr.up * (_gravity * Time.fixedDeltaTime);
            if (IsGrounded && VectorMath.GetDotProduct(verticalMomentum, tr.up) < 0f)
                verticalMomentum = Vector3.zero;

            if (!IsGrounded)
                AdjustHorizontalMomentum(ref horizontalMomentum, CalculateMovementVelocity());

            if (StateMachine.CurrentState is SlidingState)
                HandleSliding(ref horizontalMomentum);

            var friction = IsGrounded ? _groundFriction : _airFriction;
            horizontalMomentum = Vector3.MoveTowards(horizontalMomentum, Vector3.zero, friction * Time.fixedDeltaTime);

            _momentum = horizontalMomentum + verticalMomentum;

            if (StateMachine.CurrentState is SlidingState)
            {
                _momentum = Vector3.ProjectOnPlane(_momentum, _playerMover.GetGroundNormal);
                if (VectorMath.GetDotProduct(_momentum, tr.up) > 0f)
                    _momentum = VectorMath.RemoveDotVector(_momentum, tr.up);

                var slideDirection = Vector3.ProjectOnPlane(-tr.up, _playerMover.GetGroundNormal).normalized;
                _momentum += slideDirection * (_slideGravity * Time.fixedDeltaTime);
            }

            if (_useLocalMomentum)
                _momentum = worldToLocalMatrix * _momentum;
        }

        public void OnGroundContactLost()
        {
            if (_useLocalMomentum)
                _momentum = tr.localToWorldMatrix * _momentum;

            var velocity = GetMovementVelocity;
            if (velocity.sqrMagnitude >= 0f && _momentum.sqrMagnitude > 0f)
            {
                var projectedMomentum = Vector3.Project(_momentum, velocity.normalized);
                var dot = VectorMath.GetDotProduct(projectedMomentum.normalized, velocity.normalized);

                if (projectedMomentum.sqrMagnitude >= velocity.sqrMagnitude && dot > 0f)
                    velocity = Vector3.zero;
                else if (dot > 0f)
                    velocity -= projectedMomentum;
            }
            _momentum += velocity;

            if (_useLocalMomentum)
                _momentum = tr.worldToLocalMatrix * _momentum;
        }

        public void OnGroundContactRegained()
        {
            Vector3 collisionVelocity = _useLocalMomentum ? tr.localToWorldMatrix * _momentum : _momentum;
            OnLand?.Invoke(collisionVelocity);
        }

        public void OnFallStart()
        {
            var currentUpMomentum = VectorMath.ExtractDotVector(_momentum, tr.up);
            _momentum = VectorMath.RemoveDotVector(_momentum, tr.up);
            _momentum -= tr.up * currentUpMomentum.magnitude;
        }

        void AdjustHorizontalMomentum(ref Vector3 horizontalMomentum, Vector3 movementVelocity)
        {
            if (horizontalMomentum.magnitude > _movementSpeed)
            {
                if (VectorMath.GetDotProduct(movementVelocity, horizontalMomentum.normalized) > 0f)
                    movementVelocity = VectorMath.RemoveDotVector(movementVelocity, horizontalMomentum.normalized);
                horizontalMomentum += movementVelocity * (Time.fixedDeltaTime * _airControlRate * 0.25f);
            }
            else
            {
                horizontalMomentum += movementVelocity * (Time.fixedDeltaTime * _airControlRate);
                horizontalMomentum = Vector3.ClampMagnitude(horizontalMomentum, _movementSpeed);
            }
        }

        void HandleSliding(ref Vector3 horizontalMomentum)
        {
            var pointDownVector = Vector3.ProjectOnPlane(_playerMover.GetGroundNormal, tr.up).normalized;
            var movementVelocity = CalculateMovementVelocity();
            movementVelocity = VectorMath.RemoveDotVector(movementVelocity, pointDownVector);
            horizontalMomentum += movementVelocity * Time.fixedDeltaTime;
        }
    }
}