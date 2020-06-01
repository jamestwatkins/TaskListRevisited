using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskListRevisitedLab.Models;

namespace TaskListRevisitedLab.Controllers
{
    [Authorize]
    public class TaskListController : Controller
    {
        private readonly TaskListDbContext _context;
        public TaskListController(TaskListDbContext context)
        {
            _context = context;
        }

        public IActionResult TaskIndex()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var thisUsersTasks = _context.Tasks.Where(x => x.UserId == id).ToList();
            return View(thisUsersTasks);
        }

        public IActionResult TaskForm()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddTask()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddTask(Tasks newTask)
        {
            newTask.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                _context.Tasks.Add(newTask);
                _context.SaveChanges();
                return RedirectToAction("TaskIndex");
            }
            else
            {
                return View();
            }
        }

        public IActionResult UpdateComplete(int id)
        {
            Tasks found = _context.Tasks.Find(id);

            if(found != null)
            {
                if(found.Completed == "false")
                {
                    found.Completed = "true";
                }
                else
                {
                    found.Completed = "false";
                }

                _context.Entry(found).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.Update(found);
                _context.SaveChanges();
            }

            return RedirectToAction("TaskIndex");
        }

        public IActionResult DeleteTask(int id)
        {
            Tasks found = _context.Tasks.Find(id);
            if (found != null)
            {
                _context.Tasks.Remove(found);
                _context.SaveChanges();
            }
            return RedirectToAction("TaskIndex");
        }
    }
}