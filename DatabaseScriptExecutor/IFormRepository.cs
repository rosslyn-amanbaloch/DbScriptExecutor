using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseScriptExecutor
{
    public interface IFormRepository
    {
        Task<IEnumerable<string>> GetDbNamesAsync(string connectionString);
    }
}
