namespace willcopy
{
    public class Output
    {
        private List<string> logOut;
        bool quiet;
        string logfile;

        public Output(bool quiet, string? logfile)
        {
            this.quiet = quiet;
            if (logfile == null)
            {
                this.logfile = "";
            }
            else
            {
                this.logfile = logfile;
                // Check for existance of log file, create it if it doesn't exist
                if (logfile != "" && !File.Exists(logfile))
                {
                    try
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(logfile, true))
                        {
                            file.Write("Log file created " + DateTime.Today.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error writing to log file:\n" + e.Message);
                        logfile = "";
                    }
                }
            }
            this.logOut = new List<string>();
        }

        public void WriteLine(string op)
        {
            logOut.Add(op);
            if (!quiet)
            {
                Console.WriteLine(op);
            }
        }

        public void WriteLog()
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(logfile, true))
                {
                    foreach (string line in logOut)
                    {
                        file.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error!\n{0}", e.Message);
            }
        }
    }
}