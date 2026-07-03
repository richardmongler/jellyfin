using System;
using System.Collections.Generic;
using System.Reflection;
using Emby.Server.Implementations;
using Emby.Server.Implementations.Session;
using Jellyfin.Api.WebSocketListeners;
using Jellyfin.Database.Implementations;
using Jellyfin.Drawing;
using Jellyfin.Drawing.Skia;
using Jellyfin.LiveTv;
using Jellyfin.Server.Implementations.Activity;
using Jellyfin.Server.Implementations.Activity.Brokers;
using Jellyfin.Server.Implementations.Activity.Orchestration;
using Jellyfin.Server.Implementations.Activity.Services;
using Jellyfin.Server.Implementations.Devices;
using Jellyfin.Server.Implementations.Devices.Brokers;
using Jellyfin.Server.Implementations.Devices.Services;
using Jellyfin.Server.Implementations.Events;
using Jellyfin.Server.Implementations.Extensions;
using Jellyfin.Server.Implementations.Security;
using Jellyfin.Server.Implementations.Security.Brokers;
using Jellyfin.Server.Implementations.Security.Services;
using Jellyfin.Server.Implementations.Trickplay;
using Jellyfin.Server.Implementations.Users;
using Jellyfin.Server.Implementations.Users.Brokers;
using Jellyfin.Server.Implementations.Users.Services;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.BaseItemManager;
using MediaBrowser.Controller.Devices;
using MediaBrowser.Controller.Drawing;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Lyrics;
using MediaBrowser.Controller.Net;
using MediaBrowser.Controller.Trickplay;
using MediaBrowser.Model.Activity;
using MediaBrowser.Providers.Lyric;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Server
{
    /// <summary>
    /// Implementation of the abstract <see cref="ApplicationHost" /> class.
    /// </summary>
    public class CoreAppHost : ApplicationHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreAppHost" /> class.
        /// </summary>
        /// <param name="applicationPaths">The <see cref="ServerApplicationPaths" /> to be used by the <see cref="CoreAppHost" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" /> to be used by the <see cref="CoreAppHost" />.</param>
        /// <param name="options">The <see cref="StartupOptions" /> to be used by the <see cref="CoreAppHost" />.</param>
        /// <param name="startupConfig">The <see cref="IConfiguration" /> to be used by the <see cref="CoreAppHost" />.</param>
        public CoreAppHost(
            IServerApplicationPaths applicationPaths,
            ILoggerFactory loggerFactory,
            IStartupOptions options,
            IConfiguration startupConfig)
            : base(
                applicationPaths,
                loggerFactory,
                options,
                startupConfig)
        {
        }

        /// <inheritdoc/>
        protected override void RegisterServices(IServiceCollection serviceCollection)
        {
            // Register an image encoder
            bool useSkiaEncoder = SkiaEncoder.IsNativeLibAvailable();
            Type imageEncoderType = useSkiaEncoder
                ? typeof(SkiaEncoder)
                : typeof(NullImageEncoder);
            serviceCollection.AddSingleton(typeof(IImageEncoder), imageEncoderType);

            // Log a warning if the Skia encoder could not be used
            if (!useSkiaEncoder)
            {
                Logger.LogWarning("Skia not available. Will fallback to {ImageEncoder}.", nameof(NullImageEncoder));
            }

            serviceCollection.AddEventServices();
            serviceCollection.AddSingleton<IBaseItemManager, BaseItemManager>();
            serviceCollection.AddSingleton<IEventManager, EventManager>();

            serviceCollection.AddSingleton<IActivityManager, ActivityManager>();
            serviceCollection.AddSingleton<IUserManager, UserManager>();
            serviceCollection.AddSingleton<IAuthenticationProvider, DefaultAuthenticationProvider>();
            serviceCollection.AddSingleton<IAuthenticationProvider, InvalidAuthProvider>();
            serviceCollection.AddSingleton<IPasswordResetProvider, DefaultPasswordResetProvider>();
            serviceCollection.AddSingleton<IDisplayPreferencesManager, DisplayPreferencesManager>();
            serviceCollection.AddSingleton<IDeviceManager, DeviceManager>();
            serviceCollection.AddSingleton<ITrickplayManager, TrickplayManager>();

            // TODO search the assemblies instead of adding them manually?
            serviceCollection.AddSingleton<IWebSocketListener, SessionWebSocketListener>();
            serviceCollection.AddSingleton<IWebSocketListener, ActivityLogWebSocketListener>();
            serviceCollection.AddSingleton<IWebSocketListener, ScheduledTasksWebSocketListener>();
            serviceCollection.AddSingleton<IWebSocketListener, SessionInfoWebSocketListener>();

            serviceCollection.AddSingleton<IAuthorizationContext, AuthorizationContext>();

            // The-Standard: ApiKey Broker and Foundation Service
            serviceCollection.AddScoped<IApiKeyBroker, ApiKeyBroker>();
            serviceCollection.AddScoped<IApiKeyService, ApiKeyService>();

            // The-Standard: Device Entity Broker and Foundation Service
            serviceCollection.AddScoped<IDeviceBroker, DeviceBroker>();
            serviceCollection.AddScoped<IDeviceService, DeviceService>();

            // The-Standard: DeviceOptions Entity Broker and Foundation Service
            serviceCollection.AddScoped<IDeviceOptionsBroker, DeviceOptionsBroker>();
            serviceCollection.AddScoped<IDeviceOptionsService, DeviceOptionsService>();

            // The-Standard: ActivityLog Entity Broker
            serviceCollection.AddScoped<IActivityLogBroker, ActivityLogBroker>();

            // The-Standard: ActivityLog Foundation Service
            serviceCollection.AddScoped<IActivityLogService, ActivityLogService>();

            // The-Standard: ActivityLog Orchestration Service (Std 2.3, Two-Three Florance over ActivityLog + User Foundations)
            serviceCollection.AddScoped<IActivityLogOrchestrationService, ActivityLogOrchestrationService>();

            // The-Standard: User Entity Broker and Foundation Service
            serviceCollection.AddScoped<IUserBroker, UserBroker>();
            serviceCollection.AddScoped<IUserService, UserService>();

            foreach (var type in GetExportTypes<ILyricProvider>())
            {
                serviceCollection.AddSingleton(typeof(ILyricProvider), type);
            }

            foreach (var type in GetExportTypes<ILyricParser>())
            {
                serviceCollection.AddSingleton(typeof(ILyricParser), type);
            }

            base.RegisterServices(serviceCollection);
        }

        /// <inheritdoc />
        protected override IEnumerable<Assembly> GetAssembliesWithPartsInternal()
        {
            // Jellyfin.Server
            yield return typeof(CoreAppHost).Assembly;

            // Jellyfin.Database.Implementations
            yield return typeof(JellyfinDbContext).Assembly;

            // Jellyfin.Server.Implementations
            yield return typeof(ServiceCollectionExtensions).Assembly;

            // Jellyfin.LiveTv
            yield return typeof(LiveTvManager).Assembly;
        }
    }
}
