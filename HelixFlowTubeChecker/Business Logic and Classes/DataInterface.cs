
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.IO;
using System.ComponentModel;


public class DataInterface
{
    public DataInterface(string connectionString)
    {
        this.ConnectionString = connectionString;
    }

    public string ConnectionString { get; private set; }
    public bool LimitTubeCount { get; set; }
    public int LimitTubeMax { get; set; }

    public void InsertEventLog(string EventMessage,
                                string EventType,
                                string EventStack = "",
                                string EventLocation = "",
                                string EventSource = "",
                                string EventUsername = ""
        )
    {

        if (EventUsername == "")
            EventUsername = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
        using (SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandText = "InsertEventLog",
            CommandType = CommandType.StoredProcedure
        })
        {
            command.Parameters.AddWithValue("@EventMessage", EventMessage);
            command.Parameters.AddWithValue("@EventStack", EventStack);
            command.Parameters.AddWithValue("@EventType", EventType);
            command.Parameters.AddWithValue("@EventLocation", EventLocation);
            command.Parameters.AddWithValue("@EventSource", EventSource);
            command.Parameters.AddWithValue("@EventUsername", EventUsername);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

    }


    public void UpdateVerficationInfo(FlowTubeList list)
   
    {

        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
        using (SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandText = "UpdateVerificationInfo",
            CommandType = CommandType.StoredProcedure
        })
        {
            command.Parameters.AddWithValue("@flow_tube_list_id", list.flow_tube_list_id);
            command.Parameters.AddWithValue("@verified_by", list.verified_by);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

    }

