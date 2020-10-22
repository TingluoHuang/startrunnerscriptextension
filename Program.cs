using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using System.Linq;

namespace startrunnerscriptextension
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var creds = new AzureCredentialsFactory().FromFile("my.azureauth");
            var azure =  await Azure.Configure()
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

            var tihuangvmss = vmsss.Where(v=>v.Name=="tihuang-vmss-test").FirstOrDefault();

            var vms = await tihuangvmss.VirtualMachines.ListAsync();

            Console.WriteLine(vms.Count());

            tihuangvmss = await tihuangvmss.Update()
                .DefineNewExtension("CustomScriptForLinux")
                .WithPublisher("Microsoft.Azure.Extensions")
                .WithType("CustomScript")
                .WithVersion("2.0")
                .WithMinorVersionAutoUpgrade()
                .WithPublicSetting("fileUris", "https://raw.githubusercontent.com/TingluoHuang/startrunnerscriptextension/main/run.sh")
                .Attach().ApplyAsync();
        }
    }
}
