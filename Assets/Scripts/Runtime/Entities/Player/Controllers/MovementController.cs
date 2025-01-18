using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Runtime.Entities.Player.Controllers
{
    /// <summary>
    /// Building on top of SimplePlayerControllerBase, this is the 3D character controller.  
    /// It provides the following services and settings:
    /// 
    /// - Damping (applied to the player's velocity, and to the player's rotation)
    /// - Strafe Mode
    /// - Gravity
    /// - Input Frames (which reference frame is used fo interpreting input: Camera, World, or Player)
    /// - Ground Detection (using raycasts, or delegating to Character Controller)
    /// - Camera Override (camera is used only for determining the input frame)
    /// 
    /// This behaviour should be attached to the player GameObject's root.  It moves the GameObject's 
    /// transform.  If the GameObject also has a Unity Character Controller component, the Simple Player 
    /// Controller delegates grounded state and movement to it.  If the GameObject does not have a 
    /// Character Controller, the Simple Player Controller manages its own movement and does raycasts 
    /// to test for grounded state.
    /// 
    /// Simple Player Controller does its best to interpret User input in the context of the 
    /// selected reference frame.  Generally, this works well, but in Camera mode, the user
    /// may potentially transition from being upright relative to the camera to being inverted.  
    /// When this happens, there can be a discontinuity in the interpretation of the input.  
    /// The Simple Player Controller has an ad-hoc technique of resolving this discontinuity, 
    /// (you can see this in the code), but it is only used in this very specific situation.
    /// </summary>
    public class MovementController : MovementControllerBase
    {
        public enum ForwardModes { Camera, Player, World };
        public enum UpModes { Player, World };

        [SerializeField, Self] CharacterController m_Controller;
        [SerializeField, Self] DetectionController detection;
        [SerializeField, Self] Transform tr;
        [SerializeField] ForwardModes InputForward = ForwardModes.Camera;
        [SerializeField] UpModes UpMode = UpModes.World;
        [SerializeField] Camera CameraOverride;
        [SerializeField] float Damping = 0.5f;
        [SerializeField] bool Strafe = false;
        [SerializeField] float Gravity = 10f;
        bool m_IsSprinting, m_IsJumping;
        float m_TimeLastGrounded = 0f, m_CurrentVelocityY;
        Vector3 m_CurrentVelocityXZ, m_LastInput;
        const float kDelayBeforeInferringJump = 0.3f;
        // These are part of a strategy to combat input gimbal lock when controlling a player
        // that can move freely on surfaces that go upside-down relative to the camera.
        // This is only used in the specific situation where the character is upside-down relative to the input frame,
        // and the input directives become ambiguous.
        // If the camera and input frame are travelling along with the player, then these are not used.
        bool m_InTopHemisphere = true;
        float m_TimeInHemisphere = 100f;
        Vector3 m_LastRawInput;
        Quaternion m_Upsidedown = Quaternion.AngleAxis(180f, Vector3.left);

        public override void SetStrafeMode(bool b) => Strafe = b;
        public override bool IsMoving => m_LastInput.sqrMagnitude > 0.01f;
        public bool IsSprinting => m_IsSprinting;
        public bool IsJumping => m_IsJumping;
        public Camera Camera => CameraOverride == null ? Camera.main : CameraOverride;
        public Vector3 UpDirection => UpMode == UpModes.World ? Vector3.up : tr.up;

        void OnEnable()
        {
            m_CurrentVelocityY = 0f;
            m_IsSprinting = false;
            m_IsJumping = false;
            m_TimeLastGrounded = Time.time;
        }

        void Update()
        {
            PreUpdate.Invoke();

            // Process Jump and gravity
            var justLanded = ProcessJump();

            // Get the reference frame for the input
            var rawInput = new Vector3(MoveX.Value, 0f, MoveZ.Value);
            var inputFrame = GetInputFrame(Vector3.Dot(rawInput, m_LastRawInput) < 0.8f);
            m_LastRawInput = rawInput;

            // Read the input from the user and put it in the input frame
            m_LastInput = inputFrame * rawInput;
            if (m_LastInput.sqrMagnitude > 1f)
                m_LastInput.Normalize();

            // Compute the new velocity and move the player, but only if not mid-jump
            if (!m_IsJumping)
            {
                m_IsSprinting = Sprint.Value > 0.5f;
                var desiredVelocity = m_LastInput * (m_IsSprinting ? SprintSpeed : Speed);
                var damping = justLanded ? 0f : Damping;
                if (Vector3.Angle(m_CurrentVelocityXZ, desiredVelocity) < 100f)
                    m_CurrentVelocityXZ = Vector3.Slerp(
                        m_CurrentVelocityXZ, desiredVelocity, Damper.Damp(1f, damping, Time.deltaTime));
                else
                    m_CurrentVelocityXZ += Damper.Damp(
                        desiredVelocity - m_CurrentVelocityXZ, damping, Time.deltaTime);
            }

            // Apply the position change
            ApplyMotion();

            // If not strafing, rotate the player to face movement direction
            if (!Strafe && m_CurrentVelocityXZ.sqrMagnitude > 0.001f)
            {
                var fwd = inputFrame * Vector3.forward;
                var qA = tr.rotation;
                var qB = Quaternion.LookRotation(
                    (InputForward == ForwardModes.Player && Vector3.Dot(fwd, m_CurrentVelocityXZ) < 0f)
                        ? -m_CurrentVelocityXZ : m_CurrentVelocityXZ, UpDirection);
                var damping = justLanded ? 0f : Damping;
                tr.rotation = Quaternion.Slerp(qA, qB, Damper.Damp(1f, damping, Time.deltaTime));
            }

            if (PostUpdate != null)
            {
                // Get local-space velocity
                var vel = Quaternion.Inverse(tr.rotation) * m_CurrentVelocityXZ;
                vel.y = m_CurrentVelocityY;
                PostUpdate(vel, m_IsSprinting ? JumpSpeed / SprintJumpSpeed : 1f);
            }
        }

        // Get the reference frame for the input.  The idea is to map camera fwd/right
        // to the player's XZ plane.  There is some complexity here to avoid
        // gimbal lock when the player is tilted 180 degrees relative to the input frame.
        Quaternion GetInputFrame(bool inputDirectionChanged)
        {
            // Get the raw input frame, depending of forward mode setting
            var frame = Quaternion.identity;
            switch (InputForward)
            {
                case ForwardModes.Camera: frame = Camera.transform.rotation; break;
                case ForwardModes.Player: return tr.rotation;
                case ForwardModes.World: break;
            }

            // Map the raw input frame to something that makes sense as a direction for the player
            var playerUp = tr.up;
            var up = frame * Vector3.up;

            // Is the player in the top or bottom hemisphere?  This is needed to avoid gimbal lock,
            // but only when the player is upside-down relative to the input frame.
            const float BlendTime = 2f;
            m_TimeInHemisphere += Time.deltaTime;
            var inTopHemisphere = Vector3.Dot(up, playerUp) >= 0f;
            if (inTopHemisphere != m_InTopHemisphere)
            {
                m_InTopHemisphere = inTopHemisphere;
                m_TimeInHemisphere = Mathf.Max(0f, BlendTime - m_TimeInHemisphere);
            }

            // If the player is untilted relative to the input frmae, then early-out with a simple LookRotation
            var axis = Vector3.Cross(up, playerUp);
            if (axis.sqrMagnitude < 0.001f && inTopHemisphere)
                return frame;

            // Player is tilted relative to input frame: tilt the input frame to match
            var angle = UnityVectorExtensions.SignedAngle(up, playerUp, axis);
            var frameA = Quaternion.AngleAxis(angle, axis) * frame;

            // If the player is tilted, then we need to get tricky to avoid gimbal-lock
            // when player is tilted 180 degrees.  There is no perfect solution for this,
            // we need to cheat it :/
            var frameB = frameA;
            if (!inTopHemisphere || m_TimeInHemisphere < BlendTime)
            {
                // Compute an alternative reference frame for the bottom hemisphere.
                // The two reference frames are incompatible where they meet, especially
                // when player up is pointing along the X axis of camera frame. 
                // There is no one reference frame that works for all player directions.
                frameB = frame * m_Upsidedown;
                var axisB = Vector3.Cross(frameB * Vector3.up, playerUp);
                if (axisB.sqrMagnitude > 0.001f)
                    frameB = Quaternion.AngleAxis(180f - angle, axisB) * frameB;
            }
            // Blend timer force-expires when user changes input direction
            if (inputDirectionChanged)
                m_TimeInHemisphere = BlendTime;

            // If we have been long enough in one hemisphere, then we can just use its reference frame
            if (m_TimeInHemisphere >= BlendTime)
                return inTopHemisphere ? frameA : frameB;

            // Because frameA and frameB do not join seamlessly when player Up is along X axis,
            // we blend them over a time in order to avoid degenerate spinning.
            // This will produce weird movements occasionally, but it's the lesser of the evils.
            if (inTopHemisphere)
                return Quaternion.Slerp(frameB, frameA, m_TimeInHemisphere / BlendTime);
            return Quaternion.Slerp(frameA, frameB, m_TimeInHemisphere / BlendTime);
        }

        bool ProcessJump()
        {
            var justLanded = false;
            var now = Time.time;
            var grounded = detection.IsGrounded;

            m_CurrentVelocityY -= Gravity * Time.deltaTime;

            if (!m_IsJumping)
            {
                // Process jump command
                if (grounded && Jump.Value > 0.01f)
                {
                    m_IsJumping = true;
                    m_CurrentVelocityY = m_IsSprinting ? SprintJumpSpeed : JumpSpeed;
                }
                // If we are falling, assume the jump pose
                if (!grounded && now - m_TimeLastGrounded > kDelayBeforeInferringJump)
                    m_IsJumping = true;

                if (m_IsJumping)
                {
                    StartJump.Invoke();
                    grounded = false;
                }
            }

            if (grounded)
            {
                m_TimeLastGrounded = Time.time;
                m_CurrentVelocityY = 0f;

                // If we were jumping, complete the jump
                if (m_IsJumping)
                {
                    EndJump.Invoke();
                    m_IsJumping = false;
                    justLanded = true;
                    Landed.Invoke();
                }
            }
            return justLanded;
        }

        void ApplyMotion()
        {
            if (m_Controller != null)
                m_Controller.Move((m_CurrentVelocityY * UpDirection + m_CurrentVelocityXZ) * Time.deltaTime);
            else
            {
                var pos = tr.position + m_CurrentVelocityXZ * Time.deltaTime;

                // Don't fall below ground
                var up = UpDirection;
                var altitude = detection.GetDistanceFromGround(pos, up);
                if (altitude < 0f && m_CurrentVelocityY <= 0f)
                {
                    pos -= altitude * up;
                    m_CurrentVelocityY = 0f;
                }
                else if (m_CurrentVelocityY < 0f)
                {
                    var dy = -m_CurrentVelocityY * Time.deltaTime;
                    if (dy > altitude)
                    {
                        pos -= altitude * up;
                        m_CurrentVelocityY = 0f;
                    }
                }
                tr.position = pos + m_CurrentVelocityY * Time.deltaTime * up;
            }
        }
    }
}