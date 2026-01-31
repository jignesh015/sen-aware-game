using UnityEngine;

namespace SenAware
{
    [CreateAssetMenu(fileName = "New Game", menuName = "Game Info", order = 0)]
    public class GameInfo : ScriptableObject
    {
        public string gameID;
        public string gameTitle;
        [TextArea(3, 10)]
        public string gameDescription;
        public Sprite gameThumbnail;
        public string gameSceneName;
    }
}