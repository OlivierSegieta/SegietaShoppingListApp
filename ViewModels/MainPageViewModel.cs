using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Maui.Storage;
using SegietaShoppingListApp.Models;

namespace SegietaShoppingListApp.ViewModels;


public class MainPageViewModel : BaseViewModel
{
    private static readonly string FilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "ShoppingListData.xml");

    public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
    public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
    public ObservableCollection<Product> ProductList { get; set; } = new ObservableCollection<Product>();

    public string NewCategoryName { get; set; } = string.Empty;

    public string NewProductName { get; set; } = string.Empty;
    public string NewProductUnit { get; set; } = string.Empty;
    public Category? SelectedCategory { get; set; }

    public ICommand AddCategoryCommand { get; }
    public ICommand AddProductCommand { get; }
    public ICommand OnIsPurchasedChangedCommand { get; }
    public ICommand IncreaseQuantityCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }
    public ICommand DeleteProductCommand { get; }
    public ICommand ToggleCategoryCommand { get; }

    public MainPageViewModel()
    {
        AddCategoryCommand = new Command(AddCategory);
        AddProductCommand = new Command(AddProduct);
        IncreaseQuantityCommand = new Command<Product>(IncreaseQuantity);
        DecreaseQuantityCommand = new Command<Product>(DecreaseQuantity);
        DeleteProductCommand = new Command<Product>(DeleteProduct);
        OnIsPurchasedChangedCommand = new Command<Product>(OnIsPurchasedChanged);
        ToggleCategoryCommand = new Command(ToggleProductListVisibility);

        LoadFromXml();
        InitializeDefaultCategories();
    }

    private void InitializeDefaultCategories()
    {
        if (Categories.Count == 0)
        {
            Categories.Add(new Category { Name = "Fruits" });
            Categories.Add(new Category { Name = "Vegetables" });
            Categories.Add(new Category { Name = "Dairy" });
        }
    }

    private void AddCategory()
    {
        if (!string.IsNullOrWhiteSpace(NewCategoryName))
        {
            Categories.Add(new Category { Name = NewCategoryName });
            NewCategoryName = string.Empty;
            OnPropertyChanged(nameof(NewCategoryName));
        }
    }

    public class Category
    {
        public string Name { get; set; }
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
    }

    private void AddProduct()
    {
        if (SelectedCategory != null && !string.IsNullOrWhiteSpace(NewProductName))
        {
            Products.Add(new Product
            {
                Name = NewProductName,
                Quantity = 1,
                Unit = NewProductUnit,
                CategoryName = SelectedCategory.Name,
                IsPurchased = false
            });
        }
    }


    private bool _isProductListVisible;
    public bool IsProductListVisible
    {
        get => _isProductListVisible;
        set
        {
            if (_isProductListVisible != value)
            {
                _isProductListVisible = value;
                OnPropertyChanged(nameof(IsProductListVisible));
            }
        }
    }

    public void ToggleProductListVisibility()
    {
        IsProductListVisible = !IsProductListVisible;
    }



    private void IncreaseQuantity(Product? product)
    {
        if (product != null)
        {
            product.Quantity++;
            SortProducts();
            OnPropertyChanged(nameof(Products));
            SaveToXml();
        }
    }

    private void DecreaseQuantity(Product? product)
    {
        if (product != null && product.Quantity > 0)
        {
            product.Quantity--;
            SortProducts();
            OnPropertyChanged(nameof(Products));
            SaveToXml();
        }
    }

    /*private void OnCategoryToggleClicked(object sender, EventArgs e)
    {
        ProductList.IsVisible = !ProductList.IsVisible;
    }*/


    private void DeleteProduct(Product? product)
    {
        if (product != null)
        {
            Products.Remove(product);
            SaveToXml();
        }
    }

    private void OnIsPurchasedChanged(Product product)
    {
        if (product.IsPurchased)
        {
            Products.Remove(product);
            Products.Add(product);
        }
        else
        {

            Products.Remove(product);
            Products.Insert(0, product);
        }

        OnPropertyChanged(nameof(Products));
        SaveToXml();
    }




    private void SortProducts()
    {
        var sortedProducts = Products
            .OrderBy(p => p.IsPurchased)
            .ThenBy(p => p.Name) 
            .ToList();

        Products.Clear();
        foreach (var product in sortedProducts)
        {
            Products.Add(product);
        }

        OnPropertyChanged(nameof(Products));
    }


    private void SaveToXml()
    {
        try
        {
            var data = new ShoppingListData
            {
                Categories = Categories.ToList(),
                Products = Products.ToList()
            };

            using var writer = new StreamWriter(FilePath);
            var serializer = new XmlSerializer(typeof(ShoppingListData));
            serializer.Serialize(writer, data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }

    private void LoadFromXml()
    {
        if (!File.Exists(FilePath)) return;

        try
        {
            using var reader = new StreamReader(FilePath);
            var serializer = new XmlSerializer(typeof(ShoppingListData));
            var data = (ShoppingListData)serializer.Deserialize(reader);

            Categories = new ObservableCollection<Category>(data.Categories);
            Products = new ObservableCollection<Product>(data.Products);

            OnPropertyChanged(nameof(Categories));
            OnPropertyChanged(nameof(Products));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    [Serializable]
    public class ShoppingListData
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Product> Products { get; set; } = new List<Product>();
    }
}


