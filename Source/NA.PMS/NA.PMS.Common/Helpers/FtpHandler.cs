using NA.PMS.Common.Extension;
using NA.PMS.Model;
using NA.PMS.Model.NIC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Common
{
    public class FtpHandler
    {
        //static string ftpPath = "ftp://52.172.186.197/UploadDocuments/";
        static string ftpPath = ConfigurationManager.AppSettings["FTPPath"];
        static string ftpNDPath = ConfigurationManager.AppSettings["RootPath"];
        static string fTPPath_NM = ConfigurationManager.AppSettings["FTPPath_NM"];
        static string username = ConfigurationManager.AppSettings["FTPUsername"];
        static string password = ConfigurationManager.AppSettings["FTPPassword"];

        public static bool IsDirectoryExists(string path, string directory)
        {
            bool directoryExists;

            FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(path + "/" + directory));
            ftpClient.Credentials = new System.Net.NetworkCredential(username.Normalize(), password.Normalize());
            ftpClient.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
            try
            {
                using (ftpClient.GetResponse())
                {
                    directoryExists = true;
                }
            }
            catch (WebException)
            {
                directoryExists = false;
            }
            return directoryExists;
        }

        public static bool CreateDirectory(string path, string directory)
        {
            bool IsDirectoryCreated = false;

            FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(path + "/" + directory));
            ftpClient.Credentials = new System.Net.NetworkCredential(username.Normalize(), password.Normalize());
            ftpClient.Method = System.Net.WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                using (ftpClient.GetResponse())
                {
                    IsDirectoryCreated = true;
                }
            }
            catch (WebException)
            {
                IsDirectoryCreated = false;
            }
            return IsDirectoryCreated;
        }

        private static void UploadBinaryFiles(string RegistrationId, string filename, byte[] fileData)
        {
            bool IsDirectory = false;
            if (IsDirectoryExists(ftpPath, RegistrationId))
            {
                IsDirectory = true;
            }
            else
            {
                IsDirectory = CreateDirectory(ftpPath, RegistrationId);
            }

            if (IsDirectory == true)
            {
                FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath + "/" + RegistrationId + "/" + filename));
                ftpClient.Credentials = new System.Net.NetworkCredential(username.Normalize(), password.Normalize());
                ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

                ftpClient.ContentLength = fileData.Length;
                byte[] buffer = new byte[4097];
                int bytes = fileData.Length;
                int total_bytes = (int)fileData.Length;
                try
                {
                    System.IO.Stream rs = ftpClient.GetRequestStream();
                    while (total_bytes > 0)
                    {
                        rs.Write(fileData, 0, bytes);
                        total_bytes = total_bytes - bytes;
                    }
                    rs.Close();
                }
                catch (Exception e)
                {
                    throw e;
                }
                FtpWebResponse uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
                var value = uploadResponse.StatusDescription;
                uploadResponse.Close();
            }
        }

        private static void UploadBinaryFiles(string RegistrationId, string RequestId, string filename, byte[] fileData)
        {
            bool IsDirectory = false;
            bool IsServiceDirectory = false;
            if (IsDirectoryExists(ftpPath, RegistrationId))
            {
                IsDirectory = true;
                if (IsDirectoryExists(ftpPath + "/" + RegistrationId, RequestId))
                {
                    IsServiceDirectory = true;
                }
                else
                {
                    IsServiceDirectory = CreateDirectory(ftpPath + "/" + RegistrationId, RequestId);
                }
            }
            else
            {
                IsDirectory = CreateDirectory(ftpPath, RegistrationId);
                IsServiceDirectory = CreateDirectory(ftpPath + "/" + RegistrationId, RequestId);
            }

            if (IsDirectory == true && IsServiceDirectory == true)
            {
                //FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath + "/" + Convert.ToString(RegistrationId) + "/" + Convert.ToString(RequestId) + "/" + filename));
                FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath + "/" + RegistrationId + "/" + RequestId + "/" + filename));
                ftpClient.Credentials = new System.Net.NetworkCredential(username.Normalize(), password.Normalize());
                ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

                ftpClient.ContentLength = fileData.Length;
                byte[] buffer = new byte[4097];
                int bytes = fileData.Length;
                int total_bytes = (int)fileData.Length;
                try
                {
                    System.IO.Stream rs = ftpClient.GetRequestStream();
                    //System.IO.FileStream fs = fileData.op
                    while (total_bytes > 0)
                    {
                        rs.Write(fileData, 0, bytes);
                        total_bytes = total_bytes - bytes;
                    }
                    rs.Close();
                }
                catch (Exception e)
                {
                    throw e;
                }
                FtpWebResponse uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
                var value = uploadResponse.StatusDescription;
                uploadResponse.Close();
            }
        }

        private static void UploadNDBinaryFiles(string RegistrationId, string RequestId, string filename, byte[] fileData)
        {
            bool IsDirectory = false;
            bool IsServiceDirectory = false;
            if (IsDirectoryExists(ftpNDPath, RegistrationId))
            {
                IsDirectory = true;
                if (IsDirectoryExists(ftpNDPath + "/" + RegistrationId, RequestId))
                {
                    IsServiceDirectory = true;
                }
                else
                {
                    IsServiceDirectory = CreateDirectory(ftpNDPath + "/" + RegistrationId, RequestId);
                }
            }
            else
            {
                IsDirectory = CreateDirectory(ftpNDPath, RegistrationId);
                IsServiceDirectory = CreateDirectory(ftpNDPath + "/" + RegistrationId, RequestId);
            }

            if (IsDirectory == true && IsServiceDirectory == true)
            {
                //FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath + "/" + Convert.ToString(RegistrationId) + "/" + Convert.ToString(RequestId) + "/" + filename));
                FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpNDPath + "/" + RegistrationId + "/" + RequestId + "/" + filename));
                ftpClient.Credentials = new System.Net.NetworkCredential(username.Normalize(), password.Normalize());
                ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

                ftpClient.ContentLength = fileData.Length;
                byte[] buffer = new byte[4097];
                int bytes = fileData.Length;
                int total_bytes = (int)fileData.Length;
                try
                {
                    System.IO.Stream rs = ftpClient.GetRequestStream();
                    //System.IO.FileStream fs = fileData.op
                    while (total_bytes > 0)
                    {
                        rs.Write(fileData, 0, bytes);
                        total_bytes = total_bytes - bytes;
                    }
                    rs.Close();
                }
                catch (Exception e)
                {
                    throw e;
                }
                FtpWebResponse uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
                var value = uploadResponse.StatusDescription;
                uploadResponse.Close();
            }
        }

        public static void UploadBinaryFiles(string RegistrationId, string RequestId, string filename, byte[] fileData, string RefId)
        {
            bool IsDirectory = false;
            bool IsServiceDirectory = false;
            bool IsRefDirectory = false;
            if (IsDirectoryExists(ftpPath, RegistrationId))
            {
                IsDirectory = true;
                if (IsDirectoryExists(ftpPath + "/" + RegistrationId, RequestId))
                {
                    IsServiceDirectory = true;
                    if (IsDirectoryExists(ftpPath + "/" + RegistrationId + "/" + RequestId, RefId))
                    {
                        IsRefDirectory = true;
                    }
                    else
                    {
                        IsRefDirectory = CreateDirectory(ftpPath + "/" + RegistrationId + "/" + RequestId, RefId);
                    }
                }
                else
                {
                    IsServiceDirectory = CreateDirectory(ftpPath + "/" + RegistrationId, RequestId);
                    IsRefDirectory = CreateDirectory(ftpPath + "/" + RegistrationId + "/" + RequestId, RefId);
                }
            }
            else
            {
                IsDirectory = CreateDirectory(ftpPath, RegistrationId);
                IsServiceDirectory = CreateDirectory(ftpPath + "/" + RegistrationId, RequestId);
                IsRefDirectory = CreateDirectory(ftpPath + "/" + RegistrationId + "/" + RequestId, RefId);
            }

            if (IsDirectory == true && IsServiceDirectory == true && IsServiceDirectory == true)
            {
                //FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath + "/" + Convert.ToString(RegistrationId) + "/" + Convert.ToString(RequestId) + "/" + filename));
                FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath + "/" + RegistrationId + "/" + RequestId + "/" + RefId + "/" + filename));
                ftpClient.Credentials = new System.Net.NetworkCredential(username.Normalize(), password.Normalize());
                ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

                ftpClient.ContentLength = fileData.Length;
                byte[] buffer = new byte[4097];
                int bytes = fileData.Length;
                int total_bytes = (int)fileData.Length;
                try
                {
                    System.IO.Stream rs = ftpClient.GetRequestStream();
                    //System.IO.FileStream fs = fileData.op
                    while (total_bytes > 0)
                    {
                        rs.Write(fileData, 0, bytes);
                        total_bytes = total_bytes - bytes;
                    }
                    rs.Close();
                }
                catch (Exception e)
                {
                    throw e;
                }
                FtpWebResponse uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
                var value = uploadResponse.StatusDescription;
                uploadResponse.Close();
            }
        }

        public static bool UploadFiles(IEnumerable<HttpPostedFileBase> files, string registrationId)
        {
            var flag = false;
            var countIndex = 0;
            if (files != null && files.ToList().Count > 0)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }
                        var name = file.FileName;
                        string filename = StringExtensions.RemoveWhitespace(name);
                        UploadBinaryFiles(registrationId, filename, fileData);
                        countIndex++;
                    }
                }
            }
            return flag;
        }

        public static bool UploadFiles(IEnumerable<HttpPostedFileBase> files, string registrationId, int requestId)
        {
            var flag = false;
            var countIndex = 0;
            if (files != null && files.ToList().Count > 0)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }

                        var name = file.FileName;
                        string filename = StringExtensions.RemoveWhitespace(name);
                        UploadBinaryFiles(registrationId, requestId.ToString(), filename, fileData);

                        countIndex++;
                    }
                }
            }
            return flag;
        }

        public static bool UploadFiles(IEnumerable<HttpPostedFileBase> files, string registrationId, string requestId)
        {
            var flag = false;
            var countIndex = 0;
            if (files != null && files.ToList().Count > 0)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }

                        var name = file.FileName;
                        string filename = StringExtensions.RemoveWhitespace(name);
                        UploadBinaryFiles(registrationId, requestId, filename, fileData);

                        countIndex++;
                    }
                }
            }
            return flag;
        }

        public static bool UploadFileByName(HttpPostedFileBase file, string fileName, string registrationId, string requestId)
        {
            if (file != null)
            {
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileData = binaryReader.ReadBytes(file.ContentLength);
                }
                //var name = file.FileName;
                //string filename = StringExtensions.RemoveWhitespace(name);
                ////string extension = Path.GetExtension(file.FileName);
                ////string flname = fileName + extension;
                UploadBinaryFiles(registrationId, requestId, fileName, fileData);
                return true;
            }
            else return false;
        }

        public static bool UploadFileByName(HttpPostedFileBase file, string fileName, string registrationId, string requestId, string RefId)
        {
            if (file != null)
            {
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileData = binaryReader.ReadBytes(file.ContentLength);
                }
                //var name = file.FileName;
                //string filename = StringExtensions.RemoveWhitespace(name);
                ////string extension = Path.GetExtension(file.FileName);
                ////string flname = fileName + extension;
                UploadBinaryFiles(registrationId, requestId, fileName, fileData, RefId);
                return true;
            }
            else return false;
        }

        public static bool UploadPropertyFile(HttpPostedFileBase file, string fileName, string directory, string subdirectory)
        {
            if (file != null)
            {
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileData = binaryReader.ReadBytes(file.ContentLength);
                }
                //var name = file.FileName;
                //string filename = StringExtensions.RemoveWhitespace(name);
                ////string extension = Path.GetExtension(file.FileName);
                ////string flname = fileName + extension;
                UploadNDBinaryFiles(directory, subdirectory, fileName, fileData);
                return true;
            }
            else return false;
        }

        public static bool UploadKYAFiles(IEnumerable<HttpPostedFileBase> files, string registrationId, string requestId)
        {
            var flag = false;
            var countIndex = 0;
            if (files != null && files.ToList().Count > 0)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }

                        var name = file.FileName;
                        string filename = StringExtensions.RemoveWhitespace(name);
                        UploadBinaryFiles(registrationId, requestId, filename, fileData);

                        countIndex++;
                    }
                }
            }
            return flag;
        }

        public static bool UploadFileRange(IEnumerable<HttpPostedFileBase> files, string directory, string subdirectory)
        {
            var flag = false;
            var countIndex = 0;
            if (files != null && files.ToList().Count > 0)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }

                        var name = file.FileName;
                        string filename = StringExtensions.RemoveWhitespace(name);
                        UploadBinaryFiles(directory, subdirectory, filename, fileData);

                        countIndex++;
                    }
                }
            }
            return flag;
        }

        public static List<string> ToListFiles(string path)
        {
            List<String> fileList = new List<String>();
            try
            {
                var request = OpenFTPConnection(path, WebRequestMethods.Ftp.ListDirectory);
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, true))
                        {
                            while (!reader.EndOfStream)
                            {
                                fileList.Add(reader.ReadLine());
                            }
                        }
                    }
                }
            }
            catch (System.Exception excpt)
            {
            }

            return fileList;
        }

        public static FtpWebRequest OpenFTPConnection(string uri, string method)
        {
            var request = (FtpWebRequest)WebRequest.Create(uri);
            request.Credentials = new NetworkCredential(username, password);
            request.Method = method;
            return request;
        }


        public string GetDocumentPath(int? rid, string file, bool IsFtp)
        {
            string Path = string.Empty;
            if (IsFtp)
            {
                Path = System.Configuration.ConfigurationManager.AppSettings["RootPath"] + "\\" + rid;
            }
            else { Path = "http://" + System.Configuration.ConfigurationManager.AppSettings["SiteRootPath"] + "//" + rid + "//" + file; }
            return Path;
        }

        public static string GetPropertyDocumentPath(int? rid, string subdirectory, string file, bool flag)
        {
            string Path = string.Empty;
            if (flag)
            {
                if (string.IsNullOrEmpty(subdirectory))
                {
                    Path = System.Configuration.ConfigurationManager.AppSettings["RootPath"] + "\\" + rid;
                }
                else
                {
                    Path = System.Configuration.ConfigurationManager.AppSettings["RootPath"] + "\\" + rid + "\\" + subdirectory;
                }
            }
            else {
                if (string.IsNullOrEmpty(subdirectory))
                {
                    Path = "http://" + System.Configuration.ConfigurationManager.AppSettings["SiteRootPath"] + "//" + rid + "//" + file;
                }
                else
                {
                    Path = "http://" + System.Configuration.ConfigurationManager.AppSettings["SiteRootPath"] + "//" + rid + "//"+subdirectory + "//" + file;
                }
            }
            return Path;
        }

        public static string GetServicesDocumentPath(int? rid, int? id, string file, bool flag)
        {
            string Path = string.Empty;
            if (flag)
            {
                Path = System.Configuration.ConfigurationManager.AppSettings["FTPRootPath"] + "\\" + rid + "\\" + id;
            }
            else { Path = "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + rid + "//" + id + "//" + file; }
            return Path;
        }

        public string GetDocumentPathForCustomer(int? rid, int? id, string file, bool IsFtp)
        {
            string Path = string.Empty;
            if (IsFtp)
            {
                Path = System.Configuration.ConfigurationManager.AppSettings["FTPRootPath"] + "\\" + rid + "\\" + id;
            }
            else { Path = "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + rid + "//" + id + "//" + file; }
            return Path;
        }

        public static string GetDocumentPathForKYA(string rid, string id, string file, bool IsFtp)
        {
            string Path = string.Empty;
            if (IsFtp)
            {
                //Path = IsOldFormat == false ? System.Configuration.ConfigurationManager.AppSettings["FTPRootPath"] + "\\" + rid + "\\" + id + "\\" + kId : System.Configuration.ConfigurationManager.AppSettings["FTPRootPath"] + "\\" + rid + "\\" + id;
                Path = System.Configuration.ConfigurationManager.AppSettings["FTPRootPath"] + "\\" + rid + "\\" + id;

                //Path = "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "\\" + rid + "\\" + id;
            }
            else { Path = "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + rid + "//" + id + "//" + file; }
            return Path;
        }

        public static string GetDocumentPathForKYA(string rid, string id, string kId, string file, bool IsFtp, bool IsOldFormat)
        {
            string Path = string.Empty;
            string prevPath = string.Empty;
            if (IsFtp)
            {
                Path = IsOldFormat == false ? System.Configuration.ConfigurationManager.AppSettings["FTPRootPath"] + "\\" + rid + "\\" + id + "\\" + kId : System.Configuration.ConfigurationManager.AppSettings["FTPRootPath"] + "\\" + rid + "\\" + id;
                //Path = System.Configuration.ConfigurationManager.AppSettings["FTPRootPath"] + "\\" + rid + "\\" + id;

                //Path = "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "\\" + rid + "\\" + id;
            }
            else
            {
                Path = IsOldFormat == false ? "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + rid + "//" + id + "//" + kId + "//" + file : "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + rid + "//" + id + "//" + file;
            }
            return Path;
        }

        /// <summary>
        /// Used for searching Files in FTP Server.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> DirSearch(string path)
        {
            List<String> files = new List<String>();
            try
            {
                var request = CreateRequest(path, WebRequestMethods.Ftp.ListDirectory);
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, true))
                        {
                            while (!reader.EndOfStream)
                            {
                                files.Add(reader.ReadLine());
                            }
                        }
                    }
                }
            }
            catch (System.Exception excpt)
            {
            }

            return files;
        }

        public FtpWebRequest CreateRequest(string uri, string method)
        {
            var r = (FtpWebRequest)WebRequest.Create(uri);

            r.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["FTPUsername"], System.Configuration.ConfigurationManager.AppSettings["FTPPassword"]);
            r.Method = method;
            return r;
        }

        public void RenameFiles(string currentFileName, string newFileName, string path)
        {
            try
            {
                Stream ftpStream = null;
                var r = (FtpWebRequest)WebRequest.Create(path + "/" + currentFileName);
                r.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["FTPUsernameSave"], System.Configuration.ConfigurationManager.AppSettings["FTPPasswordSave"]);
                r.Method = WebRequestMethods.Ftp.Rename;
                r.RenameTo = newFileName;
                FtpWebResponse response = (FtpWebResponse)r.GetResponse();
                ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (System.Exception excpt)
            {
            }
        }

        public string GetDocumentPathForServiceRequest(int? RequestId, bool IsFtp)
        {
            string Path = string.Empty;
            if (IsFtp)
            {
                Path = ftpPath + "\\" + RequestId + "//";
            }
            else
            {
                Path = "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + RequestId + "//";
            }
            return Path;
        }

        public List<DocumentViewModel> GetServiceRequestDocuments(int RequestId)
        {
            List<DocumentViewModel> documentList = new List<DocumentViewModel>();
            if (IsDirectoryExists(ftpPath, Convert.ToString(RequestId)))
            {
                var filepath = ftpPath + RequestId + "/";
                List<string> fileList = DirSearch(filepath);
                string filePath = GetDocumentPathForServiceRequest(RequestId, false);// "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + RequestId + "//";
                if (fileList != null)
                {
                    for (int x = 0; x < fileList.Count(); x++)
                    {
                        DocumentViewModel document = new DocumentViewModel();
                        document.Id = x + 1;
                        document.DocumentName = fileList.ElementAt(x).ToString();
                        document.DocumentPath = filePath + document.DocumentName;
                        documentList.Add(document);
                    }
                }
            }
            return documentList;
        }

        public List<DocumentViewModel> GetServiceRequestUploadedDocumentsById(int registrationId, int RequestId)
        {
            string docpath = ConfigurationManager.AppSettings["FTPPath"];
            List<DocumentViewModel> documentList = new List<DocumentViewModel>();
            docpath = docpath + "/" + registrationId;
            if (IsDirectoryExists(docpath, Convert.ToString(RequestId)))
            {
                var filepath = docpath + "/" + RequestId + "/";
                List<string> fileList = DirSearch(filepath);
                //string filePath = GetDocumentPathForServiceRequest(RequestId, false);
                string filePath = "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + registrationId + "//" + RequestId + "//";
                if (fileList != null)
                {
                    for (int x = 0; x < fileList.Count(); x++)
                    {
                        DocumentViewModel document = new DocumentViewModel();
                        document.Id = x + 1;
                        document.DocumentName = fileList.ElementAt(x).ToString();
                        document.DocumentPath = filePath + document.DocumentName;
                        documentList.Add(document);
                    }
                }
            }
            return documentList;
        }

        public static bool IsDocumentAvailable(int rid)
        {
            var flag = true;
            string path = System.Configuration.ConfigurationManager.AppSettings["RootPath"] + "\\" + rid;
            List<string> fileList = ToListFiles(path);
            if (fileList == null || fileList.Count == 0) flag = false;
            return flag;
        }


        #region Nivesh Mitra Services

        public static bool UploadFiles_NIC(IEnumerable<HttpPostedFileBase> files, string RequestId)
        {
            var flag = false;
            var countIndex = 0;
            if (files != null && files.ToList().Count > 0)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }
                        //  var name = file.FileName;
                        var name = RequestId.ToString() + ".pdf";
                        string filename = StringExtensions.RemoveWhitespace(name);
                        UploadBinaryFiles_NIC(RequestId, filename, fileData);
                        countIndex++;
                    }
                }
            }
            return flag;
        }

        private static void UploadBinaryFiles_NIC(string RequestId, string filename, byte[] fileData)
        {
            bool IsDirectory = false;
            if (IsDirectoryExists(fTPPath_NM, RequestId))
            {
                IsDirectory = true;
            }
            else
            {
                IsDirectory = CreateDirectory(fTPPath_NM, RequestId);
            }

            if (IsDirectory == true)
            {
                FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(fTPPath_NM + "/" + RequestId + "/" + filename));
                ftpClient.Credentials = new System.Net.NetworkCredential(username.Normalize(), password.Normalize());
                ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

                ftpClient.ContentLength = fileData.Length;
                byte[] buffer = new byte[4097];
                int bytes = fileData.Length;
                int total_bytes = (int)fileData.Length;
                try
                {
                    System.IO.Stream rs = ftpClient.GetRequestStream();
                    while (total_bytes > 0)
                    {
                        rs.Write(fileData, 0, bytes);
                        total_bytes = total_bytes - bytes;
                    }
                    rs.Close();
                }
                catch (Exception e)
                {
                    throw e;
                }
                FtpWebResponse uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
                var value = uploadResponse.StatusDescription;
                uploadResponse.Close();
            }
        }

        public List<DocumentVM> GetServiceRequestDocuments_NIC(int RequestId)
        {
            List<DocumentVM> documentList = new List<DocumentVM>();
            if (IsDirectoryExists(ftpPath, Convert.ToString(RequestId)))
            {
                var filepath = ftpPath + RequestId + "/";
                List<string> fileList = DirSearch(filepath);
                string filePath = GetDocumentPathForServiceRequest(RequestId, false);
                if (fileList != null)
                {
                    for (int x = 0; x < fileList.Count(); x++)
                    {
                        DocumentVM document = new DocumentVM();
                        document.Id = x + 1;
                        document.DocumentName = fileList.ElementAt(x).ToString();
                        document.DocumentPath = filePath + document.DocumentName;
                        documentList.Add(document);
                    }
                }
            }
            return documentList;
        }

        public string GetUploadedDocumentByServiceId(int registrationId, int RequestId)
        {
            string docpath = ConfigurationManager.AppSettings["FTPPath"];
            docpath = docpath + "/" + registrationId;
            string completeFilePath = string.Empty;
            if (IsDirectoryExists(docpath, Convert.ToString(RequestId) + Constants.ReqComplete))
            {
                var filepath = docpath + "/" + RequestId + Constants.ReqComplete + "/";
                List<string> fileList = DirSearch(filepath);
                string filePath = "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + registrationId + "//" + RequestId + Constants.ReqComplete + "//";
                if (fileList.Count > 0)
                {
                    string documentName = fileList.ElementAt(0).ToString();
                    completeFilePath = filePath + documentName;
                }
            }
            return completeFilePath;
        }

        public List<DocumentVM> GetServiceRequestUploadedDocumentsById_NIC(int registrationId, int RequestId)
        {
            string docpath = ConfigurationManager.AppSettings["FTPPath"];
            List<DocumentVM> documentList = new List<DocumentVM>();
            docpath = docpath + "/" + registrationId;
            if (IsDirectoryExists(docpath, Convert.ToString(RequestId)))
            {
                var filepath = docpath + "/" + RequestId + "/";
                List<string> fileList = DirSearch(filepath);
                //string filePath = GetDocumentPathForServiceRequest(RequestId, false);
                string filePath = "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + registrationId + "//" + RequestId + "//";
                if (fileList != null)
                {
                    for (int x = 0; x < fileList.Count(); x++)
                    {
                        DocumentVM document = new DocumentVM
                        {
                            Id = x + 1,
                            DocumentName = fileList.ElementAt(x).ToString()
                        };
                        document.DocumentPath = filePath + document.DocumentName;
                        documentList.Add(document);
                    }
                }
            }
            return documentList;
        }

        #endregion
    }

}
