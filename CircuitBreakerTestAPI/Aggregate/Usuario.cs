namespace CircuitBreakerTestAPI.Aggregate
{
    public class Usuario : IAggregateRoot
    {
        public string Nome { get; set; }
        public int Idade { get; set; }
    }
}
