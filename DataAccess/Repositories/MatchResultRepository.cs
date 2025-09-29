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
    public class MatchResultRepository : Repository<MatchResult>, IMatchResultRepository
    {
        private readonly ApplicationDbContext _context;
        public MatchResultRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
