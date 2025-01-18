using Unity.Cinemachine;
using UnityEngine;
using KBCore.Refs;

namespace Game.Runtime.Entities.Player.Controllers
{
    /// <summary> 
    /// When there is an active ThirdPersonFollow camera with noise cancellation,
    /// the position of this object is the aim target for the ThirdPersonAim camera.
    /// </summary> 
    public class AimTargetManager : MonoBehaviour
    {
        [SerializeField, Self] Transform aimTargetTransform;
        [SerializeField] Canvas reticleCanvas;
        [SerializeField] RectTransform aimTargetIndicator;
        bool haveAimTarget;

        void OnEnable() => CinemachineCore.CameraUpdatedEvent.AddListener(UpdateAimTarget);

        void OnDisable() => CinemachineCore.CameraUpdatedEvent.RemoveListener(UpdateAimTarget);

        /// <summary>
        /// Updates the aim target position and reticle visibility based on the active camera's third-person aim component.
        /// </summary>
        /// <param name="brain">The CinemachineBrain controlling the camera.</param>
        void UpdateAimTarget(CinemachineBrain brain)
        {
            haveAimTarget = false;

            if (brain == null || brain.OutputCamera == null)
            {
                CinemachineCore.CameraUpdatedEvent.RemoveListener(UpdateAimTarget);
                return;
            }

            if (TryGetActiveCinemachineCamera(brain, out var liveCam) && 
                liveCam.TryGetComponent(out CinemachineThirdPersonAim aim) && aim.enabled)
            {
                // Set the world-space aim target position
                haveAimTarget = aim.NoiseCancellation;
                aimTargetTransform.position = aim.AimTarget;

                // Update the screen-space aim target indicator position
                if (aimTargetIndicator != null)
                    aimTargetIndicator.position = brain.OutputCamera.WorldToScreenPoint(aimTargetTransform.position);
            }

            // Toggle reticle canvas visibility
            if (reticleCanvas != null)
                reticleCanvas.enabled = haveAimTarget;
        }

        /// <summary>
        /// Attempts to retrieve the currently active CinemachineCamera from the brain.
        /// </summary>
        /// <param name="brain">The CinemachineBrain controlling the camera.</param>
        /// <param name="liveCam">The currently active CinemachineCamera.</param>
        /// <returns>True if an active CinemachineCamera is found, false otherwise.</returns>
        bool TryGetActiveCinemachineCamera(CinemachineBrain brain, out CinemachineCamera liveCam) 
        => liveCam = brain.ActiveVirtualCamera is CinemachineCameraManagerBase managerCam ?
            managerCam.LiveChild as CinemachineCamera : brain.ActiveVirtualCamera as CinemachineCamera;

        /// <summary>
        /// Gets the firing direction based on the aim target.
        /// </summary>
        /// <param name="firingOrigin">The origin of the firing.</param>
        /// <param name="firingDirection">The intended firing direction.</param>
        /// <returns>The adjusted firing direction if an aim target exists, otherwise the original firing direction.</returns>
        public Vector3 GetAimDirection(Vector3 firingOrigin, Vector3 firingDirection)
        {
            if (haveAimTarget)
            {
                var direction = aimTargetTransform.position - firingOrigin;
                var length = direction.magnitude;

                if (length > Mathf.Epsilon)
                    return direction / length;
            }

            return firingDirection;
        }
    }
}
