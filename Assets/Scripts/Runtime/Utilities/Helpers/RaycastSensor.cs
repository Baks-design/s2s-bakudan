using UnityEngine;

namespace Game.Runtime.Utilities.Helpers
{
    public class RaycastSensor
    {
        public enum CastDirection { Forward, Right, Up, Backward, Left, Down }

        public LayerMask layermask = Physics.AllLayers;
        public float CastLength = 1f;
        CastDirection castDirection = CastDirection.Forward;
        Vector3 origin = Vector3.zero;
        RaycastHit hitInfo;
        readonly Transform tr;

        public bool HasDetectedHit => hitInfo.collider != null;
        public float GetDistance => hitInfo.distance;
        public Vector3 GetNormal => hitInfo.normal;
        public Vector3 GetPosition => hitInfo.point;
        public Collider GetCollider => hitInfo.collider;
        public Transform GetTransform => hitInfo.transform;

        public RaycastSensor(Transform transform) => tr = transform;

        public void SetCastDirection(CastDirection direction) => castDirection = direction;

        public void SetCastOrigin(Vector3 position) => origin = tr.InverseTransformPoint(position);

        public void Cast()
        {
            var worldOrigin = tr.TransformPoint(origin);
            var worldDirection = GetCastDirection();
            Physics.Raycast(worldOrigin, worldDirection, out hitInfo, CastLength, layermask, QueryTriggerInteraction.Ignore);
        }

        Vector3 GetCastDirection() => castDirection switch
        {
            CastDirection.Forward => tr.forward,
            CastDirection.Right => tr.right,
            CastDirection.Up => tr.up,
            CastDirection.Backward => -tr.forward,
            CastDirection.Left => -tr.right,
            CastDirection.Down => -tr.up,
            _ => throw new System.ArgumentOutOfRangeException(nameof(castDirection), "Invalid cast direction."),
        };

        public void DrawDebug(bool debugMode = true)
        {
            if (!debugMode || !HasDetectedHit) return;

            DrawHitNormal();
            DrawHitMarker();
        }

        void DrawHitNormal() => Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red, Time.deltaTime);

        void DrawHitMarker()
        {
            const float markerSize = 0.2f;
            Vector3[] directions = { Vector3.up, Vector3.right, Vector3.forward };

            foreach (var dir in directions)
                Debug.DrawLine(hitInfo.point + dir * markerSize, hitInfo.point - dir * markerSize, Color.green, Time.deltaTime);
        }
    }
}