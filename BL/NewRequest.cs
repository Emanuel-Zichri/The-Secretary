namespace FinalProject.BL
{
    public class NewRequest
    {
        public Customer customerDetails { get; set; }
        public SpaceDetails [] spaceDetails  { get; set; }

        public int Insert()
        {
           int returnedID = Customer.Register(customerDetails);
            DBservices db = new DBservices();
            try
            {
               
                    foreach (var space in spaceDetails)
                    {
                        db.InsertSpaceDetails(space, returnedID);
                    }
                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting new request: {ex.Message}");
                return 0;
            }
        }

    }
    
}

