using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ITS_System.Data;
using ITS_System.Models;
using FlexAppealFitness.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace FlexAppealFitness.Areas.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ClassScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ClassScheduleController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/ClassSchedule

        public async Task<IActionResult> Index(string searchString)
        {
            var classSchedules = from c in _context.Schedule
                                 select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                classSchedules = classSchedules.Where(s => s.Instructor.Email.Contains(searchString) || s.DateTime.ToString().Contains(searchString));
            }

            return View(await classSchedules.Include("Instructor").Include("Room").OrderBy(s => s.DateTime).ToListAsync());
        }

        // GET: Admin/ClassSchedule/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Schedule == null)
            {
                return NotFound();
            }

            var classSchedule = await _context.Schedule
                .Include(c => c.Instructor)
                .Include(c => c.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classSchedule == null)
            {
                return NotFound();
            }

            return View(classSchedule);
        }

        // GET: Admin/ClassSchedule/Create
        public async Task<IActionResult> Create()
        {
            ViewData["InstructorId"] = new SelectList(await _userManager.GetUsersInRoleAsync("Instructor"), "Id", "Email");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Description");
            ViewData["EquipmentList"] = new SelectList(_context.Equpiments, "Id", "Name");
            return View();
        }


        // POST: Admin/ClassSchedule/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassSchedule classSchedule, List<int> EquipmentList)
        {
            if (classSchedule.InstructorId != null &&
               classSchedule.DateTime != null &&
               classSchedule.RoomId != null &&
               classSchedule.MaxNumbersOfBooking > 0 &&
               classSchedule.Status != null)
            {
                _context.Schedule.Add(classSchedule);
                await _context.SaveChangesAsync();

                foreach (int equipmentId in EquipmentList)
                {
                    var equipmentListEntry = new EquipmentListEntry
                    {
                        Equipment = _context.Equpiments.Find(equipmentId),
                        AddedOn = DateTime.Now
                    };

                    classSchedule.EquipmentList.Add(equipmentListEntry);
                }

                _context.Schedule.Update(classSchedule);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["InstructorId"] = new SelectList(await _userManager.GetUsersInRoleAsync("Instructor"), "Id", "Email", classSchedule.InstructorId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Description", classSchedule.RoomId);
            ViewData["EquipmentList"] = new SelectList(_context.Equpiments, "Id", "Name");
            return View(classSchedule);
        }

        // GET: Admin/ClassSchedule/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classSchedule = await _context.Schedule
                .Include(c => c.Instructor)
                .Include(c => c.Room)
                .Include(c => c.EquipmentList)
                    .ThenInclude(e => e.Equipment)
                .Include(c => c.WaitingList)
                    .ThenInclude(w => w.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (classSchedule == null)
            {
                return NotFound();
            }

            // Convert the current equipment list and waiting list to select lists for the view
            ViewData["InstructorId"] = new SelectList(await _userManager.GetUsersInRoleAsync("Instructor"), "Id", "Email", classSchedule.InstructorId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Description", classSchedule.RoomId);

            // Get all equipments and users for selection
            ViewData["EquipmentList"] = new SelectList(_context.Equpiments, "Id", "Description");
            ViewData["WaitingList"] = new SelectList(_context.Users, "Id", "Email");

            // Mark the currently selected items
            ViewData["SelectedEquipment"] = classSchedule.EquipmentList.Select(e => e.Equipment.Id).ToList();
            ViewData["SelectedUsers"] = classSchedule.WaitingList.Select(w => w.Customer.Id).ToList();

            return View(classSchedule);
        }

        // POST: Admin/ClassSchedule/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateTime,InstructorId,MaxNumbersOfBooking,RoomId,Status")] ClassSchedule classSchedule, string[] WaitingList, string[] EquipmentList)
        {
            if (id != classSchedule.Id)
            {
                return NotFound();
            }

            try
            {
                // Load the existing class schedule from the database
                var existingClassSchedule = await _context.Schedule.Include(s => s.WaitingList).Include(s => s.EquipmentList)
                    .FirstOrDefaultAsync(m => m.Id == id);

                // Update the existing class schedule with the values from the form
                existingClassSchedule.DateTime = classSchedule.DateTime;
                existingClassSchedule.InstructorId = classSchedule.InstructorId;
                existingClassSchedule.MaxNumbersOfBooking = classSchedule.MaxNumbersOfBooking;
                existingClassSchedule.RoomId = classSchedule.RoomId;
                existingClassSchedule.Status = classSchedule.Status;

                // Update waiting list
                existingClassSchedule.WaitingList.Clear();
                foreach (var userId in WaitingList)
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        existingClassSchedule.WaitingList.Add(new WaitingListEntry { Customer = user, AddedOn = DateTime.Now });
                    }
                }

                // Update equipment list
                existingClassSchedule.EquipmentList.Clear();
                foreach (var equipmentId in EquipmentList)
                {
                    var equipment = await _context.Equpiments.FindAsync(int.Parse(equipmentId));
                    if (equipment != null)
                    {
                        existingClassSchedule.EquipmentList.Add(new EquipmentListEntry { Equipment = equipment, AddedOn = DateTime.Now });
                    }
                }

                _context.Update(existingClassSchedule);
                await _context.SaveChangesAsync();


            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassScheduleExists(classSchedule.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: Admin/ClassSchedule/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Schedule == null)
            {
                return NotFound();
            }

            var classSchedule = await _context.Schedule
                .Include(c => c.Instructor)
                .Include(c => c.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classSchedule == null)
            {
                return NotFound();
            }

            return View(classSchedule);
        }

        // POST: Admin/ClassSchedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Schedule == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Schedule'  is null.");
                }
                var classSchedule = await _context.Schedule.FindAsync(id);
                if (classSchedule != null)
                {
                    _context.Schedule.Remove(classSchedule);
                }

                await _context.SaveChangesAsync();
                
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: { ex.Message }"); //Send and check error
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClassScheduleExists(int id)
        {
            return (_context.Schedule?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
