using UnityEngine;

namespace Game.Runtime.Components.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Material healthBarMaterial;
        int shaderPropertyID;

        void Start()
        {
            shaderPropertyID = Shader.PropertyToID("_FillAmount");
            healthBarMaterial.SetFloat(shaderPropertyID, 100f);
        }

        void Update() => healthBarMaterial.SetFloat(shaderPropertyID, 100f);
    }
}
