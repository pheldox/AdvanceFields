using AdvanceFields.Models;
using AdvanceFields.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Channels;
using System;
using Dapper;
using Microsoft.CodeAnalysis.Classification;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Microsoft.Extensions.Logging;

namespace AdvanceFields.SqlRepository
{
    public class AdvanceSqlRepository:ITranslation
    {
        protected readonly IConfiguration _configuration;
        protected string _SQLServer = string.Empty;
        private readonly ILogger<AdvanceSqlRepository> _logger;




        public AdvanceSqlRepository(IConfiguration configuration, ILogger<AdvanceSqlRepository> logger) {
            _configuration = configuration;
            _SQLServer = configuration.GetConnectionString("SqlTranslationConnection");
            _logger = logger;
        }
        public string Name => "SqlAdvance";

  
        public void SaveTranslation(RqTranslate rqTranslate)
        {
            try
            {
                using(IDbConnection db = new SQLiteConnection(_SQLServer))
                {
                    string sql = @"INSERT INTO Translations([Text],[Translated]) Values(@Text,@Translated)";
                    db.Execute(sql, new { Text = rqTranslate.Text, Translated = rqTranslate.Translated });
                }
            }catch(Exception ex)
            {
                throw;
            }
        }

        public List<RqTranslate> LoadTranslation()
        {
            try
            {
                using (IDbConnection db = new SQLiteConnection(_SQLServer))
                {
                    string sql = @"SELECT * FROM Translations";
                  
                  return db.Query<RqTranslate>(sql).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
