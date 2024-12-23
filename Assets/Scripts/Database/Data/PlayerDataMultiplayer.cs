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
        public int colorId;
        public FixedString64Bytes playerName;
        public FixedString64Bytes playerId;


        public bool Equals(PlayerDataMultiplayer other) {
            return 
                clientId == other.clientId ;
                // colorId == other.colorId &&
                // playerName == other.playerName &&
                // playerId == other.playerId;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref clientId);
            // serializer.SerializeValue(ref colorId);
            // serializer.SerializeValue(ref playerName);
            // serializer.SerializeValue(ref playerId);
        }

    }
}
