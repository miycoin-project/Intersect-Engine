﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using Intersect.Logging;
using Lidgren.Network;

namespace Intersect.Network
{
    public class ClientNetwork : AbstractNetwork, IClient
    {
        public HandleConnectionEvent OnConnected { get; set; }
        public HandleConnectionEvent OnConnectionApproved { get; set; }
        public HandleConnectionEvent OnDisconnected { get; set; }

        public bool IsConnected { get; private set; }
        public bool IsServerOnline { get; }

        private Guid mGuid;
        public override Guid Guid => mGuid;

        private readonly LidgrenInterface mLidgrenInterface;

        public ClientNetwork(NetworkConfiguration configuration, RSAParameters rsaParameters)
            : base(configuration)
        {
            mGuid = Guid.Empty;

            IsConnected = false;
            IsServerOnline = false;

            mLidgrenInterface = new LidgrenInterface(this, typeof(NetClient), rsaParameters);
            mLidgrenInterface.OnConnected += HandleInterfaceOnConnected;
            mLidgrenInterface.OnConnectionApproved += HandleInterfaceOnConnectonApproved;
            mLidgrenInterface.OnDisconnected += HandleInterfaceOnDisconnected;
            AddNetworkLayerInterface(mLidgrenInterface);
        }

        public bool Connect()
        {
            if (IsConnected) Disconnect("client_starting_connection");
            if (mLidgrenInterface == null) return false;
            StartInterfaces();
            return true;
        }

        protected virtual void HandleInterfaceOnConnected(IConnection connection)
        {
            Log.Info($"Connected [{connection?.Guid}].");
            IsConnected = true;
            OnConnected?.Invoke(connection);
        }

        protected virtual void HandleInterfaceOnConnectonApproved(IConnection connection)
        {
            Log.Info($"Connection approved [{connection?.Guid}].");
            OnConnectionApproved?.Invoke(connection);
        }

        protected virtual void HandleInterfaceOnDisconnected(IConnection connection)
        {
            Log.Info($"Disconnected [{connection?.Guid}].");
            IsConnected = false;
            OnDisconnected?.Invoke(connection);
        }

        internal void AssignGuid(Guid guid) => mGuid = guid;

        public override bool Send(IPacket packet)
            => mLidgrenInterface?.SendPacket(packet) ?? false;

        public override bool Send(IConnection connection, IPacket packet)
            => Send(packet);

        public override bool Send(ICollection<IConnection> connections, IPacket packet)
            => Send(packet);

        protected override IDictionary<TKey, TValue> CreateDictionaryLegacy<TKey, TValue>()
        {
            return new ConcurrentDictionary<TKey, TValue>();
        }
    }
}