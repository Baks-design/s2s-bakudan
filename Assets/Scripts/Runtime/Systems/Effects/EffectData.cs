
using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Runtime.Systems.VFX
{
    [Serializable]
    public class EffectData
    {
        public Transform tr;
        public VisualEffectAsset vfxAsset;
        public float duration;
    }
}