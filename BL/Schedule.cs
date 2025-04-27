namespace FinalProject.BL
{
    public class Schedule
    {
        public static int AssignRequestToSlot(ScheduleSlotAssignment assignment)
        {
            DBservices db = new DBservices();
            try
            {
                return db.AssignRequestToSlot(assignment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BL AssignRequestToSlot: {ex.Message}");
                return 0;
            }
        }
    }
}
