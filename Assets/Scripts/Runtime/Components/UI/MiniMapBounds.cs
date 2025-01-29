using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Components.UI.Minimap
{
    public class MiniMapBounds : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField] Transform topRight;
        [SerializeField] Transform bottomLeft;

        void OnEnable()
        {
#if UNITY_EDITOR
            if (tr == null) return;

            if (tr.lossyScale != Vector3.one)
                Debug.LogError("[MiniMapBounds] Transform's lossy scale must be Vector3.one to avoid incorrect minimap positions.", tr);
            if (tr.rotation != Quaternion.identity)
                Debug.LogError("[MiniMapBounds] Transform's rotation must be Quaternion.identity to avoid incorrect minimap positions.", tr);
#endif
            DetectChildTransforms();
        }

        void OnValidate() => DetectChildTransforms();

        void OnDrawGizmosSelected()
        {
            if (topRight == null || bottomLeft == null) return;
            
            var worldBounds = GetWorldRect();
            Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
        }

        void DetectChildTransforms()
        {
            topRight = EnsureChildTransform(topRight, "TopRight", new Vector3(-1f, 0f, -1f), 0);
            bottomLeft = EnsureChildTransform(bottomLeft, "BottomLeft", new Vector3(1f, 0f, 1f), 1);
        }

        Transform EnsureChildTransform(Transform child, string name, Vector3 localPosition, int siblingIndex)
        {
            if (child != null) return child;

            if (tr.childCount > siblingIndex)
                return tr.GetChild(siblingIndex);

            var obj = new GameObject(name).transform;
            obj.SetParent(tr);
            obj.localPosition = localPosition;
            obj.SetSiblingIndex(siblingIndex);
            return obj;
        }

        public Bounds GetWorldRect()
        {
            var size = topRight.position - bottomLeft.position;
            var center = bottomLeft.position + size * 0.5f;
            return new Bounds(center, size);
        }
    }
}