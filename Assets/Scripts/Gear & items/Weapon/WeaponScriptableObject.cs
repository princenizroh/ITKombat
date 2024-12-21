using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Add Weapon Data", menuName = "Weapon/AddWeaponData", order =1)]
    public class WeaponScriptableObject : ScriptableObject
    {
        public string weapon_name;
        public int weapon_damage;
        public int weapon_group_id;
        public int weapon_set_bonus_id_1;
        public int weapon_set_bonus_id_2;
        public int weapon_set_bonus_value_1;
        public int weapon_set_bonus_value_2;
        public Sprite weapon_sprite;
        [TextArea(3, 10)]
        public string weapon_desc;
    }
}