    public BindingList<FlowTube> GetFlowTubes(int flow_tube_list_id)
    {
        BindingList<FlowTube> output = new BindingList<FlowTube>();
        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
        using (SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandText = "GetFlowTubes",
            CommandType = CommandType.StoredProcedure
        })
        {
            command.Parameters.AddWithValue("@flow_tube_list_id", flow_tube_list_id);
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            connection.Open();
            adapter.Fill(dataSet);
            connection.Close();


            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                FlowTube tube = new FlowTube();
                tube.AccessionNumber = row["accession_number"].ToString();
                tube.SeqNbr = Convert.ToInt32(row["sequence_number"]);
                if (this.LimitTubeCount && tube.SeqNbr > this.LimitTubeMax) break;
                tube.TubeLabel = row["label"].ToString();
                tube.MRN = row["MRN"].ToString();
                tube.Name = row["name"].ToString();
                output.Add(tube);
            }
        }
        return output;
    }

    public List<FlowTubeList> GetMatchingTubeLists(string barcode)
    {
        List<FlowTubeList> output = new List<FlowTubeList>();
        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
        using (SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandText = "GetMatchingTubeLists",
            CommandType = CommandType.StoredProcedure
        })
        {
            command.Parameters.AddWithValue("@first_accession_number", barcode);
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            connection.Open();
            adapter.Fill(dataSet);
            connection.Close();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                FlowTubeList info = new FlowTubeList(row["path"].ToString(), false, this);
                info.filename = row["filename"].ToString();
                info.directory = row["directory"].ToString();
                info.is_newly_uploaded = false;
                info.flow_tube_list_id = Convert.ToInt32(row["flow_tube_list_id"]);
                info.date_parsed = Convert.ToDateTime(row["date_parsed"]);
                info.date_created = Convert.ToDateTime(row["date_created"]);
                if (!row["date_verified"].Equals(DBNull.Value))
                    info.date_verified = Convert.ToDateTime(row["date_verified"]);
                if (!row["verified_by"].Equals(DBNull.Value))
                    info.verified_by = row["verified_by"].ToString();
                info.first_tube_barcode = row["first_tube_barcode"].ToString();
                output.Add(info);
            }
        }


        return output;
    }


    public DataTable  GetRecentTubeLists ()
    {

        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
        using (SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandText = "GetRecentTubeLists",
            CommandType = CommandType.StoredProcedure
        })
        {
          
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            connection.Open();
            adapter.Fill(dataSet);
            connection.Close();
            return dataSet.Tables[0];

        }

 
    }

    public void GetFlowTubeListID(FlowTubeList info)
    {

        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
        using (SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandText = "GetFlowTubeListID",
            CommandType = CommandType.StoredProcedure
        })
        {
            command.Parameters.AddWithValue("@filename", info.filename);
            command.Parameters.AddWithValue("@path", info.path);
            command.Parameters.AddWithValue("@directory", info.directory);
            command.Parameters.AddWithValue("@file_hash", info.file_hash);
            command.Parameters.AddWithValue("@file_data", info.file_data);
            command.Parameters.AddWithValue("@is_valid", info.is_valid);
            command.Parameters.AddWithValue("@date_created", info.date_created);
            command.Parameters.AddWithValue("@first_tube_barcode", info.first_tube_barcode);
            command.Parameters.AddWithValue("@containing_directory", info.containing_directory);
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            connection.Open();
            adapter.Fill(dataSet);
            connection.Close();
            DataRow row = dataSet.Tables[0].Rows[0];
            info.flow_tube_list_id = Convert.ToInt32(row["flow_tube_list_id"]);
            info.is_newly_uploaded = Convert.ToBoolean(row["is_new_tube_list"]);
        }

    }

    public int? GetFlowTubeListMatchingHash(string file_hash )
    {

        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
        using (SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandText = "GetFlowTubeListMatchingHash",
            CommandType = CommandType.StoredProcedure
        })
        {
            command.Parameters.AddWithValue("@file_hash", file_hash);
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            connection.Open();
            adapter.Fill(dataSet);
            connection.Close();
            if (dataSet.Tables[0].Rows.Count == 0) return null;
            DataRow row = dataSet.Tables[0].Rows[0];
           int flow_tube_list_id =  Convert.ToInt32(row["flow_tube_list_id"]);
           return flow_tube_list_id;
        }

    }


    public void DeleteAllFlowData(string confirmation)
    {
        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
        using (SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandText = "DeleteAllFlowData",
            CommandType = CommandType.StoredProcedure
        })
        {
            command.Parameters.AddWithValue("@confirmation", confirmation);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public void InsertFlowTubes(BindingList<FlowTube> tubes, FlowTubeList list_info)
    {

        DataTable dt;
        // create data table to insert items
        dt = new DataTable("Tubes");
        dt.Columns.Add("flow_tube_list_id", typeof(int));
        dt.Columns.Add("label", typeof(string));
        dt.Columns.Add("MRN", typeof(string));
        dt.Columns.Add("name", typeof(string));
        dt.Columns.Add("accession_number", typeof(string));
        dt.Columns.Add("sequence_number", typeof(int));
        foreach (var r in tubes)
            dt.Rows.Add(list_info.flow_tube_list_id, r.TubeLabel, r.MRN, r.Name, r.AccessionNumber, r.SeqNbr);

        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
        using (SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandText = "InsertFlowTubes",
            CommandType = CommandType.StoredProcedure
        })
        {
            SqlParameter tvpParam = command.Parameters.AddWithValue("@tube_list", dt);
            tvpParam.SqlDbType = SqlDbType.Structured;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }




    }




    public bool HasFileBeenParsed(string path)
    {

        int failureCount = 0;
        int sleepTime = 2000;
        bool success = false;
        Exception lastException = null;
        string data = null;

        do
        {
            try
            {
               data= File.ReadAllText(path);
                success = true;
            }
            catch (Exception ex)
            {
                lastException = ex;
                System.Threading.Thread.Sleep(sleepTime);
                failureCount++;
            }
        }
        while (!success && failureCount < 10);

        if (!success) throw lastException;

        if (data == null) return false;

        string hash = OncoSeek.Core.Shared.GetSHAHash(data);
        int? flow_tube_list_id = GetFlowTubeListMatchingHash(hash);
        return (flow_tube_list_id.HasValue);

    }


    public bool ParseFile(string path)
    {
        string filename = Path.GetFileName(path);
        if (filename.StartsWith("~"))
            return false;


        //if we have previously parsed this file, we will abort

        if (HasFileBeenParsed(path)) return false;

        FlowTubeList listinfo = new FlowTubeList(path, true, this);

        if (listinfo.exception != null)
            InsertEventLog(listinfo.exception.Message, "File parse error", path, Environment.MachineName, Environment.MachineName);

        if (listinfo.FlowTubes.Count >= 1)
            listinfo.first_tube_barcode = listinfo.FlowTubes[0].AccessionNumberUnformatted;
        this.GetFlowTubeListID(listinfo);
        if (listinfo.is_newly_uploaded && listinfo.is_valid)
        {
            this.InsertFlowTubes(listinfo.FlowTubes, listinfo);
            return true;
        }
        return false;
    }

    public List<string> ParseDirectory(string path)
    {

        if (!Directory.Exists(path)) throw new ApplicationException("Path " + path + " not found or insufficient priveleges");
        List<string> new_files = new List<string>();
        foreach (string filepath in Directory.GetFiles(path, "*.xml"))
        {
            if (ParseFile(filepath))
                new_files.Add(filepath);
        }
        foreach (string filepath in Directory.GetFiles(path, "*.xlsx"))
        {
            if (ParseFile(filepath))
                new_files.Add(filepath);
        }

        return new_files;

    }



}


