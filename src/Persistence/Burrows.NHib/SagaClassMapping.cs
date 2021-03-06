﻿// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

using Burrows.Saga;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Burrows.NHib
{
    /// <summary>
    /// A base class that can be used to define an NHibernate saga map, which
    /// automatically sets the saga to not be lazy and defined the Id property
    /// to have the correct generator of Assigned
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SagaClassMapping<T> :
        ClassMapping<T>
        where T : class, ISaga
    {
        protected SagaClassMapping()
        {
            DynamicInsert(true);
            DynamicUpdate(true);
            Lazy(false);
            Id(x => x.CorrelationId, x => x.Generator(Generators.Assigned));
        }
    }
}