using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkyBook.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
            //******Moze ovako****
            //var objFromDb = _db.Companies.FirstOrDefault(c => c.Id == company.Id);

            //if (objFromDb != null)
            //{
            //    _db.Companies.Update(company);
            //}
            //**********

            // Ili
            _db.Update(company);
        }
    }
}
