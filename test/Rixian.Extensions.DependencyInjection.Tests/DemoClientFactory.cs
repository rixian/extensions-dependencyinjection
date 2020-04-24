// Copyright (c) Rixian. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Options;
using Rixian.Extensions.DependencyInjection;

public class DemoClientFactory : GenericFactory<DemoClientConfig, DemoClient>
{
    public DemoClientFactory(IServiceProvider services, IOptionsMonitor<DemoClientConfig> options, IOptions<GenericFactoryOptions<DemoClientConfig, DemoClient>> factoryOptions)
        : base(services, options, factoryOptions)
    {
    }
}