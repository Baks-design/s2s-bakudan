using KBCore.Refs;
using UnityEngine;
using Game.Runtime.Components.Damage;
using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Entities.Player.Components;

namespace Game.Runtime.Entities.Player.Controllers
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField, Self] Animator animator;
        [SerializeField, Parent] PlayerMovementController movementController;
        [SerializeField, Parent] PlayerMover playerMover;
        [SerializeField, Parent] Damageable damageable;

        void OnEnable() => SubscribeToEvents();

        void OnDisable() => UnsubscribeFromEvents();

        void SubscribeToEvents()
        {
            damageable.OnDeath += HandleDeath;
            movementController.OnIdle += HandleIdle;
            movementController.OnMove += HandleRun;
        }

        void UnsubscribeFromEvents()
        {
            damageable.OnDeath -= HandleDeath;
            movementController.OnIdle -= HandleIdle;
            movementController.OnMove -= HandleRun;
        }

        void HandleRun() => animator.CrossFade(AnimationHashes.RunState, 0.1f);

        void HandleIdle() => animator.CrossFade(AnimationHashes.IdleState, 0.1f);

        void HandleDeath() => animator.CrossFade(AnimationHashes.DyingState, 0.1f);
    }
}