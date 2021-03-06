﻿// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
using System.Linq;
using Burrows.Configuration.Configuration;
using Burrows.Configuration.Configurators;
using Magnum.Extensions;

namespace Burrows.Configuration.EnvironmentConfigurators
{
    public interface IEnvironmentsConfigurator :
        IConfigurator
    {
        /// <summary>
        /// Add an environment to the configuration
        /// </summary>
        /// <param name="environmentName">The name of the environment</param>
        /// <param name="environmentFactory">The factory to create the environment class instance</param>
        void Add(string environmentName, Func<IServiceBusEnvironment> environmentFactory);

        /// <summary>
        /// Selects the current environment, which determines the environment class(es) that will be executed.
        /// </summary>
        /// <param name="environmentName">The name of the current environment</param>
        void Select(string environmentName);
    }

    public class EnvironmentsConfigurator :
		IEnvironmentsConfigurator
	{
		readonly IDictionary<string, Func<IServiceBusEnvironment>> _environments;
		string _currentEnvironment;

		public EnvironmentsConfigurator()
		{
			_environments = new Dictionary<string, Func<IServiceBusEnvironment>>();
		}

		public IEnumerable<IValidationResult> Validate()
		{
			if (_currentEnvironment == null)
			{
			    var msg = "A current enviroment was not specified. Known options are '{0}'";
			    var knownEnvironments = _environments.Select(kvp => kvp.Key).Aggregate((l, r) => l + ", " + r);
			    
				yield return this.Failure("Current", msg.FormatWith(knownEnvironments));
			}
		}


		public void Add(string environmentName, Func<IServiceBusEnvironment> environmentFactory)
		{
			_environments[environmentName.ToLowerInvariant()] = () => environmentFactory();
		}

		public void Select(string environmentName)
		{
			_currentEnvironment = environmentName;
		}

		public IServiceBusEnvironment GetCurrentEnvironment()
		{
			ConfigurationResult.CompileResults(Validate());

			string environment = _currentEnvironment.ToLowerInvariant();

			if (_environments.ContainsKey(environment))
			{
				return _environments[environment]();
			}

			return null;
		}
	}
}