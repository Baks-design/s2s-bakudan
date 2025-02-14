using UnityEngine;

namespace Game.Runtime.Utilities.Helpers
{
    public class RaycastSensor
    {
        public enum CastDirection { Forward, Right, Up, Backward, Left, Down }

        public LayerMask LayerMask = Physics.AllLayers;
        public float CastLength = 1f;
        readonly Transform _transform;
        RaycastHit _hitInfo;

        public CastDirection Direction { get; set; } = CastDirection.Forward;
        public Vector3 Origin { get; set; } = Vector3.zero;
        public bool HasDetectedHit => _hitInfo.collider != null;
        public float Distance => _hitInfo.distance;
        public Vector3 Normal => _hitInfo.normal;
        public Vector3 Position => _hitInfo.point;
        public Collider Collider => _hitInfo.collider;
        public Transform HitTransform => _hitInfo.transform;

        public RaycastSensor(Transform transform)
        => _transform = transform != null ? transform : throw new System.ArgumentNullException(nameof(transform));

        public void SetCastDirection(CastDirection direction) => Direction = direction;

        public void SetCastOrigin(Vector3 position) => Origin = _transform.InverseTransformPoint(position);

        public void Cast()
        {
            var worldOrigin = _transform.TransformPoint(Origin);
            var worldDirection = GetCastDirection();
            Physics.Raycast(worldOrigin, worldDirection, out _hitInfo, CastLength, LayerMask, QueryTriggerInteraction.Ignore);
        }

        Vector3 GetCastDirection() => Direction switch
        {
            CastDirection.Forward => _transform.forward,
            CastDirection.Right => _transform.right,
            CastDirection.Up => _transform.up,
            CastDirection.Backward => -_transform.forward,
            CastDirection.Left => -_transform.right,
            CastDirection.Down => -_transform.up,
            _ => throw new System.ArgumentOutOfRangeException(nameof(Direction), "Invalid cast direction."),
        };

        public void DrawDebug(bool debugMode = true)
        {
            if (!debugMode || !HasDetectedHit) return;

            DrawHitNormal();
            DrawHitMarker();
        }

        void DrawHitNormal() => Debug.DrawRay(_hitInfo.point, _hitInfo.normal, Color.red, Time.deltaTime);

        void DrawHitMarker()
        {
            const float markerSize = 0.2f;
            Vector3[] directions = { Vector3.up, Vector3.right, Vector3.forward };

            foreach (var dir in directions)
                Debug.DrawLine(_hitInfo.point + dir * markerSize, _hitInfo.point - dir * markerSize, Color.green, Time.deltaTime);
        }
    }
}