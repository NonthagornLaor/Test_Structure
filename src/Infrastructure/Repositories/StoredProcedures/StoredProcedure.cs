using Application.Interfaces.Repositories.StoredProcedures;
using Domain.Entities.web;
using Infrastructure.ContractDbs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Domain.Entities.api;

namespace Infrastructure.Repositories.StoredProcedures
{
    public class StoredProcedure : IStoredProcedure
    {
        private readonly DbsContext _context;

        public StoredProcedure(DbsContext context)
        {
            _context = context;
        }

        
    }
}
