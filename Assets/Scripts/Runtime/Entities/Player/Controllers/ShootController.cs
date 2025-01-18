using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Runtime.Entities.Player.Controllers
{
    /// <summary>
    /// This component manages player shooting.  It is expected to be on the player object, 
    /// or on a child SimplePlayerAimController object of the player.
    /// 
    /// If an AimTargetManager is specified, then the behaviour aims at that target.
    /// Otherwise, the behaviour aims in the forward direction of the player object,
    /// or of the SimplePlayerAimController object if it exists and is not decoupled
    /// from the player rotation.
    /// </summary>
    public class ShootController : MonoBehaviour, IInputAxisOwner
    {
        [SerializeField] Transform tr;
        [SerializeField] AimTargetManager aimTargetManager;
        [SerializeField] AimController aimController;
        [SerializeField] GameObject bulletPrefab;
        [SerializeField] InputAxis fire = InputAxis.DefaultMomentary;
        [SerializeField] float maxBulletsPerSecond = 10f;
        float lastFireTime;
        readonly List<GameObject> bulletPool = new();

        public Action FireEvent = delegate { };

        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new IInputAxisOwner.AxisDescriptor
            {
                DrivenAxis = () => ref fire,
                Name = "Shoot"
            });
        }

        void OnValidate() => maxBulletsPerSecond = Mathf.Max(1f, maxBulletsPerSecond);

        void Update()
        {
            if (!CanFire())
                return;

            lastFireTime = Time.time;

            var firingDirection = GetFiringDirection();
            HandleRecenter();

            var spawnPosition = tr.position + firingDirection;
            var spawnRotation = Quaternion.LookRotation(firingDirection, tr.up);

            var bullet = GetOrCreateBullet(spawnPosition, spawnRotation);

            bullet.SetActive(true);
            FireEvent.Invoke();
        }

        bool CanFire() => bulletPrefab != null && Time.time - lastFireTime > 1f / maxBulletsPerSecond && fire.Value > 0.1f;

        Vector3 GetFiringDirection()
        {
            var direction = tr.forward;

            if (aimController != null && aimController.PlayerRotation == AimController.CouplingMode.Decoupled)
                direction = tr.parent.forward;

            if (aimTargetManager != null)
                direction = aimTargetManager.GetAimDirection(tr.position, direction).normalized;

            return direction;
        }

        void HandleRecenter()
        {
            if (aimController == null || aimController.PlayerRotation == AimController.CouplingMode.Decoupled)
                return;

            var rotationDamping = aimController.RotationDamping;
            if (Time.time - lastFireTime <= rotationDamping || CanFire())
                aimController.RecenterPlayer(rotationDamping);
        }

        GameObject GetOrCreateBullet(Vector3 position, Quaternion rotation)
        {
            foreach (var pooledBullet in bulletPool)
            {
                if (!pooledBullet.activeInHierarchy)
                {
                    bulletPool.Remove(pooledBullet);
                    pooledBullet.transform.SetPositionAndRotation(position, rotation);
                    return pooledBullet;
                }
            }

            var newBullet = Instantiate(bulletPrefab, position, rotation);
            bulletPool.Add(newBullet);
            return newBullet;
        }
    }
}