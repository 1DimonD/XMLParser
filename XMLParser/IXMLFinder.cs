using System.Collections.Generic;

namespace XMLParser {

	public struct Scientist {
		string fullName;
		string faculty;
		string department;
		string scientificDegree;
		Dictionary<string, string> academicTitles;
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