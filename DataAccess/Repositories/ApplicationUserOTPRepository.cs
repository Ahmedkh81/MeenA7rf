using DataAccess.Data;
using DataAccess.Repositories.IRepositories;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ApplicationUserOTPRepository : Repository<ApplicationUserOTP>, IApplicationUserOTPRepository
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUserOTPRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
