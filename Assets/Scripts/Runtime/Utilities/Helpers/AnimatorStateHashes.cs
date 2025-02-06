using UnityEngine;

namespace Game.Runtime.Utilities.Helpers
{
    public static class AnimatorStateHashes
    {
        public static readonly int LandingState = Animator.StringToHash("LAND");
        public static readonly int DyingState = Animator.StringToHash("DIE");
        public static readonly int LiftState = Animator.StringToHash("LIFT");
        public static readonly int ThrowState = Animator.StringToHash("THROW");
        public static readonly int RunState = Animator.StringToHash("RUN");
        public static readonly int IdleState = Animator.StringToHash("IDLE");
    }
}