using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentBase.Models
{
    public class User
    {
        public virtual long id { get; set; }

        [Required]
        public virtual string login { get; set; }

        [Required]
        [DataType(DataType.Password)]      
        public virtual string password { get; set; }
    }
}