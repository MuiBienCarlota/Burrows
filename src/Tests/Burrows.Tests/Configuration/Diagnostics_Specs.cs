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

using Burrows.Configuration;

namespace Burrows.Tests.Configuration
{
    using System.Diagnostics;
    using Burrows.Diagnostics;
    using Burrows.Services.HealthMonitoring.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class Diagnostics_Specs
    {
         
        [Test][Explicit]
        public void X()
        {
            using(var bus = ServiceBusFactory.New(sbc =>
                {
                    sbc.ReceiveFrom("loopback://localhost/test");
                    
                    sbc.UseJsonSerializer();
                    sbc.UseHealthMonitoring(3);
                }))
            {
                    bus.WriteIntrospectionToFile("x.txt");
            }

            //check file
            Process.Start("notepad", "x.txt");
        }
    }
}