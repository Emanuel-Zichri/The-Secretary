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
}

