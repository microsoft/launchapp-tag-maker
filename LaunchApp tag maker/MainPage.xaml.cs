/* created by wenlu 
 * to demonstrate the wiki article http://www.developer.nokia.com/Community/Wiki/How_to_launch_apps_available_in_the_store_using_NFC_LaunchApp_tag#How_to_write_LaunchApp_content_into_a_NFC_tag
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LaunchApp_tag_maker.Resources;

namespace LaunchApp_tag_maker
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void StartTagMaking(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/WriteLaunchAppTag.xaml", UriKind.Relative));
        }

    }
}