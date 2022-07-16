using Microsoft.AspNetCore.Mvc;
using System.Linq;

using TaskBoard.Data;
using TaskBoard.WebApp.Models;
using TaskBoard.WebApp.Models.Task;

using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Globalization;
using System.Collections.Generic;


namespace TaskBoard.WebApp.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {

        private readonly ApplicationDbContext dbContext;
        public ProjectsController(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        [Route("/{controller}")]
        public IActionResult All()
        {
            var projects = this.dbContext.Projects
                .Select(p => new ProjectViewModel()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description
                });
            return View(projects);
        }

        public IActionResult Create()
        {
            ProjectFormModel projectModel = new ProjectFormModel();

            return View(projectModel);
        }

        [HttpPost]
        public IActionResult Create(ProjectFormModel projectModel)
        {
            if (!ModelState.IsValid)
            {
                return View(projectModel);
            }
            Project project = new Project()
            {
                Title = projectModel.Title,
                Description = projectModel.Description
            };
            this.dbContext.Projects.Add(project);
            this.dbContext.SaveChanges();

            var boards = this.dbContext.Boards;

            return RedirectToAction("All", "Projects");
        }
        public IActionResult Delete(int id)
        {
            Project project = dbContext.Projects.Find(id);
            if (project == null)
            {
                // When project with this id doesn't exist
                return BadRequest();
            }

            ProjectViewModel projectModel = new ProjectViewModel()
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description
            };

            return View(projectModel);
        }

        [HttpPost]
        public IActionResult Delete(ProjectViewModel projectModel)
        {
            Project project = this.dbContext.Projects.Find(projectModel.Id);
            if (project == null)
            {
                // When project with this id doesn't exist
                return BadRequest();
            }
            var taskIds = this.dbContext.ProjectTasks.Where(x => x.ProjectId == project.Id).ToList();
            foreach(var task in taskIds)
            {
                this.dbContext.Tasks.Remove(this.dbContext.Tasks.Find(task.TaskId));
                this.dbContext.ProjectTasks.Remove(this.dbContext.ProjectTasks.Find(task.ProjectId, task.TaskId));
            }

            this.dbContext.Projects.Remove(project);
            this.dbContext.SaveChanges();
            return RedirectToAction("All", "Projects");
        }

        public IActionResult Details(int id)
        {
            var tasks = this.dbContext.Tasks.Join(this.dbContext.ProjectTasks,
                t => t.Id,
                pt => pt.TaskId,
                (t, pt) => new
                {
                    TaskId = t.Id,
                    ProjectId = pt.ProjectId,
                    Title = t.Title,
                    Description = t.Description,
                    Owner = t.Owner,
                    BoardId = t.BoardId
                }).Where(x => x.ProjectId == id).AsQueryable();


            var boards = this.dbContext.Boards
                .Select(b => new BoardViewModel()
                {
                    Id = b.Id,
                    Name = b.Name,
                    Tasks = tasks.Where(ts => ts.BoardId == b.Id).Select(t => new TaskViewModel()
                    {
                        Id = t.TaskId,
                        Title = t.Title,
                        Description = t.Description,
                        Owner = t.Owner.UserName
                    }).ToList()
                })
                .ToList();

            return View(boards);
        }

        public IActionResult CreateProjectTask(int Id)
        {
            TaskFormModel taskModel = new TaskFormModel() { ProjectId = Id };

            return View(taskModel);
        }
        [HttpPost]
        public IActionResult CreateProjectTask(TaskFormModel taskModel)
        {
            if (!ModelState.IsValid)
            {
                return View(taskModel);
            }
            string currentUserId = GetUserId();
            Task task = new Task()
            {
                Title = taskModel.Title,
                Description = taskModel.Description,
                CreatedOn = DateTime.Now,
                BoardId = dbContext.Boards.FirstOrDefault(b => b.Name == "Open").Id,
                OwnerId = currentUserId
            };
            this.dbContext.Tasks.Add(task);
            this.dbContext.SaveChanges();
            ProjectTask pt = new ProjectTask()
                {
                    ProjectId = taskModel.ProjectId,
                    TaskId = this.dbContext.Tasks.Where(x => x.Title == task.Title && x.CreatedOn == task.CreatedOn && x.OwnerId == task.OwnerId).FirstOrDefault().Id
                };
                this.dbContext.ProjectTasks.Add(pt);
                this.dbContext.SaveChanges();
                return RedirectToAction("All", "Projects");
            
        }
        private string GetUserId()
            => this.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
