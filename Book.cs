using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
	public class Book
	{
		public string Title { get; set; }
		public string Author { get; set; }
		public string ISBN { get; set; }
		public bool IsAvailable { get; set; }

		public Book(string title, string author, string isbn)
		{
			Title = title;
			Author = author;
			ISBN = isbn;
			IsAvailable = true;
		}

		public override string ToString()
		{
			string availability = IsAvailable ? "Available" : "Not Available";
			return $"{Title} by {Author} (ISBN: {ISBN}) - {availability}";
		}
	}
}
