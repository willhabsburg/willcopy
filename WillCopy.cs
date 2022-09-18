/*
Will Habsburg
willcopy
WillCopy.cs - main logic

*/

namespace willcopy
{
    public class WillCopy
    {
        Output logOut; // for logging
        Helpers h; // some helper functions
        string fromDir, // directory to copy from
            toDir, // directory to copy to
            command = "c", // default command (see below)
            mode = "u"; // default mode (see below)
        bool recur, // recursive copy?
            quiet, // prevent output to console
            hidden, // copy hidden files?
            move = false, // flag for moving instead of copying
            list = false; // flag for listing instead of copying
        // counter variables to display stats to user
        private long dirCreate = 0, dirDelete = 0, dirFail = 0, fileCreate = 0, fileDelete = 0, fileFail = 0, fileSkip = 0;

        /*
            Constructor for WillCopy - set properties based on command line options
        */
        public WillCopy(string fromDir, string toDir, string? command, string? mode, string? log, bool recur, bool quiet, bool hidden)
        {
            // create log output instance
            logOut = new Output(quiet, log);
            h = new Helpers(logOut);

            this.recur = recur;
            this.quiet = quiet;
            this.hidden = hidden;

            //Check to see if source directory exists
            if (!Directory.Exists(fromDir))
                h.fatalError("Source Directory does not exist.");
            this.fromDir = fromDir;

            // Check to see if destination directory exists, create it if it doesn't
            if (!Directory.Exists(toDir) && command != "l")
            {
                if (h.createDir(toDir))
                {
                    dirCreate++;
                }
                else
                {
                    h.fatalError("Cannot create destination directory.");
                }
            }
            this.toDir = toDir;

            // append directory separator characters to end of paths
            if (this.fromDir.ToCharArray()[this.fromDir.Length - 1] != Path.DirectorySeparatorChar)
                this.fromDir += Path.DirectorySeparatorChar;
            if (this.toDir.ToCharArray()[this.toDir.Length - 1] != Path.DirectorySeparatorChar)
                this.toDir += Path.DirectorySeparatorChar;

            // Check for proper command ("m"ove, "c"reate, "l"ist) (see Program.cs)
            if (command == null)
            {
                this.command = "c";
            }
            else if (!Regex.Match(command, @"m|c|l").Success)
            {
                h.fatalError("Not a valid command!  Must be one of m, c, or l");
            }
            else
            {
                this.command = command;
                if (command == "m")
                {
                    move = true;
                }
                else if (command == "l")
                {
                    list = true;
                }
            }

            // Check for proper mode ("a"lways overwrite, "n"ever overwrite, "u"pdate if newer, "c"lone)
            // see Program.cs
            // Default mode = update
            if (mode == null)
            {
                this.mode = "u";
            }
            // if mode specified is not valid, give an error
            else if (!Regex.Match(mode, @"a|n|u|c").Success)
            {
                h.fatalError("Not a vaid mode!  Must be one of a, n, u, or c");
            }
            // else set the mode
            else
            {
                this.mode = mode;
            }
        }

