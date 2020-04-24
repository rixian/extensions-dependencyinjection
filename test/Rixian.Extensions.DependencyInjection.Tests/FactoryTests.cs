// Copyright (c) Rixian. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Rixian.Extensions.DependencyInjection;
using Rixian.Extensions.Errors;
using Xunit;
using Xunit.Abstractions;

public class FactoryTests
{
    private readonly ITestOutputHelper logger;

    public FactoryTests(ITestOutputHelper logger)
    {
        this.logger = logger;
    }

    [Fact]
    public void Factory_NoConfig_Fail()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFactory<DemoClientConfig, DemoClient>();
        IServiceProvider services = serviceCollection.BuildServiceProvider();
        IFactory<DemoClientConfig, DemoClient> factory = services.GetRequiredService<IFactory<DemoClientConfig, DemoClient>>();

        // Act
        Result<DemoClient> item = factory.GetItem();

        // Assert
        item.IsFail.Should().BeTrue();
        item.Error.Code.Should().Be(Rixian.Extensions.DependencyInjection.ErrorCodes.NoFactoryItemGeneratorDefined);
    }

    [Fact]
    public void Factory_DefaultItem_GetDefault_Success()
    {
        // Arrange
        string testValue = Guid.NewGuid().ToString();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFactory<DemoClientConfig, DemoClient>()
            .AddDefaultItem(new DemoClient { ConfigValue = testValue });
        IServiceProvider services = serviceCollection.BuildServiceProvider();
        IFactory<DemoClientConfig, DemoClient> factory = services.GetRequiredService<IFactory<DemoClientConfig, DemoClient>>();

        // Act
        Result<DemoClient> item = factory.GetItem();

        // Assert
        item.IsSuccess.Should().BeTrue();
        item.Value.ConfigValue.Should().Be(testValue);
    }

    [Fact]
    public void Factory_DefaultItem_GetNamed_Fail()
    {
        // Arrange
        string testValue = Guid.NewGuid().ToString();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFactory<DemoClientConfig, DemoClient>()
            .AddDefaultItem(new DemoClient { ConfigValue = testValue });
        IServiceProvider services = serviceCollection.BuildServiceProvider();
        IFactory<DemoClientConfig, DemoClient> factory = services.GetRequiredService<IFactory<DemoClientConfig, DemoClient>>();

        // Act
        Result<DemoClient> item = factory.GetItem("NOT_EXIST");

        // Assert
        item.IsFail.Should().BeTrue();
        item.Error.Code.Should().Be(Rixian.Extensions.DependencyInjection.ErrorCodes.NoFactoryItemGeneratorDefined);
    }

    [Fact]
    public void Factory_NamedItem_GetNamed_Success()
    {
        // Arrange
        string testName = Guid.NewGuid().ToString();
        string testValue = Guid.NewGuid().ToString();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFactory<DemoClientConfig, DemoClient>()
            .AddItem(testName, new DemoClient { ConfigValue = testValue });
        IServiceProvider services = serviceCollection.BuildServiceProvider();
        IFactory<DemoClientConfig, DemoClient> factory = services.GetRequiredService<IFactory<DemoClientConfig, DemoClient>>();

        // Act
        Result<DemoClient> item = factory.GetItem(testName);

        // Assert
        item.IsSuccess.Should().BeTrue();
        item.Value.ConfigValue.Should().Be(testValue);
    }

    [Fact]
    public void Factory_DefaultConfigure_GetDefault_Success()
    {
        // Arrange
        string testValue = Guid.NewGuid().ToString();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFactory<DemoClientConfig, DemoClient>((svc, o) => new DemoClient(o))
            .Configure(o => o.Value = testValue);
        IServiceProvider services = serviceCollection.BuildServiceProvider();
        IFactory<DemoClientConfig, DemoClient> factory = services.GetRequiredService<IFactory<DemoClientConfig, DemoClient>>();

        // Act
        Result<DemoClient> item = factory.GetItem();

        // Assert
        item.IsSuccess.Should().BeTrue();
        item.Value.ConfigValue.Should().Be(testValue);
    }

    [Fact]
    public void Factory_NamedConfigure_GetNamed_Success()
    {
        // Arrange
        string testName = Guid.NewGuid().ToString();
        string testValue = Guid.NewGuid().ToString();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFactory<DemoClientConfig, DemoClient>((svc, o) => new DemoClient(o))
            .Configure(testName, o => o.Value = testValue);
        IServiceProvider services = serviceCollection.BuildServiceProvider();
        IFactory<DemoClientConfig, DemoClient> factory = services.GetRequiredService<IFactory<DemoClientConfig, DemoClient>>();

        // Act
        Result<DemoClient> item = factory.GetItem(testName);

        // Assert
        item.IsSuccess.Should().BeTrue();
        item.Value.ConfigValue.Should().Be(testValue);
    }

    [Fact]
    public void Factory_DerivedClass_GetNamed_Success()
    {
        // Arrange
        string testName = Guid.NewGuid().ToString();
        string testValue = Guid.NewGuid().ToString();
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddSingleton<DemoClientFactory>()
            .AddSingleton<IFactory<DemoClientConfig, DemoClient>, DemoClientFactory>(svc => svc.GetRequiredService<DemoClientFactory>())
            .ConfigureFactory<DemoClientConfig, DemoClient>((svc, o) => new DemoClient(o))
            .Configure(testName, o => o.Value = testValue);
        IServiceProvider services = serviceCollection.BuildServiceProvider();
        DemoClientFactory factory1 = services.GetRequiredService<DemoClientFactory>();
        IFactory<DemoClientConfig, DemoClient> factory2 = services.GetRequiredService<IFactory<DemoClientConfig, DemoClient>>();

        // Act
        Result<DemoClient> item1 = factory1.GetItem(testName);
        Result<DemoClient> item2 = factory2.GetItem(testName);

        // Assert
        item1.IsSuccess.Should().BeTrue();
        item1.Value.ConfigValue.Should().Be(testValue);

        item2.IsSuccess.Should().BeTrue();
        item2.Value.ConfigValue.Should().Be(testValue);
    }
}
