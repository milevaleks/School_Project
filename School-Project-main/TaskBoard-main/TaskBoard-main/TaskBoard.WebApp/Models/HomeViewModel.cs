using System.Collections.Generic;

namespace TaskBoard.WebApp.Models
{
    public class HomeViewModel
    {
        public int AllTasksCount { get; init; }

        public List<HomeBoardModel> BoardsWithTasksCount { get; init; }
        public int UserTasksCount { get; init; }
        public int UserOpenTasksCount { get; init; }
        public int UserInProgressTasksCount { get; init; }
        public int UserDoneTasksCount { get; init; }
        public int CreatedUsersCount { get; init; }
        public int TotalProjects { get; init; }
    }
}
