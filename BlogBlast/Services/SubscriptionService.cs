using BlogBlast.Data;
using BlogBlast.Data.Entities;
using BlogBlast.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Services;

/// <summary>
/// Interface for managing subscriptions.
/// </summary>
public interface ISubscriptionService
{
    /// <summary>
    /// Subscribes a user to a service.
    /// </summary>
    /// <param name="subscription">The subscription details.</param>
    /// <returns>A string message indicating subscription status, or null if successful.</returns>
    Task<string?> SubscriptionAsync(Subscription subscription);

    /// <summary>
    /// Retrieves subscriptions based on the provided startIndex and pageSize.
    /// </summary>
    /// <param name="startIndex">The start index for retrieving subscriptions.</param>
    /// <param name="pageSize">The number of subscriptions to retrieve.</param>
    /// <returns>A paged result of Subscription objects.</returns>
    Task<PagedResult<Subscription>> GetSubscriptionsAsync(int startIndex, int pageSize);
}

/// <summary>
/// Service for managing subscriptions.
/// </summary>
public class SubscriptionService : ISubscriptionService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    /// <summary>
    /// Constructor for SubscriptionService.
    /// </summary>
    /// <param name="contextFactory">The context factory for creating the database context.</param>
    public SubscriptionService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Subscribes a user to a service.
    /// </summary>
    /// <param name="subscription">The subscription details.</param>
    /// <returns>A string message indicating subscription status, or null if successful.</returns>
    public async Task<string?> SubscriptionAsync(Subscription subscription)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSubscribed = await context.Subscriptions
                                        .AsNoTracking()
                                        .AnyAsync(s => s.Email == subscription.Email);

        if (isSubscribed)
        {
            return "You are already subscribed!";
        }

        subscription.SubscribedOn = DateTime.UtcNow;
        await context.Subscriptions.AddAsync(subscription);
        await context.SaveChangesAsync();

        return null;
    }

    /// <summary>
    /// Retrieves subscriptions based on the provided startIndex and pageSize.
    /// </summary>
    /// <param name="startIndex">The start index for retrieving subscriptions.</param>
    /// <param name="pageSize">The number of subscriptions to retrieve.</param>
    /// <returns>A paged result of Subscription objects.</returns>
    public async Task<PagedResult<Subscription>> GetSubscriptionsAsync(int startIndex, int pageSize)
    {
        using var context = _contextFactory.CreateDbContext();
        var query = context.Subscriptions
                            .AsNoTracking()
                            .OrderByDescending(s => s.SubscribedOn);
        var totalCount = await query.CountAsync();
        var records = await query
                            .Skip(startIndex)
                            .Take(pageSize)
                            .ToArrayAsync();

        return new PagedResult<Subscription>(records, totalCount);
    }
}