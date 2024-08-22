using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Data;
using Sintec.Tool;
using System.Windows.Forms;

namespace KaraStart
{
    public class Database
    {
        private static void closeConnection(SqlCeConnection cn)
        {
            if (cn!=null && cn.State != ConnectionState.Closed)
            {
                cn.Close();
            }
        }
        private static int tableSearch(String query, out DataTable dataTable, out SqlCeConnection cn)// throws SqlException;
        {
            dataTable = new DataTable();
            string connectionString = "Data Source=" + AppSettingsManager.Istance.getAppSettings("dataBase") + ";Persist Security Info=True";
            //MessageBox.Show(connectionString);
            //MessageBox.Show(AppSettingsManager.Istance.getAppSettings("dataBase"));
            cn = new SqlCeConnection(connectionString);
            SqlCeDataAdapter da = new SqlCeDataAdapter(query, cn);
            cn.Open();
            int recordsAffected = da.Fill(dataTable);
            return recordsAffected;
        }

        public static void saveRating(System.Guid id, double rating)
        {
            SqlCeConnection cn = null;
            DataTable dataTable;
            Guid idFound = Guid.Empty;
            if (tableSearch("SELECT * FROM fileData where idFile LIKE '" + id.ToString() + "' ORDER BY rating", out dataTable, out cn) > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    idFound = (System.Guid)dr[0];
                    break;
                }
            }
            if (idFound != Guid.Empty)
            {
                SqlCeCommand cmd =
                     new SqlCeCommand("UPDATE fileData SET rating=" + rating.ToString("0.00").Replace(",", ".") + "  where idFile LIKE '" + id.ToString() + "'", cn);
                cmd.ExecuteNonQuery();
            }
            else
            {
                SqlCeCommand cmd =
                       new SqlCeCommand("INSERT INTO fileData (idFile, rating, volume) " +
                           " VALUES ('" + id.ToString() + "'," + rating.ToString("0.00").Replace(",", ".") + ",100.0)", cn);
                cmd.ExecuteNonQuery();
            }
        }

