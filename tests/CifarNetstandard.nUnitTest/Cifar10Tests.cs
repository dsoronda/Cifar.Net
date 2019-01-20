using System.IO;
using NUnit.Framework;

namespace CifarNetstandard.nUnitTest {
	public class Cifar10Tests {
		[SetUp]
		public void Setup() {
		}

		[Test]
		public void CanDownloadDataset_Success() {
			var tempPath = System.IO.Path.GetTempPath();
			Assert.IsTrue( System.IO.Directory.Exists( tempPath ) );

			var archiveFullPath = Path.Combine( tempPath, CifarNetstandard.Cifar10.Cifar10DatasetFileName );

			// download archive if we didn't do it already
			if (!File.Exists( archiveFullPath ))
				CifarNetstandard.Cifar10.DownloadDataset( tempPath );

			Assert.IsTrue( File.Exists( archiveFullPath ) );
			var fileInfo = new FileInfo( archiveFullPath );
			Assert.IsTrue( fileInfo.Length == 170_052_171 );

		}

		[Test]
		public void CanExtractDataset_Success() {
			var targetDir = @"c:\__storage\ML\";

			var downloadPath = System.IO.Path.GetTempPath();
			var archiveFullPath = Path.Combine( downloadPath, CifarNetstandard.Cifar10.Cifar10DatasetFileName );
			Assert.IsTrue( File.Exists( archiveFullPath ), "missing archive file" );

			CifarNetstandard.Cifar10.DecompressTarArchive( archiveFullPath, targetDir );
			var extractedFullPath = Path.Combine( targetDir, "cifar-10-batches-bin" );
			Assert.IsTrue( Directory.Exists( extractedFullPath ), "missing target directory" );
			var dirInfo = new DirectoryInfo( extractedFullPath );
			var files = dirInfo.GetFiles();
			Assert.AreEqual( 8, files.Length, "not all files extracted" );
		}
	}
}
