using System;
using System.Collections.Generic;
using System.Linq;
using Game.Runtime.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Components.UI.Minimap
{
    public class MiniMapView : MonoBehaviour
    {
        [SerializeField] RectTransform centeredDotCanvas;
        [SerializeField] RectTransform otherDotCanvas;
        [SerializeField] Sprite defaultSprite;
        [SerializeField] Image uiDotPrefab;
        [SerializeField] MiniMapBounds miniMapBounds;
        readonly Dictionary<Transform, RectTransform> redDotMap = new();
        KeyValuePair<Transform, RectTransform> mainMap = new();

        void OnEnable()
        {
            if (miniMapBounds != null) return;
            miniMapBounds = FindAnyObjectByType<MiniMapBounds>();
        }

        void Update() => UpdateTranslate();

        void OnDrawGizmosSelected()
        {
            if (miniMapBounds == null) return;
            var worldBounds = miniMapBounds.GetWorldRect();
            Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
        }

        void UpdateTranslate()
        {
            if (mainMap.Key != null)
            {
                var target = mainMap.Key;
                var redDot = mainMap.Value;
                TranslateReverse(target, redDot);
            }

            foreach (var pair in redDotMap)
            {
                var target = pair.Key;
                var redDot = pair.Value;
                if (target != null)
                    Translate(target, redDot);
            }
        }

        void Translate(Transform worldObj, RectTransform dot)
        {
            var worldBounds = miniMapBounds.GetWorldRect();

            var sizeDif = CalculateSizeDifference(worldBounds);

            var originToLocalMatrix = Matrix4x4.TRS(worldBounds.center, Quaternion.identity, Vector3.one);
            var transformedMatrix = originToLocalMatrix * worldObj.localToWorldMatrix;

            ApplyDotTransform(sizeDif, transformedMatrix, dot);
        }

        void TranslateReverse(Transform worldObj, RectTransform dot)
        {
            var worldBounds = miniMapBounds.GetWorldRect();

            var sizeDif = CalculateSizeDifference(worldBounds);

            var localToOriginMatrix = Matrix4x4.TRS(-worldBounds.center, Quaternion.identity, Vector3.one);
            var transformedMatrix = worldObj.worldToLocalMatrix * localToOriginMatrix;

            ApplyDotTransform(sizeDif, transformedMatrix, dot);
        }

        Vector3 CalculateSizeDifference(Bounds worldBounds) => new(
            otherDotCanvas.sizeDelta.x / worldBounds.size.x,
            1f,
            otherDotCanvas.sizeDelta.y / worldBounds.size.z
        );

        void ApplyDotTransform(Vector3 sizeDif, Matrix4x4 transformedMatrix, RectTransform dot)
        {
            // Compute position and rotation
            var localPosition = Vector3.Scale(sizeDif, transformedMatrix.GetPosition()).XZ();
            var localRotationZ = -transformedMatrix.GetRotation().eulerAngles.y;

            // Apply to RectTransform
            dot.localPosition = localPosition;
            dot.localEulerAngles = new Vector3(0f, 0f, localRotationZ);
        }

        /// <summary>
        /// Follow target over the minimap, returns Generated MiniMap Image object
        /// </summary>
        //[Button]
        public Image FollowCentered(Transform target, Sprite icon = null)
        {
            if (centeredDotCanvas == null)
                throw new NullReferenceException("[MiniMapView] centeredDotCanvas is null");
            if (uiDotPrefab == null)
                throw new NullReferenceException("[MiniMapView] uiDotPrefab is null");
            if (target.lossyScale.x != 1)
                Debug.LogWarning("[MiniMapView] target.lossyScale != 1, this causes wrong positions over minimap", target);

            if (mainMap.Key != null)
                UnfollowTarget(mainMap.Key);

            var uiDot = Instantiate(uiDotPrefab, centeredDotCanvas);
            uiDot.sprite = icon != null ? icon : defaultSprite;
            mainMap = new KeyValuePair<Transform, RectTransform>(target, uiDot.transform as RectTransform);
            return uiDot;
        }

        /// <summary>
        /// Follow target over the minimap, returns Generated MiniMap Image object
        /// </summary>
        //[Button]
        public Image Follow(Transform target, Sprite icon = null)
        {
            if (otherDotCanvas == null)
                throw new NullReferenceException("[MiniMapView] otherDotCanvas is null");
            if (uiDotPrefab == null)
                throw new NullReferenceException("[MiniMapView] uiDotPrefab is null");

            UnfollowTarget(target);

            var uiDot = Instantiate(uiDotPrefab, otherDotCanvas);
            uiDot.sprite = icon != null ? icon : defaultSprite;
            redDotMap.Add(target, uiDot.transform as RectTransform);
            return uiDot;
        }

        //[Button]
        public void UnfollowTarget(Transform target)
        {
            if (mainMap.Key == target)
            {
                if (mainMap.Value != null)
                    Destroy(mainMap.Value.gameObject);
                mainMap = new KeyValuePair<Transform, RectTransform>();
            }
            else if (redDotMap.TryGetValue(target, out var redDot))
            {
                if (redDot != null)
                    Destroy(redDot.gameObject);
                redDotMap.Remove(target);
            }
        }

        //[Button]
        public void ClearTargets()
        {
            if (mainMap.Key != null)
                UnfollowTarget(mainMap.Key);
            foreach (var redDot in redDotMap.ToList())
                UnfollowTarget(redDot.Key);
        }
    }
}