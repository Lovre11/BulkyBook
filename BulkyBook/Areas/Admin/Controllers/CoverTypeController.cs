using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();

            if (id == null)
            {
                // Create
                return View(coverType);
            }
            // Edit
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);

            coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter); // coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            
            if (coverType == null)
            {
                return NotFound();
            }
            
            return View(coverType);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Upsert(CoverType coverType)
        {
            if (!ModelState.IsValid)
            {
                return View(coverType);
            }
            var parameter = new DynamicParameters();
            parameter.Add("@Name", coverType.Name);

            if (coverType.Id != 0)
            {
                parameter.Add("@Id", coverType.Id);
                _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameter); // _unitOfWork.CoverType.Update(coverType);
            }
            else
            {
                _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameter); // _unitOfWork.CoverType.Add(coverType);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll, null); // _unitOfWork.CoverType.GetAll();
            return Json(new { data = allObj });
        }

        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            var objFromDb = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter); // _unitOfWork.CoverType.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting!" });
            }
            else
            {
                _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter); // _unitOfWork.CoverType.Remove(id);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Delete Successful!" });
            }
        }
        #endregion
    }
}
