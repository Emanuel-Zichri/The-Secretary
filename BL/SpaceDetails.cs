namespace FinalProject.BL
{
    public class SpaceDetails
    {
        public int SpaceID { get; set; }
        public int RequestID { get; set; }
        public decimal? Size { get; set; }
        public string FloorType { get; set; }
        public string MediaURL { get; set; }
        public string Notes { get; set; }
        public string ParquetType { get; set; }

        public SpaceDetails() { }

        public static int Register(int workrequestID,SpaceDetails space)
        {
            DBservices db = new DBservices();
            try
            {
                return db.InsertSpaceDetails(workrequestID,space);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting space details: {ex.Message}");
                return 0;
            }
        }


    }

}
