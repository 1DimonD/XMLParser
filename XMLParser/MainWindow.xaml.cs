using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace XMLParser {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow {
		public MainWindow() {
			InitializeComponent();
		}

		private void FindButton_OnClick(object sender, RoutedEventArgs e) {
			IXMLFinder finder = GenerateFinder();

			Scientist condition = new Scientist();
			
			List<Scientist> scientists = finder.Find(condition);
			
			
		}

		private IXMLFinder GenerateFinder() {
			if (RadioDOM.IsChecked ?? false) return new DOMFinder();
			if (RadioDOM.IsChecked ?? false) return new DOMFinder();
			return new LINQFinder(); 
		}
	}
}