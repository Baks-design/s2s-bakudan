using UnityEngine;

namespace Game.Runtime.Utilities.Patterns.ServiceLocator
{
    [DisallowMultipleComponent, RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        bool _hasBeenBootstrapped;
        ServiceLocator _container;

        internal ServiceLocator Container => _container = _container != null ? _container : GetComponent<ServiceLocator>();

        void Awake() => BootstrapOnDemand();

        public void BootstrapOnDemand()
        {
            if (_hasBeenBootstrapped)
                return;

            _hasBeenBootstrapped = true;
            Bootstrap();
        }

        protected abstract void Bootstrap();
    }
}