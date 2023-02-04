using UnityEngine;

namespace RootsOfTheGods.Scripts.Collectibles
{
    [CreateAssetMenu(fileName = "CollectiblesConfigurations", menuName = "Collectibles/New Collectibles Configurations", order = 0)]
    public class CollectiblesConfigurations : ScriptableObject
    {
        [TextArea]
        public string[] StartGameTexts;
        
        [field: SerializeField]
        public CollectiblesData Collectibles { get; private set; }
        
        [TextArea]
        public string EndGameText;
        [TextArea]
        public string EndGameTextBeforeDark;
        [TextArea]
        public string EndGameTextAfterDark;
    }
}