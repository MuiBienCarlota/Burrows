// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Burrows.Subscriptions.Coordinator
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Logging;
    using Magnum.Caching;
    using Magnum.Reflection;
    using Util;

    public class EndpointSubscriptionConnectorCache
    {
        private static readonly ILog _log = Logger.Get(typeof (EndpointSubscriptionConnectorCache));
        private readonly IServiceBus _bus;

        private readonly Cache<Type, EndpointSubscriptionConnector> _cache;
        private readonly TypeConverter _typeConverter;

        public EndpointSubscriptionConnectorCache(IServiceBus bus)
        {
            _bus = bus;
            _typeConverter = TypeDescriptor.GetConverter(typeof (string));
            _cache = new GenericTypeCache<EndpointSubscriptionConnector>(typeof (EndpointSubscriptionConnector<>),
                CreateConnector);
        }

        public UnsubscribeAction Connect(string messageName, Uri endpointUri, string correlationId)
        {
            Type messageType = Type.GetType(messageName);
            if (messageType == null)
            {
                _log.InfoFormat("Unknown message type '{0}', unable to add subscription", messageName);
                return () => true;
            }

            EndpointSubscriptionConnector connector = _cache[messageType];

            return connector.Connect(endpointUri, correlationId);
        }

        EndpointSubscriptionConnector CreateConnector(Type messageType)
        {
            Type correlationType = messageType.GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof (ICorrelatedBy<>))
                .Select(x => x.GetGenericArguments()[0])
                .FirstOrDefault();

            if (correlationType != null)
            {
                return this.FastInvoke<EndpointSubscriptionConnectorCache, EndpointSubscriptionConnector>(
                    new[] {messageType, correlationType}, "CreateCorrelatedConnector", _bus);
            }

            return this.FastInvoke<EndpointSubscriptionConnectorCache, EndpointSubscriptionConnector>(
                new[] {messageType}, "CreateConnector", _bus);
        }

        [UsedImplicitly]
        EndpointSubscriptionConnector CreateConnector<TMessage>(IServiceBus bus)
            where TMessage : class
        {
            return new EndpointSubscriptionConnector<TMessage>(bus);
        }

        [UsedImplicitly]
        EndpointSubscriptionConnector CreateCorrelatedConnector<TMessage, TKey>(IServiceBus bus)
            where TMessage : class, ICorrelatedBy<TKey>
        {
            return new EndpointSubscriptionConnector<TMessage, TKey>(bus, GetKeyConverter<TKey>());
        }

        Func<string, TKey> GetKeyConverter<TKey>()
        {
            Type keyType = typeof (TKey);

            if (keyType == typeof (Guid))
            {
                return x => (TKey) ((object) new Guid(x));
            }

            if (_typeConverter.CanConvertTo(keyType))
            {
                return x => (TKey) _typeConverter.ConvertTo(x, keyType);
            }

            throw new InvalidOperationException(
                "The correlationId in the subscription could not be converted to the CorrelatedBy type: " +
                keyType.FullName);
        }
    }
}