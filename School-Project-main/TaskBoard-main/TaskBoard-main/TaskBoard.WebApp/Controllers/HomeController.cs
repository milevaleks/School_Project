using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

using TaskBoard.Data;
using TaskBoard.WebApp.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace TaskBoard.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        public IActionResult Index()
        {
            var taskBoards = this.dbContext
                .Boards
                .Select(b => b.Name)
                .Distinct();

            var tasksCounts = new List<HomeBoardModel>();
            foreach (var boardName in taskBoards)
            {
                var tasksInBoard = this.dbContext.Tasks.Where(t => t.Board.Name == boardName).Count();
                tasksCounts.Add(new HomeBoardModel()
                {
                    BoardName = boardName,
                    TasksCount = tasksInBoard
                });
            }

            var userTasksCount = -1;
            var userOpenTasksCount = -1;
            var userInProgressTasksCount = -1;
            var userDoneTasksCount = -1;



            var userStore = new UserStore<User>(dbContext);
            var hasher = new PasswordHasher<User>();
            var normalizer = new UpperInvariantLookupNormalizer();
            var factory = new LoggerFactory();
            var logger = new Logger<UserManager<User>>(factory);
            var userManager = new UserManager<User>(
                userStore, null, hasher, null, null, normalizer, null, null, logger);

            if (this.User.Identity.IsAuthenticated)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                userTasksCount = this.dbContext.Tasks.Where(t => t.OwnerId == currentUserId).Count();
                userOpenTasksCount = this.dbContext.Tasks.Where(t => t.Board.Name == "Open" && t.OwnerId == currentUserId).Count();
                userInProgressTasksCount = this.dbContext.Tasks.Where(t => t.Board.Name == "In Progress" && t.OwnerId == currentUserId).Count();
                userDoneTasksCount = this.dbContext.Tasks.Where(t => t.Board.Name == "Done" && t.OwnerId == currentUserId).Count();
            }

            int totalProjects = this.dbContext.Projects.Count();

            var homeModel = new HomeViewModel()
            {
                AllTasksCount = this.dbContext.Tasks.Count(),
                BoardsWithTasksCount = tasksCounts,
                UserTasksCount = userTasksCount,
                CreatedUsersCount = userManager.Users.Count(),
                UserOpenTasksCount = userOpenTasksCount,
                UserDoneTasksCount = userDoneTasksCount,
                UserInProgressTasksCount = userInProgressTasksCount,
                TotalProjects = totalProjects,
            };


            return View(homeModel);
        }

        public IActionResult Error() 
            => View();

        public IActionResult Error401() 
            => View();

        public IActionResult Error404()
            => View();
    }
}
