/*
 * This class defines the SubscriptionService, which provides methods for managing subscriptions.
 */

using BlogBlast.Data;
using BlogBlast.Data.Entities;
using BlogBlast.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Services
{
    // Represents the interface for managing subscriptions
    public interface ISubscriptionService
    {
        Task<string?> SubscriptionAsync(Subscription subscription); // Subscribes a user to a service
        Task<PagedResult<Subscription>> GetSubscriptionsAsync(int startIndex, int pageSize); // Retrieves subscriptions based on pagination
    }

    // Represents the service class for managing subscriptions
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory; // Database context factory

        public SubscriptionService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory; // Initializes the context factory
        }

        // Subscribes a user to a service
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

        // Retrieves subscriptions based on pagination
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
}