// Copyright (c) Rixian. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

public class DemoClient
{
    public DemoClient()
    {
    }

    public DemoClient(DemoClientConfig? config)
    {
        this.ConfigValue = config?.Value;
    }

    public string? ConfigValue { get; set; }
}
