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
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _context.Products.ToListAsync());
            } catch (Exception ex)
            {
                return View("Error");
            }
            
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var product = await _context.Products
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (product == null)
                {
                    return NotFound();
                }

                return View(product);
            } catch (Exception ex)
            {
                return View("Error");
            }
            
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Mpn")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            } catch (Exception ex)
            {
                return View("Error");
            }
            
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            } catch (Exception ex)
            {
                return View("Error");
            }
            
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Mpn")] Product product)
        {
            try
            {
                if (id != product.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductExists(product.Id))
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
                return View(product);
            } catch
            {
                return View("Error");
            }
            
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var product = await _context.Products
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (product == null)
                {
                    return NotFound();
                }

                return View(product);
            } catch (Exception ex)
            {
                return View("Error");
            }
            
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } catch (Exception ex)
            {
                return View("Error");
            }
            
        }

        private bool ProductExists(int id)
        {
            try{
                return _context.Products.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
    }
}
