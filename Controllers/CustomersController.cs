using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using hm_4_2.Data;
using hm_4_2.Models;

namespace hm_4_2.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _context.Customers.ToListAsync());
            } catch (Exception ex)
            {
                return View("Error");
            }
            
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (customer == null)
                {
                    return NotFound();
                }

                return View(customer);
            } catch (Exception ex)
            {
                return View("Error");
            }
            
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Email")] Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(customer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(customer);
            } catch (Exception ex)
            {
                return View("Error");
            }
            
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return View(customer);
            } catch(Exception ex)
            {
                return View("Error");
            }
            
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Email")] Customer customer)
        {
            try
            {
                if (id != customer.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(customer);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CustomerExists(customer.Id))
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
                return View(customer);
            } catch (Exception ex)
            {
                return View("Error");
            }
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (customer == null)
                {
                    return NotFound();
                }

                return View(customer);
            } catch (Exception ex)
            {
                return View("Error");
            }
            
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer != null)
                {
                    _context.Customers.Remove(customer);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } catch (Exception ex)
            {
                return View("Error");
            }
        }

        private bool CustomerExists(int id)
        {
            try
            {
                return _context.Customers.Any(e => e.Id == id);
            } catch (Exception ex)
            {
                return false;
            }
            
        }
    }
}
