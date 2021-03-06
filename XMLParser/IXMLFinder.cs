using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace XMLParser {

	public static class Test {
		private static string coreCatalog = Environment.CurrentDirectory + @"..\..\..\TestFiles\";
		public static string filePathXML = coreCatalog + "Test1.xml";
		public static string filePathXSL = coreCatalog + "Table.xsl";
		public static string filePathHTML = coreCatalog + "Table.html";
		public static string filePathFoundXML = coreCatalog + "Table.xml";
	}

	public struct Scientist {
		public string fullName;
		public string faculty;
		public string department;
		public string scientificDegree;
		public Dictionary<string, string> datesToAcademicTitles;

		public Scientist(string fn, string f, string d, string scD, Dictionary<string, string> dTAT) {
			fullName = fn;
			faculty = f;
			department = d;
			scientificDegree = scD;
			datesToAcademicTitles = new Dictionary<string, string>();
			foreach (var kvp in dTAT) {
				datesToAcademicTitles.Add(kvp.Key, kvp.Value);
			}
		}
		
		public Scientist(List<string> conditionParameters) {
			if (conditionParameters.Count < 4) throw new Exception("Can`t create recording");
			
			fullName = conditionParameters[0];
			faculty = conditionParameters[1];
			department = conditionParameters[2];
			scientificDegree = conditionParameters[3];
			
			datesToAcademicTitles = new Dictionary<string, string>();
			var titles = conditionParameters[4].Split(',');
			foreach(var title in titles)
				if(title.Trim() != "") datesToAcademicTitles.Add("Any", title);
		}

		public Scientist(XElement scientist) {
			fullName = scientist.Element("FullName")?.Value;
			faculty = scientist.Element("Faculty")?.Value;
			department = scientist.Element("Department")?.Value;
			scientificDegree = scientist.Element("ScientificDegree")?.Value;

			datesToAcademicTitles = new Dictionary<string, string>();
			foreach(var title in scientist.Element("AcademicTitles").Elements()) {
				datesToAcademicTitles.Add(title.Attribute("date").Value, title.Value);
			}
		}

		public static bool operator ==(Scientist left, Scientist condition) {
			if(!left.fullName.Contains(condition.fullName) && condition.fullName != "") return false;
			if(!left.faculty.Contains(condition.faculty) && condition.faculty != "") return false;
			if(!left.department.Contains(condition.department) && condition.department != "") return false;
			if(!left.scientificDegree.Contains(condition.scientificDegree) && condition.scientificDegree != "") return false;

			if (condition.datesToAcademicTitles.Count == 0) return true;
			foreach (var kvp in left.datesToAcademicTitles) {
				foreach (var rkvp in condition.datesToAcademicTitles) {
					if (kvp.Value.Contains(rkvp.Value)) return true;
				}
			}

			return false;
		}
		
		public static bool operator !=(Scientist left, Scientist right) {
			return !(left == right);
		}
	}

	public interface IXMLFinder {
		List<Scientist> Find(Scientist condition);
	}

	class DOMFinder : IXMLFinder {
		
		public List<Scientist> Find(Scientist condition) {
			XmlDocument xDoc = new XmlDocument();
			xDoc.Load(Test.filePathXML);

			var list = xDoc.DocumentElement;
			
			string fullName = "";
			string faculty = "";
			string department = "";
			string scientificDegree = "";
			string date = "";
			var academicTitles = new Dictionary<string, string>();

			var scientists = new List<Scientist>();
			
			foreach (XmlNode scientist in list) {
				
				foreach (XmlNode xNode in scientist) {
					switch (xNode.Name) {
						case "FullName":         fullName = xNode.InnerText; break;
						case "Faculty":          faculty = xNode.InnerText; break;
						case "Department":       department = xNode.InnerText; break;
						case "ScientificDegree": scientificDegree = xNode.InnerText; break;
						case "AcademicTitles": {
							foreach (XmlNode xNode1 in xNode) {
								date = xNode1.Attributes["date"].Value;
								if(!academicTitles.ContainsKey(date)) academicTitles.Add(date, xNode1.InnerText);
							}
							break;
						}
					}
				}
				
				scientists.Add(
					new Scientist(
						fullName,
						faculty,
						department,
						scientificDegree,
						academicTitles
					)
				);
				
				academicTitles.Clear();
				
			}

			Scientist[] copy = new Scientist[scientists.Count];
			scientists.CopyTo(copy);
			foreach (var sc in copy) {
				if (sc != condition) scientists.Remove(sc);
			}
			return scientists;
		}
		
	}
	
	
	class SAXFinder : IXMLFinder {
		
		public List<Scientist> Find(Scientist condition) {
			List<Scientist> scientists = new List<Scientist>();
			
			using (XmlReader xr = XmlReader.Create(Test.filePathXML)) {
				string fullName = "";
				string faculty = "";
				string department = "";
				string scientificDegree = "";
				string date = "";
				string title = "";
				var academicTitles = new Dictionary<string, string>();

				string element = "";

				while (xr.Read()) {
					switch (xr.NodeType) {
						case XmlNodeType.Element: {
							element = xr.Name;
							if (element == "Title") {
								date = xr.GetAttribute("date");
							}
							break;
						}

						case XmlNodeType.Text: {
							switch (element) {
								case "FullName": fullName = xr.Value; break;
								case "Faculty":  faculty = xr.Value; break;
								case "Department":  department = xr.Value; break;
								case "ScientificDegree":  scientificDegree = xr.Value; break;
								case "Title":  title = xr.Value; break;
							}

							break;
						}

						case XmlNodeType.EndElement: {
							if(xr.Name == "Title") {
								if(!academicTitles.ContainsKey(date)) academicTitles.Add(date, title);
							}

							if (xr.Name == "Scientist") {
								scientists.Add(
									new Scientist(
											fullName,
											faculty,
											department,
											scientificDegree,
											academicTitles
										)
								);
								academicTitles.Clear();
							}
							
							break;
						}
					}
				}
			}

			Scientist[] copy = new Scientist[scientists.Count];
			scientists.CopyTo(copy);
			foreach (var sc in copy) {
				if (sc != condition) scientists.Remove(sc);
			}
			return scientists;
		}
		
	}
	
	
	class LINQFinder : IXMLFinder {
		
		public List<Scientist> Find(Scientist condition) {
			XDocument xdoc = XDocument.Load(Test.filePathXML);
			if (xdoc == null) throw new Exception("Can`t open the file");

			var matches = from scientist in xdoc.Element("List").Elements("Scientist")
				let transformedScientist = new Scientist(scientist)
				where transformedScientist == condition
				select transformedScientist;

			var result = new List<Scientist>();
			foreach(var match in matches) result.Add(match);
			return result;
		}
		
	}
}