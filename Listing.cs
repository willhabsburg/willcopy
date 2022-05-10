namespace willcopy
{
    public class Listing
    {
        private DirectoryInfo dirname;
        private List<DirectoryInfo> ld = new List<DirectoryInfo>();
        private List<FileInfo> lf = new List<FileInfo>();
        private bool recur, hidden;
        private EnumerationOptions eo = new EnumerationOptions();
        Output op;
        string filemask = "*";

        public Listing(string dir, bool recur, Output op, bool hidden)
        {
            this.dirname = new DirectoryInfo(dir);
            this.ld.Add(dirname);
            this.recur = recur;
            this.op = op;
            this.hidden = hidden;
            eo.RecurseSubdirectories = false;
            eo.AttributesToSkip = (hidden)?(0):(FileAttributes.Hidden | FileAttributes.System);
            eo.IgnoreInaccessible = false;

        }

        private void GetDirs(DirectoryInfo dir)
        {
            DirectoryInfo[]? d = null;
            FileInfo[]? f = null;
            try
            {
                d = dir.GetDirectories(filemask, SearchOption.TopDirectoryOnly);
            }
            catch (System.Security.SecurityException)
            {
                op.WriteLine("NonFatalError: You do not have permission to get directory listings for " + dir.FullName);
            }
            catch (System.UnauthorizedAccessException)
            {
                op.WriteLine("NonFatalError: You do not have permission to get directory listings for " + dir.FullName);
            }
            catch (System.Exception e)
            {
                op.WriteLine("NonFatalError: Other Exception occurred getting directory listings for " + dir.FullName + "\n" + e);
            }
            
            try
            {
                f = dir.GetFiles(filemask, eo);
            }
            catch (System.Security.SecurityException)
            {
                op.WriteLine("NonFatalError: You do not have permission to get file listings for " + dir.FullName);
            }
            catch (System.UnauthorizedAccessException)
            {
                op.WriteLine("NonFatalError: You do not have permission to get file listings for " + dir.FullName);
            }
            catch (System.Exception e)
            {
                op.WriteLine("NonFatalError: Other Exception occurred getting file listings for " + dir.FullName + "\n" + e);
            }
            if (f != null)
            {
                foreach (FileInfo fil in f)
                {
                    lf.Add(fil);
                }
            }
            if (d != null)
            {
                foreach (DirectoryInfo di in d)
                {
                    ld.Add(di);
                    if (recur)
                    {
                        GetDirs(di);
                    }
                }
            }
        }

        public void GetListings()
        {
            GetDirs(dirname);
        }

        public FileInfo[] GetFiles()
        {
            return lf.ToArray();
        }

        public DirectoryInfo[] GetDirectories()
        {
            return ld.ToArray();
        }
    }
}