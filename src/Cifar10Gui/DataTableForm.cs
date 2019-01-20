using System.Data;
using System.Linq;
using Eto.Forms;

namespace Cifar10Gui {
	class DataTableForm : Form {
		public DataTableForm() {
			var table = new DataTable();
			table.Columns.Add( "TextColumn" );
			table.Columns.Add( "CheckColumn", typeof( bool ) );

			table.Rows.Add( "Jurica 1", true );
			table.Rows.Add( "Perica", false );

			var collection = table.Rows.Cast<DataRow>()
				.Select( x => new { Text = x[0], Check = x[1] } )
				.ToList();

			var grid = new GridView { DataStore = collection };

			grid.Columns.Add( new GridColumn {
				DataCell = new TextBoxCell( "Text" ),
				HeaderText = "Text"
			} );

			grid.Columns.Add( new GridColumn {
				DataCell = new CheckBoxCell( "Check" ),
				HeaderText = "Check"
			} );

			Content = grid;
		}
	}
}
