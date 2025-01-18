using UnityEngine;

namespace Game.Runtime.Systems.Interaction
{
    public interface IHoverable
    {
        public string Tooltip { get; set; }
        public Transform TooltipTransform { get; }

        public void OnHoverStart();
        
        public void OnHoverEnd();
    }
}