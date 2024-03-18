using System;
using Unity.Collections;
using Unity.Netcode;

public struct RankBoardEntityState : INetworkSerializable, IEquatable<RankBoardEntityState>
{
    public ulong clientId;
    public FixedString32Bytes playerName;
    public int score;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref score);
    }

    public bool Equals(RankBoardEntityState other)
    {
        return clientId == other.clientId;
    }

}
