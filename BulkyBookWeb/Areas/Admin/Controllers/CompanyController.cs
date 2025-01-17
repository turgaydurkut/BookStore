﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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

        //GET
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null || id == 0)
            {
                return View(company);
            }
            else
            {
                company=_unitOfWork.Company.GetFirstOrDefault(c=>c.Id==id);
                return View(company);
            }
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    TempData["success"] = "Company updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new {data=companyList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var company = _unitOfWork.Company.GetFirstOrDefault(c=>c.Id==id);
            if(company == null)
            {
                return Json(new { success = false, message = "Error While deleting" });
            }
            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfuly" });
        }


        #endregion
    }
}
