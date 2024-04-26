using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.api;
using Domain.Entities.web;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ContractDbs
{
    public partial class DbsContext : DbContext
    {
        public DbsContext()
        {
        }

        public DbsContext(DbContextOptions<DbsContext> options)
            : base(options)
        {
        }
    }
}
