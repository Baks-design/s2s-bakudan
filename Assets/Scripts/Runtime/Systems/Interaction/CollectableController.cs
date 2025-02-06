using UnityEngine;
using System;
using Game.Runtime.Systems.Inputs;
using KBCore.Refs;

namespace Game.Runtime.Systems.Interaction
{
    public class CollectableController : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField] InputReader inputReader;
        [Header("Settings")]
        [SerializeField] LayerMask interactableLayer;
        [SerializeField] float sphereRadius = 2f;
        [SerializeField] float followSpeed = 5f;
        [SerializeField] float spacing = 1.5f;
        [SerializeField] float throwForce = 5f;
        [SerializeField] float upwardThrowAngle = 0.5f;
        [SerializeField] bool drawDebugGizmos = false;
        InteractableDetector _interactableDetector;
        ItemCollector _itemCollector;
        ItemThrower _itemThrower;
        IInteractable _currentInteractable;
        Collider _hitCollider;

        public bool IsThrow { get; set; }

        public event Action OnLiftUp = delegate { };
        public event Action OnThrow = delegate { };

        void Awake()
        {
            _interactableDetector = new InteractableDetector(tr, sphereRadius, interactableLayer);
            _itemCollector = new ItemCollector(tr, followSpeed, spacing);
            _itemThrower = new ItemThrower(tr, throwForce, upwardThrowAngle);
        }

        void OnEnable() => inputReader.Throw += HandleThrow;

        void Update()
        {
            DetectAndHandleInteractables();
            _itemCollector.UpdateItemPositions();
        }

        void OnDisable() => inputReader.Throw -= HandleThrow;

        void OnDrawGizmos()
        {
            if (!drawDebugGizmos)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(tr.position, sphereRadius);
            Gizmos.DrawLine(tr.position, tr.position + tr.forward * sphereRadius);

            Gizmos.color = Color.green;
            foreach (var item in _itemCollector.GetCollectedItems())
                if (item != null)
                    Gizmos.DrawLine(tr.position, item.position);
        }

        void DetectAndHandleInteractables()
        {
            _currentInteractable = _interactableDetector.DetectClosestInteractable(out _hitCollider);
            if (_currentInteractable != null && _hitCollider != null)
            {
                _currentInteractable.Collect();
                _itemCollector.AddItem(_hitCollider.transform);
            }
        }

        void HandleThrow(bool isThrow)
        {
            if (isThrow)
                OnLiftUp.Invoke();
            else
            {
                OnThrow.Invoke();
                _itemThrower.ThrowItems(_itemCollector.GetCollectedItems());
                _itemCollector.ClearItems();
            }
        }
    }
}