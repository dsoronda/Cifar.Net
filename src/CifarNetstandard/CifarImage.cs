namespace CifarNetstandard {
	public class CifarImage {
		public const int Width = 32;
		public const int Height = 32;

		public int LabelId { get; set; }
		public Cifar10Labels CifarLabel { get; }
		public byte[] RawData { get; set; } = new byte[3072];

		/// <summary>
		/// 32 * 32 Image Red component
		/// </summary>
		public byte[] Red { get; }

		/// <summary>
		/// 32 * 32 Image Green component
		/// </summary>
		public byte[] Green { get; }

		/// <summary>
		/// 32 * 32 Image Blue component
		/// </summary>
		public byte[] Blue { get; }

		/// <summary>
		/// Create new Image data as RGB RGB RGB pixels vs RRR GGG BBB components
		/// </summary>
		public byte[] RGBData {
			get {
				var rgbBytes = new byte[Width * Height * 3];
				for (var pixelOffset = 0; pixelOffset < Width * Height; pixelOffset++) {
					rgbBytes[( pixelOffset * 3 ) + 0] = Red[pixelOffset];   // Red
					rgbBytes[( pixelOffset * 3 ) + 1] = Green[pixelOffset]; // Green
					rgbBytes[( pixelOffset * 3 ) + 2] = Blue[pixelOffset];  // Blue
				}
				return rgbBytes;
			}
		}
	}
}
