using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace ITKombat
{
       public struct PlayerDataMultiplayer : IEquatable<PlayerDataMultiplayer>, INetworkSerializable {


        public ulong clientId;
        public int prefabId;
        public FixedString64Bytes playerName;
        public FixedString64Bytes playerId;


        public bool Equals(PlayerDataMultiplayer other) {
            return 
                clientId == other.clientId &&
                prefabId == other.prefabId && 
                playerName == other.playerName;
                // playerId == other.playerId;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref clientId);
            serializer.SerializeValue(ref prefabId);
            serializer.SerializeValue(ref playerName);
            serializer.SerializeValue(ref playerId);
        }

    }
}
