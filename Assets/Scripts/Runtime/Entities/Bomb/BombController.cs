using System.Collections;
using KBCore.Refs;
using UnityEngine;
using Game.Runtime.Utilities.Helpers.Timers;
using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Entities.Player.Controllers;
using Game.Runtime.Components.Damage;

namespace Game.Runtime.Entities.Bomb
{
    public class BombController : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Self] BombMovement movement;
        [SerializeField, Child] BombUI ui;
        [SerializeField, Child] BombSFX sfx;
        [SerializeField, Child] BombVFX bombVFX;
        [SerializeField] float explosionRadius = 5f;
        [SerializeField] float explosionForce = 10f;
        readonly Collider[] colliders = new Collider[50];
        readonly CountdownTimer timer = new(5f);

        void Update()
        {
            ui.UpdateTimer(timer);
            movement.FollowEntities();
        }

        void OnDestroy() => timer.Dispose();

        public void StartBomb()
        {
            timer.Start();
            StartCoroutine(ExplodeOnCountdownEnds());
        }

        IEnumerator ExplodeOnCountdownEnds()
        {
            while (!timer.IsFinished)
                yield return null;

            var colliderCount = Physics.OverlapSphereNonAlloc(tr.position, explosionRadius, colliders);
            for (var i = 0; i < colliderCount; i++)
            {
                var nearbyObject = colliders[i];

                if (nearbyObject.TryGetComponent<PlayerHealth>(out var health))
                    health.TakeDamage(5f, gameObject);

                if (nearbyObject.TryGetComponent<ForcesController>(out var mover))
                    mover.ApplyForceBy(explosionForce, explosionRadius, Vector3.up);
            }

            sfx.PlaySFX();
            bombVFX.PlayVFX();

            yield return WaitFor.Seconds(0.5f);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tr.position, explosionRadius);
        }
    }
}