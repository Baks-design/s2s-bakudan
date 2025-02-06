using System.Collections;
using KBCore.Refs;
using UnityEngine;
using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Components.Damage;
using Game.Runtime.Systems.Interaction;
using System;

namespace Game.Runtime.Entities.Bomb
{
    public class BombController : MonoBehaviour, IInteractable
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Self] Rigidbody rb;

        [Header("Bomb Settings")]
        [SerializeField] LayerMask groundLayerMask;
        [SerializeField] int damage = 20;
        [SerializeField] float explosionRadius = 5f;
        [SerializeField] float explosionForce = 10f;
        bool isCollected = false;
        bool isThrown = false;
        const float fuseTime = 3f;
        readonly Collider[] collidersBuffer = new Collider[10];

        public event Action OnExploded = delegate { };

        void Awake() => SetRigigBody(true);

        void OnCollisionStay(Collision collisionInfo)
        {
            if (Helpers.IsInLayerMask(collisionInfo.collider.gameObject, groundLayerMask))
                SetRigigBody(false);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tr.position, explosionRadius);
        }

        public void Collect()
        {
            if (isCollected)
                return;

            SetRigigBody(false);
            isCollected = true;
            Debug.Log("Bomb collected!");
        }

        public void Drop()
        {
            if (!isCollected || isThrown)
                return;

            SetRigigBody(true);
            isThrown = true;
            Debug.Log($"Bomb thrown! Exploding in {fuseTime} seconds.");

            StartCoroutine(ExplodeAfterFuse());
        }

        void SetRigigBody(bool isGet)
        {
            if (isGet)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
            else
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }

        IEnumerator ExplodeAfterFuse()
        {
            yield return WaitFor.Seconds(fuseTime);

            Debug.Log("Bomb exploded!");

            OnExploded?.Invoke();

            var hitCount = Physics.OverlapSphereNonAlloc(tr.position, explosionRadius, collidersBuffer);
            for (var i = 0; i < hitCount; i++)
            {
                var collider = collidersBuffer[i];
                if (collider == null)
                    continue;

                if (collider.TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(damage, gameObject);

                if (collider.TryGetComponent(out IImpactable impactable))
                    impactable.ApplyForce(Vector3.up, explosionForce);
            }

            Destroy(gameObject);
        }
    }
}