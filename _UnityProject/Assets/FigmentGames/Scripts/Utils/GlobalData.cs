using UnityEngine;

namespace FigmentGames
{
    [CreateAssetMenu(fileName = "GlobalData", menuName = "Figment Games/Parameters/Global Data")]
    public class GlobalData : SingletonScriptableObject<GlobalData>
    {
        [SerializeField] private SystemLanguage userLanguage = SystemLanguage.English;

        public SystemLanguage GetUserLanguage()
        {
            return userLanguage;
        }
    }
}