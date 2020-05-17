using BotRules.Models;
using BotRules.Services;
using BotRules.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Controllers
{
    public class EditorController:Controller
    {
        IEditorService editServ;
        IAccountService accServ;
        public EditorController(IEditorService editServ, IAccountService accServ)
        {
            this.editServ = editServ;
            this.accServ = accServ;
        }

        [HttpPost]
        public async Task<JsonResult> CreateStorage([FromBody]Storage storage)
        {
            if (await editServ.AddStorage(await accServ.GetCurrentUserAsync(User.Identity.Name), storage) == true)
                return Json(true);
            return Json("Не удалось создать хранилище. Убедитесь, что хранилища с таким же именнем не существует.");
        }

        [HttpPost]
        public async Task<JsonResult> GetStorageData(string storageName)
        {
            int userId = Int32.Parse(accServ.GetUserClaimValue("Id"));
            return Json(await editServ.GetStorageDataAsync(userId,storageName));
        }

        [HttpPost]
        public JsonResult GetStorages()
        {
            return Json(editServ.GetUsersStorages(Int32.Parse(accServ.GetUserClaimValue("Id"))));
        }

        [HttpPost]
        public async Task<JsonResult> RemoveStorage(string storageName)
        {
            return Json(editServ.RemoveStorage((await accServ.GetCurrentUserAsync(User.Identity.Name)).Id, storageName));
        }

        [HttpPost]
        public async Task<IActionResult> UploadStorage(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                User curUser = await accServ.GetCurrentUserAsync(User.Identity.Name);
                string filename = uploadedFile.FileName;
                string type = filename.Substring(filename.Length-3);
                if (curUser != null && (type == "txt" || type == "csv"))
                {                    
                    string path = AppConfig.UsersPath +"/"+curUser.Id+"/Storages/" +uploadedFile.FileName;                    
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                }               
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
