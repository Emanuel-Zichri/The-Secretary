namespace FinalProject.BL
{
    public class WorkRequest
    {
        public int RequestID { get; set; }
        public int CustomerID { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? PreferredSlot { get; set; }

        public static int Register(int costumerID, DateTime PreferredDate,int PreferredSlot)
        {
            DBservices db = new DBservices();
            try
            {
                return db.InsertWorkRequest(costumerID, PreferredDate, PreferredSlot);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting WorkRequest: {ex.Message}");
                return 0;
            }
        }
        public static int UpdateStatus(int workRequestID, string workRequestNewStatus)
        {
            DBservices db = new DBservices();
            try
            {
                return db.UpdateWorkRequestStatus(workRequestID, workRequestNewStatus);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating WorkRequest status: {ex.Message}");
                return 0;
            }
        }
        public static int UpdateTimeWhenJobCompleted(int customerID)
        {
            DBservices db = new DBservices();
            try
            {
                return db.UpdateWorkRequestTimeWhenJobCompleted(customerID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating WorkRequest completion time: {ex.Message}");
                return 0;
            }
        }

    }

}
