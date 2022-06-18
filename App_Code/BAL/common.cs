using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Net;
using System.Configuration;
namespace IN_eFDR
{
    public class common
    {
        public DataSet GetDoc(string FILE_NAME, string FILE_PATH)
        {
            string TEMP_PATH = "";
            string FILE_LOCATION = "";
            if (!string.IsNullOrEmpty(FILE_PATH))
            {
                TEMP_PATH = FILE_PATH;
                FILE_LOCATION = TEMP_PATH;
            }
            else
            {
                TEMP_PATH = "";// Sy_f_Get_Deposits_Path();
                //'TEMP_PATH = "\\HDFCJOGNAS2\SW_PRODCN\TEMP\"
                FILE_LOCATION = TEMP_PATH + FILE_NAME;
            }

            byte[] bytes = null;
            byte[] binFile = null;
            string filename = FILE_NAME;
            BinaryReader binReader = new BinaryReader(File.Open(FILE_LOCATION, FileMode.Open, FileAccess.Read, FileShare.None));
            binReader.BaseStream.Position = 0;
            binFile = binReader.ReadBytes(Convert.ToInt32(binReader.BaseStream.Length));
            binReader.Close();
            bytes = binFile;

            if (bytes.ToString().Length > 0)
            {
                return GetByte("1", "OK", binFile, filename);
            }
            else
            {
                return GetByte("-1", "NOK", null, "");
            }
        }

        public DataSet GetByte(string ID, string MESSAGE, byte[] OBYTE, string FILE_NAME)
        {
            DataSet ds = new DataSet();
            string str = "";
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("RETURN_CODE"));
            dt.Columns.Add(new DataColumn("MESSAGE"));
            dt.Columns.Add(new DataColumn("FILE_NAME"));
            dt.Columns.Add(new DataColumn("FILE_DATA", System.Type.GetType("System.Byte[]")));
            dt.Columns.Add(new DataColumn("BASE64STRING"));//Added by Amresh on 17/10/2016

            DataRow dr = default(DataRow);
            dr = dt.NewRow();
            dr["RETURN_CODE"] = ID;
            dr["MESSAGE"] = MESSAGE;
            dr["FILE_NAME"] = FILE_NAME;
            if (OBYTE == null)
            {
                dr["FILE_DATA"] = "";
                dr["BASE64STRING"] = "";
            }
            else
            {
                dr["FILE_DATA"] = OBYTE;
                dr["BASE64STRING"] = Convert.ToBase64String(OBYTE);
            }
            dt.Rows.Add(dr);
            dt.TableName = "MESSAGE";
            ds.Tables.Add(dt);
            return ds;
            //String base64String = Base64.encodeBase64String(bytes);
        }


