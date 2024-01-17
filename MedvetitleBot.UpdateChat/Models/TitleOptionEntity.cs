namespace MedvetitleBot.UpdateChat.Models
{
	public class TitleOptionEntity : BaseEntity
	{
		public string Title { get; set; }
        public bool Enabled { get; set; } = true;
    }
}

