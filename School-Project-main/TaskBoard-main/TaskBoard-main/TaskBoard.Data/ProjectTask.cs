using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Data
{
    public class ProjectTask
    {
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; init; }

        [ForeignKey("Task")]
        public int TaskId { get; set; }
        public Task Task { get; init; }
    }
}
