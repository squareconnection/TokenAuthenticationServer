using System;
using System.ComponentModel.DataAnnotations;

namespace TokenAuth.Models
{
    public class EntityBase
    {
		protected EntityBase()
		{
			this.Id = Guid.NewGuid().ToString();
		}

		protected EntityBase(string Id)
		{
			this.Id = Id;
		}

        [Key]
		public string Id { get; set; }

		[Timestamp]
		public byte[] Version { get; set; }

		public DateTimeOffset? CreatedAt { get; set; }

		public DateTimeOffset? UpdatedAt { get; set; }

		public bool Deleted { get; set; }

		public string SyncedStatus { get; set; }

		public string UpdatedBy { get; set; }
		public string CreatedBy { get; set; }
    }
}
