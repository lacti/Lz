using System;
using System.Collections.Generic;
using System.Reflection;

namespace LzEngine.Packet
{
    public class PacketDispatcher
    {
        private readonly Dictionary<Type, KeyValuePair<object, MethodInfo>>
            _handlerMap =
                new Dictionary<Type, KeyValuePair<object, MethodInfo>>();

        public void RegisterHandler(object handlerInstance)
        {
            var handlerClassType = handlerInstance.GetType();
            foreach (
                var method in
                    handlerClassType.GetMethods(BindingFlags.Instance |
                                                BindingFlags.Public |
                                                BindingFlags.NonPublic))
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                    continue;

                var firstParam = parameters[0].ParameterType;
                if (!typeof (IPacket).IsAssignableFrom(firstParam))
                    continue;

                _handlerMap.Add(firstParam, new KeyValuePair<object, MethodInfo>(handlerInstance, method));
            }
        }

        public void Dispatch(IPacket packet, PacketSession peerSession)
        {
            var packetType = packet.GetType();
            var pair = _handlerMap[packetType];
            pair.Value.Invoke(pair.Key, new object[] { packet, peerSession });
        }
    }
}