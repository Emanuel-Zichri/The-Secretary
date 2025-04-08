using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using FinalProject;
using FinalProject.BL;


/// <summary>
/// DBServices is a class created by me to provides some DataBase Services
/// </summary>
public class DBservices
{

    public DBservices(){ }

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
            throw (ex);
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
        cmd = CreateCommandWithStoredProcedureGeneral("InsertCustomer", con, paramDic);          // create the command
        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
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
    public int InsertSpaceDetails(SpaceDetails space)
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
        { "@RequestID", space.RequestID },
        { "@Size", space.Size },
        { "@FloorType", space.FloorType },
        { "@MediaURL", space.MediaURL },
        { "@Notes", space.Notes }
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
    public int InsertWorkRequest(WorkRequest request)
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

        cmd.Parameters.AddWithValue("@CustomerID", request.CustomerID);
        cmd.Parameters.AddWithValue("@Status", request.Status ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Notes", request.Notes ?? (object)DBNull.Value);

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
