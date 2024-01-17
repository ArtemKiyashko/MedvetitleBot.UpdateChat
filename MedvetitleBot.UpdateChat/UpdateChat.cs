using System;
using System.Threading.Tasks;
using MedvetitleBot.UpdateChat.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace MedvetitleBot.UpdateChat
{
    public class UpdateChat
    {
        private readonly IStorageRepository _storageRepository;
        private readonly ILogger<UpdateChat> _logger;
        private readonly ITelegramBotClient _telegramBotClient;

        public UpdateChat(
            IStorageRepository storageRepository,
            ILogger<UpdateChat> logger,
            ITelegramBotClient telegramBotClient)
        {
            _storageRepository = storageRepository;
            _logger = logger;
            _telegramBotClient = telegramBotClient;
        }

        [FunctionName("UpdateChat")]
        public async Task Run([TimerTrigger("%FUNCTION_SCHEDULE%", RunOnStartup = true)]TimerInfo myTimer)
        {
            var chats = await _storageRepository.GetChats();
            var titleOptions = await _storageRepository.GetTitleOptions();

            foreach (var chat in chats)
            {
                try
                {
                    var newTitle = titleOptions[Random.Shared.Next(titleOptions.Count)].Title;
                    _logger.LogInformation($"New title selected: {newTitle}");
                    await _telegramBotClient.SetChatTitleAsync(chat.Id, newTitle);
                    _logger.LogInformation($"New title set. ChatId: {chat.Id}, NewTitle: {newTitle}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating chat title. ChatId: {chat.Id}");
                }
            }
        }
    }
}

