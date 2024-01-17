namespace MedvetitleBot.UpdateChat.Options
{
	public class UpdateChatOptions
	{
        public string ChatTitlesTableName { get; set; } = "ChatTitles";
        public string ChatsTableName { get; set; } = "Chats";
        public string TableServiceConnection { get; set; } = "UseDevelopmentStorage=true";
        public string DefaultPartitionKey { get; set; } = "primary";
        public string TelegramBotToken { get; set; }
    }
}

