using System.Collections.Generic;
using Eto.Drawing;

namespace Cifar10Gui {
	public static class ImageExtensions {
		//public static Task<Image> ToImage(this byte[] rgbArray ) {
		public static Image RGBToImage( this byte[] rgbArray ) {
			var pixels = new List<Color>();

			for (var counter = 0; counter < rgbArray.Length; counter += 3) {
				var red = rgbArray[counter + 0] /255f;
				var green = rgbArray[counter + 1] / 255f;
				var blue = rgbArray[counter + 2] /255f;
				var color = new Color( red: red, green: green, blue: blue, alpha: 1 );
				//var color = new Color( rgbArray[counter + 2], rgbArray[counter + 0], rgbArray[counter + 1] );
				//var color = new Color( rgbArray[counter + 1], rgbArray[counter + 2], rgbArray[counter + 0] );

				//var color = new Color( rgbArray[counter + 0], rgbArray[counter + 2], rgbArray[counter + 1] );
				//var color = new Color( rgbArray[counter + 2], rgbArray[counter + 1], rgbArray[counter + 0] );
				//var color = new Color(red: rgbArray[counter + 1], green: rgbArray[counter + 0], rgbArray[counter + 2] );
				pixels.Add( color );
			}

			var image = new Eto.Drawing.Bitmap( 32, 32, PixelFormat.Format32bppRgb, pixels );
			return image;
		}

		public static Image ChannelsToImage( this byte[] byteArray ) {
			var pixels = new List<Color>();

			for (var counter = 0; counter < 32 * 32; counter++) {
				var color = new Color( byteArray[counter], byteArray[counter + 1024], byteArray[counter + 2048] );
				pixels.Add( color );
			}

			var image = new Eto.Drawing.Bitmap( 32, 32, PixelFormat.Format24bppRgb, pixels );
			return image;
		}


		public static Image DrawEvenPixels( int width = 32, int height = 32 ) {
			var pixelsCount = width * height;
			var colors = new List<Color>( width * height );

			for (var counter = 0; counter < pixelsCount; counter++) {
				var isOn = counter % 2 == 0;
				var colorValue = isOn ? 1f : 0f;
				var color = new Color( red: colorValue, green: colorValue, blue: colorValue, alpha: 1 );
				//Color color = new Color( 1, 1, 1, 1 );
				colors.Add( color );
			}

			var image = new Eto.Drawing.Bitmap( width, height, PixelFormat.Format32bppRgba, colors );

			return image;
		}

	}
}
