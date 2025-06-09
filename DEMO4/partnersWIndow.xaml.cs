using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DEMO4.Utils;

namespace DEMO4
{

    public class AddPartnerCardControl : Border, INotifyPropertyChanged
    {
        private readonly DEMO_v3Entities _db;
        private readonly Action _refreshCallback;

        private string _partnerName = "";
        private string _address = "";
        private string _phone = "";
        private string _partnerType = "1"; // Default type

        public AddPartnerCardControl(DEMO_v3Entities db, Action refreshCallback)
        {
            _db = db;
            _refreshCallback = refreshCallback;

            // UI Setup
            BorderBrush = Brushes.Black;
            BorderThickness = new Thickness(2);
            Margin = new Thickness(10);
            Background = new SolidColorBrush(Colors.LightGray); // Different color to distinguish
            Padding = new Thickness(10);

            var sp = new StackPanel();

            AddField(sp, "Имя:", nameof(PartnerName));
            AddField(sp, "Адрес:", nameof(Address));
            AddField(sp, "Номер:", nameof(Phone));
            AddField(sp, "Тип:", nameof(PartnerType), true);

            // Only show add button if user has admin role (0)
            if (UserSession.Role == 0)
            {
                var addButton = new Button
                {
                    Content = "Добавить партнера",
                    Margin = new Thickness(0, 10, 0, 0),
                    Background = Brushes.LightGreen
                };
                addButton.Click += AddPartner_Click;
                sp.Children.Add(addButton);
            }
            Child = sp;
        }

        public string PartnerName
        {
            get => _partnerName;
            set { _partnerName = value; OnPropertyChanged(); }
        }

        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        public string PartnerType
        {
            get => _partnerType;
            set
            {
                if (byte.TryParse(value, out _))
                {
                    _partnerType = value;
                    OnPropertyChanged();
                }
            }
        }

        private void AddField(StackPanel parent, string label, string propertyName, bool isNumeric = false)
        {
            parent.Children.Add(new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 5, 0, 0)
            });

            var binding = new Binding(propertyName)
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            var textBox = new TextBox
            {
                Margin = new Thickness(0, 2, 0, 5),
                IsEnabled = true
            };

            if (isNumeric)
            {
                textBox.PreviewTextInput += (s, e) =>
                    e.Handled = !char.IsDigit(e.Text, 0);
            }

