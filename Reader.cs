	using System;
	using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	namespace Library
	{
		public class Reader
		{
			public string Name { get; set; }
			public List<Book> BorrowedBooks { get; set; }

			public Reader(string name)
			{
				Name = name;
				BorrowedBooks = new List<Book>();
			}

			public void BorrowBook(Book book)
			{
				if (book != null && book.IsAvailable)
				{
					BorrowedBooks.Add(book);
					book.IsAvailable = false;
				}
				else
				{
					throw new InvalidOperationException("Book is not available.");
				}
			}

			public void ReturnBook(Book book)
			{
				if (BorrowedBooks.Contains(book))
				{
					book.IsAvailable = true;
					BorrowedBooks.Remove(book);
				}
				else
				{
					throw new InvalidOperationException("This book was not borrowed by this reader.");
				}
			}

			public override string ToString()
			{
				return $"{Name}";
			}
		}
	}
