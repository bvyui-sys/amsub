using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;



namespace Attendance_Monitoring_System
{
    class MySQL
    {

        static readonly string DatabaseServer = Properties.Settings.Default.ServerIP;
        static readonly string MySQLPort = Properties.Settings.Default.Port;
        static readonly string Database = Properties.Settings.Default.Schema;
        static readonly string UserID = Properties.Settings.Default.Username;
        static readonly string Password = Properties.Settings.Default.Password;


        public static readonly string ConnectionString = "Server=" + DatabaseServer + ";Port=" + MySQLPort + ";Database=" + Database + ";user id=" + UserID + ";password=" + Password + " ; Connection Reset=true; SslMode=None; AllowPublicKeyRetrieval=True; Convert Zero Datetime=True";

        public static void CreateAttendanceTable()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS attendance (
                            id INT AUTO_INCREMENT PRIMARY KEY,
                            employee_id INT NOT NULL,
                            attendance_date DATE NOT NULL,
                            time_in DATETIME NULL,
                            time_out DATETIME NULL,
                            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                            updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                            FOREIGN KEY (employee_id) REFERENCES employees(ID) ON DELETE CASCADE,
                            INDEX idx_employee_date (employee_id, attendance_date),
                            INDEX idx_attendance_date (attendance_date)
                        )";
                    
                    using (MySqlCommand cmd = new MySqlCommand(createTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating attendance table: " + ex.Message);
            }
        }
        //static readonly string ConnectionString = "Server=127.0.0.1;Port=8000;Database=crud;user id=root;password=vincemgrm;" +
        //"Connection Reset=true;SslMode=None;AllowPublicKeyRetrieval=True;Convert Zero Datetime=True";


        public static bool IsConnected()
        {

            MySqlConnection cnnDBConnection = new MySqlConnection(ConnectionString);
            try
            {
                cnnDBConnection.Open();
                cnnDBConnection.Dispose();
                return true;
            }
            catch
            {
                cnnDBConnection.Dispose();
                return false;
            }

        }
        public static void Push(string SQLStatement)
        {
            MySqlConnection xSqlConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = SQLStatement,
                Connection = xSqlConnection
            };
            xSqlConnection.Open();
            cmd.ExecuteNonQuery();
            xSqlConnection.Close();
            cmd.Dispose();
        }
        public static DataTable Pull(string SQLStatement)
        {
            using (MySqlConnection SQLConn = new MySqlConnection(ConnectionString))
            {
                MySqlDataAdapter SqlDA = new MySqlDataAdapter();
                DataTable d = new DataTable();
                SQLConn.Open();
                try
                {
                    SqlDA.SelectCommand = new MySqlCommand(SQLStatement, SQLConn);
                    SqlDA.Fill(d);
                }
                catch { }
                SQLConn.Close();
                SqlDA.Dispose();
                d.Dispose();
                return d;
            }

        }
        public static int GetCount(string SQLStatement)
        {
            MySqlConnection SQLConn = new MySqlConnection(ConnectionString);
            MySqlDataAdapter SqlDA = new MySqlDataAdapter();
            DataSet myDataSet = new DataSet();
            SQLConn.Open();
            try
            {
                SqlDA.SelectCommand = new MySqlCommand(SQLStatement, SQLConn);
                SqlDA.Fill(myDataSet);
            }
            catch { }
            SQLConn.Close();
            SqlDA.Dispose();
            myDataSet.Dispose();
            return myDataSet.Tables[0].Rows.Count;
        }
        public static void Execute(string query, Dictionary<string, object> parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }

}

