using UnityEngine;

namespace Game.Runtime.Systems.Inputs
{
    public interface IInputReader
    {        
        Vector2 Direction { get; }
        
        void EnablePlayerActions();
    }
}