using UnityEngine;

namespace Game.Runtime.Utilities.Patterns.ServiceLocator
{
    [AddComponentMenu("ServiceLocator/ServiceLocator Scene")]
    public class ServiceLocatorScene : Bootstrapper
    {
        protected override void Bootstrap() => Container.ConfigureForScene();
    }
}