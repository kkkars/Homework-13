using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DepsWebApp.Models
{
    /// <summary>
    /// Login data model.
    /// </summary>
    [Table("Accounts")]
    public class Account
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get;set; }
        /// <summary>
        /// Login for the account.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(AllowEmptyStrings = false)]
        [StringLength(64, MinimumLength = 6)]
        [Key]
        public string LoginId { get; set; }

        /// <summary>
        /// Password for the account.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(64, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
