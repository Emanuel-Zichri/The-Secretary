using System.Security.Cryptography.X509Certificates;

namespace FinalProject.BL
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? Notes { get; set; }

        public Customer() { }

        public static int Register(Customer newCustomer)
        {
            DBservices dbServices = new DBservices();
            try
            {
                int result = dbServices.Register(newCustomer);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return 0;
            }
            
        }
        public static int UpdateCustomer(Customer customer)
        {
            DBservices dbServices = new DBservices();
            try
            {
                int result = dbServices.UpdateCustomer(customer);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
                return 0;
            }
        }
    }
}
