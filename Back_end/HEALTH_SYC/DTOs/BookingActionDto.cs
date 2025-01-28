public class BookingActionDto
{
    public int BookingId { get; set; }
    public string Action { get; set; } // Accept, Reject, Reschedule, Postpone
    public DateTime? NewDate { get; set; } // Required for Reschedule
    public string? Reason { get; set; } // Optional for Reject or Postpone
}
