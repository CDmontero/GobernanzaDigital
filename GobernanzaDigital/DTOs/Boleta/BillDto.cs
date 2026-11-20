using GobernanzaDigital.Models;

namespace GobernanzaDigital.DTOs
{
    // Lo que le devolvemos a PayLink cuando consulta una factura: GET /bills/{billId}
    public class BillDto
    {
        public int Id { get; set; }
        public string BillId { get; set; } = "";       // el CodigoPago de la boleta
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Period { get; set; } = "";
        public string ServiceName { get; set; } = "";
        public EstadoBoleta Status { get; set; }
    }
}
