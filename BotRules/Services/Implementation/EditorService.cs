using BotRules.Models;
using BotRules.Services.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Services.Implementation
{
    public class EditorService : IEditorService
    {
        public async Task<bool> AddStorage(User user, Storage storage)
        {
            string Name = AppConfig.UsersPath + $"/{user.Id}/Storages";
            string Data = "";
                    
            if (storage.ColumnNames.Length > 0)
            {
                Data = "Id;";
                for (int i = 0; i < storage.ColumnNames.Length; i++)
                {
                    if(!String.IsNullOrWhiteSpace(storage.ColumnNames[i]))
                        Data += storage.ColumnNames[i] + (storage.ColumnNames.Length-1!=i?";":"");
                }
                Data +=  Environment.NewLine;
                if (!File.Exists(Name))
                {
                    Directory.CreateDirectory(Name);
                    await File.WriteAllTextAsync(Name + $"/{storage.Name}.txt", Data);
                    return true;
                }
            }            
            return false;
        }

        public string[] GetUsersStorages(int userId)
        {
            string[] FilesName = Directory.GetFiles(AppConfig.UsersPath + $"/{userId}/Storages").Select(Path.GetFileName).ToArray();
            for (int i = 0; i < FilesName.Length; i++)
                FilesName[i] = FilesName[i].Remove(FilesName[i].Length - 4);
            return FilesName;
        }

        public bool RemoveStorage(int userId, string storageName)
        {
            if (File.Exists(Environment.CurrentDirectory + $"/wwwroot/users/{userId}/Storages/{storageName}.txt"))
            {
                File.Delete(Environment.CurrentDirectory + $"/wwwroot/users/{userId}/Storages/{storageName}.txt");
                return true;
            }
            return false;
        }

        public async Task<string[]> GetStorageDataAsync(int userId, string storageName)
        {
            string Name = Environment.CurrentDirectory+$"/wwwroot/users/{userId}/Storages/{storageName}.txt";
            if (File.Exists(Name))
                return await File.ReadAllLinesAsync(Name);
            return null;
        }
    }
}
