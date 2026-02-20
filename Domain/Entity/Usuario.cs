namespace Domain.Entity
{
    public class Usuario : EntityBase
    {
        public string Nome { get; set; }
        public DateTime DataDeNascimento { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
