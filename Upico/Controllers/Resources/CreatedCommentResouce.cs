using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Controllers.Resources
{
    public class CreatedCommentResouce
    {
        [Required]
        public string Username { set; get; }
        [Required]
        public string Content { set; get; }

        [Required]
        public string PostId { set; get; }
        
    }
}
