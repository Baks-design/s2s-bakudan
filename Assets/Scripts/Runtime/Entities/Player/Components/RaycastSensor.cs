using UnityEngine;

namespace Game.Runtime.Entities.Player.Components
{
    public class RaycastSensor
    {
        public enum CastDirection { Forward, Right, Up, Backward, Left, Down }
        public float castLength = 1f;
        public LayerMask layermask = 255;
        Vector3 origin = Vector3.zero;
        readonly Transform tr;
        Vector3 castDirection;
        RaycastHit hitInfo;

        public bool HasDetectedHit => hitInfo.collider != null;
        public float GetDistance => hitInfo.distance;
        public Vector3 GetNormal => hitInfo.normal;
        public Vector3 GetPosition => hitInfo.point;
        public Collider GetCollider => hitInfo.collider;
        public Transform GetTransform => hitInfo.transform;

        public RaycastSensor(Transform playerTransform) => tr = playerTransform;

        public void DrawDebug()
        {
            if (!HasDetectedHit) return;
            Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red, Time.deltaTime);
            const float markerSize = 0.2f;
            Debug.DrawLine(hitInfo.point + Vector3.up * markerSize, hitInfo.point - Vector3.up * markerSize, Color.green, Time.deltaTime);
            Debug.DrawLine(hitInfo.point + Vector3.right * markerSize, hitInfo.point - Vector3.right * markerSize, Color.green, Time.deltaTime);
            Debug.DrawLine(hitInfo.point + Vector3.forward * markerSize, hitInfo.point - Vector3.forward * markerSize, Color.green, Time.deltaTime);
        }

        public void SetCastDirection(Vector3 direction) => castDirection = direction;

        public void SetCastOrigin(Vector3 pos) => origin = tr.InverseTransformPoint(pos);

        public void Cast()
        {
            var worldOrigin = tr.TransformPoint(origin);
            var worldDirection = castDirection;
            Physics.Raycast(worldOrigin, worldDirection, out hitInfo, castLength, layermask, QueryTriggerInteraction.Ignore);
        }
    }
}