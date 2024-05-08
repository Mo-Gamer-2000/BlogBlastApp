/*
 * This class defines a no-operation (NoOp) implementation of the IEmailSender interface for Identity in the BlogBlast application.
 * It is used as a placeholder implementation until a real email sender is implemented.
 */

using BlogBlast.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BlogBlast.Components.Account
{
    internal sealed class IdentityNoOpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IEmailSender emailSender = new NoOpEmailSender(); // Uses a NoOpEmailSender as the underlying email sender

        // Sends a confirmation link email
        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

        // Sends a password reset link email
        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

        // Sends a password reset code email
        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
    }
}