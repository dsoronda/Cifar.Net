using System;
using System.Collections.Generic;
using System.IO;


namespace CifarNetstandard {
	public class Cifar10 {
		public const string Cifar10DatasetFileName = "cifar-10-binary.tar.gz";
		public const string Cifar10DatasetUrl = "http://www.cs.toronto.edu/~kriz/cifar-100-binary.tar.gz";
		public const string TestBatchFileName = "test_batch.bin";
		public const string MetaFileName = "batches.meta.txt";

		//public const string Cifar100DatasetUrl = "http://www.cs.toronto.edu/~kriz/cifar-10-binary.tar.gz";
		private List<string> datasetFileNames = new List<string>();
		//private readonly string[] metaTags;

		public Cifar10( string datasetDirectoryPath ) {
			if (string.IsNullOrWhiteSpace( datasetDirectoryPath ))
				throw new ArgumentException( "message", nameof( datasetDirectoryPath ) );

			if (!Directory.Exists( datasetDirectoryPath ))
				throw new ArgumentException( "Dataset location not found" );

			for (var counter = 1; counter < 5; counter++) {
				datasetFileNames.Add( $"data_batch_{counter}.bin" );
			}

			var metaFilePath = Path.Combine( datasetDirectoryPath, MetaFileName );
			if (!File.Exists( metaFilePath ))
				throw new ArgumentException( "Metafile not found" );

			var metatags = File.ReadAllLines( MetaFileName );
		}

		public void ParseDataset() {
			// do we have all files ? dataset + test set
			// do we have metafile ? do we need metafile ?
			// load files as stream
			// parse in parallel

			throw new NotImplementedException();
		}

		public static void DownloadDataset( string savePath ) {

			if (string.IsNullOrWhiteSpace( savePath )) {
				throw new ArgumentException( "Missing destination save path value", nameof( savePath ) );
			}

			if (!System.IO.Directory.Exists( savePath ))
				throw new ArgumentException( "Destination save path not found" );
			var fullPath = System.IO.Path.Combine( savePath, Cifar10DatasetFileName );
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

	}
}
