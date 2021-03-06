using System;

namespace CifarNetCore {
	public class CifarImage {
		public const int Width = 32;
		public const int Height = 32;
		public const int PixelsPerPlane = Width * Height;

		public CifarImage( byte[] rawImageDataWithLabel ) {
			if (rawImageDataWithLabel.Length != ( ( 3 * PixelsPerPlane ) + 1 )) {// + 1 for label!
				throw new ArgumentOutOfRangeException( "Invalid Image size" );
			} 

			LabelId = rawImageDataWithLabel[0];

			//RawData = ( new Span<byte>( rawImageDataWithLabel ) ).Slice( 1, 3 * 1024 );
			// I'm copy bytes for better byte aligment in memory (originals are offset by 1 from label)
			Buffer.BlockCopy( src: rawImageDataWithLabel, srcOffset: 1, dst: RawData, dstOffset: 0, count: 3 * PixelsPerPlane );
		}

		public int LabelId { get; private set; }
		public Cifar10Labels CifarLabel => (Cifar10Labels) LabelId;
		public byte[] RawData { get; set; } = new byte[3 * PixelsPerPlane];

		public Span<byte> Red => RawData.AsSpan().Slice( 0, length: PixelsPerPlane );
		public Span<byte> Green => RawData.AsSpan().Slice( start: PixelsPerPlane, length: PixelsPerPlane );
		public Span<byte> Blue => RawData.AsSpan().Slice( start: 2 * PixelsPerPlane, length: PixelsPerPlane );

		public Span<byte> Data => RawData.AsSpan();


		/// <summary>
		/// Create new Image data as BGR BGR BGR pixels vs RRR GGG BBB components
		/// BMP is little-endian format
		/// </summary>
		public byte[] BGR_Data {
			get {
				var bytes = new byte[PixelsPerPlane * 3];
				for (var planePixelOffset = 0; planePixelOffset < PixelsPerPlane; planePixelOffset++) {
					var rgbOffset = planePixelOffset * 3;
					bytes[rgbOffset + 0] = Blue[planePixelOffset];  // Blue
					bytes[rgbOffset + 1] = Green[planePixelOffset]; // Green
					bytes[rgbOffset + 2] = Red[planePixelOffset];   // Red
				}
				return bytes;
			}
		}
	}
}
