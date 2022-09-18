# willcopy
A C# console application to backup my files.

This program backs up from the specified source directory to the specified destination directory, with command-line options to backup recursively, include hidden files, and log to a file.

Command line options can also be used to move files, copy if newer, never copy if backup exsists and clone (see below)

For clone mode, if a file already exists in the backup, and the source file is newer, the previous backed up file will be preserved with the date and time being appended to the file.  This is to have a sort of version control.  Preserved backup files are put in a directory with theyear and week number:
E.g.: backup to directory "/backup", preserved files would be in "/backup-2022-38" because I am writing this in week 38 of 2022.  The backup directory will be an exact clone of the source directory.

Files and directories can be excluded, but this feature is in it's infancy and may not work correctly for wildcards.
