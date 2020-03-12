using System;

namespace DatingApp.API.Dtos
{
	public class PhotoForDetailedDto
	{
		public DateTime DateAdded { get; set; }
		public int Id { get; set; }
		public string Url { get; set; }
		public string Description { get; set; }
		public bool IsMain { get; set; }
	}
}
