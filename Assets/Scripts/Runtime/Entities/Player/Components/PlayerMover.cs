using Game.Runtime.Utilities.Helpers;
using KBCore.Refs;
using UnityEngine;
using System;

namespace Game.Runtime.Entities.Player.Components
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] Transform tr;
        [SerializeField, Self] CapsuleCollider col;

        [Header("Collider Settings:")]
        [SerializeField, Range(0f, 1f)] float stepHeightRatio = 0.1f;
        [SerializeField] float colliderHeight = 2f;
        [SerializeField] float colliderThickness = 1f;
        [SerializeField] Vector3 colliderOffset = Vector3.zero;

        [Header("Sensor Settings:")]
        [SerializeField] bool isInDebugMode;
        
        bool isUsingExtendedSensorRange = true;
        bool isGrounded;
        int currentLayer;
        float baseSensorRange;
        Vector3 currentGroundAdjustmentVelocity;
        RaycastSensor sensor;

        public bool IsGrounded => isGrounded;
        public Vector3 GetGroundNormal => sensor.GetNormal;
        public Collider GetColliderHit => sensor.GetCollider;

        void Awake()
        {
            Setup();
            RecalculateColliderDimensions();
        }

        void OnValidate()
        {
            if (gameObject.activeInHierarchy)
                RecalculateColliderDimensions();
        }

        void LateUpdate()
        {
            if (isInDebugMode)
                sensor.DrawDebug();
        }

        public void CheckForGround()
        {
            if (currentLayer != gameObject.layer)
                RecalculateSensorLayerMask();

            currentGroundAdjustmentVelocity = Vector3.zero;
            sensor.CastLength = isUsingExtendedSensorRange
                ? baseSensorRange + colliderHeight * tr.localScale.x * stepHeightRatio
                : baseSensorRange;
            sensor.Cast();

            isGrounded = sensor.HasDetectedHit;
            if (!isGrounded) return;

            var distance = sensor.GetDistance;
            var upperLimit = colliderHeight * tr.localScale.x * (1f - stepHeightRatio) * 0.5f;
            var middle = upperLimit + colliderHeight * tr.localScale.x * stepHeightRatio;
            var distanceToGo = middle - distance;

            currentGroundAdjustmentVelocity = tr.up * (distanceToGo / Time.fixedDeltaTime);
        }

        public void SetVelocity(Vector3 velocity) => rb.linearVelocity = velocity + currentGroundAdjustmentVelocity;

        public void SetExtendSensorRange(bool isExtended) => isUsingExtendedSensorRange = isExtended;

        public void LockRigidBody() => rb.constraints = RigidbodyConstraints.FreezePosition;

        void Setup()
        {
            rb.freezeRotation = true;
            rb.useGravity = false;
        }

        void RecalculateColliderDimensions()
        {
            if (col == null)
                Setup();

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
            sensor.CastLength = length * tr.localScale.x;
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