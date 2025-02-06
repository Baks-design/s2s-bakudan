using UnityEngine;

namespace Game.Runtime.Systems.Interaction
{
    public interface IImpactable
    {
        void ApplyForce(Vector3 direction, float force);
    }
}