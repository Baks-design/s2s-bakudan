using KBCore.Refs;
using Game.Runtime.Entities.Player.Components;
using UnityEngine;

namespace Game.Runtime.Entities.Player.Controllers
{
    /// <summary>
    /// This is a behaviour whose job it is to drive animation based on the player's motion.  
    /// It is a sample implementation that you can modify or replace with your own.  As shipped, it is 
    /// hardcoded to work specifically with the sample `CameronSimpleController` Animation controller, which 
    /// is set up with states that the SimplePlayerAnimator knows about.  You can modify 
    /// this class to work with your own animation controller.
    /// 
    /// SimplePlayerAnimator works with or without a SimplePlayerControllerBase alongside.  
    /// Without one, it monitors the transform's position and drives the animation accordingly.  
    /// You can see it used like this in some of the sample scenes, such as RunningRace or ClearShot.  
    /// In this mode, is it unable to detect the player's grounded state, and so it always 
    /// assumes that the player is grounded.
    /// 
    /// When a SimplePlayerControllerBase is detected, the SimplePlayerAnimator installs callbacks 
    /// and expects to be driven by the SimplePlayerControllerBase using the STartJump, EndJump, 
    /// and PostUpdate callbacks.
    /// </summary>
    public class AnimatorController : MonoBehaviour
    {
        public enum States { Idle, Run, Die, Throw }

        [SerializeField, Parent] MovementControllerBase controller;
        [SerializeField, Self] Transform transformReference;
        [SerializeField, Self] Animator animator;
        //[SerializeField] float normalWalkSpeed = 1.7f;
        [SerializeField] float sprintSpeed = 5f;
        [SerializeField] float maxSprintScale = 1.4f;
        //[SerializeField] float jumpAnimationScale = 0.65f;
        Vector3 previousPosition;
        //const float IdleThreshold = 0.2f;
        AnimationParams animationParams;
        //static readonly int DirXHash = Animator.StringToHash("DirX");
        //static readonly int DirZHash = Animator.StringToHash("DirZ");
        //static readonly int MotionScaleHash = Animator.StringToHash("MotionScale");
        //static readonly int WalkingHash = Animator.StringToHash("Walking");
        static readonly int IdlingHash = Animator.StringToHash("IsIdling");
        static readonly int RunningHash = Animator.StringToHash("IsRunning");
        static readonly int DyingHash = Animator.StringToHash("IsDying");
        static readonly int ThrowingHash = Animator.StringToHash("IsThrowing");
        //static readonly int JumpScaleHash = Animator.StringToHash("JumpScale");
        //static readonly int JumpHash = Animator.StringToHash("Jump");
        //static readonly int LandHash = Animator.StringToHash("Land");

        public States CurrentState
        {
            get
            {
                //if (animationParams.IsJumping)
                // return animationParams.IsRunning ? States.RunJump : States.Jump;
                if (animationParams.IsRunning)
                    return States.Run;
                return animationParams.IsRunning ? States.Run : States.Idle;
            }
        }

        void Start()
        {
            previousPosition = transform.position;

            //controller.StartJump += () => animationParams.JumpTriggered = true;
            //controller.EndJump += () => animationParams.LandTriggered = true;
            controller.PostUpdate += UpdateAnimationState;
        }

        void LateUpdate()
        {
            var currentPosition = transformReference.position;
            var velocity = Quaternion.Inverse(transformReference.rotation) *
                            (currentPosition - previousPosition) / Time.deltaTime;
            previousPosition = currentPosition;

            UpdateAnimationState(velocity, 1f);
        }

        void UpdateAnimationState(Vector3 velocity, float jumpAnimationScale)
        {
            velocity.y = 0f;
            var speed = velocity.magnitude;

            var isRunning = speed > sprintSpeed * 2f + (animationParams.IsRunning ? -0.15f : 0.15f);
            //var isWalking = !isRunning && speed > IdleThreshold + (animationParams.IsWalking ? -0.05f : 0.05f);

            //animationParams.IsWalking = isWalking;
            animationParams.IsRunning = isRunning;

            // animationParams.Direction = speed > IdleThreshold ? velocity / speed : Vector3.zero;
            // animationParams.MotionScale = isWalking ? speed / normalWalkSpeed : 1f;
            // animationParams.JumpScale = jumpAnimationScale * this.jumpAnimationScale;

            // if (isRunning)
            //     animationParams.MotionScale = (speed < normalSprintSpeed)
            //         ? speed / normalSprintSpeed
            //         : Mathf.Min(maxSprintScale, 1f + (speed - normalSprintSpeed) / (3f * normalSprintSpeed));

            UpdateAnimator(animationParams);

            // animationParams.IsJumping = animationParams.JumpTriggered || 
            //     animationParams.IsJumping && !animationParams.LandTriggered;
            // animationParams.JumpTriggered = false;
            // animationParams.LandTriggered = false;
        }

        void UpdateAnimator(AnimationParams animationParams)
        {
            // animator.SetFloat(DirXHash, animationParams.Direction.x);
            // animator.SetFloat(DirZHash, animationParams.Direction.z);
            // animator.SetFloat(MotionScaleHash, animationParams.MotionScale);
            //animator.SetBool(WalkingHash, animationParams.IsWalking);
            animator.SetBool(IdlingHash, !animationParams.IsRunning);
            animator.SetBool(RunningHash, animationParams.IsRunning);
            animator.SetBool(DyingHash, animationParams.IsDying);
            animator.SetBool(ThrowingHash, animationParams.IsThrowing);
            //animator.SetFloat(JumpScaleHash, animationParams.JumpScale);
            // if (animationParams.JumpTriggered)
            //     animator.SetTrigger(JumpHash);
            // if (animationParams.LandTriggered)
            //     animator.SetTrigger(LandHash);
        }
    }
}
