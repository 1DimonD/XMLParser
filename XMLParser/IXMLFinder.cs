using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XMLParser {

	public struct Scientist {
		public string fullName;
		public string faculty;
		public string department;
		public string scientificDegree;
		public Dictionary<string, string> datesToAcademicTitles;

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
			if(left.fullName != condition.fullName && condition.fullName != "") return false;
			if(left.faculty != condition.faculty && condition.faculty != "") return false;
			if(left.department != condition.department && condition.department != "") return false;
			if(left.scientificDegree != condition.scientificDegree && condition.scientificDegree != "") return false;

			if (condition.datesToAcademicTitles.Count == 0) return true;
			foreach (var kvp in left.datesToAcademicTitles) {
				foreach (var rkvp in condition.datesToAcademicTitles) {
					if (kvp.Value == rkvp.Value) return true;
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
			return new List<Scientist>();
		}
		
	}
	
	class SAXFinder : IXMLFinder {
		
		public List<Scientist> Find(Scientist condition) {
			return new List<Scientist>();
		}
		
	}
	
	class LINQFinder : IXMLFinder {
		
		public List<Scientist> Find(Scientist condition) {
			XDocument xdoc = XDocument.Load(@"D:\Univ_C#\Work\XMLParser\XMLParser\TestFiles\Test1.xml");
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