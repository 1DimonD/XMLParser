using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Xsl;

namespace XMLParser {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow {
		private List<CheckBox> checkboxes = new();
		private List<TextBox> textboxes = new();
		
		string[] options = {"ПІП", "Факультет", "Кафедра", "Науковий ступінь", "Вченні звання"};
		
		public MainWindow() {
			InitializeComponent();
			FillConditions();
		}

		private void FillConditions() {
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
			try {
				IXMLFinder finder = GenerateFinder();

				List<Scientist> scientists = finder.Find(GetCondition());

				if (scientists.Count == 0) MessageBox.Show("Nothing were found");
				CurrentDocument.Text = "";
				foreach (var sc in scientists) Print(sc);
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
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

			CurrentDocument.Text += "\n";
		}

		private void ConvertButton_OnClick(object sender, RoutedEventArgs e) {
			var xct = new XslCompiledTransform();
			xct.Load(Test.filePathXSL);
			string input = CreateCurrentXML();
			xct.Transform(input, Test.filePathHTML);
			
			System.Diagnostics.Process.Start(Test.filePathHTML);
		}

		private string CreateCurrentXML() {
			string filePath = Test.filePathFoundXML;
			
			XmlDocument xDoc = new XmlDocument();

			XmlElement list = xDoc.CreateElement("List");
			xDoc.AppendChild(list);

			foreach (var option in options) {
				XmlElement label = xDoc.CreateElement("Label");
				label.InnerText = option;
				list.AppendChild(label);
			}

			var args =  CurrentDocument.Text.Split('\n');

			int i = 0;
			XmlElement scientist = null;
			foreach (var arg in args) {
				if (arg == "") {
					i = 0;
					continue;
				}
				
				if (i == 0) {
					scientist = xDoc.CreateElement("Scientist");
					list.AppendChild(scientist);
					i = 0;
				}

				string name = i switch {
					0 => "FullName",
					1 => "Faculty",
					2 => "Department",
					3 => "ScientificDegree",
					_ => "AcademicTitles"
				};

				XmlElement argument = xDoc.CreateElement(name);
				argument.InnerText = arg;
				scientist.AppendChild(argument);
				i++;
			}
			
			xDoc.Save(filePath);
			return filePath;
		}

	}
}