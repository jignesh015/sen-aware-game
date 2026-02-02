using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SenAware.HomeScene
{
    public class GameRoundInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text roundNumberText;
        [SerializeField] private TMP_Text timeToFirstInteractionText;
        [SerializeField] private TMP_Text timeToSuccessfulInteractionText;
        [SerializeField] private TMP_Text numberOfMistakesText;
        [SerializeField] private TMP_Text difficultyLevelText;
        
        [SerializeField] private Color valueHighlightColor = Color.yellow;
        
        public void Initialize(int roundNumber, float timeToFirstInteraction, float timeToSuccessfulInteraction, int numberOfMistakes, string difficultyLevel)
        {
            roundNumberText.text = $"Round {'#' + HighlightedString((roundNumber + 1).ToString())}";
            timeToFirstInteractionText.text = $"Time to first interaction: {HighlightedString(timeToFirstInteraction.ToString("F2") + "s")}";
            timeToSuccessfulInteractionText.text = $"Time to successful interaction: {HighlightedString(timeToSuccessfulInteraction.ToString("F2") + "s")}";
            numberOfMistakesText.text = $"Number of mistakes: {HighlightedString(numberOfMistakes.ToString())}";
            difficultyLevelText.text = $"Difficulty Level: {HighlightedString(difficultyLevel)}";
        }
        
        private string HighlightedString(string value)
        {
            return $"<b><color=#{ColorUtility.ToHtmlStringRGB(valueHighlightColor)}>{value}</color></b>";
        }
    }
}