using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using FinalProject;
using FinalProject.BL;
using System.Security.Cryptography.X509Certificates;


/// <summary>
/// DBServices is a class created by me to provides some DataBase Services
/// </summary>
public class DBservices
{

    public DBservices() { }

    //--------------------------------------------------------------------------------------------------
    // This method creates a connection to the database according to the connectionString name in the web.config 
    //--------------------------------------------------------------------------------------------------
    public SqlConnection connect(String conString)
    {

        // read the connection string from the configuration file
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json").Build();
        string cStr = configuration.GetConnectionString("myProjDB");
        SqlConnection con = new SqlConnection(cStr);
        con.Open();
        return con;
    }

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureGeneral(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }
    //--------------------------------------------------------------------------------------------------
    // This method Register new client to the database 
    //--------------------------------------------------------------------------------------------------
    public int Register(Customer newCustomer)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@FirstName", newCustomer.FirstName);
        paramDic.Add("@LastName", newCustomer.LastName);
        paramDic.Add("@Phone", newCustomer.Phone);
        paramDic.Add("@Email", newCustomer.Email);
        paramDic.Add("@City", newCustomer.City);
        paramDic.Add("@Street", newCustomer.Street);
        paramDic.Add("@Number", newCustomer.Number);
        paramDic.Add("@Notes", newCustomer.Notes);

        cmd = CreateCommandWithStoredProcedureGeneral("InsertCustomer", con, paramDic); // create the command

        try
        {
            // Using ExecuteScalar to fetch the returned new customer id from the stored procedure
            object result = cmd.ExecuteScalar();
            int newCustomerID = Convert.ToInt32(result);
            return newCustomerID;
        }
        catch (Exception ex)
        {
            // write to log
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method insert space details 
    //--------------------------------------------------------------------------------------------------
    public int InsertSpaceDetails(int workrequestID, SpaceDetails space)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>()
    {
        { "@RequestID", workrequestID },
        { "@Size", space.Size },
        { "@FloorType", space.FloorType },
        { "@MediaURL", space.MediaURL },
        { "@Notes", space.Notes },
        { "@ParquetType", space.ParquetType }
    };

        cmd = CreateCommandWithStoredProcedureGeneral("InsertSpaceDetails", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method insert work request
    //--------------------------------------------------------------------------------------------------
    public int InsertWorkRequest(int costumerID, DateTime PreferredDate, int PreferredSlot)
    {
        Console.WriteLine($"🔄 DBservices.InsertWorkRequest נקרא עם CustomerID: {costumerID}");
        
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
            Console.WriteLine("✅ חיבור למסד נתונים הצליח");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ שגיאה בחיבור למסד נתונים: {ex.Message}");
            throw ex;
        }

        // וידוא שהCustomerID קיים
        try
        {
            var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Customer WHERE CustomerID = @CustomerID", con);
            checkCmd.Parameters.AddWithValue("@CustomerID", costumerID);
            int customerExists = (int)checkCmd.ExecuteScalar();
            
            if (customerExists == 0)
            {
                Console.WriteLine($"❌ CustomerID {costumerID} לא קיים בטבלת Customer");
                throw new Exception($"CustomerID {costumerID} לא קיים בטבלת Customer");
            }
            Console.WriteLine($"✅ CustomerID {costumerID} נמצא בטבלה");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ שגיאה בבדיקת CustomerID: {ex.Message}");
            throw ex;
        }

        cmd = new SqlCommand("InsertWorkRequest", con);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@CustomerID", costumerID);
        cmd.Parameters.AddWithValue("@PreferredDate", PreferredDate);
        cmd.Parameters.AddWithValue("@PreferredSlot", PreferredSlot);

        SqlParameter outputIdParam = new SqlParameter("@RequestID", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(outputIdParam);

        Console.WriteLine($"📊 פרמטרים לפרוצדורה InsertWorkRequest:");
        Console.WriteLine($"   @CustomerID: {costumerID}");
        Console.WriteLine($"   @PreferredDate: {PreferredDate}");
        Console.WriteLine($"   @PreferredSlot: {PreferredSlot}");

        try
        {
            Console.WriteLine("🚀 מפעיל פרוצדורה InsertWorkRequest...");
            
            // שימוש ב-ExecuteScalar כדי לקבל גם את הSELECT וגם לטפל ב-OUTPUT
            object scalarResult = cmd.ExecuteScalar();
            Console.WriteLine($"📊 ExecuteScalar תוצאה: {scalarResult}");
            
            // בדיקת פרמטר OUTPUT
            Console.WriteLine($"📊 OUTPUT Parameter Value: {outputIdParam.Value}");
            Console.WriteLine($"📊 OUTPUT Parameter Type: {outputIdParam.Value?.GetType()}");
            
            int newRequestId = 0;
            
            // ניסיון לקבל RequestID מהפרמטר OUTPUT
            if (outputIdParam.Value != null && outputIdParam.Value != DBNull.Value)
            {
                newRequestId = (int)outputIdParam.Value;
                Console.WriteLine($"✅ RequestID מפרמטר OUTPUT: {newRequestId}");
            }
            // אם OUTPUT לא עובד, ננסה מהSELECT
            else if (scalarResult != null && scalarResult != DBNull.Value)
            {
                newRequestId = Convert.ToInt32(scalarResult);
                Console.WriteLine($"✅ RequestID מ-ExecuteScalar: {newRequestId}");
            }
            else
            {
                Console.WriteLine("❌ גם OUTPUT וגם ExecuteScalar החזירו NULL");
                throw new Exception("פרוצדורת InsertWorkRequest לא החזירה RequestID תקף");
            }
            
            if (newRequestId <= 0)
            {
                Console.WriteLine($"❌ RequestID לא תקף: {newRequestId}");
                throw new Exception($"RequestID לא תקף: {newRequestId}");
            }
            
            Console.WriteLine($"✅ פרוצדורה הושלמה, RequestID שנוצר: {newRequestId}");
            
            // בדיקה נוספת שהRequestID אכן נוצר במסד הנתונים
            var verifyCmd = new SqlCommand("SELECT COUNT(*) FROM WorkRequest WHERE RequestID = @RequestID", con);
            verifyCmd.Parameters.AddWithValue("@RequestID", newRequestId);
            int requestExists = (int)verifyCmd.ExecuteScalar();
            
            if (requestExists == 0)
            {
                Console.WriteLine($"❌ RequestID {newRequestId} לא נמצא במסד הנתונים!");
                throw new Exception($"RequestID {newRequestId} לא נמצא במסד הנתונים");
            }
            
            Console.WriteLine($"✅ RequestID {newRequestId} אומת במסד הנתונים");
            return newRequestId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ שגיאה בביצוע פרוצדורה InsertWorkRequest: {ex.Message}");
            Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method Read from DB to dashboard or to specific customer
    //--------------------------------------------------------------------------------------------------
    public object GetDashboardData(DashboardFilterDto filter)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw (ex);
        }

        List<object> dashboardTable = new List<object>();

        try
        {
            // יצירת מילון של פרמטרים
            Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@CustomerID", (object?)filter.CustomerID ?? DBNull.Value },
            { "@City", (object?)filter.City ?? DBNull.Value },
            { "@FromDate", (object?)filter.FromDate ?? DBNull.Value },
            { "@ToDate", (object?)filter.ToDate ?? DBNull.Value },
            { "@FloorType", (object?)filter.FloorType ?? DBNull.Value },
            { "@Status", (object?)filter.Status ?? DBNull.Value }
        };

            // יצירת פקודה עם הפרוצדורה והפרמטרים
            cmd = CreateCommandWithStoredProcedureGeneral("GetDashboardData", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                var row = new Dictionary<string, object>
    {
        { "CustomerID", reader["CustomerID"] },
        { "FirstName", reader["FirstName"].ToString() },
        { "LastName", reader["LastName"].ToString() },
        { "Phone", reader["Phone"].ToString() },
        { "City", reader["City"].ToString() },
        { "Street", reader["Street"].ToString() },
        { "Number", reader["Number"].ToString() },
        { "Notes", reader["Notes"] != DBNull.Value ? reader["Notes"].ToString() : null },
        { "CustomerCreatedAt", reader["CustomerCreatedAt"] },
        { "Email", reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null },

        { "RequestID", reader["RequestID"] },
        { "PlannedDate", reader["PlannedDate"] != DBNull.Value ? (DateTime?)reader["PlannedDate"] : null },
        { "CompletedDate", reader["CompletedDate"] != DBNull.Value ? (DateTime?)reader["CompletedDate"] : null },
        { "PreferredDate", reader["PreferredDate"] != DBNull.Value ? (DateTime?)reader["PreferredDate"] : null },
        { "PreferredSlot", reader["PreferredSlot"] != DBNull.Value ? (int?)Convert.ToInt32(reader["PreferredSlot"]) : null },
        { "Status", reader["Status"].ToString() },

        // עבור לקוח ספציפי ישמש לבניית כרטיס הלקוח בהמשך-ייתכן ונצטרך לעדכן לינק לסרטון והערות
        { "SpaceID", ColumnExists(reader, "SpaceID") ? reader["SpaceID"] : null },
        { "Size", ColumnExists(reader, "Size") ? reader["Size"] : null },
        { "FloorType", ColumnExists(reader, "FloorType") ? reader["FloorType"].ToString() : null },
        { "ParquetType", ColumnExists(reader, "ParquetType") ? reader["ParquetType"].ToString() : null },
        { "SpaceNotes", ColumnExists(reader, "SpaceNotes") ? reader["SpaceNotes"].ToString() : null },
        { "MediaURL", ColumnExists(reader, "MediaURL") ? reader["MediaURL"].ToString() : null },
    


        // עבור תצוגה מקובצת- כלומר לא נשלח מזהה לקוח
        { "SpaceCount", ColumnExists(reader, "SpaceCount") ? reader["SpaceCount"] : null },
        { "TotalSpaceSize", ColumnExists(reader, "TotalSpaceSize") ? reader["TotalSpaceSize"] : null },
        { "FloorTypes", ColumnExists(reader, "FloorTypes") ? reader["FloorTypes"].ToString() : null }

    };

                dashboardTable.Add(row);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }

        return dashboardTable;


    }

    public List<string> GetVideosByCustomerID(int CustomerID)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<string> videoLinks = new List<string>();

        try
        {
            con = connect("myProjDB"); // מחבר למסד הנתונים שלך
        }
        catch (Exception ex)
        {
            throw ex; // אפשר להוסיף כאן לוג אם תרצה
        }

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@CustomerID", CustomerID }
        };

            cmd = CreateCommandWithStoredProcedureGeneral("GetVideosByCustomerID", con, paramDic); // צור את הפקודה

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                string mediaUrl = reader["MediaURL"] != DBNull.Value ? reader["MediaURL"].ToString() : null;
                if (!string.IsNullOrEmpty(mediaUrl))
                {
                    videoLinks.Add(mediaUrl);
                }
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex; // אפשר להחליף ב־Console.WriteLine או לוג מתאים
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }

