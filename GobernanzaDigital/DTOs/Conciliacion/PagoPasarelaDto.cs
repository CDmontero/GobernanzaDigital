namespace GobernanzaDigital.DTOs
{
    // Un pago como lo devuelve PayLink en GET /api/payments?invoiceId=...
    // El JSON viene en camelCase.
    public class PagoPasarelaDto
    {
        public int Id { get; set; }
        public string InvoiceId { get; set; } = "";   // el CodigoPago de la boleta
        public decimal Amount { get; set; }
        public string State { get; set; } = "";       // "APPROVED", "REJECTED" o "PENDING"
        public DateTime? PaymentDate { get; set; }
    }
}
