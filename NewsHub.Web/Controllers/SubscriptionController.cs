using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Site.Interfaces.Services;

namespace NewsHub.Web.Controllers;

public class SubscriptionController : Controller
{
    private readonly ISubscriptionService _subscriptionService;
    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(string email , string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
        {
            TempData["SubscribeSuccess"] = false;
            TempData["SubscribeMessage"] = "Please enter a valid email address.";
            return RedirectBack(returnUrl);
        }

        try
        {
            await _subscriptionService.SubscribeAsync(email);
            TempData["SubscribeSuccess"] = true;
            TempData["SubscribeMessage"] = "You're subscribed — thanks!";
        }
        catch (Exception)
        {
            // swap for a specific exception type if SubscribeAsync throws one
            // (e.g. "already subscribed") so you can give a more precise message
            TempData["SubscribeSuccess"] = false;
            TempData["SubscribeMessage"] = "Something went wrong. Please try again.";
        }
        return RedirectBack(returnUrl);
    }
    
    private IActionResult RedirectBack(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
}