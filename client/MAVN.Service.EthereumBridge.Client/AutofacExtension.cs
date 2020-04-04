using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.HttpClientGenerator;
using Lykke.HttpClientGenerator.Infrastructure;

namespace MAVN.Service.EthereumBridge.Client
{
    /// <summary>
    /// Extension for client registration.
    /// </summary>
    [PublicAPI]
    public static class AutofacExtension
    {
        /// <summary>
        /// Registers <see cref="EthereumBridgeClient"/> in Autofac container using <see cref="EthereumBridgeServiceClientSettings"/>.
        /// </summary>
        /// <param name="builder">The Autofac container builder.</param>
        /// <param name="settings">The client settings.</param>
        /// <param name="builderConfigure">The client builder.</param>
        public static void RegisterEthereumBridgeClient(
            [NotNull] this ContainerBuilder builder,
            [NotNull] EthereumBridgeServiceClientSettings settings,
            [CanBeNull] Func<HttpClientGeneratorBuilder, HttpClientGeneratorBuilder> builderConfigure = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(settings.ServiceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.",
                    nameof(EthereumBridgeServiceClientSettings.ServiceUrl));

            var clientBuilder = Lykke.HttpClientGenerator.HttpClientGenerator.BuildForUrl(settings.ServiceUrl)
                .WithAdditionalCallsWrapper(new ExceptionHandlerCallsWrapper());

            clientBuilder = builderConfigure?.Invoke(clientBuilder) ?? clientBuilder.WithoutRetries();

            builder.RegisterInstance(new EthereumBridgeClient(clientBuilder.Create()))
                .As<IEthereumBridgeClient>()
                .SingleInstance();
        }
    }
}