        public bool DownloadFileUsingFTP(string sDestinationPath, string sFileName, string sFTPHost, string sTempPath, string sUser, string sPwd, ref string err_msg)
        {
            try
            {
                string URI = sFTPHost + sFileName;
                System.Net.FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(URI);

                ftp.Credentials = new System.Net.NetworkCredential(sUser, sPwd);
                ftp.KeepAlive = false;
                ftp.UseBinary = true;

                using (System.Net.FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
                {
                    using (System.IO.Stream responseStream = response.GetResponseStream())
                    {
                        //loop to read & write to file
                        using (System.IO.FileStream fs = new System.IO.FileStream(sTempPath + sFileName, System.IO.FileMode.Create))
                        {
                            byte[] buffer = new byte[fs.Length + 1];
                            int read = 0;
                            do
                            {
                                read = responseStream.Read(buffer, 0, buffer.Length);
                                fs.Write(buffer, 0, read);
                            } while (!(read == 0));
                            responseStream.Close();
                            fs.Flush();
                            fs.Close();
                        }
                        responseStream.Close();
                    }
                    response.Close();
                }

                if (sDestinationPath.ToString() != "")
                {
                    try
                    {
                        return UploadFileUsingFTP(sDestinationPath, sFileName, sTempPath, sUser, sPwd, ref err_msg);
                    }
                    catch (Exception ex)
                    {
                        err_msg = "ERROR..!" + ex.Message.ToString();
                        return false;
                        // return "N" + "-DL - " + ex.Message + sDestinationPath + " - " + sFileName + " - " + sFTPHost;
                    }
                }
                else
                {
                    err_msg = "";
                    return true;
                }
            }
            catch (Exception ex)
            {
                err_msg = "ERROR..!" + ex.Message.ToString();
                return false;
            }
        }


        public bool UploadFileUsingFTP(string sDestinationPath, string sFileName, string sSourcePath, string sUser, string sPwd, ref string err_msg)
        {
            err_msg = "";
            try
            {
                if (!string.IsNullOrEmpty(sDestinationPath) & !string.IsNullOrEmpty(sFileName) & !string.IsNullOrEmpty(sSourcePath) & !string.IsNullOrEmpty(sUser) & !string.IsNullOrEmpty(sPwd))
                {
                    //string strNetworkPath = null;
                    //Create a FTP Request Object and Specfiy a Complete Path 
                    FtpWebRequest reqObj = null;

                    //FileStream object read file from Local Drive
                    FileStream streamObj = null;

                    try
                    {
                        //Create a FTP Request Object and Specfiy a Complete Path 
                        //reqObj = WebRequest.Create(sDestinationPath + sFileName);
                        reqObj = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(new Uri(sDestinationPath + sFileName));

                        //Call A FileUpload Method of FTP Request Object
                        reqObj.Method = WebRequestMethods.Ftp.UploadFile;
                        //.DownloadFile 

                        //If you want to access Resourse Protected You need to give User Name and PWD
                        reqObj.Credentials = new NetworkCredential(sUser, sPwd);

                        //FileStream object read file from Local Drive
                        streamObj = new FileStream(sSourcePath + sFileName, FileMode.Open, FileAccess.Read);

                        //Store File in Buffer
                        byte[] buffer = new byte[streamObj.Length + 1];
                        int bytesRead = 0;
                        int bytesTotalWritten = 0;

                        Stream uploadStream = reqObj.GetRequestStream();
                        while ((true))
                        {
                            bytesRead = streamObj.Read(buffer, 0, buffer.Length);
                            if ((bytesRead == 0))
                                break; // TODO: might not be correct. Was : Exit While
                            //uploadStream.Write(buffer, 0, bytesRead);
                            uploadStream.Write(buffer, 0, bytesRead);

                            bytesTotalWritten = bytesTotalWritten + bytesRead;
                        }
                        //Close the upload stream
                        uploadStream.Close();
                        uploadStream.Dispose();

                        //Close FileStream Object Set its Value to nothing
                        streamObj.Close();
                        streamObj = null;
                        reqObj = null;
                        try
                        {
                            // System.IO.File.Delete(sSourcePath + sFileName);
                        }
                        catch (Exception ex)
                        {
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        //Close FileStream Object Set its Value to nothing
                        streamObj.Close();
                        streamObj = null;
                        reqObj = null;
                        err_msg = "ERROR..!" + ex.Message.ToString();
                        return false;
                        //return "N" + "-UP - " + ex.Message + sDestinationPath + " - " + sFileName + " - " + sSourcePath + " - " + sUser + " - " + sPwd;
                    }
                }
                else
                {
                    err_msg = "ERROR..!" + " Incomplete Inputs => ";
                    return false;
                    //return "N" + "-UP - Incomplete Inputs => " + sDestinationPath + " - " + sFileName + " - " + sSourcePath + " - " + sUser + " - " + sPwd;
                }
            }
            catch (Exception ex)
            {
                err_msg = "ERROR..!" + ex.Message.ToString();
                return false;
                //return "N" + "-UP - " + ex.Message + sDestinationPath + " - " + sFileName + " - " + sSourcePath + " - " + sUser + " - " + sPwd;
            }
        }



        //Below not  in use- start here ----------------

        public bool EmailDownloadFileUsingFTP(string sDestinationPath, string sFileName, string sFTPHost, string sTempPath, string sUser, string sPwd, ref string err_msg)
        {
            err_msg = "";
            try
            {
                string username = System.Configuration.ConfigurationManager.AppSettings["USER"];
                string password = System.Configuration.ConfigurationManager.AppSettings["PWD"];
                string URI = sFTPHost + sFileName;
                System.Net.FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(URI);

                //ftp.Credentials = new System.Net.NetworkCredential(username, password);
                ftp.Credentials = new System.Net.NetworkCredential(sUser, sPwd); //added by aaaaaaaa

                ftp.KeepAlive = false;
                ftp.UseBinary = true;

                using (System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)ftp.GetResponse())
                {
                    using (System.IO.Stream responseStream = response.GetResponseStream())
                    {
                        //loop to read & write to file
                        using (System.IO.FileStream fs = new System.IO.FileStream(sTempPath + sFileName, System.IO.FileMode.Create))
                        {
                            //Using fs As New IO.FileStream(sDestinationPath & sFileName, IO.FileMode.Create)
                            byte[] buffer = new byte[fs.Length + 1];
                            int read = 0;
                            do
                            {
                                read = responseStream.Read(buffer, 0, buffer.Length);
                                fs.Write(buffer, 0, read);
                            } while (!(read == 0));
                            responseStream.Close();
                            fs.Flush();
                            fs.Close();
                        }
                        responseStream.Close();
                    }
                    response.Close();
                }



                try
                {
                    return EmailUploadFileUsingFTP(sDestinationPath, sFileName, sTempPath, sUser, sPwd, ref err_msg);
                }
                catch (Exception ex)
                {
                    err_msg = "ERROR..!" + ex.Message.ToString();
                    return false;
                    // return "N" + "-DL - " + ex.Message + sDestinationPath + " - " + sFileName + " - " + sFTPHost;
                }
            }
            catch (Exception ex)
            {
                err_msg = "ERROR..!" + ex.Message.ToString();
                return false;
                //return "N" + "-DL - " + ex.Message + sDestinationPath + " - " + sFileName + " - " + sFTPHost;
            }
        }


        public bool EmailUploadFileUsingFTP(string sDestinationPath, string sFileName, string sSourcePath, string sUser, string sPwd, ref string err_msg)
        {
            err_msg = "";
            try
            {
                if (!string.IsNullOrEmpty(sDestinationPath) & !string.IsNullOrEmpty(sFileName) & !string.IsNullOrEmpty(sSourcePath) & !string.IsNullOrEmpty(sUser) & !string.IsNullOrEmpty(sPwd))
                {
                    //string strNetworkPath = null;
                    //Create a FTP Request Object and Specfiy a Complete Path 
                    FtpWebRequest reqObj = null;

                    //FileStream object read file from Local Drive
                    FileStream streamObj = null;

                    try
                    {
                        //Create a FTP Request Object and Specfiy a Complete Path 
                        //reqObj = WebRequest.Create(sDestinationPath + sFileName);
                        reqObj = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(new Uri(sDestinationPath + sFileName));

                        //Call A FileUpload Method of FTP Request Object
                        reqObj.Method = WebRequestMethods.Ftp.UploadFile;
                        //.DownloadFile 

                        //If you want to access Resourse Protected You need to give User Name and PWD
                        reqObj.Credentials = new NetworkCredential(sUser, sPwd);

                        //FileStream object read file from Local Drive
                        streamObj = new FileStream(sSourcePath + sFileName, FileMode.Open, FileAccess.Read);

                        //Store File in Buffer
                        byte[] buffer = new byte[streamObj.Length + 1];
                        int bytesRead = 0;
                        int bytesTotalWritten = 0;

                        Stream uploadStream = reqObj.GetRequestStream();
                        while ((true))
                        {
                            bytesRead = streamObj.Read(buffer, 0, buffer.Length);
                            if ((bytesRead == 0))
                                break; // TODO: might not be correct. Was : Exit While
                            //uploadStream.Write(buffer, 0, bytesRead);
                            uploadStream.Write(buffer, 0, bytesRead);

                            bytesTotalWritten = bytesTotalWritten + bytesRead;
                        }
                        //Close the upload stream
                        uploadStream.Close();
                        uploadStream.Dispose();

                        //Close FileStream Object Set its Value to nothing
                        streamObj.Close();
                        streamObj = null;
                        reqObj = null;
                        try
                        {
                            System.IO.File.Delete(sSourcePath + sFileName);
                        }
                        catch (Exception ex)
                        {
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        //Close FileStream Object Set its Value to nothing
                        streamObj.Close();
                        streamObj = null;
                        reqObj = null;
                        err_msg = "ERROR..!" + ex.Message.ToString();
                        return false;
                        //return "N" + "-UP - " + ex.Message + sDestinationPath + " - " + sFileName + " - " + sSourcePath + " - " + sUser + " - " + sPwd;
                    }
                }
                else
                {
                    err_msg = "ERROR..!" + " Incomplete Inputs => ";
                    return false;
                    //return "N" + "-UP - Incomplete Inputs => " + sDestinationPath + " - " + sFileName + " - " + sSourcePath + " - " + sUser + " - " + sPwd;
                }
            }
            catch (Exception ex)
            {
                err_msg = "ERROR..!" + ex.Message.ToString();
                return false;
                //return "N" + "-UP - " + ex.Message + sDestinationPath + " - " + sFileName + " - " + sSourcePath + " - " + sUser + " - " + sPwd;
            }
        }
        //Below not  in use- End here ----------------


        public bool check_ftp()
        {
            string FTPServer = ConfigurationManager.AppSettings["FTPPATH"].ToString();
            string FTPUserID = ConfigurationManager.AppSettings["FTPUserID"].ToString();
            string FTPPaswword = ConfigurationManager.AppSettings["FTPPassword"].ToString();
            FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create("ftp://ftpsrv.hdfc.com");
            requestDir.Credentials = new NetworkCredential(FTPUserID, FTPPaswword);
            try
            {
                WebResponse response = requestDir.GetResponse();
                //set your flag
                return true;
            }
            catch (Exception EX)
            {
                return false;
            }

        }
    }
}