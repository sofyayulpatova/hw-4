using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using hm_4_2.Data;
using hm_4_2.Models;
using hm_4_2.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace hm_4_2.Controllers
{
    public class Orders1Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Orders1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders1
        public async Task<IActionResult> Index()
        {

            try
            {
                var ordersWithCustomers = await _context.Orders
                                                .Include(o => o.Customer) // Include the customer data
                                                .ToListAsync();
                return View(ordersWithCustomers);
            }
            catch (Exception ex)
            {

                
                return View("Error"); 
            }
        }

        // GET: Orders1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var order = await _context.Orders
                     .Include(o => o.OrderDetails) // Include OrderDetails
                     .Include(p => p.Customer)     // Include Customer details
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                return View("Error"); 
            }
        }

        // GET: Orders1/Create
        public IActionResult Create()
        {
            // https://www.tutorialsteacher.com/mvc/viewbag-in-asp.net-mvc
            // way to translate it to the view late (dynamic)
            // ViewBag.StateList = new SelectList(new List<string> { "New", "In Process", "Completed" }, "New");
            // or
            // dictionary-based ViewData

            try
            {
                ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
                ViewData["StateList"] = new SelectList(new List<string> { "New", "In Process", "Completed" });

                
                var viewModel = new OrderViewModel
                {
                    Order = new Order(), // Initialize a new Order
                    OrderDetails = new List<OrderDetails> { new OrderDetails() } // Initialize empty OrderDetails
                };

                return View(viewModel); // ViewModel to the View
            }
            catch (Exception ex)
            {
                return View("Error"); 
            }


        }


        private async Task<bool> ValidateCustomerExistence(int customerId)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                {
                    // If no customer found > error to ModelState and refresh the customer list in ViewData
                    ModelState.AddModelError("Order.CustomerId", "Selected customer does not exist.");
                    ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> ValidateProductExistence(OrderDetails detail)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Mpn == detail.ProductName);
                if (product == null)
                {
                    ModelState.AddModelError("", $"Product {detail.ProductName} does not exist.");
                    return false;
                }
                detail.ProductID = product.Id; // Set ProductID here for clarity
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        // POST: Orders1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel viewModel)
        {
            try { 
                if (!await ValidateCustomerExistence(viewModel.Order.CustomerId))
                    return View(viewModel);

                foreach (var detail in viewModel.OrderDetails)
                {
                    if (!await ValidateProductExistence(detail))
                        return View(viewModel);
                }

                viewModel.Order.Customer = await _context.Customers.FindAsync(viewModel.Order.CustomerId);
                viewModel.Order.CustomerId = viewModel.Order.Customer.Id;


                _context.Orders.Add(viewModel.Order);
                await _context.SaveChangesAsync();

                foreach (var detail in viewModel.OrderDetails)
                {
                    detail.OrderID = viewModel.Order.Id; 
                    _context.OrderDetails.Add(detail);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            } catch (Exception ex)
            {
                return View("Error");
            }
        }

        // GET: Orders1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try { 
                if (id == null)
                {
                    return NotFound();
                }

                var order = await _context.Orders
                    .Include(o => o.Customer)  // Load Customer
                    .Include(o => o.OrderDetails) // Load OrderDetails
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (order == null)
                {
                    return NotFound();
                }
                ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
                ViewData["StateList"] = new SelectList(new List<string> { "New", "In Process", "Completed" }, order.State);
                return View(order);
            } catch (Exception ex)
            {
                return View("Error");
            }
        }

        // POST: Orders1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order, int[] DeletedDetails)
        {
            try
            {


                if (id != order.Id)
                {
                    return NotFound();
                }

                var existingOrder = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == id);
                if (existingOrder == null)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        // Update the order's scalar properties
                        _context.Entry(existingOrder).CurrentValues.SetValues(order);

                        // Update existing details or add new ones
                        foreach (var detail in order.OrderDetails)
                        {
                            var existingDetail = existingOrder.OrderDetails.FirstOrDefault(d => d.Id == detail.Id);
                            if (existingDetail != null)
                            {
                                _context.Entry(existingDetail).CurrentValues.SetValues(detail);
                            }
                            else if (detail.Id == 0)
                            {
                                if (await ValidateProductExistence(detail))
                                {
                                    existingOrder.OrderDetails.Add(detail);
                                }

                                
                            }
                        }

                        // Process deletions
                        if (DeletedDetails != null)
                        {
                            foreach (var detailId in DeletedDetails)
                            {
                                var detailToDelete = existingOrder.OrderDetails.FirstOrDefault(d => d.Id == detailId);
                                if (detailToDelete != null)
                                {

                                    _context.OrderDetails.Remove(detailToDelete);
                                }
                            }
                        }

                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = order.Id });
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!OrderExists(order.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
                ViewData["StateList"] = new SelectList(new List<string> { "New", "In Process", "Completed" }, order.State);
                return View(order);
            } catch(Exception ex)
            {
                return View("Error");
            }
        }






        // GET: Orders1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try { 
                if (id == null)
                {
                    return NotFound();
                }

                var order = await _context.Orders
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            } catch (Exception ex)
            {
                return View("Error");
            }
        }

        // POST: Orders1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order != null)
                {
                    _context.Orders.Remove(order);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } catch (Exception ex)
            {
                return View("Error");
            }
           
        }

        private bool OrderExists(int id)
        {
            try
            {
                return _context.Orders.Any(e => e.Id == id);
            } catch (Exception ex)
            {
                return false;
            }

        }
    }
}
