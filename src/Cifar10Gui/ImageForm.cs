using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;

namespace Cifar10Gui {
	public class MyImageForm : Form {
		public ImageView MyImageView = new ImageView();

		ObservableCollection<Image> imagesCollection = new ObservableCollection<Image>();

		public MyImageForm( uint width, uint height ) {

			Title = nameof( MyImageForm );
			ClientSize = new Eto.Drawing.Size( (int) width, (int) height );
			//Content = new Panel() {
			//	Content = MyImageView
			//};
			//Content = SetupImageGrid();
			//Content = new Scrollable() { Content = StackLayout };

		}

		protected override void OnSizeChanged( EventArgs e ) {
			base.OnSizeChanged( e );
			//System.Diagnostics.Debug.WriteLine( "OnSizeChanged: {0}", this.Size );
			Console.WriteLine( "OnSizeChanged: {0}", this.Size );

			if (Content != null) {
				//if(this.Content is Container) {
				//	var child = (Panel) this.Content;
				//}

				var stackLayout = (Panel)this.Content;
				Console.WriteLine( $"StackLayout width {stackLayout.Size.Width}, heigh {stackLayout.Size.Height}" );
				Console.WriteLine( $"StackLayout client width {stackLayout.ClientSize.Width}, heigh {stackLayout.ClientSize.Height}" );

			}
		}


		private StackLayout SetupStackLayout() {
			var layout = new StackLayout() {
				Spacing = 2,
				Orientation = Orientation.Horizontal,

				// Orientation
			};
			return layout;
		}

		private GridView SetupImageGrid() {
			var grid = new GridView<Image> { DataStore = imagesCollection, AllowColumnReordering = true };

			grid.Columns.Add( new GridColumn {
				//DataCell = new ImageViewCell { Binding = Binding.Property<Image, Image>( r => r.) },
				DataCell = new ImageViewCell { Binding = Binding.Property<Image, Image>( x => x ), },
				HeaderText = "Image",
				// AutoSize =true,
			} );

			return grid;
		}

		public async Task LoadImage( string imagePath ) {
			if (string.IsNullOrWhiteSpace( imagePath ))
				throw new ArgumentException( nameof( imagePath ) );

			if (!File.Exists( imagePath ))
				throw new ArgumentException( $"Image not found {imagePath}" );

			var imageLoadTask = Task.Run( () => {
				var image = new Eto.Drawing.Bitmap( imagePath );
				return image;
			} );

			await imageLoadTask;
			MyImageView.Image = await imageLoadTask;

		}

		public void ShowImages( IEnumerable<Eto.Drawing.Image> images ) {

			if (images == null)
				throw new ArgumentNullException( nameof( images ) );

			var stackLayout = SetupStackLayout();

			foreach (var item in images) {
				var imgView = new ImageView() {
					Image = item,
					Width = 64, Height = 64
				};

				stackLayout.Items.Add( imgView );
			}
			this.Content = new Scrollable { Content = stackLayout };
			//this.Content =  stackLayout ;

		}

		public void ShowImagesInGrid( IEnumerable<Eto.Drawing.Image> images ) {
			if (images == null)
				throw new ArgumentNullException( nameof( images ) );

			foreach (var item in images) {
				imagesCollection.Add( item );
			}
			this.Content = SetupImageGrid();
		}
	}

	public class VerticalStack : StackLayout {
		public VerticalStack() : base() {
			Orientation = Orientation.Vertical;
			this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
		}
	}


}