            textBox.SetBinding(TextBox.TextProperty, binding);
            parent.Children.Add(textBox);
        }

        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PartnerName))
            {
                MessageBox.Show("Имя партнера не может быть пустым");
                return;
            }

            try
            {
                var newPartner = new partner
                {
                    name = PartnerName,
                    address = Address,
                    phone = Phone,
                    type = byte.Parse(PartnerType)
                };

                using (var db = new DEMO_v3Entities())
                {
                    db.partners.Add(newPartner);
                    db.SaveChanges();
                    _refreshCallback?.Invoke();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    public partial class partnersWindow : Window
    {
        public partnersWindow()
        {
            InitializeComponent();
            LoadPartners();
        }

        private void LoadPartners()
        {
            PartnerList.Children.Clear();

            using (var db = new DEMO_v3Entities())
            {
                // Add existing partners
                foreach (var p in db.partners.ToList())
                {
                    PartnerList.Children.Add(new PartnerCardControl(p, db, RefreshList));
                }

                // Add the "Add New" card at the end
                if (UserSession.Role == 0)
                {
                    PartnerList.Children.Add(new AddPartnerCardControl(db, RefreshList));
                }
            }
        }

        private void RefreshList() => LoadPartners();

        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            var newPartner = new partner { name = "Новый партнер", type = 1 };
            using (var db = new DEMO_v3Entities())
            {
                db.partners.Add(newPartner);
                db.SaveChanges();
                LoadPartners();
            }
        }
    }

    public class PartnerCardControl : Border, INotifyPropertyChanged
    {
        private readonly DEMO_v3Entities _db;
        private readonly Action _refreshCallback;
        private partner _partnerEntity;
        private bool _isEditing;
        private Button _editButton;
        private Button _actionButton;

        private string _partnerName;
        private string _address;
        private string _phone;
        private string _partnerType;

        public PartnerCardControl(partner partner, DEMO_v3Entities db, Action refreshCallback)
        {
            _partnerEntity = partner;
            _db = db;
            _refreshCallback = refreshCallback;
            // In PartnerCardControl constructor, change the background to:
            Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)); // Light gray
            PartnerName = partner.name;
            Address = partner.address;
            Phone = partner.phone;
            PartnerType = partner.type.ToString();

            // UI Setup
            BorderBrush = Brushes.Black;
            BorderThickness = new Thickness(2);
   
            Margin = new Thickness(10);
            Background = new SolidColorBrush(Colors.White);
            Padding = new Thickness(10);

            var sp = new StackPanel();

            AddField(sp, "Имя:", nameof(PartnerName));
            AddField(sp, "Адрес:", nameof(Address));
            AddField(sp, "Номер:", nameof(Phone));
            AddField(sp, "Тип:", nameof(PartnerType), true);

            sp.Children.Add(new TextBlock
            {
                Text = $"Общий объем продаж: {CostCalc.Calculate(db, partner)} рублей",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 0)
            });

            // Only show buttons if user has admin role (0)
            if (UserSession.Role == 0)
            {
                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };

                _editButton = new Button
                {
                    Content = "Редактировать",
                    Margin = new Thickness(0, 10, 5, 0)
                };
                _editButton.Click += ToggleEditMode;

                _actionButton = new Button
                {
                    Content = "Удалить",
                    Margin = new Thickness(5, 10, 0, 0)
                };
                _actionButton.Click += ActionButton_Click;

                buttonPanel.Children.Add(_editButton);
                buttonPanel.Children.Add(_actionButton);
                sp.Children.Add(buttonPanel);
            }
            Child = sp;
        }

        public string PartnerName
        {
            get => _partnerName;
            set { _partnerName = value; OnPropertyChanged(); }
        }

        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        public string PartnerType
        {
            get => _partnerType;
            set
            {
                if (byte.TryParse(value, out _))
                {
                    _partnerType = value;
                    OnPropertyChanged();
                }
            }
        }

        private void AddField(StackPanel parent, string label, string propertyName, bool isNumeric = false)
        {
            parent.Children.Add(new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 5, 0, 0)
            });

            var binding = new Binding(propertyName)
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            var textBox = new TextBox
            {
                Margin = new Thickness(0, 2, 0, 5),
                IsEnabled = false
            };

            if (isNumeric)
            {
                textBox.PreviewTextInput += (s, e) =>
                    e.Handled = !char.IsDigit(e.Text, 0);
            }

            textBox.SetBinding(TextBox.TextProperty, binding);
            parent.Children.Add(textBox);
        }

        private void ToggleEditMode(object sender, RoutedEventArgs e)
        {
            _isEditing = !_isEditing;

            foreach (var child in ((StackPanel)Child).Children)
            {
                if (child is TextBox tb)
                    tb.IsEnabled = _isEditing;
            }

            _editButton.Content = _isEditing ? "Отменить" : "Редактировать";
            _actionButton.Content = _isEditing ? "Сохранить" : "Удалить";
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditing)
            {
                // SAVE LOGIC
                try
                {
                    using (var db = new DEMO_v3Entities())
                    {
                        var partnerToUpdate = db.partners.Find(_partnerEntity.partner_ID);
                        partnerToUpdate.name = PartnerName;
                        partnerToUpdate.address = Address;
                        partnerToUpdate.phone = Phone;
                        partnerToUpdate.type = byte.Parse(PartnerType);
                        db.SaveChanges();
                        _partnerEntity = partnerToUpdate;
                    }
                    // PROPERLY exit edit mode
                    _isEditing = false;
                    foreach (var child in ((StackPanel)Child).Children)
                    {
                        if (child is TextBox tb) tb.IsEnabled = false;
                    }
                    _editButton.Content = "Редактировать";
                    _actionButton.Content = "Удалить";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                }
            }
            else
            {
                // DELETE LOGIC
                if (MessageBox.Show("Удалить этого партнера?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (var db = new DEMO_v3Entities())
                    {
                        db.partners.Remove(db.partners.Find(_partnerEntity.partner_ID));
                        db.SaveChanges();
                        _refreshCallback?.Invoke();
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}