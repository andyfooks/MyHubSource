using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class EscalationsEmailRecipient
    {
        #region [ EF properties ]

        public int Id { get; set; }
        [ForeignKey("EscalationsFrameworkRuleConfig")]
        [Required]
        public int EscalationsFrameworkRuleConfigId { get; set; }
        public EscalationsFrameworkRuleConfigEmailUser EscalationsFrameworkRuleConfig { get; set; }
        [ForeignKey("Recipient")]
        [Required]
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region [ Non EF Properties ]

        [NotMapped]
        public string RecipientName { get; set; }
        [NotMapped]
        public string RecipientEmail { get; set; }
      
        #endregion

        public User GetRecipient(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Recipient").IsLoaded)
            {
                ctx.Entry(this).Reference("Recipient").Load();
            }

            return this.Recipient;
        }
    }
}
