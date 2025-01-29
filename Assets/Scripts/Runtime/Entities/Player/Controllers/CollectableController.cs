using UnityEngine;
using KBCore.Refs;
using System.Collections.Generic;
using Game.Runtime.Systems.Interaction;

namespace Game.Runtime.Entities.Player.Controllers
{
    public class CollectableController : ActionsControllerBase
    {
        [SerializeField, Self] Transform tr;

        [Header("SphereCast Settings")]
        [SerializeField] LayerMask interactableLayer;
        [SerializeField] float sphereRadius = 2f;
        [SerializeField] float interactionRange = 5f;

        [Header("Throw Settings")]
        [SerializeField] float followSpeed = 5f;
        [SerializeField] float spacing = 1.5f;
        [SerializeField] float upwardThrowAngle = 0.5f;

        [Header("Debug Settings")]
        [SerializeField] bool drawDebugGizmos = true;

        IInteractable currentInteractable;
        Collider hitCollider;
        readonly List<Transform> collectedItems = new();
        readonly Collider[] hits = new Collider[10];

        void Update()
        {
            DetectInteractables();
            HandleInteraction();
            UpdateItemPositions();
            ThrowItems();
        }

        void OnDrawGizmos()
        {
            if (!drawDebugGizmos) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(tr.position, sphereRadius);
            Gizmos.DrawLine(tr.position, tr.position + tr.forward * interactionRange);
        }

        void UpdateItemPositions()
        {
            if (collectedItems.Count == 0) return;

            var previousPosition = tr.position;

            foreach (var item in collectedItems)
            {
                if (item == null) continue;

                var targetPosition = previousPosition - tr.forward * spacing;
                item.position = Vector3.Lerp(item.position, targetPosition, followSpeed * Time.deltaTime);
                previousPosition = item.position;
            }
        }

        void DetectInteractables()
        {
            var hitCount = Physics.OverlapSphereNonAlloc(
                tr.position, sphereRadius, hits, interactableLayer, QueryTriggerInteraction.Ignore
            );

            if (hitCount > 0)
            {
                var closestDistance = float.MaxValue;
                IInteractable closestInteractable = null;

                for (var i = 0; i < hitCount; i++)
                {
                    hitCollider = hits[i];
                    if (hitCollider.TryGetComponent<IInteractable>(out var interactable))
                    {
                        var distance = Vector3.Distance(tr.position, hitCollider.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestInteractable = interactable;
                            interactable.Collect();
                        }
                    }
                }

                if (closestInteractable != currentInteractable)
                {
                    currentInteractable = closestInteractable;
                    Debug.Log($"Interactable object detected: {currentInteractable}");
                }
            }
            else
            {
                if (currentInteractable != null)
                {
                    currentInteractable = null;
                    Debug.Log("No interactable object in range.");
                }
            }
        }

        void HandleInteraction()
        {
            if (currentInteractable != null)
            {
                currentInteractable.Collect();
                AddItemToFollow(hitCollider.transform);
            }
        }

        void AddItemToFollow(Transform item)
        {
            if (item == null || collectedItems.Contains(item)) return;

            item.gameObject.SetActive(true);
            collectedItems.Add(item);
        }

        void ThrowItems()
        {
            if (!IsThrow || collectedItems.Count == 0) return;

            OnThrow.Invoke();

            foreach (var item in collectedItems)
            {
                if (item == null) continue;

                if (item.TryGetComponent<IInteractable>(out var interactable))
                    interactable.Drop();

                if (item.TryGetComponent<IImpactable>(out var impactable))
                {
                    var throwDirection = (tr.forward + tr.up * upwardThrowAngle).normalized;
                    impactable.ApplyForce(throwDirection, ThrowForce);
                }
            }

            collectedItems.Clear();
        }
    }
}