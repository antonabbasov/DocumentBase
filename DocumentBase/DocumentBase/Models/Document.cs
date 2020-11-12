using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DocumentBase.Models
{
    /// <summary>
    /// Модель данных документа
    /// </summary>
    public class Document
    {
        
        public virtual long id { get; set; }

        [Required(ErrorMessage="Document name is required")]     
        public virtual string name { get; set; }

        public virtual long authorId { get; set; }

        public virtual User author { get; set; }

        [Required(ErrorMessage = "File is requiered")]      
        public virtual string binaryFile { get; set; }

        [Required(ErrorMessage = "Date is requiered")]
        [DataType(DataType.Date)]        
        public virtual DateTime changeDate { get; set; }
    }
}