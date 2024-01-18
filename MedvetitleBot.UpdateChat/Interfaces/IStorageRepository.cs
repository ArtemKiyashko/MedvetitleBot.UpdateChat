using System.Collections.Generic;
using System.Threading.Tasks;
using MedvetitleBot.UpdateChat.ViewModels;

namespace MedvetitleBot.UpdateChat.Interfaces
{
	public interface IStorageRepository
	{
		Task<List<TitleOption>> GetTitleOptions();
        Task<List<Chat>> GetChats();
        Task<long> UpsertChat(Chat chat);
    }
}

