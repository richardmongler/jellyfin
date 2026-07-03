using System;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Data.Events;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Extensions;
using MediaBrowser.Model.Activity;
using Microsoft.EntityFrameworkCore;

namespace Jellyfin.Server.Implementations.Activity;

/// <summary>
/// Manages the storage and retrieval of <see cref="ActivityLog"/> instances.
/// </summary>
public class ActivityManager : IActivityManager
{
    private readonly IDbContextFactory<JellyfinDbContext> _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityManager"/> class.
    /// </summary>
    /// <param name="provider">The Jellyfin database provider.</param>
    public ActivityManager(IDbContextFactory<JellyfinDbContext> provider)
    {
        _provider = provider;
    }

    /// <inheritdoc/>
    public event EventHandler<GenericEventArgs<ActivityLogEntry>>? EntryCreated;

    /// <inheritdoc/>
    public async Task CreateAsync(ActivityLog entry)
    {
        var dbContext = await _provider.CreateDbContextAsync().ConfigureAwait(false);
        await using (dbContext.ConfigureAwait(false))
        {
            dbContext.ActivityLogs.Add(entry);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        EntryCreated?.Invoke(this, new GenericEventArgs<ActivityLogEntry>(ConvertToOldModel(entry)));
    }

    /// <inheritdoc />
    public async Task CleanAsync(DateTime startDate)
    {
        var dbContext = await _provider.CreateDbContextAsync().ConfigureAwait(false);
        await using (dbContext.ConfigureAwait(false))
        {
            await dbContext.ActivityLogs
                .Where(entry => entry.DateCreated <= startDate)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);
        }
    }

    private static ActivityLogEntry ConvertToOldModel(ActivityLog entry)
    {
        return new ActivityLogEntry(entry.Name, entry.Type, entry.UserId)
        {
            Id = entry.Id,
            Overview = entry.Overview,
            ShortOverview = entry.ShortOverview,
            ItemId = entry.ItemId,
            Date = entry.DateCreated,
            Severity = entry.LogSeverity
        };
    }
}
