using System.IO;
using System.Threading.Tasks;
using CifarNetCore;
using NUnit.Framework;

namespace Tests {
	public class Cifar10Tests {
		[SetUp]
		public void Setup() {
		}

		// download dataset, we don't to do it in all test runs
		//[Test]
		public void CanDownloadDataset_Success() {
			var tempPath = System.IO.Path.GetTempPath();
			Assert.IsTrue( System.IO.Directory.Exists( tempPath ) );

			var archiveFullPath = Path.Combine( tempPath, Cifar10.Cifar10DatasetFileName );

			// download archive if we didn't do it already
			if (!File.Exists( archiveFullPath ))
				Cifar10.DownloadDataset( tempPath );

			Assert.IsTrue( File.Exists( archiveFullPath ) );
			var fileInfo = new FileInfo( archiveFullPath );
			Assert.IsTrue( fileInfo.Length == 170_052_171 );

		}

		// download dataset, we don't to do it in all test runs
		//[Test]
		public void CanExtractDataset_Success() {
			var targetDir = @"c:\__storage\ML\";

			var downloadPath = System.IO.Path.GetTempPath();
			var archiveFullPath = Path.Combine( downloadPath, Cifar10.Cifar10DatasetFileName );
			Assert.IsTrue( File.Exists( archiveFullPath ), "missing archive file" );

			Cifar10.DecompressTarArchive( archiveFullPath, targetDir );
			var extractedFullPath = Path.Combine( targetDir, "cifar-10-batches-bin" );
			Assert.IsTrue( Directory.Exists( extractedFullPath ), "missing target directory" );
			var dirInfo = new DirectoryInfo( extractedFullPath );
			var files = dirInfo.GetFiles();
			Assert.AreEqual( 8, files.Length, "not all files extracted" );
		}

		[Test]
		public async Task ReadTestBatchFile_Success() {
			const string cifar_dataset_dir = @"c:\__storage\ML\cifar-10-batches-bin";
			var cifar10 = new Cifar10( cifar_dataset_dir );
			Assert.IsNotEmpty( cifar10.MetaTags );

			await cifar10.ParseTestBatch();

			Assert.IsNotEmpty( cifar10.TestBatchImages, "Test batch set is empty" );
			Assert.AreEqual( 10_000, cifar10.TestBatchImages.Count );

		}


		[Test]
		public async Task ReadDatasetFiles_Success() {
			const string cifar_dataset_dir = @"c:\__storage\ML\cifar-10-batches-bin";
			var cifar10 = new Cifar10( cifar_dataset_dir );
			Assert.IsNotEmpty( cifar10.MetaTags );

			await cifar10.ParseDataset();

			Assert.IsNotEmpty( cifar10.DatasetImages, "Datset is empty" );
			Assert.AreEqual( 50_000, cifar10.DatasetImages.Count );

		}
	}
}
