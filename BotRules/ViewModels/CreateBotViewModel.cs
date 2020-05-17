using BotRules.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace BotRules.ViewModels
{
    public class CreateBotViewModel
    {
        [MinLength(3)]
        [MaxLength(20)]
        [OnlyLatin]
        [Required]
        public string Name { get; set; }
        [MaxLength(100)]
        [Required]
        public string Key { get; set; }        
    }
}
