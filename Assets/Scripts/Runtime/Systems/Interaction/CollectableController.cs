using UnityEngine;
using KBCore.Refs;

namespace Game.Runtime.Systems.Interaction
{
    public class CollectableController : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField] LayerMask interactableLayer;
        [SerializeField] float sphereRadius = 2f;
        InteractableDetector _interactableDetector;
        IInteractable _currentInteractable;
        Collider _hitCollider;

        void Awake() => Setup();

        void Update() => DetectAndHandleInteractables();

        void Setup() => _interactableDetector = new InteractableDetector(tr, sphereRadius, interactableLayer);

        void DetectAndHandleInteractables()
        {
            _currentInteractable = _interactableDetector.DetectClosestInteractable(out _hitCollider);
            if (_currentInteractable != null && _hitCollider != null)
                _currentInteractable.Collect();
        }
    }
}