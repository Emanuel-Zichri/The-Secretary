namespace FinalProject.BL
{
    public class CustomerDetailsBL
    {
        private readonly DBservices _db;

        public CustomerDetailsBL()
        {
            _db = new DBservices();
        }

        /// <summary>
        /// קבלת כל פרטי הלקוח כולל בקשות עבודה וחללים
        /// </summary>
        public CustomerDetailsResponse GetCustomerDetails(int customerID)
        {
            try
            {
                // קבלת נתוני הלקוח מהדשבורד
                var dashboardFilter = new DashboardFilterDto { CustomerID = customerID };
                var dashboardData = _db.GetDashboardData(dashboardFilter);
                
                if (dashboardData == null || !((List<object>)dashboardData).Any())
                {
                    return null;
                }

                var data = (List<object>)dashboardData;
                var firstItem = (Dictionary<string, object>)data.First();

                var response = new CustomerDetailsResponse
                {
                    Customer = MapToCustomer(firstItem),
                    WorkRequest = MapToWorkRequest(firstItem),
                    SpaceDetails = MapToSpaceDetails(data),
                    TotalArea = CalculateTotalArea(data)
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting customer details: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// עדכון פרטי לקוח
        /// </summary>
        public bool UpdateCustomerDetails(int customerID, Customer customer, List<SpaceDetails> spaces)
        {
            try
            {
                customer.CustomerID = customerID;
                int customerResult = _db.UpdateCustomer(customer);
                
                bool spacesResult = true;
                if (spaces != null && spaces.Any())
                {
                    foreach (var space in spaces)
                    {
                        int result = _db.UpdateSpaceDetails(space);
                        if (result == 0) spacesResult = false;
                    }
                }

                return customerResult > 0 && spacesResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer details: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// עדכון סטטוס בקשת עבודה
        /// </summary>
        public bool UpdateWorkRequestStatus(int requestID, string newStatus)
        {
            try
            {
                int result = _db.UpdateWorkRequestStatus(requestID, newStatus);
                
                // אם הסטטוס הוא "התקנה בוצעה" - עדכן תאריך השלמה
                if (newStatus == "התקנה בוצעה")
                {
                    var workRequest = _db.GetWorkRequestByID(requestID);
                    if (workRequest != null)
                    {
                        _db.UpdateWorkRequestTimeWhenJobCompleted(workRequest.CustomerID);
                    }
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating work request status: {ex.Message}");
                return false;
            }
        }

        private Customer MapToCustomer(Dictionary<string, object> data)
        {
            return new Customer
            {
                CustomerID = (int)data["CustomerID"],
                FirstName = data["FirstName"]?.ToString(),
                LastName = data["LastName"]?.ToString(),
                Phone = data["Phone"]?.ToString(),
                Email = data["Email"]?.ToString(),
                City = data["City"]?.ToString(),
                Street = data["Street"]?.ToString(),
                Number = data["Number"]?.ToString(),
                Notes = data["Notes"]?.ToString()
            };
        }

        private WorkRequest MapToWorkRequest(Dictionary<string, object> data)
        {
            return new WorkRequest
            {
                RequestID = (int)data["RequestID"],
                CustomerID = (int)data["CustomerID"],
                Status = data["Status"]?.ToString(),
                PlannedDate = data["PlannedDate"] as DateTime?,
                CompletedDate = data["CompletedDate"] as DateTime?,
                PreferredDate = data["PreferredDate"] as DateTime?,
                PreferredSlot = data["PreferredSlot"] as int?
            };
        }

        private List<SpaceDetails> MapToSpaceDetails(List<object> data)
        {
            var spaces = new List<SpaceDetails>();
            
            foreach (var item in data)
            {
                var dict = (Dictionary<string, object>)item;
                if (dict.ContainsKey("SpaceID") && dict["SpaceID"] != null)
                {
                    spaces.Add(new SpaceDetails
                    {
                        SpaceID = (int)dict["SpaceID"],
                        RequestID = (int)dict["RequestID"],
                        Size = dict["Size"] as decimal?,
                        FloorType = dict["FloorType"]?.ToString(),
                        ParquetType = dict["ParquetType"]?.ToString(),
                        Notes = dict["SpaceNotes"]?.ToString()
                    });
                }
            }

            return spaces;
        }

        private decimal CalculateTotalArea(List<object> data)
        {
            decimal total = 0;
            foreach (var item in data)
            {
                var dict = (Dictionary<string, object>)item;
                if (dict.ContainsKey("Size") && dict["Size"] != null)
                {
                    if (decimal.TryParse(dict["Size"].ToString(), out decimal size))
                    {
                        total += size;
                    }
                }
            }
            return total;
        }
    }

    // מחלקות עזר
    public class CustomerDetailsResponse
    {
        public Customer Customer { get; set; }
        public WorkRequest WorkRequest { get; set; }
        public List<SpaceDetails> SpaceDetails { get; set; }
        public decimal TotalArea { get; set; }
    }
} 