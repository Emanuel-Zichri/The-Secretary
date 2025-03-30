namespace FinalProject.BL
{
    public class ScheduleSlot
    {
        public int SlotID { get; set; }
        public DateTime Date { get; set; }
        public int Slot { get; set; } // 1=בוקר, 2=צהריים וכו'
        public string Status { get; set; }
        public int? RequestID { get; set; }
    }
}
