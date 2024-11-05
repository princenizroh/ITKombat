using System;
using UnityEngine;

namespace ITKombat
{
    public class MultiplayerManager : MonoBehaviour
    {
        public const int MAX_PLAYER_AMOUNT = 2;

        public static MultiplayerManager Instance { get; private set;}

        public static bool playMultiplayer = true;

        public event EventHandler OnTryingToJoinGame;
        public event EventHandler OnFailedToJoinGame;
        public event EventHandler OnPlayerDataNetworkListChanged;

        

    }
}
