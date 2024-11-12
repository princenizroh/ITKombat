using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Add Gear", menuName = "GearStat/AddGear", order =1)]
    public class GearStat : ScriptableObject
    {
        public string gear_name;
        public int gear_id;
        public int gear_type_id;
        public Sprite gear_sprite;
        [TextArea(3, 10)]
        public string gear_desc;
    }
}
