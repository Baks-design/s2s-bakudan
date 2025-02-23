using Game.Runtime.Utilities.Helpers;
using KBCore.Refs;
using UnityEngine;
using System;
using Game.Runtime.Systems.Interaction;

namespace Game.Runtime.Entities.Player.Components
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerMover : MonoBehaviour, IImpactable
    {
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] Transform tr;
        [SerializeField, Self] CapsuleCollider col;
        [SerializeField, Range(0f, 1f)] float stepHeightRatio = 0.1f;
        [SerializeField] float colliderHeight = 2f;
        [SerializeField] float colliderThickness = 1f;
        [SerializeField] Vector3 colliderOffset = Vector3.zero;
        [SerializeField] bool isInDebugMode;
        bool isUsingExtendedSensorRange = true;
        int currentLayer;
        float baseSensorRange;
        Vector3 currentGroundAdjustmentVelocity;
        RaycastSensor sensor;

        public bool IsGrounded { get; private set; }
        public Vector3 GroundNormal => sensor.GetNormal;
        public Collider ColliderHit => sensor.GetCollider;

        void Awake()
        {
            rb.freezeRotation = true;
            rb.useGravity = false;
            RecalculateColliderDimensions();
        }

        void OnValidate()
        {
            if (!gameObject.activeInHierarchy)
                return;

            RecalculateColliderDimensions();
        }

        void LateUpdate()
        {
            if (!isInDebugMode)
                return;
            sensor.DrawDebug();
        }

        public void ApplyForce(Vector3 direction, float force) => SetVelocity(direction * force);

        public void CheckForGround()
        {
            if (currentLayer != gameObject.layer)
                RecalculateSensorLayerMask();

            currentGroundAdjustmentVelocity = Vector3.zero;
            sensor.castLength = isUsingExtendedSensorRange
                ? baseSensorRange + colliderHeight * tr.localScale.x * stepHeightRatio
                : baseSensorRange;
            sensor.Cast();

            IsGrounded = sensor.HasDetectedHit;
            if (!IsGrounded)
                return;

            var distance = sensor.GetDistance;
            var upperLimit = colliderHeight * tr.localScale.x * (1f - stepHeightRatio) * 0.5f;
            var middle = upperLimit + colliderHeight * tr.localScale.x * stepHeightRatio;
            var distanceToGo = middle - distance;

            currentGroundAdjustmentVelocity = tr.up * (distanceToGo / Time.fixedDeltaTime);
        }

        public void SetVelocity(Vector3 velocity) => rb.linearVelocity = velocity + currentGroundAdjustmentVelocity;

        public void SetExtendSensorRange(bool isExtended) => isUsingExtendedSensorRange = isExtended;

        void RecalculateColliderDimensions()
        {
            col.height = colliderHeight * (1f - stepHeightRatio);
            col.radius = colliderThickness / 2f;
            col.center = colliderOffset * colliderHeight + new Vector3(0f, stepHeightRatio * col.height / 2f, 0f);
            if (col.height / 2f < col.radius)
                col.radius = col.height / 2f;

            RecalibrateSensor();
        }

        void RecalibrateSensor()
        {
            sensor ??= new RaycastSensor(tr);

            sensor.SetCastOrigin(col.bounds.center);
            sensor.SetCastDirection(RaycastSensor.CastDirection.Down);
            RecalculateSensorLayerMask();

            const float safetyDistanceFactor = 0.001f;
            var length = colliderHeight * (1f - stepHeightRatio) * 0.5f + colliderHeight * stepHeightRatio;
            baseSensorRange = length * (1f + safetyDistanceFactor) * tr.localScale.x;
            sensor.castLength = length * tr.localScale.x;
        }

        void RecalculateSensorLayerMask()
        {
            var objectLayer = gameObject.layer;
            var layerMask = Physics.AllLayers;

            for (var i = 0; i < 32; i++)
                if (Physics.GetIgnoreLayerCollision(objectLayer, i))
                    layerMask &= ~(1 << i);

            var ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
            layerMask &= ~(1 << ignoreRaycastLayer);

            sensor.layermask = layerMask;
            currentLayer = objectLayer;
        }
    }
}