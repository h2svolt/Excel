using System;
using System.Collections.Generic;

namespace ExcelTranscript.Models
{
	public class AspNetUser
	{
		public string Id { get; set; }

		public string Email { get; set; }

		public bool EmailConfirmed { get; set; }

		public string PasswordHash { get; set; }

		public string SecurityStamp { get; set; }

		public string PhoneNumber { get; set; }

		public bool PhoneNumberConfirmed { get; set; }

		public bool TwoFactorEnabled { get; set; }

		public DateTime? LockoutEndDateUtc { get; set; }

		public bool LockoutEnabled { get; set; }

		public int AccessFailedCount { get; set; }

		public string UserName { get; set; }

		public int UserId { get; set; }

		public bool IsApproved { get; set; }

		public bool IsActive { get; set; }

		public string UserType { get; set; }

		public int? DistrictID { get; set; }

		public string Remarks { get; set; }

		public int? AgentID { get; set; }

		public string DeviceID { get; set; }

		public string Name { get; set; }

		public virtual ICollection<AppUserProfile> AppUserProfiles { get; set; }

		public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }

		public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }

		public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }

		public AspNetUser()
		{
			AppUserProfiles = new HashSet<AppUserProfile>();
			AspNetUserClaims = new HashSet<AspNetUserClaim>();
			AspNetUserLogins = new HashSet<AspNetUserLogin>();
			AspNetUserRoles = new HashSet<AspNetUserRole>();
		}
	}
}
