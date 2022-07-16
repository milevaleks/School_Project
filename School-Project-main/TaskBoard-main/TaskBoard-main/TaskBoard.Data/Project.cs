using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Data
{
    using static DataConstants;
    public class Project
    {
        public int Id { get; set; }

        [Required]
        //[MaxLength(MaxProjectTitle)]
        public string Title { get; set; }

        [Required]
        //[MaxLength(MaxProjectDescription)]
        public string Description { get; set; }
        public IEnumerable<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
    }
}
