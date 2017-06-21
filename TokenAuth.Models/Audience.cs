using System;

namespace TokenAuth.Models
{
	public class Audience : EntityBase
	{
		public string Base64Secret { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string ApplicationUrl { get; set; }

		public bool DisplayAudience { get; set; }
	}
}
