using CapstoneProject_.NETFSD.Data.Services;
using CapstoneProject_.NETFSD.Data.Static;
using CapstoneProject_.NETFSD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneProject_.NETFSD.Controllers
{
    [Authorize(Roles = UserRoles.Admin)] // V.97
    public class MedicinesController : Controller
    {
        private readonly IMedicinesService _Service;

        public MedicinesController(IMedicinesService Service)
        {
            _Service = Service;
        }



        // <summary>
        [AllowAnonymous] // V.96
        public async Task<IActionResult> Index()
        {
            var Data = await _Service.GetAll();
            return View(Data);
        }// End of GetAll </summary>

        [AllowAnonymous] // V.96
        public async Task<IActionResult> Filter(string searchString) 
        {
            var Allmedicines = await _Service.GetAll();
            if (!string.IsNullOrEmpty(searchString))
            {
                var FilterResult = Allmedicines.Where(n => n.Name.Contains(searchString) || n.MedicineCategory.ToString().Contains(searchString)).ToList(); 
                if (FilterResult.Count != 0)
                {
                    return View("Index", FilterResult);
                }
                TempData["Error"] = "Hmm no result, check letter case OR Sort by use category name"; 
            }
            return View("Index",Allmedicines);
        } 

        public IActionResult Create()
        {
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> Create([Bind("ImageCode,Name,Description,Price,MedicineCategory")] Medicine medicine)
        {
            if (!ModelState.IsValid)
            {
                return View(medicine);
            }
           await _Service.Add(medicine);
            return RedirectToAction(nameof(Index));

        }// End of Create Get&Post </summary>


        // <summary> Get Medicine for GetEditView + [used to get for delete also]
        public async Task<IActionResult> Edit(int id)
        {
            var MedicineDetails = await _Service.GetByID(id);
            if(MedicineDetails == null ) return View ("NotFounded"); 

            return View(MedicineDetails);
        }
       
        [HttpPost]
        public async Task<IActionResult> Edit(int id,[Bind("ID,ImageCode,Name,Description,Price,MedicineCategory")] Medicine medicine)
        {
            if (!ModelState.IsValid)
            {
                return View(medicine);
            }
           
            await _Service.Update(id,medicine);
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Delete(int id)
        {
            var MedicineDetails = await _Service.GetByID(id);
            if (MedicineDetails == null) return View ("NotFounded"); 

            await _Service.Delete(id);
            return RedirectToAction(nameof(Index));

        }

    }
}
