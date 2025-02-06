using UnityEngine;

namespace Game.Runtime.Systems.Interaction
{
    public class InteractableDetector
    {
        readonly float _sphereRadius;
        readonly Transform _transform;
        readonly LayerMask _interactableLayer;

        public InteractableDetector(Transform transform, float sphereRadius, LayerMask interactableLayer)
        {
            _transform = transform;
            _sphereRadius = sphereRadius;
            _interactableLayer = interactableLayer;
        }

        public IInteractable DetectClosestInteractable(out Collider hitCollider)
        {
            var hits = new Collider[10];
            var hitCount = Physics.OverlapSphereNonAlloc(
                _transform.position, _sphereRadius, hits, _interactableLayer, QueryTriggerInteraction.Ignore
            );

            hitCollider = null;
            if (hitCount == 0)
                return null;

            var closestDistance = float.MaxValue;
            IInteractable closestInteractable = null;

            for (var i = 0; i < hitCount; i++)
            {
                var collider = hits[i];
                if (collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    var distance = Vector3.Distance(_transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestInteractable = interactable;
                        hitCollider = collider;
                    }
                }
            }

            return closestInteractable;
        }
    }
}