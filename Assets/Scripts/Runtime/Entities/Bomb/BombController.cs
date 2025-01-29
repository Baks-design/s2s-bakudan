using System.Collections;
using KBCore.Refs;
using UnityEngine;
using Game.Runtime.Utilities.Helpers.Timers;
using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Components.Damage;
using Game.Runtime.Systems.Interaction;

namespace Game.Runtime.Entities.Bomb
{
    public class BombController : MonoBehaviour, IInteractable, IImpactable
    {
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] Transform tr;
        [SerializeField, Child] BombUI ui;
        [SerializeField, Child] BombSFX sfx;
        [SerializeField, Child] BombVFX bombVFX;
        [SerializeField] LayerMask groundLayerMask;
        [SerializeField] LayerMask aflictedLayerMask;
        [SerializeField] float explosionRadius = 5f;
        [SerializeField] float explosionForce = 10f;
        [SerializeField] float upwardThrowAngle = 10f;
        readonly Collider[] colliders = new Collider[50];
        readonly CountdownTimer timer = new(5f);
        bool isCollected;

        void Awake() => SetRigigBody(false);

        void OnEnable() => timer.OnTimerStop += HandleExplosionRoutine;

        void Update()
        {
            if (!timer.IsRunning) return;
            ui.UpdateTimer(timer);
            CollectedState();
        }
        void OnCollisionStay(Collision collisionInfo)
        {
            if (IsInLayerMask(collisionInfo.collider.gameObject, groundLayerMask))
                SetRigigBody(true);

            static bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) != 0;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tr.position, explosionRadius);
        }

        void OnDisable() => timer.OnTimerStop -= HandleExplosionRoutine;

        void OnDestroy() => timer.Dispose();

        public void Collect()
        {
            StartBomb();
            SetRigigBody(true);
            isCollected = true;
        }

        public void Drop()
        {
            SetRigigBody(false);
            isCollected = false;
        }

        public void ApplyForce(Vector3 throwDirection, float throwForce)
            => rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);

        void StartBomb() => timer.Start();

        void CollectedState()
        {
            if (!isCollected) return;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        void SetRigigBody(bool isGet)
        {
            if (isGet)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            else
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }

        void HandleExplosionRoutine() => StartCoroutine(ExplodeOnCountdownEnds());

        IEnumerator ExplodeOnCountdownEnds()
        {
            while (!timer.IsFinished)
                yield return null;

            var colliderCount = Physics.OverlapSphereNonAlloc(
                tr.position, explosionRadius, colliders, aflictedLayerMask, QueryTriggerInteraction.Ignore);
            for (var i = 0; i < colliderCount; i++)
            {
                var nearbyObject = colliders[i];

                if (nearbyObject.TryGetComponent(out IDamageable health))
                    health.TakeDamage(5f, gameObject);

                if (nearbyObject.TryGetComponent(out IImpactable impactable))
                {
                    var throwDirection = (tr.forward + tr.up * upwardThrowAngle).normalized;
                    impactable.ApplyForce(throwDirection, explosionForce);
                }
            }

            sfx.PlaySFX();
            bombVFX.PlayVFX();

            yield return WaitFor.Seconds(0.5f);
        }
    }
}