using Autofac;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using Lykke.Job.EthereumBridge.Domain.Repositories;
using Lykke.Job.EthereumBridge.MsSqlRepositories;
using Lykke.Job.EthereumBridge.MsSqlRepositories.Repositories;
using Lykke.Job.EthereumBridge.Settings;
using Lykke.Job.EthereumBridge.Settings.JobSettings;
using Lykke.SettingsReader;

namespace Lykke.Job.EthereumBridge.Modules
{
    [UsedImplicitly]
    public class DataLayerModule : Module
    {
        private readonly DbSettings _settings;

        public DataLayerModule(IReloadingManager<AppSettings> appSettings)
        {
            _settings = appSettings.CurrentValue.EthereumBridgeJob.Db;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMsSql(
                _settings.DataConnString,
                connString => new EthereumBridgeContext(connString, false),
                dbConn => new EthereumBridgeContext(dbConn));

            builder.RegisterType<IndexingStateRepository>()
                .As<IIndexingStateRepository>()
                .SingleInstance();

            builder.RegisterType<NoncesRepository>()
                .As<INoncesRepository>()
                .SingleInstance();

            builder.RegisterType<OperationsRepository>()
                .As<IOperationsRepository>()
                .SingleInstance();
        }
    }
}
