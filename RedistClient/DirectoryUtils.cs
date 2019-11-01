using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NLog;

namespace RedistServ
{
    public static class DirectoryUtils
    {
        public static void TryDeleteDirectory(string targetDir)
        {
                File.SetAttributes(targetDir, FileAttributes.Normal);

                var files = Directory.GetFiles(targetDir);
                var dirs = Directory.GetDirectories(targetDir);

                foreach (var file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                foreach (var dir in dirs)
                    TryDeleteDirectory(dir);
                Directory.Delete(targetDir, false);
        }

        public static void DeleteDirectory(string path, Action<string> trace)
        {
            for (;;)
            {
                try
                {
                    if (Directory.Exists(path))
                        TryDeleteDirectory(path);
                    return;
                }
                catch (Exception e)
                {
                    trace(e.Message);
                }
                Thread.Sleep(1000);
            }
        }

        public static void Copy(string sourcedir, string targetDir,Action<string> logger)
        {
            if (Directory.Exists(targetDir))
                File.SetAttributes(targetDir, FileAttributes.Normal);
            else
                Directory.CreateDirectory(targetDir);
            //copy source files if changed
            var sourecefiles = Directory.GetFiles(sourcedir);
            foreach (var file in sourecefiles)
            {
                var filename = Path.GetFileName(file);
                var targetfilepath = Path.Combine(targetDir, filename);
                DirectoryUtils.UpdateFile(file, targetfilepath,logger);
            }

            var filenames = sourecefiles.Select(Path.GetFileName).ToArray();
            var targetfiles = Directory.GetFiles(targetDir);
            foreach (var file in targetfiles)
            {
                var filename = Path.GetFileName(file);
                if(filenames.Contains(filename,StringComparer.InvariantCultureIgnoreCase))
                    continue;
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    
                }
            }
            var dirs = Directory.GetDirectories(targetDir);
            
        }

        private static void UpdateFile(string sourcefile, string targetfile,Action<string> logger)
        {
            var sourcecreate = File.GetCreationTimeUtc(sourcefile);
            var sourcewritetime = File.GetLastWriteTimeUtc(sourcefile);
            if (File.Exists(targetfile))
            {
                var targetcreate = File.GetCreationTimeUtc(targetfile);
                var targetwrite = File.GetLastWriteTimeUtc(targetfile);
                if(targetcreate==sourcecreate && targetwrite==sourcewritetime)
                    return; //skip not changed file
            }

            if (!TryCopy(sourcefile, targetfile,logger))
            {
                if (!TryRenameToTemorary(targetfile,logger))
                    throw   new Exception($"Can't overwrite {targetfile}");
                if(!TryCopy(sourcefile, targetfile,logger))
                    logger?.Invoke("secondary copy failed");
            }


            try
            {
                File.SetCreationTimeUtc(targetfile,sourcecreate);
                File.SetLastWriteTimeUtc(targetfile,sourcewritetime);
            }
            catch (Exception e)
            {
                logger?.Invoke(e.Message);
            }
            
            var targetcreateaft = File.GetCreationTimeUtc(targetfile);
            var targetwriteaft = File.GetLastWriteTimeUtc(targetfile);
        }

        private static bool TryRenameToTemorary(string targetfile, Action<string> logger)
        {
            var directory = Path.GetDirectoryName(targetfile);
            for (int i = 0; i < 100; i++)
            {
                var guid = Guid.NewGuid();
                
                var tmpname = Path.Combine(directory,$"{guid}.tmp");
                try
                {
                    logger?.Invoke($"Rename {targetfile} to {tmpname}");
                    
                    
                    File.Move(targetfile,tmpname);
                    return true;
                }
                catch (Exception e)
                {
                    logger?.Invoke($"Fail to rename to {tmpname}");
                } 
            }

            return false;
        }

        private static bool TryCopy(string sourcefile, string targetfile, Action<string> logger)
        {
            try
            {
                File.Copy(sourcefile,targetfile,true);
                return true;
            }
            catch (Exception e)
            {
                logger?.Invoke(e.Message);
                return false;
            }
        }
    }
}