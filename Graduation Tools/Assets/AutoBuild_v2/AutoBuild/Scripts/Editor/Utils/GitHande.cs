using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Custom.Tool
{
    public static class GitHande
    {
        public static void RunGitCommand(string command)
        {
#if UNITY_EDITOR_OSX
            ProcessStartInfo procStartInfo = new ProcessStartInfo(@"C:/Program Files/Git/git-bash.exe");
            return;
#else

            ProcessStartInfo procStartInfo = new ProcessStartInfo(@"C:/Program Files/Git/git-bash.exe");
#endif
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.ErrorDialog = false;
            procStartInfo.Arguments = command;
            Process process = new Process();
            process.StartInfo = procStartInfo;
            process.Start();
            process.WaitForExit();
        }

        //public static string GetGitOutput(string command)
        //{
        //    string output = "";
        //    try
        //    {
        //        var process = new Process
        //        {
        //            StartInfo = new ProcessStartInfo
        //            {
        //                FileName = @"C:/Program Files/Git/git-bash.exe",
        //                Arguments = command,
        //                UseShellExecute = false,
        //                RedirectStandardOutput = true,
        //                CreateNoWindow = true
        //            }
        //        };
        //        process.Start();
        //        process.WaitForExit();
        //    }
        //    catch (System.Exception)
        //    {

        //        throw;
        //    }
        //    return output;
        //}
    }
}
