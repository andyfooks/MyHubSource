using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AJG.VirtualTrainer.Helper.Encryption;
using AJG.VirtualTrainer.Services;

namespace VirtualTrainer
{
    public class ExchangeAccountDetails : VirtualTrainerBase
    {
        #region [ Mapped Properties ]
        public int Id { get; set; }
        [Required]
        public string DisplayName { get; set; }
        public string Description { get; set; }
        [Required]
        public string AutoDiscoverUserName { get; set; }
        [Required]
        public string AutoDiscoverUserPassword { get; set; }
        [Required]
        public string AutoDiscoverUserDomain { get; set; }
        [Required]
        public string AutoDiscoverEmail { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region [ Not Mapped Properties ]

        public string GetAutoDiscoverUserPasswordDecrypted()
        {
            AdminService service = new AdminService();
            return EncryptionHelper.Decrypt(this.AutoDiscoverUserPassword, service.GetEncryptionKey());
        }

        #endregion
    }
}