        return videoLinks;
    }



    private bool ColumnExists(SqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
    //--------------------------------------------------------------------------------------------------
    // This method add new calculator item
    //--------------------------------------------------------------------------------------------------
    public int AddCalcItem(PriceCalculatorItem newItem)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@ItemName", newItem.ItemName);
        paramDic.Add("@Description", newItem.Description);
        paramDic.Add("@Price", newItem.Price);
        cmd = CreateCommandWithStoredProcedureGeneral("AddPriceCalculatorItem", con, paramDic); // create the command
        try
        {
            int newcalcID = cmd.ExecuteNonQuery();
            return newcalcID;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method delete calculator item
    //--------------------------------------------------------------------------------------------------
    public int DeleteCalcItem(PriceCalculatorItem itemID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@CalculatorItemID", itemID.CalculatorItemID);
        cmd = CreateCommandWithStoredProcedureGeneral("DeletePriceCalculatorItem", con, paramDic); // create the command
        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    public int UpdateCalcItem(PriceCalculatorItem item)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@CalculatorItemID", item.CalculatorItemID);
        paramDic.Add("@ItemName", item.ItemName);
        paramDic.Add("@Description", item.Description);
        paramDic.Add("@Price", item.Price);
        paramDic.Add("@IsActive", item.IsActive);
        cmd = CreateCommandWithStoredProcedureGeneral("UpdatePriceCalculatorItem", con, paramDic); // create the command
        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method gets all calculator items
    //--------------------------------------------------------------------------------------------------
    public List<PriceCalculatorItem> GetCalculatorItems()
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        List<PriceCalculatorItem> items = new List<PriceCalculatorItem>();
        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            cmd = CreateCommandWithStoredProcedureGeneral("GetAllPriceCalculatorItems", con, paramDic); // create the command
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                PriceCalculatorItem item = new PriceCalculatorItem
                {
                    CalculatorItemID = Convert.ToInt32(reader["CalculatorItemID"]),
                    ItemName = reader["ItemName"].ToString(),
                    Description = reader["Description"].ToString(),
                    Price = Convert.ToDecimal(reader["Price"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                };
                items.Add(item);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
        return items;
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets item suggestions for autocomplete
    //--------------------------------------------------------------------------------------------------
    public List<ItemSuggestion> GetItemSuggestions(string searchText, int maxResults = 10)
    {
        SqlConnection con;
        SqlCommand cmd;
        
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        List<ItemSuggestion> suggestions = new List<ItemSuggestion>();

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@SearchText", searchText);
        paramDic.Add("@MaxResults", maxResults);

        cmd = CreateCommandWithStoredProcedureGeneral("GetItemSuggestions", con, paramDic);

        try
        {
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                ItemSuggestion suggestion = new ItemSuggestion()
                {
                    SuggestionText = dataReader["SuggestionText"].ToString(),
                    Source = dataReader["Source"].ToString(),
                    SuggestedPrice = Convert.ToDecimal(dataReader["SuggestedPrice"]),
                    Priority = Convert.ToInt32(dataReader["Priority"])
                };
                suggestions.Add(suggestion);
            }
            dataReader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return suggestions;
    }

    //--------------------------------------------------------------------------------------------------
    // This method updates the customer details
    //--------------------------------------------------------------------------------------------------
    public int UpdateCustomer(Customer customer)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@CustomerID", customer.CustomerID);
        paramDic.Add("@FirstName", customer.FirstName);
        paramDic.Add("@LastName", customer.LastName);
        paramDic.Add("@Phone", customer.Phone);
        paramDic.Add("@Email", customer.Email);
        paramDic.Add("@City", customer.City);
        paramDic.Add("@Street", customer.Street);
        paramDic.Add("@Number", customer.Number);
        paramDic.Add("@Notes", customer.Notes);
        cmd = CreateCommandWithStoredProcedureGeneral("UpdateCustomerDetails", con, paramDic); // create the command
        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method updates the spaces details
    //--------------------------------------------------------------------------------------------------
    public int UpdateSpaceDetails(SpaceDetails space)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@SpaceID", space.SpaceID);
        paramDic.Add("@Size", space.Size);
        paramDic.Add("@FloorType", space.FloorType);
        paramDic.Add("@MediaURL", space.MediaURL);
        paramDic.Add("@Notes", space.Notes);
        paramDic.Add("@ParquetType", space.ParquetType);
        cmd = CreateCommandWithStoredProcedureGeneral("UpdateSpaceDetails", con, paramDic); // create the command
        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method deactivate a specific user 
    //--------------------------------------------------------------------------------------------------
    public int DeactivateCustomer(int customerID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@CustomerID", customerID);
        cmd = CreateCommandWithStoredProcedureGeneral("DeactivateCustomer", con, paramDic);
        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method reactivate a specific user 
    //--------------------------------------------------------------------------------------------------
    public int ReactivateCustomer(int customerID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@CustomerID", customerID);
        cmd = CreateCommandWithStoredProcedureGeneral("ReactivateCustomer", con, paramDic);
        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method updates customer status
    //--------------------------------------------------------------------------------------------------
    public int UpdateCustomerStatus(int customerID, string newStatus)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@CustomerID", customerID);
        paramDic.Add("@NewStatus", newStatus);
        cmd = CreateCommandWithStoredProcedureGeneral("UpdateCustomerStatus", con, paramDic);
        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method insert a new Quote
    //--------------------------------------------------------------------------------------------------
    public int InsertQuote(Quote quote)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@RequestID", quote.RequestID);
        paramDic.Add("@TotalPrice", quote.TotalPrice);
        paramDic.Add("@DiscountAmount", quote.DiscountAmount ?? (object)DBNull.Value);
        paramDic.Add("@DiscountPercent", quote.DiscountPercent ?? (object)DBNull.Value);
        cmd = CreateCommandWithStoredProcedureGeneral("AddQuote", con, paramDic); // create the command
        try
        {
            object result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method add quoteitem 
    //--------------------------------------------------------------------------------------------------
    public int AddQuoteItem(QuoteItem item)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>
    {
        { "@QuoteID", item.QuoteID },
        { "@CalculatorItemID", item.CalculatorItemID ?? (object)DBNull.Value },
        { "@CustomItemName", string.IsNullOrWhiteSpace(item.CustomItemName) ? DBNull.Value : item.CustomItemName },
        { "@PriceForItem", item.PriceForItem },
        { "@Quantity", item.Quantity },
        { "@FinalPrice", item.FinalPrice },
        { "@Notes", string.IsNullOrWhiteSpace(item.Notes) ? DBNull.Value : item.Notes }
    };
        cmd = CreateCommandWithStoredProcedureGeneral("AddQuoteItem", con, paramDic); // create the command
        try
        {
            object result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method return quotes for a specific customer
    //--------------------------------------------------------------------------------------------------
    public List<QuoteItemExtended> GetQuoteItemExtendedByCustomerID(int customerID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        List<QuoteItemExtended> items = new List<QuoteItemExtended>();

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@CustomerID", customerID }
        };

            cmd = CreateCommandWithStoredProcedureGeneral("GetQuoteDetailsByCustomerID", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                QuoteItemExtended item = new QuoteItemExtended
                {
                    QuoteItemID = Convert.ToInt32(reader["QuoteItemID"]),
                    QuoteID = Convert.ToInt32(reader["QuoteID"]),
                    RequestID = Convert.ToInt32(reader["RequestID"]),
                    CalculatorItemID = reader["CalculatorItemID"] != DBNull.Value ? (int?)Convert.ToInt32(reader["CalculatorItemID"]) : null,
                    CustomItemName = reader["CustomItemName"].ToString(),
                    PriceForItem = Convert.ToDecimal(reader["PriceForItem"]),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    FinalPrice = Convert.ToDecimal(reader["FinalPrice"]),
                    Notes = reader["Notes"]?.ToString(),

                    PlannedDate = reader["PlannedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["PlannedDate"]) : null,
                    CompletedDate = reader["CompletedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["CompletedDate"]) : null,
                    Status = reader["Status"].ToString(),

                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
                    DiscountAmount = reader["DiscountAmount"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["DiscountAmount"]) : null,
                    DiscountPercent = reader["DiscountPercent"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["DiscountPercent"]) : null,
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                };

                items.Add(item);
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return items;
    }

    //--------------------------------------------------------------------------------------------------
    // This method deletes all quote items for a specific quote
    //--------------------------------------------------------------------------------------------------
    public int DeleteQuoteItems(int quoteID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@QuoteID", quoteID }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("DeleteQuoteItems", con, paramDic);
            
            // Use ExecuteScalar to get the count returned by the stored procedure
            object result = cmd.ExecuteScalar();
            
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            
            return 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method deletes a quote
    //--------------------------------------------------------------------------------------------------
    public bool DeleteQuote(int quoteID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@QuoteID", quoteID }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("DeleteQuote", con, paramDic);
            
            // Use ExecuteScalar to get the count returned by the stored procedure
            object result = cmd.ExecuteScalar();
            
            if (result != null && result != DBNull.Value)
            {
                int deletedCount = Convert.ToInt32(result);
                return deletedCount > 0;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method add new parquetType 
    //--------------------------------------------------------------------------------------------------
    public int AddParquetType(ParquetType newParquet)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@TypeName", newParquet.TypeName);
        paramDic.Add("@PricePerUnit", newParquet.PricePerUnit);
        paramDic.Add("@ImageURL", newParquet.ImageURL);
        paramDic.Add("@Type", newParquet.Type);
        cmd = CreateCommandWithStoredProcedureGeneral("AddParquetType", con, paramDic); // create the command
        try
        {
            int newParquetID = cmd.ExecuteNonQuery();
            return newParquetID;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method updates parquetType 
    //--------------------------------------------------------------------------------------------------
    public int updateParquetType(ParquetType parquetType)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@ParquetTypeID", parquetType.ParquetTypeID);
        paramDic.Add("@TypeName", parquetType.TypeName);
        paramDic.Add("@PricePerUnit", parquetType.PricePerUnit);
        paramDic.Add("@ImageURL", parquetType.ImageURL);
        paramDic.Add("@Type", parquetType.Type);
        cmd = CreateCommandWithStoredProcedureGeneral("UpdateParquetType", con, paramDic); // create the command
        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method Read all parquetTypes
    //--------------------------------------------------------------------------------------------------
    public List<ParquetType> GetAllParquetTypes()
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        List<ParquetType> parquetTypes = new List<ParquetType>();
        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            cmd = CreateCommandWithStoredProcedureGeneral("GetAllParquetTypes", con, paramDic); // create the command
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                ParquetType p = new ParquetType
                {
                    ParquetTypeID = Convert.ToInt32(reader["ParquetTypeID"]),
                    TypeName = reader["TypeName"].ToString(),
                    PricePerUnit = Convert.ToDecimal(reader["PricePerUnit"]),
                    ImageURL = reader["ImageURL"].ToString(),
                    Type = reader["Type"] != DBNull.Value ? reader["Type"].ToString() : null,
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                };
                parquetTypes.Add(p);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
        return parquetTypes;
    }
    //--------------------------------------------------------------------------------------------------
    // This method soft delete parquetType 
    //--------------------------------------------------------------------------------------------------
    public int DeleteParquetType(int id)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@ParquetTypeID", id);
        cmd = CreateCommandWithStoredProcedureGeneral("DeleteParquetType", con, paramDic); // create the command
        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method Read all uncreated candidates
    //--------------------------------------------------------------------------------------------------
    public List<CalculatorItemCandidate> GetUncreatedPopularCandidates(int minOccurrences = 3)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        List<CalculatorItemCandidate> candidates = new List<CalculatorItemCandidate>();

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@MinOccurrences", minOccurrences }
        };

            cmd = CreateCommandWithStoredProcedureGeneral("GetUncreatedPopularCandidates", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                CalculatorItemCandidate candidate = new CalculatorItemCandidate
                {
                    CandidateID = Convert.ToInt32(reader["CandidateID"]),
                    CustomItemName = reader["CustomItemName"].ToString(),
                    SuggestedPrice = Convert.ToDecimal(reader["SuggestedPrice"]),
                    SuggestedDescription = reader["SuggestedDescription"].ToString(),
                    Occurrences = Convert.ToInt32(reader["Occurrences"])
                };

                candidates.Add(candidate);
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return candidates;
    }
    //--------------------------------------------------------------------------------------------------
    // This method creates a new calculator item from a candidate
    //--------------------------------------------------------------------------------------------------
    public int CreatePriceCalculatorItemFromCandidate(string itemName)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>
    {
        { "@CustomItemName", itemName }
    };
        cmd = CreateCommandWithStoredProcedureGeneral("CreatePriceCalculatorItemFromCandidate", con, paramDic); // create the command
        try
        {
            int result = cmd.ExecuteNonQuery();
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method rejects a candidate
    //--------------------------------------------------------------------------------------------------
    public int RejectCalculatorItemCandidate(string customItemName)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
    {
        { "@CustomItemName", customItemName }
    };

        cmd = CreateCommandWithStoredProcedureGeneral("RejectCalculatorItemCandidate", con, paramDic);

        try
        {
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method update work request status 
    //--------------------------------------------------------------------------------------------------
    public int UpdateWorkRequestStatus(int workRequestID, string workRequestNewStatus)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
    {
        { "@RequestID", workRequestID },
        { "@NewStatus", workRequestNewStatus }
    };

        cmd = CreateCommandWithStoredProcedureGeneral("UpdateWorkRequestStatus", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method Assign request to schedule 
    //--------------------------------------------------------------------------------------------------
    public int AssignRequestToSlot(ScheduleSlotAssignment assignment)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        try
        {
            // יצירת שורת סלוט חדש
            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@Date", assignment.Date);
            paramDic.Add("@Slot", assignment.Slot);
            paramDic.Add("@RequestID", assignment.RequestID);

            cmd = CreateCommandWithStoredProcedureGeneral("AssignRequestToSlot", con, paramDic);
            int numEffected = cmd.ExecuteNonQuery();

            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method update completed date for a work request
    //--------------------------------------------------------------------------------------------------
    public int UpdateWorkRequestTimeWhenJobCompleted(int customerID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>
    {
        { "@CustomerID", customerID }
    };
        cmd = CreateCommandWithStoredProcedureGeneral("SetCompletedTimeOnStatusChange", con, paramDic);
        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method sets completed time when status changes
    //--------------------------------------------------------------------------------------------------
    public int SetCompletedTimeOnStatusChange(int customerID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@CustomerID", customerID }
        };
        cmd = CreateCommandWithStoredProcedureGeneral("SetCompletedTimeOnStatusChange", con, paramDic);
        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }
    //--------------------------------------------------------------------------------------------------
    // This method saves price estimate to database
    //--------------------------------------------------------------------------------------------------
    public int SavePriceEstimate(PriceEstimator estimate)
    {
        Console.WriteLine($"🔄 DBservices.SavePriceEstimate נקרא עם RequestID: {estimate.RequestID}");
        
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
            Console.WriteLine("✅ חיבור למסד נתונים הצליח");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ שגיאה בחיבור למסד נתונים: {ex.Message}");
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@RequestID", estimate.RequestID },
            { "@TotalArea", estimate.TotalArea },
            { "@ParquetType", estimate.ParquetType },
            { "@RoomCount", estimate.RoomCount },
            { "@BasePrice", estimate.BasePrice },
            { "@EstimatedMinPrice", estimate.EstimatedMinPrice },
            { "@EstimatedMaxPrice", estimate.EstimatedMaxPrice },
            { "@EstimatedMinDays", estimate.EstimatedMinDays },
            { "@EstimatedMaxDays", estimate.EstimatedMaxDays },
            { "@ComplexityMultiplier", estimate.ComplexityMultiplier },
            { "@Notes", estimate.Notes }
        };

        Console.WriteLine($"📊 פרמטרים לפרוצדורה:");
        foreach (var param in paramDic)
        {
            Console.WriteLine($"   {param.Key}: {param.Value}");
        }

        cmd = CreateCommandWithStoredProcedureGeneral("SavePriceEstimate", con, paramDic);

        try
        {
            Console.WriteLine("🚀 מפעיל פרוצדורה SavePriceEstimate...");
            object result = cmd.ExecuteScalar();
            Console.WriteLine($"✅ פרוצדורה הושלמה, תוצאה: {result}");
            
            int estimateID = Convert.ToInt32(result);
            Console.WriteLine($"✅ EstimateID שנוצר: {estimateID}");
            return estimateID;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ שגיאה בביצוע פרוצדורה SavePriceEstimate: {ex.Message}");
            Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets price estimate by request ID
    //--------------------------------------------------------------------------------------------------
    public PriceEstimator GetPriceEstimateByRequestID(int requestID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        PriceEstimator estimate = null;

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@RequestID", requestID }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("GetPriceEstimateByRequestID", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (reader.Read())
            {
                estimate = new PriceEstimator
                {
                    EstimateID = Convert.ToInt32(reader["EstimateID"]),
                    RequestID = Convert.ToInt32(reader["RequestID"]),
                    TotalArea = Convert.ToDecimal(reader["TotalArea"]),
                    ParquetType = reader["ParquetType"].ToString(),
                    RoomCount = Convert.ToInt32(reader["RoomCount"]),
                    BasePrice = Convert.ToDecimal(reader["BasePrice"]),
                    EstimatedMinPrice = Convert.ToDecimal(reader["EstimatedMinPrice"]),
                    EstimatedMaxPrice = Convert.ToDecimal(reader["EstimatedMaxPrice"]),
                    EstimatedMinDays = Convert.ToInt32(reader["EstimatedMinDays"]),
                    EstimatedMaxDays = Convert.ToInt32(reader["EstimatedMaxDays"]),
                    ComplexityMultiplier = Convert.ToDecimal(reader["ComplexityMultiplier"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    Notes = reader["Notes"] != DBNull.Value ? reader["Notes"].ToString() : null
                };
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return estimate;
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets latest work request by customer ID
    //--------------------------------------------------------------------------------------------------
    public WorkRequest GetLatestWorkRequestByCustomerID(int customerID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        WorkRequest workRequest = null;

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@CustomerID", customerID }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("GetLatestWorkRequestByCustomerID", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (reader.Read())
            {
                workRequest = new WorkRequest
                {
                    RequestID = Convert.ToInt32(reader["RequestID"]),
                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                    CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : null,
                    Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : null,
                    Notes = reader["Notes"] != DBNull.Value ? reader["Notes"].ToString() : null,
                    PlannedDate = reader["PlannedDate"] != DBNull.Value ? Convert.ToDateTime(reader["PlannedDate"]) : null,
                    CompletedDate = reader["CompletedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CompletedDate"]) : null,
                    PreferredSlot = reader["PreferredSlot"] != DBNull.Value ? Convert.ToInt32(reader["PreferredSlot"]) : null
                };
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return workRequest;
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets work request by request ID
    //--------------------------------------------------------------------------------------------------
    public WorkRequest GetWorkRequestByID(int requestID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        WorkRequest workRequest = null;

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@RequestID", requestID }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("GetWorkRequestByID", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (reader.Read())
            {
                workRequest = new WorkRequest
                {
                    RequestID = Convert.ToInt32(reader["RequestID"]),
                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                    CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : null,
                    Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : null,
                    Notes = reader["Notes"] != DBNull.Value ? reader["Notes"].ToString() : null,
                    PlannedDate = reader["PlannedDate"] != DBNull.Value ? Convert.ToDateTime(reader["PlannedDate"]) : null,
                    CompletedDate = reader["CompletedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CompletedDate"]) : null,
                    PreferredSlot = reader["PreferredSlot"] != DBNull.Value ? Convert.ToInt32(reader["PreferredSlot"]) : null
                };
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return workRequest;
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets system settings
    //--------------------------------------------------------------------------------------------------
    public List<SystemSetting> GetSystemSettings(string settingKey = null)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        List<SystemSetting> settings = new List<SystemSetting>();

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@SettingKey", settingKey ?? (object)DBNull.Value }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("GetSystemSettings", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                SystemSetting setting = new SystemSetting
                {
                    SettingID = Convert.ToInt32(reader["SettingID"]),
                    SettingKey = reader["SettingKey"].ToString(),
                    SettingValue = reader["SettingValue"].ToString(),
                    SettingType = reader["SettingType"].ToString(),
                    Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                };

                settings.Add(setting);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return settings;
    }

    //--------------------------------------------------------------------------------------------------
    // This method updates or creates a system setting
    //--------------------------------------------------------------------------------------------------
    public int UpdateSystemSetting(SystemSetting setting)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@SettingKey", setting.SettingKey },
            { "@SettingValue", setting.SettingValue },
            { "@SettingType", setting.SettingType ?? "STRING" },
            { "@Description", setting.Description ?? (object)DBNull.Value }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("UpdateSystemSetting", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets work request statuses
    //--------------------------------------------------------------------------------------------------
    public List<WorkRequestStatus> GetWorkRequestStatuses()
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        List<WorkRequestStatus> statuses = new List<WorkRequestStatus>();

        try
        {
            cmd = CreateCommandWithStoredProcedureGeneral("GetWorkRequestStatuses", con, null);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                WorkRequestStatus status = new WorkRequestStatus
                {
                    StatusID = Convert.ToInt32(reader["StatusID"]),
                    StatusName = reader["StatusName"].ToString(),
                    StatusOrder = Convert.ToInt32(reader["StatusOrder"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                };

                statuses.Add(status);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return statuses;
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets BI statistics
    //--------------------------------------------------------------------------------------------------
    public List<BIStatistic> GetBIStatistics(DateTime? fromDate = null, DateTime? toDate = null)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        List<BIStatistic> statistics = new List<BIStatistic>();

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@FromDate", fromDate ?? (object)DBNull.Value },
                { "@ToDate", toDate ?? (object)DBNull.Value }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("GetBIStatistics", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                BIStatistic statistic = new BIStatistic
                {
                    StatType = reader["StatType"].ToString(),
                    StatValue = reader["StatValue"] != DBNull.Value ? Convert.ToDecimal(reader["StatValue"]) : 0,
                    StatCount = reader["StatCount"] != DBNull.Value ? Convert.ToInt32(reader["StatCount"]) : 0,
                    StatText = reader["StatText"] != DBNull.Value ? reader["StatText"].ToString() : null
                };

                statistics.Add(statistic);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return statistics;
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets dashboard summary for management
    //--------------------------------------------------------------------------------------------------
    public DashboardSummary GetDashboardSummary()
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        DashboardSummary summary = null;

        try
        {
            cmd = CreateCommandWithStoredProcedureGeneral("GetDashboardSummary", con, null);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (reader.Read())
            {
                summary = new DashboardSummary
                {
                    WaitingForDate = reader["WaitingForDate"] != DBNull.Value ? Convert.ToInt32(reader["WaitingForDate"]) : 0,
                    PendingInstalls = reader["PendingInstalls"] != DBNull.Value ? Convert.ToInt32(reader["PendingInstalls"]) : 0,
                    PendingQuotes = reader["PendingQuotes"] != DBNull.Value ? Convert.ToInt32(reader["PendingQuotes"]) : 0,
                    PendingApproval = reader["PendingApproval"] != DBNull.Value ? Convert.ToInt32(reader["PendingApproval"]) : 0,
                    WaitingForVideo = reader["WaitingForVideo"] != DBNull.Value ? Convert.ToInt32(reader["WaitingForVideo"]) : 0,
                    CompletedInstalls = reader["CompletedInstalls"] != DBNull.Value ? Convert.ToInt32(reader["CompletedInstalls"]) : 0,
                    TotalActiveCustomers = reader["TotalActiveCustomers"] != DBNull.Value ? Convert.ToInt32(reader["TotalActiveCustomers"]) : 0,
                    TotalActiveRequests = reader["TotalActiveRequests"] != DBNull.Value ? Convert.ToInt32(reader["TotalActiveRequests"]) : 0,
                    ThisWeekInstalls = reader["ThisWeekInstalls"] != DBNull.Value ? Convert.ToInt32(reader["ThisWeekInstalls"]) : 0,
                    ThisMonthInstalls = reader["ThisMonthInstalls"] != DBNull.Value ? Convert.ToInt32(reader["ThisMonthInstalls"]) : 0
                };
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return summary ?? new DashboardSummary();
    }

    //--------------------------------------------------------------------------------------------------
    // This method calculates advanced price estimate using stored procedure
    //--------------------------------------------------------------------------------------------------
    public PriceEstimateCalculation CalculatePriceEstimateAdvanced(
        decimal totalArea, 
        string parquetType, 
        int roomCount, 
        bool hasSmallRooms = false,
        bool hasSpecialRequirements = false,
        int differentFloorTypes = 1)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        PriceEstimateCalculation calculation = null;

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@TotalArea", totalArea },
                { "@ParquetType", parquetType },
                { "@RoomCount", roomCount },
                { "@HasSmallRooms", hasSmallRooms },
                { "@HasSpecialRequirements", hasSpecialRequirements },
                { "@DifferentFloorTypes", differentFloorTypes }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("CalculatePriceEstimateAdvanced", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (reader.Read())
            {
                calculation = new PriceEstimateCalculation
                {
                    BasePricePerM2 = Convert.ToDecimal(reader["BasePricePerM2"]),
                    BasePrice = Convert.ToDecimal(reader["BasePrice"]),
                    EstimatedMinPrice = Convert.ToDecimal(reader["EstimatedMinPrice"]),
                    EstimatedMaxPrice = Convert.ToDecimal(reader["EstimatedMaxPrice"]),
                    EstimatedMinDays = Convert.ToInt32(reader["EstimatedMinDays"]),
                    EstimatedMaxDays = Convert.ToInt32(reader["EstimatedMaxDays"]),
                    ComplexityMultiplier = Convert.ToDecimal(reader["ComplexityMultiplier"])
                };
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return calculation ?? new PriceEstimateCalculation();
    }

    //--------------------------------------------------------------------------------------------------
    // This method inserts customer feedback
    //--------------------------------------------------------------------------------------------------
    public int InsertCustomerFeedback(CustomerFeedback feedback)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@CustomerID", feedback.CustomerID },
            { "@RequestID", feedback.RequestID },
            { "@Sent", feedback.Sent },
            { "@SentAt", feedback.SentAt ?? (object)DBNull.Value }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("InsertCustomerFeedback", con, paramDic);

        try
        {
            object result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets customer feedback
    //--------------------------------------------------------------------------------------------------
    public CustomerFeedback GetCustomerFeedback(int customerID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        CustomerFeedback feedback = null;

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@CustomerID", customerID }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("GetCustomerFeedback", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (reader.Read())
            {
                feedback = new CustomerFeedback
                {
                    FeedbackID = Convert.ToInt32(reader["FeedbackID"]),
                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                    RequestID = Convert.ToInt32(reader["RequestID"]),
                    Sent = Convert.ToBoolean(reader["Sent"]),
                    SentAt = reader["SentAt"] != DBNull.Value ? Convert.ToDateTime(reader["SentAt"]) : null
                };
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return feedback;
    }

    //--------------------------------------------------------------------------------------------------
    // This method marks feedback as sent
    //--------------------------------------------------------------------------------------------------
    public int MarkFeedbackAsSent(int feedbackID)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@FeedbackID", feedbackID }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("MarkFeedbackAsSent", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets monthly sales report data
    //--------------------------------------------------------------------------------------------------
    public List<FinalProject.BL.MonthlySalesData> GetMonthlySalesReport(int monthsBack = 12, DateTime? fromDate = null, DateTime? toDate = null)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        List<FinalProject.BL.MonthlySalesData> salesData = new List<FinalProject.BL.MonthlySalesData>();

        try
        {
            // חישוב monthsBack על בסיס תאריכים אם ניתנו
            if (fromDate.HasValue)
            {
                var monthsDiff = ((DateTime.Now.Year - fromDate.Value.Year) * 12) + DateTime.Now.Month - fromDate.Value.Month + 1;
                monthsBack = Math.Max(1, Math.Min(monthsDiff, 120)); // מגביל בין 1 ל-120 חודשים
                Console.WriteLine($"Calculated monthsBack from fromDate: {monthsBack}");
            }
            
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@MonthsBack", monthsBack }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("GetMonthlySalesReport", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                FinalProject.BL.MonthlySalesData data = new FinalProject.BL.MonthlySalesData
                {
                    Month = reader["Month"].ToString(),
                    MonthName = reader["MonthName"].ToString(),
                    TotalQuotes = reader["TotalQuotes"] != DBNull.Value ? Convert.ToDecimal(reader["TotalQuotes"]) : 0,
                    TotalCustomers = reader["TotalCustomers"] != DBNull.Value ? Convert.ToInt32(reader["TotalCustomers"]) : 0,
                    CompletedInstalls = reader["CompletedInstalls"] != DBNull.Value ? Convert.ToInt32(reader["CompletedInstalls"]) : 0,
                    TotalArea = reader["TotalArea"] != DBNull.Value ? Convert.ToDecimal(reader["TotalArea"]) : 0
                };

                salesData.Add(data);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return salesData;
    }

    //--------------------------------------------------------------------------------------------------
    // SAFE METHODS WITH MOCK DATA FOR DEVELOPMENT
    //--------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------
    // This method gets BI statistics - NO MOCK DATA
    //--------------------------------------------------------------------------------------------------
    public List<BIStatistic> GetBIStatisticsSafe(DateTime? fromDate = null, DateTime? toDate = null)
    {
        return GetBIStatistics(fromDate, toDate);
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets dashboard summary - NO MOCK DATA
    //--------------------------------------------------------------------------------------------------
    public DashboardSummary GetDashboardSummarySafe()
    {
        return GetDashboardSummary();
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets monthly sales report - NO MOCK DATA
    //--------------------------------------------------------------------------------------------------
    public List<FinalProject.BL.MonthlySalesData> GetMonthlySalesReportSafe(int monthsBack = 12, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return GetMonthlySalesReport(monthsBack, fromDate, toDate);
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets monthly requests report
    //--------------------------------------------------------------------------------------------------
    public List<FinalProject.BL.MonthlyRequestsData> GetMonthlyRequestsReport(int monthsBack = 12, DateTime? fromDate = null, DateTime? toDate = null)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        List<FinalProject.BL.MonthlyRequestsData> requestsData = new List<FinalProject.BL.MonthlyRequestsData>();

        try
        {
            // חישוב monthsBack על בסיס תאריכים אם ניתנו
            if (fromDate.HasValue)
            {
                var monthsDiff = ((DateTime.Now.Year - fromDate.Value.Year) * 12) + DateTime.Now.Month - fromDate.Value.Month + 1;
                monthsBack = Math.Max(1, Math.Min(monthsDiff, 120)); // מגביל בין 1 ל-120 חודשים
                Console.WriteLine($"Calculated monthsBack from fromDate: {monthsBack}");
            }
            
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@MonthsBack", monthsBack }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("GetMonthlyRequestsReport", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                FinalProject.BL.MonthlyRequestsData data = new FinalProject.BL.MonthlyRequestsData
                {
                    Month = reader["Month"].ToString(),
                    MonthName = reader["MonthName"].ToString(),
                    NewRequests = reader["NewRequests"] != DBNull.Value ? Convert.ToInt32(reader["NewRequests"]) : 0,
                    NewCustomers = reader["NewCustomers"] != DBNull.Value ? Convert.ToInt32(reader["NewCustomers"]) : 0,
                    CompletedRequests = reader["CompletedRequests"] != DBNull.Value ? Convert.ToInt32(reader["CompletedRequests"]) : 0,
                    TotalArea = reader["TotalArea"] != DBNull.Value ? Convert.ToDecimal(reader["TotalArea"]) : 0
                };

                requestsData.Add(data);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return requestsData;
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets monthly requests report - NO MOCK DATA
    //--------------------------------------------------------------------------------------------------
    public List<FinalProject.BL.MonthlyRequestsData> GetMonthlyRequestsReportSafe(int monthsBack = 12, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return GetMonthlyRequestsReport(monthsBack, fromDate, toDate);
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets work request statuses - NO MOCK DATA
    //--------------------------------------------------------------------------------------------------
    public List<WorkRequestStatus> GetWorkRequestStatusesSafe()
    {
        return GetWorkRequestStatuses();
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets system settings - NO MOCK DATA
    //--------------------------------------------------------------------------------------------------
    public List<SystemSetting> GetSystemSettingsSafe(string settingKey = null)
    {
        return GetSystemSettings(settingKey);
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets recent activity for dashboard
    //--------------------------------------------------------------------------------------------------
    public List<RecentActivity> GetRecentActivity(int limit = 10)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        List<RecentActivity> activities = new List<RecentActivity>();

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@Limit", limit }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("GetRecentActivity", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                var activity = new RecentActivity
                {
                    ActivityType = reader["ActivityType"].ToString(),
                    Description = reader["Description"].ToString(),
                    CustomerName = reader["CustomerName"].ToString(),
                    Location = reader["Location"].ToString(),
                    ActivityDate = Convert.ToDateTime(reader["ActivityDate"]),
                    RelativeTime = reader["RelativeTime"].ToString(),
                    IconType = reader["IconType"].ToString(),
                    ColorClass = reader["ColorClass"].ToString()
                };

                activities.Add(activity);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return activities;
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets upcoming installs for dashboard
    //--------------------------------------------------------------------------------------------------
    public List<UpcomingInstall> GetUpcomingInstalls(int days = 7)
    {
        SqlConnection con;
        SqlCommand cmd;
        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        List<UpcomingInstall> installs = new List<UpcomingInstall>();

        try
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@Days", days }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("GetUpcomingInstalls", con, paramDic);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                var install = new UpcomingInstall
                {
                    RequestID = Convert.ToInt32(reader["RequestID"]),
                    CustomerName = reader["CustomerName"].ToString(),
                    Location = reader["Location"].ToString(),
                    InstallDate = Convert.ToDateTime(reader["InstallDate"]),
                    TimeSlot = reader["TimeSlot"].ToString(),
                    Status = reader["Status"].ToString(),
                    StatusColor = reader["StatusColor"].ToString(),
                    FormattedDate = reader["FormattedDate"].ToString()
                };

                installs.Add(install);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return installs;
    }

    //--------------------------------------------------------------------------------------------------
    // Safe methods - NO MOCK DATA
    //--------------------------------------------------------------------------------------------------
    public List<RecentActivity> GetRecentActivitySafe(int limit = 10)
    {
        return GetRecentActivity(limit);
    }

    public List<UpcomingInstall> GetUpcomingInstallsSafe(int days = 7)
    {
        return GetUpcomingInstalls(days);
    }

    //--------------------------------------------------------------------------------------------------
    // This method updates system setting - NO MOCK DATA
    //--------------------------------------------------------------------------------------------------
    public int UpdateSystemSettingSafe(SystemSetting setting)
    {
        return UpdateSystemSetting(setting);
    }

    //--------------------------------------------------------------------------------------------------
    // פונקציות לניהול משתמשים ופרטי עסק
    //--------------------------------------------------------------------------------------------------

    /// <summary>
    /// קבלת כל המשתמשים
    /// </summary>
    public List<FinalProject.BL.User> GetAllUsers()
    {
        SqlConnection con;
        SqlCommand cmd;
        List<FinalProject.BL.User> users = new List<FinalProject.BL.User>();

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        try
        {
            cmd = new SqlCommand("SELECT * FROM Users ORDER BY CreatedAt DESC", con);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                FinalProject.BL.User user = new FinalProject.BL.User
                {
                    UserID = (int)reader["UserID"],
                    Username = reader["Username"].ToString(),
                    Password = reader["Password"].ToString(),
                    BusinessName = reader["BusinessName"].ToString(),
                    BusinessPhone = reader["BusinessPhone"]?.ToString(),
                    BusinessEmail = reader["BusinessEmail"]?.ToString(),
                    WorkingHours = reader["WorkingHours"]?.ToString(),
                    AboutUs = reader["AboutUs"]?.ToString(),
                    BusinessLogo = reader["BusinessLogo"]?.ToString(),
                    IntroVideoURL = reader["IntroVideoURL"]?.ToString(),
                    IsActive = (bool)reader["IsActive"],
                    CreatedAt = (DateTime)reader["CreatedAt"],
                    UpdatedAt = (DateTime)reader["UpdatedAt"],
                    LastLogin = reader["LastLogin"] != DBNull.Value ? (DateTime?)reader["LastLogin"] : null
                };
                users.Add(user);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return users;
    }

    /// <summary>
    /// קבלת משתמש לפי ID
    /// </summary>
    public FinalProject.BL.User GetUserById(int userId)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        try
        {
            cmd = new SqlCommand("SELECT * FROM Users WHERE UserID = @UserID", con);
            cmd.Parameters.AddWithValue("@UserID", userId);
            
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (reader.Read())
            {
                FinalProject.BL.User user = new FinalProject.BL.User
                {
                    UserID = (int)reader["UserID"],
                    Username = reader["Username"].ToString(),
                    Password = reader["Password"].ToString(),
                    BusinessName = reader["BusinessName"].ToString(),
                    BusinessPhone = reader["BusinessPhone"]?.ToString(),
                    BusinessEmail = reader["BusinessEmail"]?.ToString(),
                    WorkingHours = reader["WorkingHours"]?.ToString(),
                    AboutUs = reader["AboutUs"]?.ToString(),
                    BusinessLogo = reader["BusinessLogo"]?.ToString(),
                    IntroVideoURL = reader["IntroVideoURL"]?.ToString(),
                    IsActive = (bool)reader["IsActive"],
                    CreatedAt = (DateTime)reader["CreatedAt"],
                    UpdatedAt = (DateTime)reader["UpdatedAt"],
                    LastLogin = reader["LastLogin"] != DBNull.Value ? (DateTime?)reader["LastLogin"] : null
                };
                reader.Close();
                return user;
            }
            
            reader.Close();
            return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    /// <summary>
    /// קבלת משתמש לפי שם משתמש
    /// </summary>
    public FinalProject.BL.User GetUserByUsername(string username)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        try
        {
            cmd = new SqlCommand("SELECT * FROM Users WHERE Username = @Username", con);
            cmd.Parameters.AddWithValue("@Username", username);
            
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (reader.Read())
            {
                FinalProject.BL.User user = new FinalProject.BL.User
                {
                    UserID = (int)reader["UserID"],
                    Username = reader["Username"].ToString(),
                    Password = reader["Password"].ToString(),
                    BusinessName = reader["BusinessName"].ToString(),
                    BusinessPhone = reader["BusinessPhone"]?.ToString(),
                    BusinessEmail = reader["BusinessEmail"]?.ToString(),
                    WorkingHours = reader["WorkingHours"]?.ToString(),
                    AboutUs = reader["AboutUs"]?.ToString(),
                    BusinessLogo = reader["BusinessLogo"]?.ToString(),
                    IntroVideoURL = reader["IntroVideoURL"]?.ToString(),
                    IsActive = (bool)reader["IsActive"],
                    CreatedAt = (DateTime)reader["CreatedAt"],
                    UpdatedAt = (DateTime)reader["UpdatedAt"],
                    LastLogin = reader["LastLogin"] != DBNull.Value ? (DateTime?)reader["LastLogin"] : null
                };
                reader.Close();
                return user;
            }
            
            reader.Close();
            return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    /// <summary>
    /// הוספת משתמש חדש
    /// </summary>
    public int AddUser(FinalProject.BL.User user)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        try
        {
            string sql = @"
                INSERT INTO Users (Username, Password, BusinessName, BusinessPhone, BusinessEmail, 
                                  WorkingHours, AboutUs, BusinessLogo, IntroVideoURL, IsActive, CreatedAt, UpdatedAt)
                VALUES (@Username, @Password, @BusinessName, @BusinessPhone, @BusinessEmail,
                       @WorkingHours, @AboutUs, @BusinessLogo, @IntroVideoURL, @IsActive, @CreatedAt, @UpdatedAt);
                SELECT SCOPE_IDENTITY();";

            cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@BusinessName", user.BusinessName);
            cmd.Parameters.AddWithValue("@BusinessPhone", (object)user.BusinessPhone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BusinessEmail", (object)user.BusinessEmail ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@WorkingHours", (object)user.WorkingHours ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AboutUs", (object)user.AboutUs ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BusinessLogo", (object)user.BusinessLogo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IntroVideoURL", (object)user.IntroVideoURL ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            cmd.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
            cmd.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt);

            object result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    /// <summary>
    /// עדכון פרטי משתמש
    /// </summary>
    /// 

    public int UpdateSpaceVideoLink(int spaceID, string videoLink)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(@"Data Source=Media.ruppin.ac.il;Initial Catalog=igroup14_prod;User ID=igroup14;Password=igroup14_35916"))
            {
                string query = "UPDATE SpaceDetails SET MediaURL = @VideoLink WHERE SpaceID = @SpaceID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VideoLink", videoLink);
                    cmd.Parameters.AddWithValue("@SpaceID", spaceID);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating MediaURL: {ex.Message}");
            return 0;
        }
    }

    public List<SpaceDetails> GetSpacesByCustomerID(int customerID)
    {
        // כאן תממש קריאה ל-DB שלך, לדוגמה קריאת stored procedure שמחזירה את SpaceDetails של הלקוח
        // דוגמה דמה:
        // return dbContext.SpaceDetails.Where(s => s.CustomerID == customerID).ToList();

        throw new NotImplementedException("השלם את הקריאה ל-DB כאן");
    }

    public List<string> GetCustomerVideoLinks(int customerID)
{
    List<string> videos = new List<string>();
    SqlConnection con = connect("myProjDB");

    string query = @"
        SELECT SD.MediaURL
        FROM SpaceDetails SD
        JOIN WorkRequest WR ON SD.RequestID = WR.RequestID
        WHERE WR.CustomerID = @CustomerID
        AND SD.MediaURL IS NOT NULL AND SD.MediaURL <> ''
    ";

    SqlCommand cmd = new SqlCommand(query, con);
    cmd.Parameters.AddWithValue("@CustomerID", customerID);

    SqlDataReader reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        videos.Add(reader["MediaURL"].ToString());
    }

    reader.Close();
    con.Close();

    return videos;
}


    public bool UpdateUser(FinalProject.BL.User user)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        try
        {
            string sql = @"
                UPDATE Users SET 
                    Username = @Username,
                    Password = @Password,
                    BusinessName = @BusinessName,
                    BusinessPhone = @BusinessPhone,
                    BusinessEmail = @BusinessEmail,
                    WorkingHours = @WorkingHours,
                    AboutUs = @AboutUs,
                    BusinessLogo = @BusinessLogo,
                    IntroVideoURL = @IntroVideoURL,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt,
                    LastLogin = @LastLogin
                WHERE UserID = @UserID";

            cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@UserID", user.UserID);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@BusinessName", user.BusinessName);
            cmd.Parameters.AddWithValue("@BusinessPhone", (object)user.BusinessPhone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BusinessEmail", (object)user.BusinessEmail ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@WorkingHours", (object)user.WorkingHours ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AboutUs", (object)user.AboutUs ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BusinessLogo", (object)user.BusinessLogo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IntroVideoURL", (object)user.IntroVideoURL ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            cmd.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt);
            cmd.Parameters.AddWithValue("@LastLogin", (object)user.LastLogin ?? DBNull.Value);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

}

// מחלקות עזר חדשות
public class SystemSetting
{
    public int SettingID { get; set; }
    public string SettingKey { get; set; }
    public string SettingValue { get; set; }
    public string SettingType { get; set; }
    public string Description { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class WorkRequestStatus
{
    public int StatusID { get; set; }
    public string StatusName { get; set; }
    public int StatusOrder { get; set; }
    public bool IsActive { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BIStatistic
{
    public string StatType { get; set; }
    public decimal StatValue { get; set; }
    public int StatCount { get; set; }
    public string StatText { get; set; }
}

public class DashboardSummary
{
    public int WaitingForDate { get; set; }
    public int PendingInstalls { get; set; }
    public int PendingQuotes { get; set; }
    public int PendingApproval { get; set; }
    public int WaitingForVideo { get; set; }
    public int CompletedInstalls { get; set; }
    public int TotalActiveCustomers { get; set; }
    public int TotalActiveRequests { get; set; }
    public int ThisWeekInstalls { get; set; }
    public int ThisMonthInstalls { get; set; }
}

public class PriceEstimateCalculation
{
    public decimal BasePricePerM2 { get; set; }
    public decimal BasePrice { get; set; }
    public decimal EstimatedMinPrice { get; set; }
    public decimal EstimatedMaxPrice { get; set; }
    public int EstimatedMinDays { get; set; }
    public int EstimatedMaxDays { get; set; }
    public decimal ComplexityMultiplier { get; set; }
}






