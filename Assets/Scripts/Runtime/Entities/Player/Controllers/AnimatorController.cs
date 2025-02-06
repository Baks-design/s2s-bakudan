using KBCore.Refs;
using UnityEngine;
using Game.Runtime.Components.Damage;
using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Systems.Interaction;
using System.Collections;
using Game.Runtime.Entities.Player.Components;

namespace Game.Runtime.Entities.Player.Controllers
{
    public class AnimatorController : MonoBehaviour
    {
        [SerializeField, Self] Animator animator;
        [SerializeField, Parent] MovementController movementController;
        [SerializeField, Parent] CollectableController collectableController;
        [SerializeField, Parent] PlayerMover playerMover;
        [SerializeField, Parent] Damageable damageable;

        void OnEnable() => SubscribeToEvents();

        void Update() => ResetPlayerVelocity();

        void OnDisable() => UnsubscribeFromEvents();

        void ResetPlayerVelocity()
        {
            var currentState = animator.GetCurrentAnimatorStateInfo(0);
            if (currentState.tagHash.Equals(AnimatorStateHashes.ThrowState))
                playerMover.LockRigidBody();
        }

        void SubscribeToEvents()
        {
            damageable.OnDeath += HandleDeath;
            collectableController.OnLiftUp += HandleLiftUp;
            collectableController.OnThrow += HandleThrow;
            movementController.OnIdle += HandleIdle;
            movementController.OnRun += HandleRun;
        }

        void UnsubscribeFromEvents()
        {
            damageable.OnDeath -= HandleDeath;
            collectableController.OnLiftUp -= HandleLiftUp;
            collectableController.OnThrow -= HandleThrow;
            movementController.OnIdle -= HandleIdle;
            movementController.OnRun -= HandleRun;
        }

        void HandleRun() => animator.CrossFade(AnimatorStateHashes.RunState, 0f);

        void HandleIdle() => animator.CrossFade(AnimatorStateHashes.IdleState, 0f);

        void HandleDeath() => animator.CrossFade(AnimatorStateHashes.DyingState, 0f);

        void HandleLiftUp() => animator.CrossFade(AnimatorStateHashes.LiftState, 0f);

        void HandleThrow()
        {
            animator.CrossFade(AnimatorStateHashes.ThrowState, 0f);
            StartCoroutine(TransitionToState(AnimatorStateHashes.IdleState));
        }

        IEnumerator TransitionToState(int nextState)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return WaitFor.Seconds(stateInfo.length);
            animator.CrossFade(nextState, 0f);
        }
    }
}