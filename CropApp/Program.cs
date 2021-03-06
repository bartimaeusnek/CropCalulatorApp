using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CropApp.Backend;
using CropApp.Frontend;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace CropApp
{
    public class Program
    {
        public static ImpressumData ImpressumData;
        public static async Task Main(string[] args)
        {
            
            TypeDescriptor.AddAttributes(typeof((string, string)),
                                         new TypeConverterAttribute(typeof(TypeConverterStringTouple)));

            //TODO: Simple Backend JSON config
            
            using var r    = new StreamReader("wwwroot/CropRegistry.json");
            using var ir = new StreamReader("wwwroot/Impressum.json");
            var       json = await r.ReadToEndAsync();
            var ijson = ir.ReadToEndAsync();
          
            CropCalculation.AllCrops = JsonConvert.DeserializeObject<List<CropModel>>(json);
            for (var i = 0; i < CropCalculation.AllCrops.Count; i++) CropCalculation.AllCrops[i].interalID = i;
            await CropCalculation.ProcessBreeding();
            
            ImpressumData = JsonConvert.DeserializeObject<ImpressumData>(await ijson);
            
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                                          {
                                              webBuilder.UseStartup<Startup>();
                                              webBuilder.UseUrls("http://*:5000");
                                          });
    }
}