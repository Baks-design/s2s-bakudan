using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Systems.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class InteractableObject : MonoBehaviour, IInteractable, IPickable, IHoverable
    {
        [SerializeField, Self] Rigidbody rigid;

        public string Tooltip { get; set; }
        public bool HoldInteract { get; }
        public bool IsInteractable { get; }
        public Transform TooltipTransform { get; }
        public Rigidbody Rigid
        {
            get => rigid;
            set => rigid = value;
        }

        #region Interact
        public void OnInteract() => Debug.Log($"INTERACTED: {gameObject.name}");
        #endregion

        #region Pickup
        public void OnPickUp() { }

        public void OnHold() { }

        public void OnRelease() { }
        #endregion

        #region Hover
        public void OnHoverStart() { }

        public void OnHoverEnd() { }
        #endregion
    }
}