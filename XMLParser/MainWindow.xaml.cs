using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace XMLParser {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow {
		private List<CheckBox> checkboxes = new List<CheckBox>();
		private List<TextBox> textboxes = new List<TextBox>();
		
		public MainWindow() {
			InitializeComponent();
			FillConditions();
		}

		private void FillConditions() {
			string[] options = {"ПІП", "Факультет", "Кафедра", "Науковий ступінь", "Вченні звання"};

			foreach (var option in options) {
				var stack = new StackPanel();
				
				var checkbox = new CheckBox();
				checkbox.Content = option;
				stack.Children.Add(checkbox);
				checkboxes.Add(checkbox);

				var textBox = new TextBox();
				textBox.Height = 15;
				textBox.Margin = new Thickness(0, 0, 0, 40);
				stack.Children.Add(textBox);
				textboxes.Add(textBox);
				
				Conditions.Children.Add(stack);
			}
		}

		private void FindButton_OnClick(object sender, RoutedEventArgs e) {
			IXMLFinder finder = GenerateFinder();

			List<Scientist> scientists = finder.Find(GetCondition());

			if (scientists.Count == 0) MessageBox.Show("Nothing were found");
			foreach(var sc in scientists) Print(sc);
		}

		private IXMLFinder GenerateFinder() {
			if (RadioDOM.IsChecked == true) return new DOMFinder();
			if (RadioSAX.IsChecked == true) return new SAXFinder();
			return new LINQFinder(); 
		}

		private Scientist GetCondition() {
			List<string> parameters = new List<string>();
			for (int i = 0; i < checkboxes.Count; i++) {
				parameters.Add(checkboxes[i].IsChecked == true ? textboxes[i].Text : "");
			}
			return new Scientist(parameters);
		}
		
		private void Print(Scientist sc) {
			CurrentDocument.Text += sc.fullName + "\n";
			CurrentDocument.Text += sc.faculty + "\n";
			CurrentDocument.Text += sc.department + "\n";
			CurrentDocument.Text += sc.scientificDegree + "\n";
			foreach (var kvp in sc.datesToAcademicTitles) {
				var date = kvp.Key;
				var title = kvp.Value;
				CurrentDocument.Text += title + ": " + date + "\n";
			}
		}
		
	}
}