using Game.Runtime.Components.Inputs;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Runtime.Components.CameraRig
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Self] CinemachineInputAxisController inputAxisController;
        [SerializeField] InputReader inputReader;

        void Update() => CameraActiveControl();

        void CameraActiveControl() => inputAxisController.enabled = inputReader.IsPlayerMapActive;
    }
}
