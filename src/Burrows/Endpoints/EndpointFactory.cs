// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

using System;
using System.Collections.Generic;
using Burrows.Configuration.Builders;
using Burrows.Configuration.EndpointConfigurators;
using Burrows.Diagnostics.Introspection;
using Burrows.Exceptions;
using Burrows.Transports;
using Magnum.Caching;
using Magnum.Extensions;
using Burrows.Util;

namespace Burrows.Endpoints
{
    public class EndpointFactory :
        IEndpointFactory
    {
        private readonly IEndpointFactoryDefaultSettings _defaults;
        private readonly Cache<Uri, IEndpointBuilder> _endpointBuilders;
        private readonly Cache<string, ITransportFactory> _transportFactories;
        bool _disposed;

        /// <summary>
        /// Creates a new endpoint factory instance
        /// </summary>
        /// <param name="transportFactories">Dictionary + contents owned by the EndpointFactory instance.</param>
        /// <param name="endpointBuilders"></param>
        /// <param name="defaults"></param>
        public EndpointFactory([NotNull] IDictionary<string, ITransportFactory> transportFactories,
            [NotNull] IDictionary<Uri, IEndpointBuilder> endpointBuilders,
            [NotNull] IEndpointFactoryDefaultSettings defaults)
        {
            if (transportFactories == null)
                throw new ArgumentNullException("transportFactories");
            if (endpointBuilders == null)
                throw new ArgumentNullException("endpointBuilders");
            if (defaults == null)
                throw new ArgumentNullException("defaults");
            _transportFactories = new ConcurrentCache<string, ITransportFactory>(transportFactories);
            _defaults = defaults;
            _endpointBuilders = new ConcurrentCache<Uri, IEndpointBuilder>(endpointBuilders);
        }

        public IEndpoint CreateEndpoint(Uri requestedUri)
        {
            string scheme = requestedUri.Scheme.ToLowerInvariant();

            if (_transportFactories.Has(scheme))
            {
                ITransportFactory transportFactory = _transportFactories[scheme];
                try
                {
                    IEndpointAddress address = transportFactory.GetAddress(requestedUri);

                    var uriPath = new Uri(address.Uri.GetLeftPart(UriPartial.Path));
                    IEndpointBuilder builder = _endpointBuilders.Get(uriPath, key =>
                        {
                            var configurator = new EndpointConfigurator(address, _defaults);
                            return configurator.CreateBuilder();
                        });

                    return builder.CreateEndpoint(transportFactory);
                }
                catch (Exception ex)
                {
                    throw new EndpointException(requestedUri, "Failed to create endpoint", ex);
                }
            }

            throw new ConfigurationException(
                "The {0} scheme was not handled by any registered transport.".FormatWith(requestedUri.Scheme));
        }

        public void AddTransportFactory(ITransportFactory factory)
        {
            string scheme = factory.Scheme.ToLowerInvariant();

            _transportFactories[scheme] = factory;
        }

        public void Inspect(IDiagnosticsProbe probe)
        {
            probe.Add("mt.default_serializer", _defaults.Serializer.GetType().ToShortTypeName());
            _transportFactories.Each(
                (scheme, factory) =>
                    {
                        probe.Add("mt.transport",
                            string.Format("[{0}] {1}", scheme, factory.GetType().ToShortTypeName()));
                    });
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _transportFactories.Each((scheme, factory) => factory.Dispose());

            _disposed = true;
        }
    }
}