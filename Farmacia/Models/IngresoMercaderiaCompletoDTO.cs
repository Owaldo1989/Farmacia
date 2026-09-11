namespace Farmacia.Models
{
    public class IngresoMercaderiaCompletoDTO
    {
        public IngresoMercaderiaDTO Ingreso { get; set; }
           = new();

        public List<IngresoMercaderiaDetalleDTO> Detalles { get; set; }
            = new();
    }

}
