using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertBulkSqlExample
{
    class Program
    {
        public const string StrConexao = @"Server=.\SQLEXPRESS;Database=ClienteBulk;Trusted_Connection=True;";
        private readonly string ScriptCriacaoTabelaTeste = @"CREATE TABLE [dbo].[Cliente](
                                                            [Id][int] NOT NULL IDENTITY(1,1),
	                                                        [Nome] [nvarchar] (50) NULL,
	                                                        [SobreNome] [nvarchar] (50) NULL,
	                                                        [Idade] [int] NULL,
	                                                        [Logradouro] [nvarchar] (50) NULL,
	                                                        [Cidade] [nvarchar] (50) NULL,
	                                                        [Numero] [nvarchar] (50) NULL,
	                                                        [Estado] [nvarchar] (50) NULL,
                                                         CONSTRAINT[PK_Cliente] PRIMARY KEY CLUSTERED
                                                        (
                                                           [Id] ASC
                                                        )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
                                                        ) ON[PRIMARY]
                                                        GO";
        static void Main(string[] args)
        {
            var listaMock = Cliente.GetLista(1000000);
            var listaMockMap = listaMock.Select(cliente => (ClienteBulkInsert)cliente);

            using (var conexao = new SqlConnection(StrConexao))
            {
                conexao.Open();
                using (SqlTransaction transacao = conexao.BeginTransaction())
                {
                    try
                    {
                        InsertBulkSqlExample(conexao, transacao, "Cliente", listaMockMap);
                        transacao.Commit();
                    }
                    catch (Exception)
                    {
                        transacao.Rollback();
                    }
                }
            }
        }

        public static bool InsertBulkSqlExample<T>(SqlConnection conexao, SqlTransaction transacao, string nomeTabela, IEnumerable<T> lista)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var bulkSql = new SqlBulkCopy(conexao,
                SqlBulkCopyOptions.Default, transacao))
            {
                using (var enumerator = lista.GetEnumerator())
                {
                    using (var dataReader = new ObjectDataReader<T>(enumerator))
                    {
                        bulkSql.DestinationTableName = nomeTabela;
                        var nomesParametros = typeof(T).GetProperties().Select(pr => pr.Name);
                        foreach (var parametro in nomesParametros)
                        {
                            bulkSql.ColumnMappings.Add(sourceColumn: parametro, destinationColumn: parametro);
                        }

                        bulkSql.EnableStreaming = true;
                        bulkSql.BatchSize = 10000;
                        bulkSql.NotifyAfter = 1000;
                        bulkSql.SqlRowsCopied += (sender, e) => Debug.WriteLine("Quantidade de linhas inseridas: " + e.RowsCopied);

                        try
                        {
                            bulkSql.WriteToServer(dataReader);
                            
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"InsertBulkSqlExample {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            stopwatch.Stop();
            Debug.WriteLine("Temo decorrido {0} ms", stopwatch.ElapsedMilliseconds);
            return true;
        }
    }
}
