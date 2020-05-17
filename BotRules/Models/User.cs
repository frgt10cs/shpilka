using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public bool ConfirmEmail { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public DateTime LastChanges { get; set; }
        public string Role { get; set; }
        public int BotCount { get; set; }
    }
}
