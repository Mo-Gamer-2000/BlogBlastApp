using BlogBlast.Data;
using BlogBlast.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Services
{
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
			var IsSubscribed = await context.Subscriptions
				.AsNoTracking()
				.AnyAsync(s => s.Email == subscription.Email);

			if (IsSubscribed)
			{
				return "You are already subscribed!";
			}

			subscription.SubscribedOn = DateTime.UtcNow;
			await context.Subscriptions.AddAsync(subscription);
			await context.SaveChangesAsync();

			return null;
		}
	}
}
