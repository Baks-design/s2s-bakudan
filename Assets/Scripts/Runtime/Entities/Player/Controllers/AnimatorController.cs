using KBCore.Refs;
using UnityEngine;
using Game.Runtime.Components.Damage;
using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Entities.Player.Components;

namespace Game.Runtime.Entities.Player.Controllers
{
    public class AnimatorController : MonoBehaviour
    {
        [SerializeField, Self] Animator animator;
        [SerializeField, Parent] MovementController movementController;
        [SerializeField, Parent] PlayerMover playerMover;
        [SerializeField, Parent] Damageable damageable;
        [SerializeField] Rect stateDebugText = new(10f, 10f, 200f, 20f);

        void OnEnable() => SubscribeToEvents();

        void Update() => ResetPlayerVelocity();

        void OnDisable() => UnsubscribeFromEvents();

        void OnGUI() => GUI.Label(stateDebugText, $"Current Animator State: {animator.GetCurrentAnimatorStateInfo(0)}");

        void ResetPlayerVelocity()
        {
            var currentState = animator.GetCurrentAnimatorStateInfo(0);
            if (currentState.tagHash.Equals(AnimatorStateHashes.ThrowState))
                playerMover.LockRigidBody();
        }

        void SubscribeToEvents()
        {
            damageable.OnDeath += HandleDeath;
            movementController.OnIdle += HandleIdle;
            movementController.OnRun += HandleRun;
        }

        void UnsubscribeFromEvents()
        {
            damageable.OnDeath -= HandleDeath;
            movementController.OnIdle -= HandleIdle;
            movementController.OnRun -= HandleRun;
        }

        void HandleRun() => animator.CrossFade(AnimatorStateHashes.RunState, 0f);

        void HandleIdle() => animator.CrossFade(AnimatorStateHashes.IdleState, 0f);

        void HandleDeath() => animator.CrossFade(AnimatorStateHashes.DyingState, 0f);

        // IEnumerator TransitionToState(int nextState)
        // {
        //     var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //     yield return WaitFor.Seconds(stateInfo.length);
        //     animator.CrossFade(nextState, 0f);
        // }
    }
}