using UnityEngine;

namespace SenAware
{
    [CreateAssetMenu(fileName = "Shape", menuName = "Shape SO", order = 0)]
    public class ShapesSO : ScriptableObject
    {
        public Sprite shapeSprite;
        public string shapeName;
    }
}