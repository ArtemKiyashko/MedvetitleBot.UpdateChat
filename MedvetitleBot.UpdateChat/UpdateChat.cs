using System;
using System.Threading.Tasks;
using MedvetitleBot.UpdateChat.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

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
        public async Task Run([TimerTrigger("%FUNCTION_SCHEDULE%", RunOnStartup = false)]TimerInfo myTimer)
        {
            var chats = await _storageRepository.GetChats();
            var titleOptions = await _storageRepository.GetTitleOptions();

            foreach (var chat in chats)
            {
                try
                {
                    chat.Title = titleOptions[Random.Shared.Next(titleOptions.Count)].Title;
                    _logger.LogInformation($"New title selected: {chat.Title}");
                    try
                    {
                        await _telegramBotClient.SetChatTitleAsync(chat.Id, chat.Title);
                        await _storageRepository.UpsertChat(chat);
                        _logger.LogInformation($"New title set. ChatId: {chat.Id}, NewTitle: {chat.Title}");
                    }
                    catch (ApiRequestException ex) when (ex.ErrorCode == 403)
                    {
                        _logger.LogWarning($"Bot has been forbidden in the chat: {chat.Id}, Title: {chat.Title}");
                        chat.Enabled = false;
                        await _storageRepository.UpsertChat(chat);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating chat title. ChatId: {chat.Id}; ChatTitle: {chat.Title}");
                }
            }
        }
    }
}

