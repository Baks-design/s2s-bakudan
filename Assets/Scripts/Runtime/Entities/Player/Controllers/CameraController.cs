using Game.Runtime.Systems.Inputs;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Runtime.Entities.Player.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Child] CinemachineInputAxisController inputAxisController;
        [SerializeField] InputReader inputReader;

        void Update() => CameraActiveControl();

        void CameraActiveControl() => inputAxisController.enabled = inputReader.IsPlayerMapActive;
    }
}
