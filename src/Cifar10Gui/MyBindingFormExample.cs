using System.Collections.ObjectModel;
using Eto.Forms;

namespace Cifar10Gui {
	class MyPoco {
		public string Text { get; set; }

		public bool Check { get; set; }
	}

	class MyBindingFormExample : Form {
		public MyBindingFormExample() {
			var collection = new ObservableCollection<MyPoco> {
				new MyPoco { Text = "Row 1", Check = true },
				new MyPoco { Text = "Row 2", Check = false }
			};

			var grid = new GridView {
				DataStore = collection,
			};

			grid.Columns.Add( new GridColumn {
				DataCell = new TextBoxCell { Binding = Binding.Property<MyPoco, string>( r => r.Text ) },
				HeaderText = "Text"
			} );

			grid.Columns.Add( new GridColumn {
				DataCell = new CheckBoxCell { Binding = Binding.Property<MyPoco, bool?>( r => r.Check ) },
				HeaderText = "Check"
			} );
			Content = grid;
		}
	}
}
