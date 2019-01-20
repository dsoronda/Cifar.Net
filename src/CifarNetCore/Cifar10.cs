using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CifarNetCore {
	public class Cifar10 {
		public const string Cifar10DatasetFileName = "cifar-10-binary.tar.gz";
		public const string Cifar10DatasetUrl = "http://www.cs.toronto.edu/~kriz/cifar-100-binary.tar.gz";
		public const string TestBatchFileName = "test_batch.bin";
		public const string MetaFileName = "batches.meta.txt";
		private const int Cifar10ImageBlockSize = 3073; // <1 x label><3072 x pixel>

		//public const string Cifar100DatasetUrl = "http://www.cs.toronto.edu/~kriz/cifar-10-binary.tar.gz";
		private List<string> datasetFileNames = new List<string>();

		/// <summary>
		/// Location where files are stored (datasets, test and metadata files, 8 in total)
		/// This is not location of tar.gz archive!
		/// </summary>
		public string DataSetDirectory { get; }

		/// <summary>
		/// List of paths to Dataset files
		/// </summary>
		private IEnumerable<string> DataSetsFilePath {
			get {
				foreach (var item in this.datasetFileNames)
					yield return Path.Combine( DataSetDirectory, item );
			}
		}

		public readonly string[] MetaTags;
		public readonly string MetaFilePath;

		private string TestBatchFilePath => Path.Combine( DataSetDirectory, TestBatchFileName );

		private readonly List<CifarImage> _testBatchImages = new List<CifarImage>();
		public IReadOnlyList<CifarImage> TestBatchImages => _testBatchImages;

		private readonly List<CifarImage> _datasetImages = new List<CifarImage>();
		public IReadOnlyList<CifarImage> DatasetImages => _datasetImages;

		public DatasetsHelper Dataset { get; private set; }

		/// <summary>
		/// Init Cifar10 module
		/// </summary>
		/// <param name="datasetDirectoryPath">Location of dataset files</param>
		public Cifar10( string datasetDirectoryPath ) {
			if (string.IsNullOrWhiteSpace( datasetDirectoryPath ))
				throw new ArgumentException( "message", nameof( datasetDirectoryPath ) );

			if (!Directory.Exists( datasetDirectoryPath ))
				throw new ArgumentException( "Dataset location not found" );

			for (var counter = 1; counter <= 5; counter++) {
				datasetFileNames.Add( $"data_batch_{counter}.bin" );
			}

			DataSetDirectory = datasetDirectoryPath;

			// do we have all files ? dataset + test set
			foreach (var item in this.DataSetsFilePath) {
				if (!File.Exists( item ))
					throw new FileNotFoundException( $"Missing file {item}" );
			}

			if (!File.Exists( TestBatchFilePath ))
				throw new FileNotFoundException( $"Missing file {TestBatchFilePath}" );


			MetaFilePath = Path.Combine( datasetDirectoryPath, MetaFileName );
			if (!File.Exists( MetaFilePath ))
				throw new ArgumentException( "Metafile not found" );

			MetaTags = File.ReadAllLines( MetaFilePath ).Where( line => !string.IsNullOrWhiteSpace( line ) ).ToArray();
		}

		public async Task Init() {
			await ParseDataset();
			await ParseTestBatch();
		}

		public async Task ParseDataset( CancellationToken cancellationToken = default ) {
			if (DatasetImages.Count == 50000)
				return;

			// do we have metafile ? do we need metafile ?
			// load files as stream or byte[]
			// TODO :parse in parallel or async ?
			foreach (var filePath in DataSetsFilePath) {
				// open file as stream or byte[] ???
				// read 1 + (3 * 1024) bytes of image data
				// converr first byte to image label
				// convert image data to 32x32 Red, Green, Blue data

				var datasetBytes = await File.ReadAllBytesAsync( filePath, cancellationToken: cancellationToken );
				for (var offset = 0; offset < datasetBytes.Length; offset += Cifar10ImageBlockSize) {
					var image = new CifarImage( ( new Span<byte>( datasetBytes ) ).Slice( offset, Cifar10ImageBlockSize ).ToArray() );
					_datasetImages.Add( image );
				}
			}

			this.Dataset = new DatasetsHelper( this.DatasetImages );
		}

		public async Task ParseTestBatch( CancellationToken cancellationToken = default, IProgress<double> progress = default ) {
			//_testBatchImages.Clear();
			if (TestBatchImages.Count == 10000)
				return;


			var bla = await File.ReadAllBytesAsync( TestBatchFilePath, cancellationToken: cancellationToken );
			for (var offset = 0; offset < bla.Length; offset += Cifar10ImageBlockSize) {
				var image = new CifarImage( ( new Span<byte>( bla ) ).Slice( offset, Cifar10ImageBlockSize ).ToArray() );
				_testBatchImages.Add( image );

				// report progress
				if (progress != null && ( offset * 100 / bla.Length ) % 100 == 0)
					progress.Report( (double) offset * 100 / bla.Length );
			}
		}

		public static void DownloadDataset( string savePath ) {

			if (string.IsNullOrWhiteSpace( savePath )) {
				throw new ArgumentException( "Missing destination save path value", nameof( savePath ) );
			}

			if (!Directory.Exists( savePath ))
				throw new ArgumentException( "Destination save path not found" );

			var fullPath = Path.Combine( savePath, Cifar10DatasetFileName );

			try {
				using (var client = new System.Net.WebClient()) {
					client.DownloadFile( Cifar10DatasetUrl, fullPath );
				}
			} catch (System.Exception) {
				throw new Exception( $"Unable to download file {Cifar10DatasetUrl} into location {savePath}" );
			}
		}

		public static void DecompressTarArchive( string archiveFilePath, string destination ) {
			if (string.IsNullOrWhiteSpace( archiveFilePath ))
				throw new ArgumentException( "message", nameof( archiveFilePath ) );
			if (string.IsNullOrWhiteSpace( destination ))
				throw new ArgumentException( "message", nameof( destination ) );


			if (!File.Exists( archiveFilePath ))
				throw new FileNotFoundException( $"{archiveFilePath} not found!" );
			if (!Directory.Exists( destination ))
				throw new DirectoryNotFoundException( $"{destination} directory not found" );

			using (var memoryTempStream = new MemoryStream()) {
				using (var gzStream = File.OpenRead( archiveFilePath )) {
					ICSharpCode.SharpZipLib.GZip.GZip.Decompress( gzStream, memoryTempStream, false );
				}

				// seek to beginning of stream
				memoryTempStream.Seek( 0, 0 );
				using (var tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive( memoryTempStream )) {
					tarArchive.ExtractContents( destination );
					tarArchive.Close();
				}
				memoryTempStream.Close();
			}

			// UnGzFile(gzip, Path.Combine(trash, Path.GetFileNameWithoutExtension(gzip)));
			// File.Delete(gzip);
			// var tar = Directory.GetFiles(trash, "*.tar")[0];
			// var stream = File.OpenRead(tar);
			// var tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(stream);
			// tarArchive.ExtractContents(trash);
			// tarArchive.Close();
			// stream.Close();

		}

		public class DatasetsHelper {
			public IReadOnlyList<CifarImage> Images { get; }

			internal DatasetsHelper( IReadOnlyList<CifarImage> datasetImages ) {
				if (datasetImages == null)
					throw new ArgumentNullException( nameof( datasetImages ) );

				this.Images = datasetImages;
			}

			public IEnumerable<CifarImage> Airplanes => Images.Where( image => image.LabelId == (int) Cifar10Labels.airplane );
			public IEnumerable<CifarImage> Automobiles => Images.Where( image => image.LabelId == (int) Cifar10Labels.automobile );
			public IEnumerable<CifarImage> Birds => Images.Where( image => image.LabelId == (int) Cifar10Labels.bird );
			public IEnumerable<CifarImage> Cats => Images.Where( image => image.LabelId == (int) Cifar10Labels.cat );
			public IEnumerable<CifarImage> Deers => Images.Where( image => image.LabelId == (int) Cifar10Labels.deer );
			public IEnumerable<CifarImage> Dogs => Images.Where( image => image.LabelId == (int) Cifar10Labels.dog );
			public IEnumerable<CifarImage> Frogs => Images.Where( image => image.LabelId == (int) Cifar10Labels.frog );
			public IEnumerable<CifarImage> Horses => Images.Where( image => image.LabelId == (int) Cifar10Labels.horse );
			public IEnumerable<CifarImage> Ships => Images.Where( image => image.LabelId == (int) Cifar10Labels.ship );
			public IEnumerable<CifarImage> Trucks => Images.Where( image => image.LabelId == (int) Cifar10Labels.truck );



		}
	}
}
