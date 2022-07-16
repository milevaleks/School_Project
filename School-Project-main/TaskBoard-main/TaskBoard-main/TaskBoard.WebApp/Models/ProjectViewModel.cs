using System.Collections.Generic;

using TaskBoard.WebApp.Models.Task;

namespace TaskBoard.WebApp.Models
{
    public class ProjectViewModel
    {
        public int Id { get; init; }

        public string Title { get; init; }
        public string Description { get; init; }
    }
}
