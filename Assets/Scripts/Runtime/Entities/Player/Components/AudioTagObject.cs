using System;
using UnityEngine;

namespace Game.Runtime.Entities.Player.Components
{
    [Flags]
    public enum SurfaceTypeTag
    {
        None = 0,
        Concrete = 1 << 0,
        Gravel = 1 << 1,
        Grass = 1 << 2,
        Water = 1 << 3
    }

    public class AudioTagObject : MonoBehaviour
    {
        [SerializeField] SurfaceTypeTag surfaceTypeTag;

        public SurfaceTypeTag SurfaceTypeTag
        {
            get => surfaceTypeTag;
            private set => surfaceTypeTag = value;
        }

        public bool HasSurfaceType(SurfaceTypeTag type) => (surfaceTypeTag & type) == type;

        public void AddSurfaceType(SurfaceTypeTag type) => SurfaceTypeTag |= type;

        public void RemoveSurfaceType(SurfaceTypeTag type) => SurfaceTypeTag &= ~type;
    }
}