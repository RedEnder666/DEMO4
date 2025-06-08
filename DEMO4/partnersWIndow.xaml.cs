using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
using System.Windows.Shapes;
using DEMO4.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace DEMO4
{
    /// <summary>
    /// Interaction logic for partnersWindow.xaml
    /// </summary>
    


    public partial class partnersWindow : Window
    {
        private bool isEditing = false; 
        private partner currentlyEditingPartner = null; 

        public void updateState(StackPanel sp, partner p)
        {
            isEditing = !isEditing;
            sp.Children[1].IsEnabled = isEditing;
            sp.Children[3].IsEnabled = isEditing;
            sp.Children[5].IsEnabled = isEditing;
            sp.Children[7].IsEnabled = isEditing;
            var editBtn = (Button)sp.Children[8];
            editBtn.Content = isEditing ? "Отменить" : "Редактировать";
            var delBtn = (Button)sp.Children[9];
            delBtn.Content = isEditing ? "Сохранить" : "Удалить";
        }
        private void LoadPartners()
        {
            PartnerList.Children.Clear();

            using (var db = new DEMO_v3Entities())
            {
                foreach (var p in db.partners.ToList())
                {
                    var total = CostCalc.Calculate(db, p);

                    var b = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(2),
                        Margin = new Thickness(15)
                    };

                    var sp = new StackPanel();
                    var nameBox = new TextBox
                    {
                        Text = p.name,
                        Margin = new Thickness(5),
                        IsEnabled = false,
                    };
                    var addressBox = new TextBox { Text = p.address, Margin = new Thickness(5), IsEnabled = false };
                    var phoneBox = new TextBox { Text = p.phone, Margin = new Thickness(5), IsEnabled = false };
                    var typeBox = new TextBox
                    {
                        Text = p.type.ToString(),
                        Margin = new Thickness(5),
                        IsEnabled = false,
                        
                    };
                    sp.Children.Add(new TextBlock { Text = "Имя:", Margin = new Thickness(5) });
                    sp.Children.Add(nameBox);
                    sp.Children.Add(new TextBlock { Text = "Адрес:", Margin = new Thickness(5) });
                    sp.Children.Add(addressBox);
                    sp.Children.Add(new TextBlock { Text = "Номер:", Margin = new Thickness(5) });
                    sp.Children.Add(phoneBox);
                    sp.Children.Add(new TextBlock { Text = "Тип:", Margin = new Thickness(5) });
                    sp.Children.Add(typeBox);
                    

                    sp.Children.Add(new TextBlock { Text = total.ToString() });

                    var deleteBtn = new Button
                    {
                        Content = "Удалить",
                        Margin = new Thickness(5),
                        Tag = p
                    };
                    deleteBtn.Click += (s, e) =>
                    {
                        if (isEditing && currentlyEditingPartner == deleteBtn.Tag)
                        {
                           using (var db2 = new DEMO_v3Entities())
                            {
                                partner partnertd = db2.partners.Find(((partner)deleteBtn.Tag).partner_ID);
                                partnertd.name = nameBox.Text;
                                partnertd.address = addressBox.Text;
                                partnertd.phone = phoneBox.Text;
                                partnertd.type = byte.Parse(typeBox.Text);
                                db.SaveChanges();
                                updateState(sp);
                            }
                        }
                        else
                        {
                            var btn = (Button)s;
                            var partnertd = (partner)btn.Tag;
                            using (var db2 = new DEMO_v3Entities())
                            {
                                db2.partners.Remove(db2.partners.Find(partnertd.partner_ID));
                                db2.SaveChanges();
                                LoadPartners();
                            }
                        }
                        
                    };

                    sp.Children.Add(deleteBtn);

                    var editBtn = new Button
                    {
                        Content = "Редактировать",
                        Margin = new Thickness(5),
                        Tag = p
                    };
                    editBtn.Click += (s, e) =>
                    {
                        updateState(sp);

                    };
                    sp.Children.Add(editBtn);

                    b.Child = sp;
                    PartnerList.Children.Add(b);
                }
            }
        }


        public partnersWindow()
        {

            InitializeComponent();
            LoadPartners();

        }
    }
}
