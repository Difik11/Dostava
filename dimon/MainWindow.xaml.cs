using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dimon
{

    public partial class MainWindow : Window
    {
        private List<MenuItem> allMenuItems;
        private List<CartItem> cartItems;
        private List<CheckBox> cuisineCheckBoxes;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();

            // Подписываемся на событие загрузки окна
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Теперь все элементы управления инициализированы
            InitializeCuisineFilters();
            LoadMenuItems();
            UpdateCartDisplay();
        }

        private void InitializeData()
        {
            cartItems = new List<CartItem>();
            cuisineCheckBoxes = new List<CheckBox>();

            // Создание тестовых данных
            allMenuItems = new List<MenuItem>
            {
                new MenuItem { Name = "Пицца Маргарита", Restaurant = "Итальянский уголок", Cuisine = "Итальянская", Price = 450 },
                new MenuItem { Name = "Паста Карбонара", Restaurant = "Итальянский уголок", Cuisine = "Итальянская", Price = 380 },
                new MenuItem { Name = "Суши Филадельфия", Restaurant = "Токио", Cuisine = "Японская", Price = 520 },
                new MenuItem { Name = "Роллы Калифорния", Restaurant = "Токио", Cuisine = "Японская", Price = 480 },
                new MenuItem { Name = "Борщ", Restaurant = "Русская душа", Cuisine = "Русская", Price = 250 },
                new MenuItem { Name = "Пельмени", Restaurant = "Русская душа", Cuisine = "Русская", Price = 320 },
                new MenuItem { Name = "Бургер", Restaurant = "Американский динер", Cuisine = "Американская", Price = 350 },
                new MenuItem { Name = "Картофель фри", Restaurant = "Американский динер", Cuisine = "Американская", Price = 150 },
                new MenuItem { Name = "Лазанья", Restaurant = "Итальянский уголок", Cuisine = "Итальянская", Price = 420 },
                new MenuItem { Name = "Темпура", Restaurant = "Токио", Cuisine = "Японская", Price = 380 },
                new MenuItem { Name = "Блины", Restaurant = "Русская душа", Cuisine = "Русская", Price = 200 },
                new MenuItem { Name = "Хот-дог", Restaurant = "Американский динер", Cuisine = "Американская", Price = 280 }
            };
        }

        private void InitializeCuisineFilters()
        {
            // Находим StackPanel с фильтрами
            var tabControl = (TabControl)this.FindName("MainTabControl");
            if (tabControl == null) return;

            var menuTab = (TabItem)tabControl.Items[0];
            var grid = (Grid)menuTab.Content;
            var filtersPanel = (StackPanel)grid.Children[0];

            // Собираем все CheckBox'ы
            cuisineCheckBoxes.Clear();
            foreach (var child in filtersPanel.Children)
            {
                if (child is CheckBox checkBox)
                {
                    cuisineCheckBoxes.Add(checkBox);
                }
            }
        }

        private void LoadMenuItems()
        {
            if (MenuItemsList == null) return;

            // Получаем выбранные кухни
            var selectedCuisines = GetSelectedCuisines();

            // Фильтруем блюда по выбранным кухням
            var filteredItems = allMenuItems.Where(item => selectedCuisines.Contains(item.Cuisine)).ToList();

            MenuItemsList.ItemsSource = filteredItems;
        }

        private List<string> GetSelectedCuisines()
        {
            var selectedCuisines = new List<string>();

            foreach (var checkBox in cuisineCheckBoxes)
            {
                if (checkBox.IsChecked == true)
                {
                    selectedCuisines.Add(checkBox.Content.ToString());
                }
            }

            return selectedCuisines;
        }

        private void CuisineFilter_Changed(object sender, RoutedEventArgs e)
        {
            LoadMenuItems();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (cartItems == null) return;

            var button = sender as Button;
            var menuItem = button?.DataContext as MenuItem;

            if (menuItem != null)
            {
                // Проверяем, есть ли уже такое блюдо в корзине
                var existingItem = cartItems.FirstOrDefault(item => item.MenuItem.Name == menuItem.Name);

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    cartItems.Add(new CartItem { MenuItem = menuItem, Quantity = 1 });
                }

                UpdateCartDisplay();
                StatusText.Text = $"Добавлено: {menuItem.Name}";
            }
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            if (cartItems == null) return;

            var button = sender as Button;
            var cartItem = button?.DataContext as CartItem;

            if (cartItem != null)
            {
                cartItems.Remove(cartItem);
                UpdateCartDisplay();
                StatusText.Text = $"Удалено: {cartItem.MenuItem.Name}";
            }
        }

        private void UpdateCartDisplay()
        {
            if (CartItemsList == null || TotalText == null) return;

            CartItemsList.ItemsSource = null;
            CartItemsList.ItemsSource = cartItems;

            var total = cartItems.Sum(item => item.TotalPrice);
            TotalText.Text = $"Итого: {total}₽";
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (cartItems == null || !cartItems.Any())
            {
                MessageBox.Show("Корзина пуста! Добавьте блюда перед оформлением заказа.", "Корзина пуста",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var total = cartItems.Sum(item => item.TotalPrice);
            var message = $"Заказ оформлен!\n\nКоличество позиций: {cartItems.Count}\nОбщая сумма: {total}₽\n\nСпасибо за заказ!";

            MessageBox.Show(message, "Заказ оформлен", MessageBoxButton.OK, MessageBoxImage.Information);

            // Очищаем корзину после заказа
            cartItems.Clear();
            UpdateCartDisplay();
            StatusText.Text = "Заказ оформлен успешно!";
        }
    }

    // Классы данных
    public class MenuItem
    {
        public string Name { get; set; }
        public string Restaurant { get; set; }
        public string Cuisine { get; set; }
        public decimal Price { get; set; }
    }

    public class CartItem
    {
        public MenuItem MenuItem { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => MenuItem.Price * Quantity;

        // Для отображения в ListView
        public string Name => MenuItem.Name;
        public decimal Price => MenuItem.Price;
    }
}