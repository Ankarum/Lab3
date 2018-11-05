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

namespace Lab3
{
    /// <summary>
    /// Шаблон для отображения названия свойства и поля для изменения свойства
    /// </summary>
    public partial class PropertyBox : UserControl
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label",
            typeof(string),
            typeof(PropertyBox));
        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value);}
        }

        public static readonly DependencyProperty PropertyBindProperty = DependencyProperty.Register("PropertyBind",
            typeof(string),
            typeof(PropertyBox));
        public string PropertyBind
        {
            get { return (string)GetValue(PropertyBindProperty); }
            set { SetValue(PropertyBindProperty, value); }
        }
        public PropertyBox()
        {
            InitializeComponent();
        }
    }
}
