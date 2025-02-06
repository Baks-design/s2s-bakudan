using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Components.UI
{
    public class MiniMapBounds : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField] Transform topRight;
        [SerializeField] Transform bottomLeft;

        void OnValidate() => DetectChildren();

        void OnEnable()
        {
#if UNITY_EDITOR
            ValidateTransform();
#endif
            DetectChildren();
        }

        void OnDrawGizmosSelected()
        {
            if (topRight == null || bottomLeft == null)
                return;

            Bounds worldBounds = GetWorldRect();
            Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
        }

        public Bounds GetWorldRect()
        {
            var size = topRight.position - bottomLeft.position;
            var center = bottomLeft.position + size / 2f;
            return new Bounds(center, size);
        }

        void DetectChildren()
        {
            if (topRight == null)
                topRight = CreateChildTransform("TopRight", new Vector3(1f, 0f, 1f), 0);
            if (bottomLeft == null)
                bottomLeft = CreateChildTransform("BottomLeft", new Vector3(-1f, 0f, -1f), 1);
        }

        Transform CreateChildTransform(string name, Vector3 localPosition, int siblingIndex)
        {
            var obj = new GameObject(name).transform;
            obj.SetParent(tr);
            obj.localPosition = localPosition;
            obj.SetSiblingIndex(siblingIndex);
            return obj;
        }

        void ValidateTransform()
        {
            if (tr.lossyScale != Vector3.one)
                Debug.LogError(
                    "[MiniMapBounds] transform.lossyScale != Vector3.one, this causes wrong positions over minimap", tr);
            if (tr.rotation != Quaternion.identity)
                Debug.LogError(
                    "[MiniMapBounds] transform.rotation != Quaternion.identity, this causes wrong positions over minimap", tr);
        }
    }
}