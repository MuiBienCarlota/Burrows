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
using Burrows.Context;
using Burrows.Pipeline;

namespace Burrows.Configuration.Configuration
{
    public class ObjectConsumerFactory<TConsumer> :
		IConsumerFactory<TConsumer>
		where TConsumer : class
	{
		readonly IConsumerFactory<TConsumer> _delegate;

		public ObjectConsumerFactory(Func<Type, object> objectFactory)
		{
			_delegate = new DelegateConsumerFactory<TConsumer>(() => (TConsumer) objectFactory(typeof (TConsumer)));
		}

		public IEnumerable<Action<IConsumeContext<TMessage>>> GetConsumer<TMessage>(
			IConsumeContext<TMessage> context, InstanceHandlerSelector<TConsumer, TMessage> selector)
			where TMessage : class
		{
			return _delegate.GetConsumer(context, selector);
		}
	}
}