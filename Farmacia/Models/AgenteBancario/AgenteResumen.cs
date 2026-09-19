namespace Farmacia.Models.AgenteBancario
{
    public class AgenteResumen
    {
        public decimal SaldoActual { get; set; }

        public decimal EntradasEfectivoHoy { get; set; }

        public decimal SalidasEfectivoHoy { get; set; }

        public decimal ComisionHoy { get; set; }

        public int OperacionesHoy { get; set; }
    }
}
