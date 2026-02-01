using UnityEngine;

namespace SenAware.ShapeMatch
{
    [CreateAssetMenu(fileName = "Shape", menuName = "Games/ShapeMatch/Shape SO", order = 0)]
    public class ShapesSO : ScriptableObject
    {
        public Sprite shapeSprite;
        public string shapeName;
    }
}