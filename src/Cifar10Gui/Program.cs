using System;
using System.Linq;
using System.Threading.Tasks;

using Eto.Forms;

namespace Cifar10Gui {
	internal class Program {
		[STAThread]
		static void Main(string[] args) {
			//new Application(Eto.Platform.Detect).Run(new MainForm());
			//new Application( Eto.Platforms.Gtk ).Run( new MainForm() );

			//new Application( Eto.Platforms.Gtk ).Run( new MyForm() );
			//return;

			//new Application( Eto.Platforms.Gtk ).Run( new DataTableForm() );
			//return;


			var cifar = new CifarNetCore.Cifar10( @"d:\data\cifar-10-batches-bin" );
			cifar.ParseDataset().Wait();
			cifar.ParseTestBatch().Wait();


			// NOTE : FIRST init Application
			var application = new Application( Eto.Platforms.Gtk );
			//var application = new Application( Eto.Platforms.WinForms );

			// NOTE : then create Form (or you will get EXCEPTION !!!)
			var myForm = new MyImageForm( 640, 480 );
			//await myForm.LoadImage( @"d:\dload\ofce.jpg" );

			//var cifarImage = cifar.Dataset.Dogs.ToList()[3];
			//myForm.MyImageView.Image = cifarImage.RGBData.RGBToImage();

			var images = cifar.Dataset.Dogs.ToList().Take( 50 ).Select( x => x.BGR_Data.RGBToImage() );
			myForm.ShowImages( images );
			application.Run( myForm );

		}

	}
}
