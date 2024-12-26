using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace ITKombat
{
    public class SelectCharacterInfoBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI characterNameText;
        // [SerializeField] private TextMeshProUGUI characterClassText;
        // [SerializeField] private TextMeshProUGUI characterTierText;
        [SerializeField] private GameObject hideWhenNoCharacterSelected;
        [SerializeField] private TextMeshProUGUI characterBaseAtkText;
        [SerializeField] private TextMeshProUGUI characterBaseDefText;
        [SerializeField] private TextMeshProUGUI characterBaseIntText;
        // [SerializeField] private Image classBaner;
        // [SerializeField] private Image Skill1;
        // [SerializeField] private Image Skill2;
        // [SerializeField] private Image Skill3;

        public void Hide()
        {
            hideWhenNoCharacterSelected.SetActive(false);
        }

        public void UpdateCharacterInfo(CharacterStat characterStat)
        {
            hideWhenNoCharacterSelected.SetActive(true);
            characterNameText.text = characterStat.characterName;
            // characterClassText.text = characterStat.characterClass;
            // characterTierText.text = characterStat.characterTier;
            characterBaseAtkText.text = characterStat.characterBaseAtk.ToString();
            characterBaseDefText.text = characterStat.characterBaseDef.ToString();
            characterBaseIntText.text = characterStat.characterBaseInt.ToString();
        }

    }
}
