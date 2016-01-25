using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseScriptExecutor
{
    public class FormService : IFormService
    {
        private readonly IFormRepository _formRepository;
        public FormService(IFormRepository formRepository)
        {
            _formRepository = formRepository;
        }

        public async Task<IEnumerable<string>> GetDbNamesAsync(string connectionString)
        {
            return await _formRepository.GetDbNamesAsync(connectionString);
        }

        public Task ExecuteSqlAsync(string connectionString, string dbName, string sql)
        {
            return _formRepository.ExecuteSqlAsync(connectionString, dbName, sql);
        }
    }
}
