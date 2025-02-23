using UnityEngine;

namespace Game.Runtime.Utilities.Helpers
{
    public static class AnimationHashes
    {
        public static readonly int DyingState = Animator.StringToHash("DIE");
        public static readonly int ThrowState = Animator.StringToHash("THROW");
        public static readonly int RunState = Animator.StringToHash("RUN");
        public static readonly int IdleState = Animator.StringToHash("IDLE");
    }
}