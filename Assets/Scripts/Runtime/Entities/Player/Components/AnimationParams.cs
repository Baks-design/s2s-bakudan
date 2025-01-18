using UnityEngine;

namespace Game.Runtime.Entities.Player.Components
{
    public struct AnimationParams
    {
        public bool IsWalking;
        public bool IsRunning;
        public bool IsJumping;
        public bool LandTriggered;
        public bool JumpTriggered;
        public Vector3 Direction;
        public float MotionScale;
        public float JumpScale;
    }
}
