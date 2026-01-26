using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVM_ERP.Models
{
    [Table("Subscription")]
    public class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubscriptionId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string PlanName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public bool Approval { get; set; }

        // Navigation property to AspNetUsers
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // Helper properties for business logic
        [NotMapped]
        public bool IsExpired => DateTime.Now > ExpiryDate;

        [NotMapped]
        public int DaysRemaining => IsExpired ? 0 : (int)(ExpiryDate - DateTime.Now).TotalDays;

        [NotMapped]
        public bool IsNearExpiry => DaysRemaining <= 14 && DaysRemaining > 0;

        [NotMapped]
        public bool CanLogin => IsActive && !IsExpired;

        [NotMapped]
        public bool IsApproved => Approval == true;

        [NotMapped]
        public bool IsNotApproved => Approval == false;

        // Approval status constants
        public const bool APPROVED = true;
        public const bool NOT_APPROVED = false;

        // Constructor
        public Subscription()
        {
            CreatedAt = DateTime.Now;
            IsActive = true;
            PlanName = "Basic";
            Approval = APPROVED; // Default to approved
        }

        // Static method to create a new basic subscription
        public static Subscription CreateBasicSubscription(string userId)
        {
            var startDate = DateTime.Now;
            return new Subscription
            {
                UserId = userId,
                PlanName = "Basic",
                StartDate = startDate,
                ExpiryDate = startDate.AddMonths(1), // 1 month from start date
                IsActive = true,
                CreatedAt = DateTime.Now,
                Approval = APPROVED // Default to approved for new subscriptions
            };
        }

        // Static method to create a renewal request subscription
        public static Subscription CreateRenewalRequest(string userId, string planName = "Basic")
        {
            var startDate = DateTime.Now;
            return new Subscription
            {
                UserId = userId,
                PlanName = planName,
                StartDate = startDate,
                ExpiryDate = startDate.AddMonths(1), // 1 month from start date
                IsActive = false, // Not active until approved
                CreatedAt = DateTime.Now,
                Approval = NOT_APPROVED // Requires approval (false)
            };
        }

        // Method to extend subscription
        public void ExtendSubscription(int months = 1)
        {
            if (IsExpired)
            {
                // If expired, start from current date
                StartDate = DateTime.Now;
                ExpiryDate = DateTime.Now.AddMonths(months);
            }
            else
            {
                // If active, extend from current expiry date
                ExpiryDate = ExpiryDate.AddMonths(months);
            }
            
            IsActive = true;
            UpdatedAt = DateTime.Now;
        }

        // Method to deactivate subscription
        public void DeactivateSubscription()
        {
            IsActive = false;
            UpdatedAt = DateTime.Now;
        }

        // Method to check and update subscription status
        public bool UpdateSubscriptionStatus()
        {
            var wasActive = IsActive;
            
            if (IsExpired && IsActive)
            {
                IsActive = false;
                UpdatedAt = DateTime.Now;
                return true; // Status changed
            }
            
            return false; // No change
        }
    }
}
