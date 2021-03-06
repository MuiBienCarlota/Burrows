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
namespace Burrows.Testing
{
    using Factories;
    using Saga;
    using ScenarioBuilders;
    using Scenarios;

    public static class ScenarioExtensions
	{
		public static HandlerTestFactory<IBusTestScenario, TMessage> InSingleBusScenario<TScenario, TMessage>(
			this IHandlerTestFactory<TScenario, TMessage> factory)
			where TScenario : ITestScenario
			where TMessage : class
		{
			return new HandlerTestFactory<IBusTestScenario, TMessage>(LoopbackBus);
		}

		public static HandlerTestFactory<ILocalRemoteTestScenario, TMessage> InLocalRemoteBusScenario<TScenario, TMessage>(
			this IHandlerTestFactory<TScenario, TMessage> factory)
			where TScenario : ITestScenario
			where TMessage : class
		{
			return new HandlerTestFactory<ILocalRemoteTestScenario, TMessage>(LoopbackLocalRemote);
		}

		public static ConsumerTestFactory<IBusTestScenario, TConsumer> InSingleBusScenario<TScenario, TConsumer>(
			this IConsumerTestFactory<TScenario, TConsumer> factory)
			where TScenario : ITestScenario
			where TConsumer : class, IConsumer
		{
			return new ConsumerTestFactory<IBusTestScenario, TConsumer>(LoopbackBus);
		}

		public static ConsumerTestFactory<ILocalRemoteTestScenario, TConsumer> InLocalRemoteBusScenario<TScenario, TConsumer>
			(
			this IConsumerTestFactory<TScenario, TConsumer> factory)
			where TScenario : ITestScenario
			where TConsumer : class, IConsumer
		{
			return new ConsumerTestFactory<ILocalRemoteTestScenario, TConsumer>(LoopbackLocalRemote);
		}

		public static SagaTestFactory<IBusTestScenario, TSaga> InSingleBusScenario<TScenario, TSaga>(
			this ISagaTestFactory<TScenario, TSaga> factory)
			where TScenario : ITestScenario
			where TSaga : class, ISaga
		{
			return new SagaTestFactory<IBusTestScenario, TSaga>(LoopbackBus);
		}

		public static SagaTestFactory<ILocalRemoteTestScenario, TSaga> InLocalRemoteBusScenario<TScenario, TSaga>
			(
			this ISagaTestFactory<TScenario, TSaga> factory)
			where TScenario : ITestScenario
			where TSaga : class, ISaga
		{
			return new SagaTestFactory<ILocalRemoteTestScenario, TSaga>(LoopbackLocalRemote);
		}

		static LoopbackBusScenarioBuilder LoopbackBus()
		{
			return new LoopbackBusScenarioBuilder();
		}

		static LoopbackLocalRemoteBusScenarioBuilder LoopbackLocalRemote()
		{
			return new LoopbackLocalRemoteBusScenarioBuilder();
		}
	}
}