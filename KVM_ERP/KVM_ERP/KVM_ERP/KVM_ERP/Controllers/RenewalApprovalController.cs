using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KVM_ERP.Models;
using Microsoft.AspNet.Identity;

namespace KVM_ERP.Controllers
{
    [Authorize]
    public class RenewalApprovalController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        // GET: RenewalApproval
        public ActionResult Index()
        {
            // Check if user is SuperAdmin group only
            if (!(Session != null && Session["Group"] != null && Session["Group"].ToString() == "SuperAdmin"))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Get all users with pending renewal requests (Approval = false)
                var pendingRenewals = (from s in _db.Subscriptions
                                     join u in _db.Users on s.UserId equals u.Id
                                     where s.Approval == false && s.IsActive == false
                                     select new RenewalRequestViewModel
                                     {
                                         SubscriptionId = s.SubscriptionId,
                                         UserId = s.UserId,
                                         UserName = u.UserName,
                                         Email = u.Email,
                                         PlanName = s.PlanName,
                                         RequestDate = s.CreatedAt,
                                         ExpiryDate = s.ExpiryDate,
                                         StartDate = s.StartDate
                                     }).OrderByDescending(x => x.RequestDate).ToList();

                ViewBag.TotalPendingRequests = pendingRenewals.Count;
                return View(pendingRenewals);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[RenewalApproval] Error: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while loading renewal requests.";
                return View(new List<RenewalRequestViewModel>());
            }
        }

        // POST: Approve renewal request
        [HttpPost]
        public ActionResult ApproveRenewal(int subscriptionId)
        {
            // Check if user is SuperAdmin group only
            if (!(Session != null && Session["Group"] != null && Session["Group"].ToString() == "SuperAdmin"))
            {
                return Json(new { success = false, message = "Unauthorized access." });
            }

            try
            {
                var subscription = _db.Subscriptions.FirstOrDefault(s => s.SubscriptionId == subscriptionId);
                if (subscription == null)
                {
                    return Json(new { success = false, message = "Subscription not found." });
                }

                // Check if already approved
                if (subscription.Approval == true && subscription.IsActive == true)
                {
                    return Json(new { success = false, message = "This renewal request is already approved." });
                }

                // Activate ALL existing subscriptions for this user (including expired ones)
                // This ensures both old and new subscriptions are active for login
                var existingSubscriptions = _db.Subscriptions.Where(s => s.UserId == subscription.UserId && s.SubscriptionId != subscriptionId).ToList();
                foreach (var existing in existingSubscriptions)
                {
                    existing.IsActive = true; // Set old subscription to active
                    // DON'T change existing.Approval - keep it as it was (likely 1)
                    existing.UpdatedAt = DateTime.Now;
                    System.Diagnostics.Debug.WriteLine($"[ApproveRenewal] Activated old subscription {existing.SubscriptionId} for user {subscription.UserId}");
                }

                // Approve and activate the new subscription
                subscription.Approval = true; // Set to approved
                subscription.IsActive = true; // Set to active
                subscription.UpdatedAt = DateTime.Now;
                
                // The renewal request already has the correct 1-month duration
                // Just ensure the start date is current if the subscription was expired
                if (subscription.ExpiryDate <= DateTime.Now)
                {
                    subscription.StartDate = DateTime.Now;
                    subscription.ExpiryDate = DateTime.Now.AddMonths(1); // Set to 1 month from now
                }
                // If not expired, keep the original expiry date from the renewal request
                // (Don't add extra months - the renewal request already has correct duration)

                _db.SaveChanges();

                System.Diagnostics.Debug.WriteLine($"[ApproveRenewal] Approved subscription {subscriptionId} for user {subscription.UserId}");
                System.Diagnostics.Debug.WriteLine($"[ApproveRenewal] New subscription details: Approval={subscription.Approval}, IsActive={subscription.IsActive}, ExpiryDate={subscription.ExpiryDate}");
                System.Diagnostics.Debug.WriteLine($"[ApproveRenewal] Activated {existingSubscriptions.Count} old subscriptions for user {subscription.UserId}");

                return Json(new { success = true, message = "Renewal request approved successfully! User subscription has been activated and extended." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[ApproveRenewal] Error: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while approving the renewal request." });
            }
        }

        // POST: Reject renewal request
        [HttpPost]
        public ActionResult RejectRenewal(int subscriptionId)
        {
            // Check if user is SuperAdmin group only
            if (!(Session != null && Session["Group"] != null && Session["Group"].ToString() == "SuperAdmin"))
            {
                return Json(new { success = false, message = "Unauthorized access." });
            }

            try
            {
                var subscription = _db.Subscriptions.FirstOrDefault(s => s.SubscriptionId == subscriptionId);
                if (subscription == null)
                {
                    return Json(new { success = false, message = "Subscription not found." });
                }

                // Delete the renewal request
                _db.Subscriptions.Remove(subscription);
                _db.SaveChanges();

                System.Diagnostics.Debug.WriteLine($"[RejectRenewal] Rejected and deleted subscription {subscriptionId}");

                return Json(new { success = true, message = "Renewal request rejected successfully!" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[RejectRenewal] Error: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while rejecting the renewal request." });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // ViewModel for renewal requests
    public class RenewalRequestViewModel
    {
        public int SubscriptionId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PlanName { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
