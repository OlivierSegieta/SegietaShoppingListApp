using System.ComponentModel;
using System.Windows.Input;
using SegietaShoppingListApp.ViewModels;
using System;

namespace SegietaShoppingListApp.Models
{
    [Serializable]
    public class Product : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; set; } = string.Empty;

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public string Unit { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        private bool _isPurchased;
        public bool IsPurchased
        {
            get => _isPurchased;
            set
            {
                if (_isPurchased != value)
                {
                    _isPurchased = value;
                    OnPropertyChanged(nameof(IsPurchased));
                }
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }
    }
}
