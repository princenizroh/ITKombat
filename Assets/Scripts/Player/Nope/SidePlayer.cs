using UnityEngine;

namespace ITKombat
{
    public class SidePlayer : MonoBehaviour
    {
        public enum Side
        {
            Left = 0,
            Right = 1
        }

        // Add this property to expose the side of the player
        public Side playerSide;
    }
}
