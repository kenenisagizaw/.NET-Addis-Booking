namespace AddisBookingAdmin.Models;

// ViewModel for error information
public class ErrorViewModel
{
    // The request identifier
    public string? RequestId { get; set; }

    // Indicates if the RequestId should be shown
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
