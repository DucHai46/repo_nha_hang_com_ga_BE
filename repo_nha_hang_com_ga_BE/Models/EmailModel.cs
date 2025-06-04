using System.ComponentModel.DataAnnotations;

namespace repo_nha_hang_com_ga_BE.Models
{
    public class EmailModel
    {
        [Required]
        [EmailAddress]
        public string To { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public bool IsHtml { get; set; } = false;
    }
}