using Game.Runtime.Entities.Player.Components;
using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Entities.Player.Controllers
{
    public class DetectionController : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Self] MovementController movement;
        [Header("Ground")]
        [SerializeField] LayerMask groundLayerMask = 1;
        [SerializeField] float groundCastLength = 10f;
        [Header("Debug")]
        [SerializeField] bool drawDebug = false;
        RaycastSensor groundSensor;

        public bool IsGrounded => GetDistanceFromGround(tr.position, movement.UpDirection) < 0.01f;
        public Collider GroundCollider => groundSensor.GetCollider;

        void Awake() => SettingDetections();

        void LateUpdate()
        {
            if (drawDebug)
                DrawDebug();
        }

        void SettingDetections()
        {
            groundSensor = new RaycastSensor(tr)
            {
                castLength = groundCastLength,
                layermask = groundLayerMask
            };
        }

        void DrawDebug() => groundSensor.DrawDebug();

        public float GetDistanceFromGround(Vector3 pos, Vector3 up)
        {
            groundSensor.SetCastOrigin(pos + up * 0f);
            groundSensor.SetCastDirection(-up);
            groundSensor.Cast();
            if (groundSensor.HasDetectedHit)
                return groundSensor.GetDistance;
            return groundCastLength + 1f;
        }
    }
}