using UnityEngine;

namespace Game.Runtime.Systems.Interaction
{
    public interface IPickable
    {
        public Rigidbody Rigid { get; set; }

        public void OnPickUp();
        public void OnHold();
        public void OnRelease();
    }
}