        /*
            Main logic for WillCopy
        */
        public void start()
        {
            string tmp; // to hold destination directory and file names
            bool found; // flag if directory or file is found
            DateTime fiDT = DateTime.Now, foDT = DateTime.Now;
            long fiSize = 0, foSize = 0;

            logOut.WriteLine("Started: " + DateTime.Now.ToString());

            Listing source = new Listing(fromDir, recur, logOut, hidden);
            source.GetListings();
            FileInfo[] sourceFiles = source.GetFiles();
            DirectoryInfo[] sourceDir = source.GetDirectories();
            logOut.WriteLine("Source: Found " + sourceFiles.Length + " files in " + sourceDir.Length + " directories.");

            Listing dest = new Listing(toDir, recur, logOut, hidden);
            FileInfo[]? destFiles = new FileInfo[0];
            DirectoryInfo[]? destDir = new DirectoryInfo[0];
            if (Directory.Exists(toDir))
            {
                dest.GetListings();
                destFiles = dest.GetFiles();
                destDir = dest.GetDirectories();
            }
            logOut.WriteLine("Destination: Found " + destFiles.Length + " files in " + destDir.Length + " directories.");

            foreach (var din in sourceDir)
            {
                found = false;
                tmp = toDir + din.FullName.Substring(fromDir.Length);
                foreach (var dout in destDir)
                {
                    if (tmp == dout.FullName)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    logOut.WriteLine("Creating Directory: " + tmp);
                    if (command != "l")
                    {
                        if (h.createDir(tmp))
                        {
                            dirCreate++;
                        }
                        else
                        {
                            dirFail++;
                        }
                    }
                    else
                    {
                        dirCreate++;
                    }
                }
            }

            switch (mode)
            {
                case "a":
                    foreach (var fin in sourceFiles)
                    {
                        tmp = toDir + fin.FullName.Substring(fromDir.Length);
                        logOut.WriteLine((move ? "Moving" : "Copying") + " file to " + tmp);
                        MoveCopyFiles(fin.FullName, tmp);
                    }
                    break;

                case "n":
                case "u":
                    foreach (var fin in sourceFiles)
                    {
                        found = false;
                        tmp = toDir + fin.FullName.Substring(fromDir.Length);
                        foreach (var fout in destFiles)
                        {
                            if (tmp == fout.FullName)
                            {
                                found = true;
                                fiDT = fin.LastWriteTime;
                                fiSize = fin.Length;
                                foDT = fout.LastWriteTime;
                                foSize = fout.Length;
                                break;
                            }
                        }
                        if (found)
                        {
                            if (mode == "u" && (fiDT > foDT || fiSize != foSize))
                            {
                                MoveCopyFiles(fin.FullName, tmp);
                            }
                            else
                            {
                                fileSkip++;
                            }
                        }
                        else
                        {
                            MoveCopyFiles(fin.FullName, tmp);
                        }
                    }
                    break;

                case "c":
                    List<FileInfo> toCopy = new List<FileInfo>();
                    List<FileInfo> toDelete = new List<FileInfo>();
                    List<string> dirToDelete = new List<string>();
                    foreach (var fin in sourceFiles)
                    {
                        found = false;
                        tmp = toDir + fin.FullName.Substring(fromDir.Length);
                        foreach (var fout in destFiles)
                        {
                            if (tmp == fout.FullName)
                            {
                                found = true;
                                fiDT = fin.LastWriteTime;
                                fiSize = fin.Length;
                                foDT = fout.LastWriteTime;
                                foSize = fout.Length;
                                break;
                            }
                        }
                        if (found)
                        {
                            if (fiDT > foDT || fiSize != foSize)
                            {
                                toCopy.Add(fin);
                            }
                            else
                            {
                                fileSkip++;
                            }
                        }
                        else
                        {
                            toCopy.Add(fin);
                        }
                    }
                    foreach (var fout in destFiles)
                    {
                        found = false;
                        tmp = fromDir + fout.FullName.Substring(toDir.Length);
                        foreach (var fin in sourceFiles)
                        {
                            if (tmp == fin.FullName)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            toDelete.Add(fout);
                        }
                    }

                    foreach (var fin in toCopy)
                    {
                        tmp = toDir + fin.FullName.Substring(fromDir.Length);
                        MoveCopyFiles(fin.FullName, tmp);
                    }
                    foreach (var fout in toDelete)
                    {
                        if (!list)
                        {
                            if (h.DeleteFile(fout.FullName))
                            {
                                fileDelete++;
                            }
                            else
                            {
                                fileFail++;
                            }
                        }
                        else
                        {
                            fileDelete++;
                        }
                    }

                    foreach (var dout in destDir)
                    {
                        found = false;
                        tmp = fromDir + dout.FullName.Substring(toDir.Length);
                        foreach (var din in sourceDir)
                        {
                            if (din.FullName == tmp)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            dirToDelete.Add(dout.FullName);
                        }
                    }
                    for (int i = dirToDelete.Count - 1; i >= 0; i--)
                    {
                        logOut.WriteLine("Deleting directory: " + dirToDelete[i]);
                        if (!list)
                        {
                            if (h.deleteDir(dirToDelete[i]))
                            {
                                dirDelete++;
                            }
                            else
                            {
                                dirFail++;
                            }
                        }
                        else
                        {
                            dirDelete++;
                        }
                    }

                    break;
            }


            if (command == "m")
            {
                h.FullyDeleteDir(new DirectoryInfo(fromDir));
                dirDelete += sourceDir.Length;
                fileDelete += sourceFiles.Length;
            }

            logOut.WriteLine("-----------------------------------------");
            logOut.WriteLine("Summary:");
            logOut.WriteLine("Total Files considered:    " + sourceFiles.Length);
            logOut.WriteLine("Total Files skipped:       " + fileSkip);
            logOut.WriteLine("Total Files copied/moved:  " + fileCreate);
            logOut.WriteLine("Total Directories made:    " + dirCreate);
            logOut.WriteLine("Total Directory errors:    " + dirFail);
            logOut.WriteLine("Total File errors:         " + fileFail);
            if (dirDelete > 0)
            {
                logOut.WriteLine("Total Directories deleted: " + dirDelete);
            }
            logOut.WriteLine("-----------------------------------------");
            if(list) {
                logOut.WriteLine("This is a listing only.  Nothing was actually done.");
            }
            logOut.WriteLine("Finished: " + DateTime.Now.ToString());
        }

        private void MoveCopyFiles(string fin, string fout)
        {
            logOut.WriteLine("Copying / Moving file: " + fout);
            if (h.CMFile(fin, fout, move, list))
            {
                fileCreate++;
            }
            else
            {
                fileFail++;
            }
        }
    }
}