using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Library
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public List<Book> Books { get; set; }
		public ObservableCollection<Reader> Readers { get; set; }
		public Book SelectedBook { get; set; }
		public Book SelectedBorrowedBook { get; set; }
		public Reader SelectedReader { get; set; }
		public ObservableCollection<Book> SearchedBooks { get; set; }
		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = this;

			if (!File.Exists("books.json") || !File.Exists("readers.json"))
			{
				Books = new List<Book>();
				Readers = new ObservableCollection<Reader>();
				GenerateSampleData();
			}
			else
			{
				LoadLibraryData();
			}
			SearchedBooks = new ObservableCollection<Book>();
			foreach (var book in Books)
			{
				SearchedBooks.Add(book);
			}
		}

		private void ReadersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedReader = (Reader)ReadersListBox.SelectedItem;
			if (SelectedReader != null)
			{
				BorrowedBooksListBox.ItemsSource = SelectedReader.BorrowedBooks;
				BorrowedBooksListBox.Items.Refresh();
			}
			else
			{
				BorrowedBooksListBox.ItemsSource = null;
			}

		}


		private void GenerateSampleData()
		{
			Books.Add(new Book("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565"));
			Books.Add(new Book("To Kill a Mockingbird", "Harper Lee", "9780061120084"));
			Books.Add(new Book("1984", "George Orwell", "9780451524935"));

			Readers.Add(new Reader("John Doe"));
			Readers.Add(new Reader("Jane Smith"));
			Readers.Add(new Reader("Alice Johnson"));
		}

		internal void AddReader(string text)
		{
			Reader reader = new Reader(text);
			Readers.Add(reader);
			Readers = new ObservableCollection<Reader>(Readers.OrderBy(r => r.Name));
		}

		public void LoadLibraryData()
		{
			Books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText("books.json"));
			Readers = JsonSerializer.Deserialize<ObservableCollection<Reader>>(File.ReadAllText("readers.json"));
		}

		public void SaveLibraryData()
		{
			string bookData = JsonSerializer.Serialize(Books);
			string readerData = JsonSerializer.Serialize(Readers);
			File.WriteAllText("books.json", bookData);
			File.WriteAllText("readers.json", readerData);
		}

		private void BorrowedBooksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedBorrowedBook = (Book)BorrowedBooksListBox.SelectedItem;
		}

		private void AddReaderButton_Click(object sender, RoutedEventArgs e)
		{
			AddReader(ReaderNameTextBox.Text);
			ReaderNameTextBox.Clear();
			SaveLibraryData();
		}

		private void DeleteReaderButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{

				foreach (var book in SelectedReader.BorrowedBooks)
				{
					book.IsAvailable = true;

				}
				Readers.Remove(SelectedReader);
				BorrowedBooksListBox.ItemsSource = null;
				BooksListBox.Items.Refresh();
				SaveLibraryData();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private void BorrowBookButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Reader reader = (Reader)ReadersListBox.SelectedItem;
				reader.BorrowBook((Book)SelectedBook);
				BooksListBox.Items.Refresh();
				if (SelectedReader != null)
				{
					BorrowedBooksListBox.ItemsSource = SelectedReader.BorrowedBooks;
					BorrowedBooksListBox.Items.Refresh();
				}
				else
				{
					BorrowedBooksListBox.ItemsSource = null;
				}
				SaveLibraryData();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ReturnBookButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{

				SelectedReader.ReturnBook((Book)SelectedBorrowedBook);


				if (SelectedReader != null)
				{
					BorrowedBooksListBox.ItemsSource = SelectedReader.BorrowedBooks;
					BorrowedBooksListBox.Items.Refresh();
					BooksListBox.Items.Refresh();
				}
				else
				{
					BorrowedBooksListBox.ItemsSource = null;
				}
				SaveLibraryData();
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private void BooksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedBook = (Book)BooksListBox.SelectedItem;

		}

		private void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			SearchedBooks.Clear();
			string searchText = SearchTextBox.Text.ToLower();
			var searchedResults = Books.Where(b => 
			b.Title.ToLower().Contains(searchText) || 
			b.Author.ToLower().Contains(searchText)).ToList();
			foreach(var book in searchedResults)
			{
				SearchedBooks.Add(book);
			}
			BooksListBox.ItemsSource = SearchedBooks;
			BooksListBox.Items.Refresh();

		}

		private void AddBookButton_Click(object sender, RoutedEventArgs e)
		{
			string isbn = generateRandomISBN();
			var book = new Book(BookTitleTextBox.Text, BookAuthorTextBox.Text, isbn);
			Books.Add(book);
			SearchedBooks.Add(book);
			SaveLibraryData();
		}


		private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Books.Remove((Book)BooksListBox.SelectedItem);
				SaveLibraryData();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private string generateRandomISBN()
		{
			Random random = new Random();
			String sb = "";
			for (int i = 0; i < 13; i++)
			{
				sb+=random.Next(0, 10).ToString();
			}
			return sb;
		}
	}
}