using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
// using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using System.Linq;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using System.Collections.Generic;
using Microsoft.Azure.Management.Compute.Fluent;

namespace startrunnerscriptextension
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var creds = new AzureCredentialsFactory().FromFile("my.azureauth");
            var azure = await Azure.Configure()
                .WithLogLevel(Microsoft.Azure.Management.ResourceManager.Fluent.Core.HttpLoggingDelegatingHandler.Level.BodyAndHeaders)
                .Authenticate(creds)
                .WithDefaultSubscriptionAsync();

            Console.WriteLine(azure.SubscriptionId);


            var allvmss = await azure.VirtualMachineScaleSets.ListAsync();

            foreach (var vmss in allvmss)
            {
                Console.WriteLine(vmss.Name);
            }

            var vmsss = await azure.VirtualMachineScaleSets.ListByResourceGroupAsync("tihuang");

            IVirtualMachineScaleSet tihuangvmss = vmsss.Where(v => v.Name == "tihuang-vmss-win").FirstOrDefault();


            await tihuangvmss.Update().WithCapacity(1).ApplyAsync();
            var vms = await tihuangvmss.VirtualMachines.ListAsync();

            Console.WriteLine(vms.Count());

            var vmId = vms.First().InstanceId;
            Console.WriteLine(vmId);

            // var command = new RunCommandInput("RunShellScript");
            // command.Script = new List<string>();
            // command.Script.Add("set -e");
            // command.Script.Add("echo \"hello world\"");
            // // command.Script.Add("cat /var/log/azure/custom-script/handler.log");
            // command.Script.Add("ls -l");
            // // command.Script.Add("rm -r *");
            // command.Script.Add("apt-get -y update && apt-get install -y curl");
            // command.Script.Add("mkdir /actions-runner && cd /actions-runner");
            // command.Script.Add("curl -O -L https://github.com/actions/runner/releases/download/v2.273.5/actions-runner-linux-x64-2.273.5.tar.gz");
            // command.Script.Add("tar xzf ./actions-runner-linux-x64-2.273.5.tar.gz");
            // command.Script.Add("ls -l");
            // command.Script.Add("echo `echo 77u/ewogICJzY2hlbWUiOiAiT0F1dGgiLAogICJkYXRhIjogewogICAgImNsaWVudElkIjogIjZlZGZmMzYyLTg5YjYtNGEwOC1iYTE0LTE0OGUxOTJjZTIyNCIsCiAgICAiYXV0aG9yaXphdGlvblVybCI6ICJodHRwczovL3ZzdG9rZW4uYWN0aW9ucy5naXRodWJ1c2VyY29udGVudC5jb20vX2FwaXMvb2F1dGgyL3Rva2VuL2JmMDhhODVlLTcyNDEtNDg1OC1hZWI4LWFjNzAwNTZhMTZkNCIKICB9Cn0= | base64 -d` > .credentials");
            // command.Script.Add("echo `echo 77u/ewogICJkIjogImFRY3ZCaSt1MFQ4UWpTUHo0YUJwOXN3RHhaclZMTEczK2N1NWRGZWlTaFpFNkI0cWI1ZXFTcjNkR3JKekEvTjJvcEc5MlRsdlRFRkNZRzBJZVFKT3NvRE5sYU5Wc1FOeUVCZ1hob21NQ0k3UnJwWitQcmpBbjlHTE0xQUtsUWxZWCtyNzVIZGZNZnlkSnppeTRsRnlVL3dVY3Btc0wyUCtkRjg1Z3p3SEpqZlhoZENuMXNSa09POVRtUEl5bFA0aUtYcnJ5VnUvR0VZdjU4ZkdUdEp0dElCU082TGNOZjZNcGNLdDdOWkdwd0FCc0RwZGZLZ3RRVGdKT3BveDYrMVd2aUZFaXcxUVA2dGg4b0pWSXFpK0lCZXVNRzBENFhIM21GcTBPUmY3N3h3c0t0WE54ZVdMRlh3Yjd6bk5yNUIxY0VKQmt1Ry9ycHAva2FVR2JXbW4wUT09IiwKICAiZHAiOiAiRnVJWEgwV0ZuMWtmMERFaUE5RXdSaWxKTzZVVk5nRjdqT2xiWjJvVmdqV1BOQzNvbUttY1poQ3FpSDRtbExVVkpZMDF6MVNPUDNpTExoRjZMdGNjSjZ6ZStodUpGWTlZZHpWbitoclR3R21LMjI1WHRKZkNyc3kveVlBWTNGVjBiL2hJS21BTzFPdFJIM1dTdnBQb2dqQVdWU2Z4YWVjNityVXBzLytwWFNVPSIsCiAgImRxIjogImhrRDBSZnJPV2N3eEFtNDhiS011aU9DNzVqdlhmYUh6UXB2QlhkblV3cEg4L0FmdlhMVVhSb1hlbG8yWVU5K2k1YU5RczM5Mi9ubXB2OHQwK1Y2Z3JTTkJ2enk1MXZ1UndzdTNsdTEvMkFkc1E3KzVjdHR3cHU2VWsrN1pNQXl5YlhmUUtCay9aVFMzWHQ1aTVIVVN2RWRMaUxhaTRENVhmaWt5SjJPQnpqRT0iLAogICJleHBvbmVudCI6ICJBUUFCIiwKICAiaW52ZXJzZVEiOiAiQThRVUEvTjFPM0N2UGNHRXREZURUMVJ6WnFienY0RC9oL3o2c2RIZmZXZFNmWDlCS2RQeVM2TmZjYTFoZFcxWGJ2b2c4NkNhVHRkaU9sUDJtbmI3MktKTVU2dmdEekxJeHJrRWtuTXYyRktudzh6ZG5QQVZSTXpDZHEvTnY4bnNrSmovSjZ3c0FEV09CeW1sWGZKNHVsTHZic2h4cXIrSDVpODIzWUtLcXVNPSIsCiAgIm1vZHVsdXMiOiAiczR1UnY0MWtXZEF2MXdDMVBiLzhUd1hZd244WC9WSlJ6REZvemtRQ1ZkWi9vQk9Sc29wT1h3emhUb1RvMHh3TEVsdjYrTFc5M04yMjhkaWZFeGkyUUVhRUtwdi9oWU5uR3lpdktJSEcxeHJ5SUdvU3FsYmw4T2poMThoRWtyR0thbDdINlRZR3dPTjE5WWRMeHh2ZXVuRWltZTd2N3g3RFJQdi8xRmF4QXhkeDBrVGEyOTFoaDVnTVV0TWh3NHJuWVZuYldPMEI3VXpNamdLQ3dSUCs0ek8wUkl5QWdsR0xzTDlENmpEaGVOMGo5ZVUvZHRHckpjd1FVKzB2YnhNWjN1TEJwSjlVUlR3K2ZNd0NuOUcvVjFKODZtYUx0dHlBSW1hZnd4N0taRUJwM25EM3dKL29hZENOTGJoRFVSUmZxTjIxT0FGQzN1ZjU2V01PcFlMNlNRPT0iLAogICJwIjogIjdkdWV4Vys3L0RheXlWaTVySkhaUFJiVnNHNTVYWFI0bXlPZkVuUVVlY1V1OTNwZUR4elVsU0xtNmZzQ1JKcmFUa3ZtcjVTMTVySXc2UllPdGdpbDd6dHRvRnhoQVBPSk9CRXV0cjZxR0N1b3Fac0cwWkhHWGU1N3hHUDZmdHNLMFV1bCtFdG9XUmFlRHFVdUZ4YTlzeDlvTWtiL0pCODMvUlVXNkE4bzFxVT0iLAogICJxIjogIndUMVg4MzJWajB2SWsrTkw5cnRtM2owblg2V3JMVjZxQUt0TndYOG1HWXFPY0VCTGFWZ2RUVWZ0b05qaVdDaktFN0RmT2RKbnFuUDFRaFRhTFZDQWh2WE9UOVJyeXBoTjVTSVYxSEdRNHhpOWhBOHY4S1VCQm43RmhMc1pFZDROOUsyRVdldnZ0WEtGUzBLbGxjSXhucTBVV211elpOUDhlTkdEL3NXOFo5VT0iCn0= | base64 -d` > .credentials_rsaparams");
            // command.Script.Add("echo `echo 77u/ewogICJhZ2VudElkIjogNzQsCiAgImFnZW50TmFtZSI6ICJodGwtbWFjIiwKICAicG9vbElkIjogMSwKICAicG9vbE5hbWUiOiAiRGVmYXVsdCIsCiAgInNlcnZlclVybCI6ICJodHRwczovL3BpcGVsaW5lcy5hY3Rpb25zLmdpdGh1YnVzZXJjb250ZW50LmNvbS9uWkpYY2drdnVwdDJsMUt1ZGxNczBXdk5OdWM0N3dMSUlqZkpIS3FwZmU1d1FsVVB6biIsCiAgImdpdEh1YlVybCI6ICJodHRwczovL2dpdGh1Yi5jb20vVGluZ2x1b0h1YW5nL21haW4iLAogICJ3b3JrRm9sZGVyIjogIl93b3JrIgp9 | base64 -d` > .runner");
            // command.Script.Add("ls -l -a");
            // command.Script.Add("cat .runner");
            // command.Script.Add("cat .credentials");
            // command.Script.Add("cat .credentials_rsaparams");
            // command.Script.Add("cp ./bin/runsvc.sh ./runsvc.sh");
            // command.Script.Add("chmod u+x ./runsvc.sh");
            // command.Script.Add("nohup ./runsvc.sh interactive &");

            var command = new RunCommandInput("RunPowerShellScript");
            command.Script = new List<string>();
            command.Script.Add("if (Test-Path \"\\actions-runner\") { Remove-Item \"\\actions-runner\" -Recurse; }");
            command.Script.Add("mkdir \\actions-runner ; cd \\actions-runner");
            command.Script.Add($"[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; Invoke-WebRequest -Uri https://github.com/actions/runner/releases/download/v2.273.5/actions-runner-win-x64-2.273.5.zip -OutFile actions-runner-win-x64-2.273.5.zip");
            command.Script.Add($"Add-Type -AssemblyName System.IO.Compression.FileSystem ; [System.IO.Compression.ZipFile]::ExtractToDirectory(\"$PWD\\actions-runner-win-x64-2.273.5.zip\", \"$PWD\")");
            command.Script.Add($"[System.IO.File]::WriteAllText(\"$PWD\\.credentials\", [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String(\"77u/ewogICJzY2hlbWUiOiAiT0F1dGgiLAogICJkYXRhIjogewogICAgImNsaWVudElkIjogIjZlZGZmMzYyLTg5YjYtNGEwOC1iYTE0LTE0OGUxOTJjZTIyNCIsCiAgICAiYXV0aG9yaXphdGlvblVybCI6ICJodHRwczovL3ZzdG9rZW4uYWN0aW9ucy5naXRodWJ1c2VyY29udGVudC5jb20vX2FwaXMvb2F1dGgyL3Rva2VuL2JmMDhhODVlLTcyNDEtNDg1OC1hZWI4LWFjNzAwNTZhMTZkNCIKICB9Cn0=\")))");
            
            command.Script.Add($"$bytes=[System.Convert]::FromBase64String(\"77u/ewogICJkIjogImFRY3ZCaSt1MFQ4UWpTUHo0YUJwOXN3RHhaclZMTEczK2N1NWRGZWlTaFpFNkI0cWI1ZXFTcjNkR3JKekEvTjJvcEc5MlRsdlRFRkNZRzBJZVFKT3NvRE5sYU5Wc1FOeUVCZ1hob21NQ0k3UnJwWitQcmpBbjlHTE0xQUtsUWxZWCtyNzVIZGZNZnlkSnppeTRsRnlVL3dVY3Btc0wyUCtkRjg1Z3p3SEpqZlhoZENuMXNSa09POVRtUEl5bFA0aUtYcnJ5VnUvR0VZdjU4ZkdUdEp0dElCU082TGNOZjZNcGNLdDdOWkdwd0FCc0RwZGZLZ3RRVGdKT3BveDYrMVd2aUZFaXcxUVA2dGg4b0pWSXFpK0lCZXVNRzBENFhIM21GcTBPUmY3N3h3c0t0WE54ZVdMRlh3Yjd6bk5yNUIxY0VKQmt1Ry9ycHAva2FVR2JXbW4wUT09IiwKICAiZHAiOiAiRnVJWEgwV0ZuMWtmMERFaUE5RXdSaWxKTzZVVk5nRjdqT2xiWjJvVmdqV1BOQzNvbUttY1poQ3FpSDRtbExVVkpZMDF6MVNPUDNpTExoRjZMdGNjSjZ6ZStodUpGWTlZZHpWbitoclR3R21LMjI1WHRKZkNyc3kveVlBWTNGVjBiL2hJS21BTzFPdFJIM1dTdnBQb2dqQVdWU2Z4YWVjNityVXBzLytwWFNVPSIsCiAgImRxIjogImhrRDBSZnJPV2N3eEFtNDhiS011aU9DNzVqdlhmYUh6UXB2QlhkblV3cEg4L0FmdlhMVVhSb1hlbG8yWVU5K2k1YU5RczM5Mi9ubXB2OHQwK1Y2Z3JTTkJ2enk1MXZ1UndzdTNsdTEvMkFkc1E3KzVjdHR3cHU2VWsrN1pNQXl5YlhmUUtCay9aVFMzWHQ1aTVIVVN2RWRMaUxhaTRENVhmaWt5SjJPQnpqRT0iLAogICJleHBvbmVudCI6ICJBUUFCIiwKICAiaW52ZXJzZVEiOiAiQThRVUEvTjFPM0N2UGNHRXREZURUMVJ6WnFienY0RC9oL3o2c2RIZmZXZFNmWDlCS2RQeVM2TmZjYTFoZFcxWGJ2b2c4NkNhVHRkaU9sUDJtbmI3MktKTVU2dmdEekxJeHJrRWtuTXYyRktudzh6ZG5QQVZSTXpDZHEvTnY4bnNrSmovSjZ3c0FEV09CeW1sWGZKNHVsTHZic2h4cXIrSDVpODIzWUtLcXVNPSIsCiAgIm1vZHVsdXMiOiAiczR1UnY0MWtXZEF2MXdDMVBiLzhUd1hZd244WC9WSlJ6REZvemtRQ1ZkWi9vQk9Sc29wT1h3emhUb1RvMHh3TEVsdjYrTFc5M04yMjhkaWZFeGkyUUVhRUtwdi9oWU5uR3lpdktJSEcxeHJ5SUdvU3FsYmw4T2poMThoRWtyR0thbDdINlRZR3dPTjE5WWRMeHh2ZXVuRWltZTd2N3g3RFJQdi8xRmF4QXhkeDBrVGEyOTFoaDVnTVV0TWh3NHJuWVZuYldPMEI3VXpNamdLQ3dSUCs0ek8wUkl5QWdsR0xzTDlENmpEaGVOMGo5ZVUvZHRHckpjd1FVKzB2YnhNWjN1TEJwSjlVUlR3K2ZNd0NuOUcvVjFKODZtYUx0dHlBSW1hZnd4N0taRUJwM25EM3dKL29hZENOTGJoRFVSUmZxTjIxT0FGQzN1ZjU2V01PcFlMNlNRPT0iLAogICJwIjogIjdkdWV4Vys3L0RheXlWaTVySkhaUFJiVnNHNTVYWFI0bXlPZkVuUVVlY1V1OTNwZUR4elVsU0xtNmZzQ1JKcmFUa3ZtcjVTMTVySXc2UllPdGdpbDd6dHRvRnhoQVBPSk9CRXV0cjZxR0N1b3Fac0cwWkhHWGU1N3hHUDZmdHNLMFV1bCtFdG9XUmFlRHFVdUZ4YTlzeDlvTWtiL0pCODMvUlVXNkE4bzFxVT0iLAogICJxIjogIndUMVg4MzJWajB2SWsrTkw5cnRtM2owblg2V3JMVjZxQUt0TndYOG1HWXFPY0VCTGFWZ2RUVWZ0b05qaVdDaktFN0RmT2RKbnFuUDFRaFRhTFZDQWh2WE9UOVJyeXBoTjVTSVYxSEdRNHhpOWhBOHY4S1VCQm43RmhMc1pFZDROOUsyRVdldnZ0WEtGUzBLbGxjSXhucTBVV211elpOUDhlTkdEL3NXOFo5VT0iCn0=\")");
            command.Script.Add($"Add-Type -AssemblyName System.Security");
            command.Script.Add($"$encryptedBytes=[System.Security.Cryptography.ProtectedData]::Protect($bytes, $null, [System.Security.Cryptography.DataProtectionScope]::LocalMachine)");
            command.Script.Add($"[System.IO.File]::WriteAllBytes(\"$PWD\\.credentials_rsaparams\", $encryptedBytes)");
            command.Script.Add($"[System.IO.File]::WriteAllLines(\"$PWD\\.runner\", [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String(\"77u/ewogICJhZ2VudElkIjogNzQsCiAgImFnZW50TmFtZSI6ICJodGwtbWFjIiwKICAicG9vbElkIjogMSwKICAicG9vbE5hbWUiOiAiRGVmYXVsdCIsCiAgInNlcnZlclVybCI6ICJodHRwczovL3BpcGVsaW5lcy5hY3Rpb25zLmdpdGh1YnVzZXJjb250ZW50LmNvbS9uWkpYY2drdnVwdDJsMUt1ZGxNczBXdk5OdWM0N3dMSUlqZkpIS3FwZmU1d1FsVVB6biIsCiAgImdpdEh1YlVybCI6ICJodHRwczovL2dpdGh1Yi5jb20vVGluZ2x1b0h1YW5nL21haW4iLAogICJ3b3JrRm9sZGVyIjogIl93b3JrIgp9\")))");
            command.Script.Add("Start-Process -NoNewWindow $PWD\\run.cmd");

            Console.WriteLine(string.Join(Environment.NewLine,  command.Script));
            // var runResult = await tihuangvmss.RunCommandVMInstanceAsync(vmId, command);
            // foreach (var res in runResult.Value)
            // {
            //     Console.WriteLine($"${res.Code}: {res.Message}");
            // }

            // Console.WriteLine(runResult.Value.First().Message);
        }
    }
}