        public static long saveLastStarting(System.Guid id, DateTime date)
        {
            long res = 1;
            SqlCeConnection cn = null;
            DataTable dataTable;
            Guid idFound = Guid.Empty;
            if (tableSearch("SELECT * FROM fileData where idFile LIKE '" + id.ToString() + "' ORDER BY rating", out dataTable, out cn) > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    idFound = (System.Guid)dr[0];
                    res = (long)dr[4];
                    break;
                }
            }
            if (idFound != Guid.Empty)
            {
                res++;
                SqlCeCommand cmd =
                     new SqlCeCommand("UPDATE fileData SET lastStartDate=@dateVal, usedNumberTimes=" + res + " where idFile LIKE '" + id.ToString() + "'", cn);
                // Add the parameters
                cmd.Parameters.Add(new SqlCeParameter("@dateVal", date));
                cmd.ExecuteNonQuery();
            }
            else
            {
                SqlCeCommand cmd =
                       new SqlCeCommand("INSERT INTO fileData (idFile, lastStartDate, usedNumberTimes, volume) " +
                           " VALUES ('" + id.ToString() + "',@dateVal,1,100.0)", cn);
                cmd.Parameters.Add(new SqlCeParameter("@dateVal", date));
                cmd.ExecuteNonQuery();
            }
            return res;
        }

        public static void saveVolume(System.Guid id, double volume)
        {
            SqlCeConnection cn = null;
            DataTable dataTable;
            Guid idFound = Guid.Empty;
            if (tableSearch("SELECT * FROM fileData where idFile LIKE '" + id.ToString() + "' ORDER BY rating", out dataTable, out cn) > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    idFound = (System.Guid)dr[0];
                    break;
                }
            }
            if (idFound != Guid.Empty)
            {
                SqlCeCommand cmd =
                     new SqlCeCommand("UPDATE fileData SET volume=" + volume.ToString("0.00").Replace(",", ".") + "  where idFile LIKE '" + id.ToString() + "'", cn);
                cmd.ExecuteNonQuery();
            }
            else
            {
                SqlCeCommand cmd =
                       new SqlCeCommand("INSERT INTO fileData (idFile, volume) " +
                           " VALUES ('" + id.ToString() + "'," + volume.ToString("0.00").Replace(",", ".") + ")", cn);
                cmd.ExecuteNonQuery();
            }
        }

        public static int getRank(System.Guid id)
        {
            SqlCeConnection cn = null;
            DataTable dataTable;
            if (tableSearch("SELECT rating FROM fileData where idFile LIKE '" + id.ToString() + "' ORDER BY rating", out dataTable, out cn) > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    return (int)Math.Round((float)dr[0] / 0.2);
                }
            }
            return 0;
        }

        public static void saveProgramName(System.Guid id, string progName)
        {
            SqlCeConnection cn = null;
            DataTable dataTable;
            Guid idFound = Guid.Empty;
            if (tableSearch("SELECT * FROM fileData where idFile LIKE '" + id.ToString() + "' ORDER BY rating", out dataTable, out cn) > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    idFound = (System.Guid)dr[0];
                    break;
                }
            }
            if (idFound != Guid.Empty)
            {
                SqlCeCommand cmd =
                     new SqlCeCommand("UPDATE fileData SET programNameLastStart ='" + progName + "'  where idFile LIKE '" + id.ToString() + "'", cn);
                cmd.ExecuteNonQuery();
            }
            else
            {
                SqlCeCommand cmd =
                       new SqlCeCommand("INSERT INTO fileData (idFile, programNameLastStart, volume) " +
                           " VALUES ('" + id.ToString() + "','" + progName + "',100.0)", cn);
                cmd.ExecuteNonQuery();
            }
        }

        public static FileData fileDataOf(System.Guid id)
        {
            SqlCeConnection cn = null;
            DataTable dataTable;
            FileData fd = null;
            try
            {

                //MessageBox.Show("SELECT * FROM fileData where idFile LIKE '" + id.ToString() + "' ORDER BY rating");
                if (tableSearch("SELECT * FROM fileData where idFile LIKE '" + id.ToString() + "' ORDER BY rating", out dataTable, out cn) > 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        fd = new FileData((System.Guid)dr[0], dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString());
                        break;
                    }
                }
            }
            catch (SqlCeException e)
            {
                string msg = "";
                for (int i = 0; i < e.Errors.Count; i++)
                {
                    msg += "Error #" + i + " Message: " + e.Errors[i].Message + "\n";
                }
                System.Console.WriteLine(msg);
                //MessageBox.Show(e.Message + "\n\r" + msg);
            }
            finally
            {
                closeConnection(cn);
            }
            if (fd == null)
            {
                fd = new FileData(id, 100.0F, "" , DateTime.MinValue, 0, 0.0F);
            }
            return fd;
        }
    }
    public class FileData
    {
        public Guid idFile;
        public float volume;
        public string programNameLastStart;
        public DateTime lastStartDate;
        public long usedNumberTimes;
        public float rating;
        public FileData(Guid idFile, float volume, string programNameLastStart, DateTime lastStartDate, long usedNumberTimes, float rating)
        {
            this.idFile = idFile;
            this.volume = volume;
            this.programNameLastStart = programNameLastStart;
            this.lastStartDate=lastStartDate;
            this.usedNumberTimes=usedNumberTimes;
            this.rating=rating;
        }
        public FileData(Guid idFile, string volume, string programNameLastStart, string lastStartDate, string usedNumberTimes, string rating)
        {
            this.idFile = idFile;
            this.volume = float.Parse("0" + volume.ToString());
            this.programNameLastStart = programNameLastStart;
            if (lastStartDate != "")
                this.lastStartDate = DateTime.Parse(lastStartDate);
            else
                this.lastStartDate = DateTime.MinValue;
            this.usedNumberTimes = long.Parse("0" + usedNumberTimes.ToString());
            this.rating = float.Parse("0" + rating.ToString());
        }
    }
}
