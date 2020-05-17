using BotRules.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Services.Interfaces
{
    public interface IEditorService
    {
        Task<bool> AddStorage(User user, Storage storage);
        bool RemoveStorage(int userId, string storageName);
        Task<string[]> GetStorageDataAsync(int userId, string storageName);
        string[] GetUsersStorages(int userId);
    }
}
