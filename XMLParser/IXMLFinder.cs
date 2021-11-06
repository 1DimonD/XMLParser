using System;
using System.Collections.Generic;

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
				datesToAcademicTitles.Add("Any", title.Trim());
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
			return new List<Scientist>();
		}
		
	}
}