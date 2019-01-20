using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CifarConsoleApp {
	public static class Program {
		public static async Task Main( string[] args ) {
			// https://blogs.msdn.microsoft.com/premier_developer/2018/04/26/setting-up-net-core-configuration-providers/
			var builder = new ConfigurationBuilder()
							.SetBasePath( Directory.GetCurrentDirectory() )
							.AddJsonFile( "appsettings.json", optional: true, reloadOnChange: true )
							.AddEnvironmentVariables()
							.AddCommandLine( args )
				//.AddEFConfiguration(options => options.UseInMemoryDatabase("InMemoryDb"));
				;


			IConfigurationRoot configuration = builder.Build();

			// add references to Microsoft.Extensions.Configuration.Binder !!?!@@#!#
			// https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.configurationbinder.get?view=aspnetcore-2.2#Microsoft_Extensions_Configuration_ConfigurationBinder_Get__1_Microsoft_Extensions_Configuration_IConfiguration_
			var appConfiguration = configuration.Get<AppConfiguration>();

			Console.WriteLine( $"connection string : {appConfiguration.Cifar10DatasetsLocation}" );
			var cifar = new CifarNetCore.Cifar10( appConfiguration.Cifar10DatasetsLocation );


			//IProgress<int> progress = new Progress<int>(x = >System.Console.WriteLine($"done : {x.}");) ;
			Progress<double> bla = null;// new Progress<double>( x => Console.WriteLine( $"progress: {x * 100:n} %" ) );

			var watch = new Stopwatch();
			watch.Start();
			await cifar.ParseTestBatch( progress: bla );
			watch.Stop();

			Console.WriteLine( $"TestBath done in  {watch.ElapsedMilliseconds} ms" );

			watch.Restart();
			await cifar.ParseDataset();
			Console.WriteLine( $"dataset read done in {watch.ElapsedMilliseconds} ms" );
			watch.Stop();

			Console.WriteLine($"total images in dataset {cifar.DatasetImages.Count}");

			Console.ReadKey();

		}
	}

	internal class AppConfiguration {
		public string Cifar10DatasetsLocation { get; set; }
		public string ConnectionString { get; set; }

		public class WindowConfiguration {
			public int Height { get; set; }
			public int Width { get; set; }
			public int Left { get; set; }
			public int Top { get; set; }
		}

		public class ProfileConfiguration {

			public string UserName { get; set; }
		}
	}

}
