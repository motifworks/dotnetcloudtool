using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace AzureCalculator.Helper
{
    public class PSScriptHelper
    {
        public static String RunScript(String psScriptPath, String logFile)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"powershell.exe";
            startInfo.Arguments = " \"" + psScriptPath + "\" >> " + logFile + " 2>&1";
            //startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = false;
            startInfo.Verb = "runas";
            startInfo.LoadUserProfile = true;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string output = "Processed";// process.StandardOutput.ReadToEnd();
            System.Diagnostics.Debug.WriteLine(output);

            string errors = "";//process.StandardError.ReadToEnd();
            System.Diagnostics.Debug.WriteLine(errors);

            return output + (!String.IsNullOrEmpty(errors) ? "<BR><BR><p style='color:red'>==================Error========================<BR>" + errors + "</p>" : "");
            //return "Processed";
        }
    }
}