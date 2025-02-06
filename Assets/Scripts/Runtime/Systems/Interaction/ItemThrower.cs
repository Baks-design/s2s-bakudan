using UnityEngine;
using System.Collections.Generic;

namespace Game.Runtime.Systems.Interaction
{
    public class ItemThrower
    {
        readonly Transform _transform;
        readonly float _throwForce;
        readonly float _upwardThrowAngle;

        public ItemThrower(Transform transform, float throwForce, float upwardThrowAngle)
        {
            _transform = transform;
            _throwForce = throwForce;
            _upwardThrowAngle = upwardThrowAngle;
        }

        public void ThrowItems(IEnumerable<Transform> items)
        {
            foreach (var item in items)
            {
                if (item == null)
                    continue;

                if (item.TryGetComponent<IInteractable>(out var interactable))
                    interactable.Drop();

                if (item.TryGetComponent<IImpactable>(out var impactable))
                {
                    var throwDirection = (_transform.forward + _transform.up * _upwardThrowAngle).normalized;
                    impactable.ApplyForce(throwDirection, _throwForce);
                }
            }
        }
    }
}