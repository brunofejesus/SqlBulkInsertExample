using System.Collections.Generic;
using System.Linq;

namespace InsertBulkSqlExample
{
    public class Cliente
    {
        public string Nome { get; set; }
        public string SobreNome { get; set; }
        public int Idade { get; set; }
        public Endereco Endereco { get; set; }

        public static IEnumerable<Cliente> GetLista(int tamanho)
        {
            return Enumerable.Range(1, tamanho).Select(index => new Cliente
            {
                Nome = $"Cliente {index}",
                SobreNome = $"Cliente Sobrenome {index}",
                Endereco = new Endereco()
                {
                    Cidade = $"Cidade {index}",
                    Estado = $"SP",
                    Logradouro = $"Rua das pedras n{index}",
                    Numero = $"{index}"
                },
                Idade = index
            });
        }
    }
}
