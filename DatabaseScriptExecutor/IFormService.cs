using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseScriptExecutor
{
    public interface IFormService
    {
        Task<IEnumerable<string>> GetDbNamesAsync(string connectionString);
        Task ExecuteSqlAsync(string connectionString, string sql);
    }
}
