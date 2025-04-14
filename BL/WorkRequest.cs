namespace FinalProject.BL
{
    public class WorkRequest
    {
        public int RequestID { get; set; }
        public int CustomerID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }

        public static int Register(int costumerID)
        {
            DBservices db = new DBservices();
            try
            {
                return db.InsertWorkRequest(costumerID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting WorkRequest: {ex.Message}");
                return 0;
            }
        }

    }

}
