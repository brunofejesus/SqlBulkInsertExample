using InsertBulkSqlExample;

namespace InsertBulkSqlExample
{
    public class ClienteBulkInsert
    {
        public string Nome { get; set; }
        public string SobreNome { get; set; }
        public int Idade { get; set; }
        public string Logradouro { get; set; }
        public string Cidade { get; set; }
        public string Numero { get; set; }
        public string Estado { get; set; }

        public static implicit operator ClienteBulkInsert(Cliente cliente)
        {
            return new ClienteBulkInsert()
            {
                Nome = cliente.Nome,
                SobreNome = cliente.SobreNome,
                Estado = cliente.Endereco.Estado,
                Cidade = cliente.Endereco.Cidade,
                Logradouro = cliente.Endereco.Logradouro,
                Numero = cliente.Endereco.Numero,
                Idade = cliente.Idade
            };
        }
    }
}
