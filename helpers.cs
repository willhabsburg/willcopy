using System.Security;
namespace willcopy
{
    public class Helpers
    {
        private Output logOut;

        public Helpers(Output logOut)
        {
            this.logOut = logOut;
        }
        public void fatalError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An error occured that has halted the program:");
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nPlease see willcopy --help for details on using this program");
            System.Environment.Exit(1);
        }

        public bool createDir(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
                return true;
            }
            catch (System.IO.PathTooLongException)
            {
                fatalError("Cannot create output directory because the path is too long: " + dir);
            }
            catch (System.IO.IOException)
            {
                fatalError("Cannot create output directory " + dir);
            }
            catch (System.UnauthorizedAccessException)
            {
                fatalError("You do not have permission to create output directory " + dir);
            }
            catch (System.Exception e)
            {
                fatalError("Exception creating output directory " + dir + "\n" + e.Message);
            }
            return false;
        }

        public bool deleteDir(string dir)
        {
            try
            {
                Directory.Delete(dir);
                return true;
            }
            catch (IOException)
            {
                fatalError("Cannot delete directory: " + dir);
            }
            catch (UnauthorizedAccessException)
            {
                fatalError("You do not have permission to delete directory: " + dir);
            }
            catch (Exception e)
            {
                fatalError("Error deleting directory " + dir + "\n" + e.Message);
            }
            return false;
        }

        public bool CMFile(string fin, string fout, bool move, bool list)
        {
            try
            {
                if (move && !list)
                {
                    File.Move(@fin, @fout, true);
                }
                else if (!list)
                {
                    File.Copy(@fin, @fout, true);
                }
                return true;
            }
            catch (System.UnauthorizedAccessException)
            {
                logOut.WriteLine("NonFatalError: You do not have permission to copy the file " + fin);
            }
            catch (System.IO.PathTooLongException)
            {
                logOut.WriteLine("NonFatalError: The path is too long trying to copy file to " + fout);
            }
            catch (System.IO.IOException e)
            {
                logOut.WriteLine("NonFatalError: An IO exception occurred trying to copy " + fin + "\n" + e.Message);
            }
            catch (System.Exception e)
            {
                logOut.WriteLine("NonFatalError: An exception occured creating file " + fout + "\n" + e.Message);
            }
            return false;
        }

        public bool DeleteFile(string fin)
        {
            try
            {
                File.Delete(fin);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                logOut.WriteLine("Non-Fatal Error:  You do not have permission to delete the file: " + fin);
            }
            catch (IOException e)
            {
                logOut.WriteLine("Non-Fatal Error: An IO exception occured trying to delete file: " + fin + "\n" + e.Message);
            }
            catch (Exception e)
            {
                logOut.WriteLine("Non-Fatal Error: An exception occured trying to delete file: " + fin + "\n" + e.Message);
            }
            return false;
        }

        public bool FullyDeleteDir(DirectoryInfo baseDir)
        {
            try
            {
                baseDir.Delete(true);
                return true;
            }
            catch (SecurityException)
            {
                logOut.WriteLine("Non-Fatal Error:  You do not have permission to delete the directory: " + baseDir.FullName);
            }
            catch (IOException e)
            {
                logOut.WriteLine("Non-Fatal Error:  An IO Exception occured trying to delete the directory: " + baseDir.FullName + "\n" + e.Message);
            }
            catch (UnauthorizedAccessException)
            {
                logOut.WriteLine("Non-Fatal Error:  Cannot delete directory: " + baseDir.FullName + " because a read-only file exists inside it");
            }
            catch (Exception e)
            {
                logOut.WriteLine("Non-Fatal Error:  Cannot delete directory: " + baseDir.FullName + "\n" + e.Message);
            }
            return false;
        }
    }
}