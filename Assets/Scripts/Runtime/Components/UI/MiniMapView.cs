using System;
using System.Collections.Generic;
using System.Linq;
using Game.Runtime.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Components.UI
{
    public class MiniMapView : MonoBehaviour
    {
        [Header("RectTransform Roots")]
        [SerializeField] RectTransform centeredDotCanvas;
        [SerializeField] RectTransform otherDotCanvas;

        [Header("Default Sprite")]
        [SerializeField] Sprite defaultSprite;

        [Header("Default Dot Prefab")]
        [SerializeField] Image uiDotPrefab;

        [Header("Bounds Object")]
        [SerializeField] MiniMapBounds miniMapBounds;

        KeyValuePair<Transform, RectTransform> mainMap = new();
        readonly Dictionary<Transform, RectTransform> redDotMap = new();

        /// <summary>
        /// Follow target over the minimap, returns Generated MiniMap Image object.
        /// </summary>
        public Image FollowCentered(Transform target, Sprite icon = null)
        {
            ValidateRequiredReferences();

            if (target.lossyScale.x != 1f)
                Debug.LogWarning("[MiniMapView] target.lossyScale != 1, this causes wrong positions over minimap", target);

            if (mainMap.Key != null)
                UnfollowTarget(mainMap.Key);

            var uiDot = Instantiate(uiDotPrefab, centeredDotCanvas);
            uiDot.sprite = icon != null ? icon : defaultSprite;
            mainMap = new KeyValuePair<Transform, RectTransform>(target, uiDot.transform as RectTransform);
            return uiDot;
        }

        /// <summary>
        /// Follow target over the minimap, returns Generated MiniMap Image object.
        /// </summary>
        public Image Follow(Transform target, Sprite icon = null)
        {
            ValidateRequiredReferences();
            UnfollowTarget(target);

            var uiDot = Instantiate(uiDotPrefab, otherDotCanvas);
            uiDot.sprite = icon != null ? icon : defaultSprite;
            redDotMap.Add(target, uiDot.transform as RectTransform);
            return uiDot;
        }

        public void UnfollowTarget(Transform target)
        {
            if (mainMap.Key == target)
            {
                Destroy(mainMap.Value.gameObject);
                mainMap = new KeyValuePair<Transform, RectTransform>();
            }
            else if (redDotMap.TryGetValue(target, out var redDot))
            {
                Destroy(redDot.gameObject);
                redDotMap.Remove(target);
            }
        }

        public void ClearTargets()
        {
            if (mainMap.Key != null)
                UnfollowTarget(mainMap.Key);

            foreach (var redDot in redDotMap.ToList())
                UnfollowTarget(redDot.Key);
        }

        void Update()
        {
            UpdateMainMapTarget();
            UpdateRedDotMapTargets();
        }

        void UpdateMainMapTarget()
        {
            if (mainMap.Key == null)
                return;

            TranslateReverse(mainMap.Key, mainMap.Value);
        }

        void UpdateRedDotMapTargets()
        {
            foreach (var pair in redDotMap)
                if (pair.Key != null)
                    Translate(pair.Key, pair.Value);
        }

        public void Translate(Transform worldObj, RectTransform dot)
        {
            if (miniMapBounds == null) return;

            var worldBounds = miniMapBounds.GetWorldRect();
            var sizeDif = new Vector3(
                otherDotCanvas.sizeDelta.x / worldBounds.size.x,
                1f,
                otherDotCanvas.sizeDelta.y / worldBounds.size.z
            );

            var originWorldToLocal = Matrix4x4.TRS(worldBounds.center, Quaternion.identity, Vector3.one);
            var m = originWorldToLocal * worldObj.localToWorldMatrix;

            dot.localPosition = Vector3.Scale(sizeDif, m.GetPosition()).XZ();
            dot.localEulerAngles = new Vector3(0f, 0f, -m.GetRotation().eulerAngles.y);
        }

        public void TranslateReverse(Transform worldObj, RectTransform dot)
        {
            if (miniMapBounds == null) return;

            var worldBounds = miniMapBounds.GetWorldRect();
            var sizeDif = new Vector3(
                otherDotCanvas.sizeDelta.x / worldBounds.size.x,
                1f,
                otherDotCanvas.sizeDelta.y / worldBounds.size.z
            );

            var originLocalToWorld = Matrix4x4.TRS(-worldBounds.center, Quaternion.identity, Vector3.one);
            var m = worldObj.worldToLocalMatrix * originLocalToWorld;

            otherDotCanvas.localPosition = Vector3.Scale(sizeDif, m.GetPosition()).XZ();
            otherDotCanvas.localEulerAngles = new Vector3(0f, 0f, -m.GetRotation().eulerAngles.y);
        }

        void ValidateRequiredReferences()
        {
            if (centeredDotCanvas != null && otherDotCanvas != null && uiDotPrefab != null)
                return;

            throw new NullReferenceException(
                "[MiniMapView] Required references (centeredDotCanvas, otherDotCanvas, or uiDotPrefab) are null.");
        }

        void OnDrawGizmosSelected()
        {
            if (miniMapBounds == null)
                return;
            
            var worldBounds = miniMapBounds.GetWorldRect();
            Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
        }
    }
}