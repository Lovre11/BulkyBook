using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new Company();

            if (id == null)
            {
                // Create
                return View(company);
            }
            // Edit
            company = _unitOfWork.Company.Get(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (!ModelState.IsValid)
            {
                return View(company);
            }

            if (company.Id == 0)
            {
                _unitOfWork.Company.Add(company);
            }
            else
            {
                _unitOfWork.Company.Update(company);
            }
            //*****Ja krivo napravija*****
            //var objFromDb = _unitOfWork.Company.Get(company.Id);

            //objFromDb.Name = company.Name;
            //objFromDb.PhoneNumber = company.PhoneNumber;
            //objFromDb.PostalCode = company.PostalCode;
            //objFromDb.State = company.State;
            //objFromDb.StreetAddress = company.StreetAddress;
            //objFromDb.City = company.City;
            //objFromDb.IsAuthorizedCompany = company.IsAuthorizedCompany;
            //*****
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObjFromDb = _unitOfWork.Company.GetAll();

            return Json(new { data = allObjFromDb });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Company.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Deleting!" });
            }
            _unitOfWork.Company.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful!" });
        }
        #endregion

    }
}
