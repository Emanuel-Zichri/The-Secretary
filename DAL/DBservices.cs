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
    public int InsertWorkRequest(int costumerID, DateTime PreferredDate,int PreferredSlot)
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

        try
        {
            cmd.ExecuteNonQuery();
            int newRequestId = (int)outputIdParam.Value;
            return newRequestId;
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
        { "Status", reader["Status"].ToString() },

        // עבור לקוח ספציפי ישמש לבניית כרטיס הלקוח בהמשך-ייתכן ונצטרך לעדכן לינק לסרטון והערות
        { "SpaceID", ColumnExists(reader, "SpaceID") ? reader["SpaceID"] : null },
        { "Size", ColumnExists(reader, "Size") ? reader["Size"] : null },
        { "FloorType", ColumnExists(reader, "FloorType") ? reader["FloorType"].ToString() : null },
        { "ParquetType", ColumnExists(reader, "ParquetType") ? reader["ParquetType"].ToString() : null },
        { "SpaceNotes", ColumnExists(reader, "SpaceNotes") ? reader["SpaceNotes"].ToString() : null },

        // עבור תצוגה מקובצת- כלומר לא נשלח מזהה לקוח
        { "SpaceCount", ColumnExists(reader, "SpaceCount") ? reader["SpaceCount"] : null },
        { "TotalSpaceSize", ColumnExists(reader, "TotalSpaceSize") ? reader["TotalSpaceSize"] : null }
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
    // This method Read all games for a specific user 
    //--------------------------------------------------------------------------------------------------
    //public List<Game> ReadUserGames(int userId)
    //{
    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    List<Game> userGames = new List<Game>();

    //    try
    //    {
    //        Dictionary<string, object> paramDic = new Dictionary<string, object>
    //    {
    //        { "@UserId", userId }
    //    };
    //        cmd = CreateCommandWithStoredProcedureGeneral("SP_ReadGamesForUserOsh", con, paramDic); // create the command
    //        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

    //        while (reader.Read())
    //        {
    //            Game g = new Game
    //            {
    //                AppID = Convert.ToInt32(reader["AppId"]),
    //                Name = reader["Name"].ToString(),
    //                ReleaseDate = Convert.ToDateTime(reader["ReleaseDate"]),
    //                Price = reader["Price"] != DBNull.Value ? Convert.ToDouble(reader["Price"]) : 0.0,
    //                Description = reader["Description"].ToString(),
    //                HeaderImage = reader["HeaderImage"].ToString(),
    //                Website = reader["Website"].ToString(),
    //                Windows = reader["Windows"] != DBNull.Value && Convert.ToBoolean(reader["Windows"]),
    //                Mac = reader["Mac"] != DBNull.Value && Convert.ToBoolean(reader["Mac"]),
    //                Linux = reader["Linux"] != DBNull.Value && Convert.ToBoolean(reader["Linux"]),
    //                ScoreRank = reader["ScoreRank"] != DBNull.Value ? Convert.ToInt32(reader["ScoreRank"]) : 0,
    //                Recommendations = reader["Recommendations"].ToString(),
    //                Publisher = reader["Publisher"].ToString(),
    //                NumberOfPurchases = Convert.ToInt32(reader["NumberOfPurchases"])
    //            };

    //            userGames.Add(g);
    //        }

    //        reader.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw (ex);
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close(); // close the db connection
    //        }
    //    }

    //    return userGames;
    //}
    //--------------------------------------------------------------------------------------------------
    // This method Change user detail and return the updated user 
    //--------------------------------------------------------------------------------------------------
    //public Userr UpdateUser(Userr changesForUser)
    //{
    //    Userr updatedUser = new Userr();
    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }


    //    try
    //    {
    //        Dictionary<string, object> paramDic = new Dictionary<string, object>
    //    {
    //        { "@Id", changesForUser.Id },
    //        { "@NewName", changesForUser.Name },
    //        { "@NewEmail", changesForUser.Email },
    //        { "@NewPassword", changesForUser.Password }
    //    };
    //        cmd = CreateCommandWithStoredProcedureGeneral("SP_EditUserOsh", con, paramDic); // create the command
    //        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

    //        if(reader.Read())
    //        {
    //            updatedUser.Id = Convert.ToInt32(reader["Id"]);
    //            updatedUser.Name = reader["Name"].ToString();
    //            updatedUser.Email = reader["Email"].ToString();
    //            updatedUser.Password = reader["Password"].ToString();

    //        }

    //        reader.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw (ex);
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close(); // close the db connection
    //        }
    //    }

    //    return updatedUser;
    //}
    //--------------------------------------------------------------------------------------------------
    // This method Changes user activation status and return Numaffected. 
    //--------------------------------------------------------------------------------------------------
    //public int changeActivation(int id, bool userActivation)
    //{
    //    Userr updatedUser = new Userr();
    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }


    //    try
    //    {
    //        Dictionary<string, object> paramDic = new Dictionary<string, object>
    //    {
    //        { "@Id", id },
    //        { "@IsActive", userActivation },

    //    };
    //        cmd = CreateCommandWithStoredProcedureGeneral("SP_ActivateUserOsh", con, paramDic); // create the command

    //            int numEffected = cmd.ExecuteNonQuery(); // execute the command
    //            return numEffected;

    //    }

    //    catch (Exception ex)
    //    {
    //        throw (ex);
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close(); // close the db connection
    //        }
    //    }
    //}
    //--------------------------------------------------------------------------------------------------
    // This method Read all games for a specific user filtered by price
    //--------------------------------------------------------------------------------------------------
    //public List<Game> GetGamesByPrice(int Userid, float Price)
    //{
    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    List<Game> userGamesFilteredByPrice = new List<Game>();

    //    try
    //    {
    //        Dictionary<string, object> paramDic = new Dictionary<string, object>
    //    {
    //        { "@UserId", Userid },
    //        { "@Price",Price }
    //    };
    //        cmd = CreateCommandWithStoredProcedureGeneral("SP_FilterGameByPriceOsh", con, paramDic); // create the command
    //        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

    //        while (reader.Read())
    //        {
    //            Game g = new Game
    //            {
    //                AppID = Convert.ToInt32(reader["AppId"]),
    //                Name = reader["Name"].ToString(),
    //                ReleaseDate = Convert.ToDateTime(reader["ReleaseDate"]),
    //                Price = reader["Price"] != DBNull.Value ? Convert.ToDouble(reader["Price"]) : 0.0,
    //                Description = reader["Description"].ToString(),
    //                HeaderImage = reader["HeaderImage"].ToString(),
    //                Website = reader["Website"].ToString(),
    //                Windows = reader["Windows"] != DBNull.Value && Convert.ToBoolean(reader["Windows"]),
    //                Mac = reader["Mac"] != DBNull.Value && Convert.ToBoolean(reader["Mac"]),
    //                Linux = reader["Linux"] != DBNull.Value && Convert.ToBoolean(reader["Linux"]),
    //                ScoreRank = reader["ScoreRank"] != DBNull.Value ? Convert.ToInt32(reader["ScoreRank"]) : 0,
    //                Recommendations = reader["Recommendations"].ToString(),
    //                Publisher = reader["Publisher"].ToString(),
    //                NumberOfPurchases = Convert.ToInt32(reader["NumberOfPurchases"])
    //            };

    //            userGamesFilteredByPrice.Add(g);
    //        }

    //        reader.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw (ex);
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close(); // close the db connection
    //        }
    //    }

    //    return userGamesFilteredByPrice;
    //}
    //--------------------------------------------------------------------------------------------------
    // This method Read all games for a specific user filtered by Rank
    //--------------------------------------------------------------------------------------------------
    //public List<Game> GetGamesByRank(int Userid, int rank)
    //{
    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    List<Game> userGamesFilteredByRank = new List<Game>();

    //    try
    //    {
    //        Dictionary<string, object> paramDic = new Dictionary<string, object>
    //    {
    //        { "@UserId", Userid },
    //        { "@Rank",rank }
    //    };
    //        cmd = CreateCommandWithStoredProcedureGeneral("SP_FilterGameByRankOsh", con, paramDic); // create the command
    //        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

    //        while (reader.Read())
    //        {
    //            Game g = new Game
    //            {
    //                AppID = Convert.ToInt32(reader["AppId"]),
    //                Name = reader["Name"].ToString(),
    //                ReleaseDate = Convert.ToDateTime(reader["ReleaseDate"]),
    //                Price = reader["Price"] != DBNull.Value ? Convert.ToDouble(reader["Price"]) : 0.0,
    //                Description = reader["Description"].ToString(),
    //                HeaderImage = reader["HeaderImage"].ToString(),
    //                Website = reader["Website"].ToString(),
    //                Windows = reader["Windows"] != DBNull.Value && Convert.ToBoolean(reader["Windows"]),
    //                Mac = reader["Mac"] != DBNull.Value && Convert.ToBoolean(reader["Mac"]),
    //                Linux = reader["Linux"] != DBNull.Value && Convert.ToBoolean(reader["Linux"]),
    //                ScoreRank = reader["ScoreRank"] != DBNull.Value ? Convert.ToInt32(reader["ScoreRank"]) : 0,
    //                Recommendations = reader["Recommendations"].ToString(),
    //                Publisher = reader["Publisher"].ToString(),
    //                NumberOfPurchases = Convert.ToInt32(reader["NumberOfPurchases"])
    //            };

    //            userGamesFilteredByRank.Add(g);
    //        }

    //        reader.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw (ex);
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close(); // close the db connection
    //        }
    //    }

    //    return userGamesFilteredByRank;
    //}
    //--------------------------------------------------------------------------------------------------
    // This method Read all games in database 
    //--------------------------------------------------------------------------------------------------
    //public List<Game> Read()
    //{
    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    List<Game> gamesToRender = new List<Game>();

    //    try
    //    {
    //        Dictionary<string, object> paramDic = new Dictionary<string, object>();
    //        cmd = CreateCommandWithStoredProcedureGeneral("SP_ReadGamesOsh", con, paramDic); // create the command
    //        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

    //        while (reader.Read())
    //        {
    //            Game g = new Game
    //            {
    //                AppID = Convert.ToInt32(reader["AppId"]),
    //                Name = reader["Name"].ToString(),
    //                ReleaseDate = Convert.ToDateTime(reader["ReleaseDate"]),
    //                Price = reader["Price"] != DBNull.Value ? Convert.ToDouble(reader["Price"]) : 0.0,
    //                Description = reader["Description"].ToString(),
    //                HeaderImage = reader["HeaderImage"].ToString(),
    //                Website = reader["Website"].ToString(),
    //                Windows = reader["Windows"] != DBNull.Value && Convert.ToBoolean(reader["Windows"]),
    //                Mac = reader["Mac"] != DBNull.Value && Convert.ToBoolean(reader["Mac"]),
    //                Linux = reader["Linux"] != DBNull.Value && Convert.ToBoolean(reader["Linux"]),
    //                ScoreRank = reader["ScoreRank"] != DBNull.Value ? Convert.ToInt32(reader["ScoreRank"]) : 0,
    //                Recommendations = reader["Recommendations"].ToString(),
    //                Publisher = reader["Publisher"].ToString(),
    //                NumberOfPurchases = Convert.ToInt32(reader["NumberOfPurchases"])
    //            };

    //            gamesToRender.Add(g);
    //        }

    //        reader.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw (ex);
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close(); // close the db connection
    //        }
    //    }

    //    return gamesToRender;
    //}



    //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure
    //---------------------------------------------------------------------------------
    //private SqlCommand CreateCommandWithStoredProcedure(String spName, SqlConnection con, Userr user)
    //{

    //    SqlCommand cmd = new SqlCommand(); // create the command object

    //    cmd.Connection = con;              // assign the connection to the command object

    //    cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

    //    cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

    //    cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

    //    cmd.Parameters.AddWithValue("@id", user.Id);

    //    cmd.Parameters.AddWithValue("@name", user.Name);




    //    return cmd;
    //}

    //--------------------------------------------------------------------------------------------------
    // This method register a user to the user table 
    //--------------------------------------------------------------------------------------------------
    //public int Register(string newName, string newEmail, string newPassword)
    //{

    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    Dictionary<string, object> paramDic = new Dictionary<string, object>();
    //    paramDic.Add("@Name", newName);
    //    paramDic.Add("@Email", newEmail);
    //    paramDic.Add("@Password", newPassword);


    //    cmd = CreateCommandWithStoredProcedureGeneral("SP_RegisterUserOsh", con, paramDic);          // create the command

    //    try
    //    {
    //        int numEffected = cmd.ExecuteNonQuery(); // execute the command
    //        return numEffected;
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    finally
    //    {
    //        if (con != null)
    //        {
    //            // close the db connection
    //            con.Close();
    //        }
    //    }

    //}

    //--------------------------------------------------------------------------------------------------
    // This method add a game and user to the GameUser table 
    //--------------------------------------------------------------------------------------------------
    //public int userBuyGame(int userID,int appID)
    //{

    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    Dictionary<string, object> paramDic = new Dictionary<string, object>();
    //    paramDic.Add("@UserID", userID);
    //    paramDic.Add("@AppId", appID);



    //    cmd = CreateCommandWithStoredProcedureGeneral("SP_UserBuyOsh", con, paramDic);          // create the command

    //    try
    //    {
    //        int numEffected = cmd.ExecuteNonQuery(); // execute the command
    //        return numEffected;
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    finally
    //    {
    //        if (con != null)
    //        {
    //            // close the db connection
    //            con.Close();
    //        }
    //    }

    //}
    //--------------------------------------------------------------------------------------------------
    // This method Delete a game from user games table 
    //--------------------------------------------------------------------------------------------------
    //public int userDeleteGame(int userID, int appID)
    //{

    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    Dictionary<string, object> paramDic = new Dictionary<string, object>();
    //    paramDic.Add("@UserID", userID);
    //    paramDic.Add("@AppId", appID);



    //    cmd = CreateCommandWithStoredProcedureGeneral("SP_UserDeleteOsh", con, paramDic);          // create the command

    //    try
    //    {
    //        int numEffected = cmd.ExecuteNonQuery(); // execute the command
    //        return numEffected;
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    finally
    //    {
    //        if (con != null)
    //        {
    //            // close the db connection
    //            con.Close();
    //        }
    //    }

    //}
    //--------------------------------------------------------------------------------------------------
    // This method update user name 
    //--------------------------------------------------------------------------------------------------
    //public int UpdateName(int currentID, string newName)
    //{

    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    Dictionary<string, object> paramDic = new Dictionary<string, object>();
    //    paramDic.Add("@UserID", currentID);
    //    paramDic.Add("@NewName", newName);



    //    cmd = CreateCommandWithStoredProcedureGeneral("SP_UpdateUserNameOsh", con, paramDic);          // create the command

    //    try
    //    {
    //        int numEffected = cmd.ExecuteNonQuery(); // execute the command
    //        return numEffected;
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    finally
    //    {
    //        if (con != null)
    //        {
    //            // close the db connection
    //            con.Close();
    //        }
    //    }

    //}
    //--------------------------------------------------------------------------------------------------
    // This method update user name 
    //--------------------------------------------------------------------------------------------------
    //public int UpdatePass(int currentID, string newPass)
    //{

    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    Dictionary<string, object> paramDic = new Dictionary<string, object>();
    //    paramDic.Add("@UserID", currentID);
    //    paramDic.Add("@NewPass", newPass);



    //    cmd = CreateCommandWithStoredProcedureGeneral("SP_UpdateUserPassOsh", con, paramDic);          // create the command

    //    try
    //    {
    //        int numEffected = cmd.ExecuteNonQuery(); // execute the command
    //        return numEffected;
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    finally
    //    {
    //        if (con != null)
    //        {
    //            // close the db connection
    //            con.Close();
    //        }
    //    }

    //}
    //--------------------------------------------------------------------------------------------------
    // This method Login a user  
    //--------------------------------------------------------------------------------------------------
    //public Userr userLogin(string email, string password)
    //{
    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex; // Log the exception if needed
    //    }

    //    Dictionary<string, object> paramDic = new Dictionary<string, object>();
    //    paramDic.Add("@Email", email);
    //    paramDic.Add("@Password", password);

    //    cmd = CreateCommandWithStoredProcedureGeneral("SP_LoginUserOsh", con, paramDic);

    //    try
    //    {
    //        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

    //        Userr u = new Userr();
    //        if (reader.Read())
    //        {
    //            u.Id = Convert.ToInt32(reader["Id"]);
    //            u.Name = reader["Name"].ToString();
    //            u.Email = reader["Email"].ToString();
    //            u.Password = reader["Password"].ToString();
    //            u.IsActive = reader["isActive"] != DBNull.Value && Convert.ToBoolean(reader["isActive"]);
    //        }
    //        return u;
    //    }
    //    //AppID = Convert.ToInt32(reader["AppId"])
    //    catch (Exception ex)
    //    {
    //        throw ex; // Log the exception if needed
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close(); // Close the DB connection
    //        }
    //    }
    //}


    //--------------------------------------------------------------------------------------------------
    // This method Read all users BI 
    //--------------------------------------------------------------------------------------------------
    //public object UsersBI()
    //{
    //    SqlConnection con;
    //    SqlCommand cmd;

    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }

    //    List<object> userBITable = new List<object>();

    //    try
    //    {
    //        Dictionary<string, object> paramDic = new Dictionary<string, object>
    //        {
    //        };
    //        cmd = CreateCommandWithStoredProcedureGeneral("SP_GetUsersBIOsh", con, paramDic); // create the command
    //        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

    //        while (reader.Read())
    //        {
    //            var o = new Dictionary<string, object>
    //            {
    //                { "Id", Convert.ToInt32(reader["Id"]) },
    //                { "Name", reader["Name"].ToString() },
    //                { "GamesPurchased", Convert.ToInt32(reader["GamesPurchased"]) },
    //                { "MoneySpent", Convert.ToDouble(reader["MoneySpent"]) },
    //                { "IsActive", reader["isActive"] != DBNull.Value && Convert.ToBoolean(reader["isActive"]) }

    //            };

    //            userBITable.Add(o);
    //        }

    //        reader.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw (ex);
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close(); // close the db connection
    //        }
    //    }

    //    return userBITable;
    //}
    //--------------------------------------------------------------------------------------------------
    // This method Read all Games BI 
    //--------------------------------------------------------------------------------------------------
    //public object GamesBI()
    //{
    //    SqlConnection con;
    //    SqlCommand cmd;
    //    try
    //    {
    //        con = connect("myProjDB"); // create the connection
    //    }
    //    catch (Exception ex)
    //    {
    //        // write to log
    //        throw (ex);
    //    }
    //    List<object> gameBITable = new List<object>();
    //    try
    //    {
    //        Dictionary<string, object> paramDic = new Dictionary<string, object>
    //        {
    //        };
    //        cmd = CreateCommandWithStoredProcedureGeneral("SP_GetGamesBIOsh", con, paramDic); // create the command
    //        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
    //        while (reader.Read())
    //        {
    //            var o = new Dictionary<string, object>
    //            {
    //                { "AppId", Convert.ToInt32(reader["AppId"]) },
    //                { "Name", reader["Name"].ToString() },
    //                { "NumberOfPurchases", Convert.ToInt32(reader["NumberOfPurchases"]) },
    //                { "Income", Convert.ToDouble(reader["Income"]) }
    //            };
    //            gameBITable.Add(o);
    //        }
    //        reader.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw (ex);
    //    }
    //    finally
    //    {
    //        if (con != null)
    //        {
    //            con.Close(); // close the db connection
    //        }
    //    }
    //    return gameBITable;
    //}

}

