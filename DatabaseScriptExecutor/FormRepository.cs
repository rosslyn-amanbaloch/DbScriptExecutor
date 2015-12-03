using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseScriptExecutor
{
    public class FormRepository : IFormRepository
    {
        public async Task<IEnumerable<string>> GetDbNamesAsync(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}
