using UnityEngine;
using System.Collections.Generic;

namespace Game.Runtime.Systems.Interaction
{
    public class ItemCollector
    {
        readonly Transform _transform;
        readonly float _followSpeed;
        readonly float _spacing;
        readonly List<Transform> _collectedItems = new();

        public ItemCollector(Transform transform, float followSpeed, float spacing)
        {
            _transform = transform;
            _followSpeed = followSpeed;
            _spacing = spacing;
        }

        public void AddItem(Transform item)
        {
            if (item == null || _collectedItems.Contains(item))
                return;

            item.gameObject.SetActive(true);
            _collectedItems.Add(item);
        }

        public void UpdateItemPositions()
        {
            if (_collectedItems.Count == 0)
                return;

            var previousPosition = _transform.position;

            foreach (var item in _collectedItems)
            {
                if (item == null)
                    continue;

                var targetPosition = previousPosition - _transform.forward * _spacing;
                item.position = Vector3.Lerp(item.position, targetPosition, _followSpeed * Time.deltaTime);
                previousPosition = item.position;
            }
        }

        public void ClearItems() => _collectedItems.Clear();

        public IEnumerable<Transform> GetCollectedItems() => _collectedItems;
    }
}