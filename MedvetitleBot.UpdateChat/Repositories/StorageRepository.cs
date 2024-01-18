using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Data.Tables;
using MedvetitleBot.UpdateChat.Interfaces;
using MedvetitleBot.UpdateChat.Models;
using MedvetitleBot.UpdateChat.Options;
using MedvetitleBot.UpdateChat.ViewModels;
using Microsoft.Extensions.Options;

namespace MedvetitleBot.UpdateChat.Repositories
{
	public class StorageRepository : IStorageRepository
	{
        private readonly TableServiceClient _tableServiceClient;
        private readonly IMapper _mapper;
        private readonly UpdateChatOptions _options;
        private readonly TableClient _titles;
        private readonly TableClient _chats;

        public StorageRepository(
            TableServiceClient tableServiceClient,
            IOptions<UpdateChatOptions> options,
            IMapper mapper)
		{
            _tableServiceClient = tableServiceClient;
            _mapper = mapper;
            _options = options.Value;

            _titles = _tableServiceClient.GetTableClient(_options.ChatTitlesTableName);
            _titles.CreateIfNotExists();

            _chats = _tableServiceClient.GetTableClient(_options.ChatsTableName);
            _chats.CreateIfNotExists();
        }

        public async Task<List<Chat>> GetChats()
        {
            var result = new List<Chat>();
            var chatModels = _chats.QueryAsync<ChatEntity>(ce => ce.PartitionKey == _options.DefaultPartitionKey && ce.Enabled);

            await foreach (var model in chatModels)
                result.Add(_mapper.Map<Chat>(model));

            return result;
        }

        public async Task<List<TitleOption>> GetTitleOptions()
        {
            var result = new List<TitleOption>();
            var titleOptionModels = _titles.QueryAsync<TitleOptionEntity>(to => to.PartitionKey == _options.DefaultPartitionKey && to.Enabled);

            await foreach (var model in titleOptionModels)
                result.Add(_mapper.Map<TitleOption>(model));

            return result;
        }

        public async Task<long> UpsertChat(Chat chat)
        {
            var model = _mapper.Map<ChatEntity>(chat);
            model.PartitionKey = _options.DefaultPartitionKey;
            model.RowKey = chat.Id.ToString();
            await _chats.UpsertEntityAsync(model);
            return model.Id;
        }
    }
}

