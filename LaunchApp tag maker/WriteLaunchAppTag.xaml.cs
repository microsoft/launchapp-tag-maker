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
using Windows.Networking.Proximity;
using Windows.Storage.Streams;
using System.Xml.Linq;

namespace LaunchApp_tag_maker
{
    public partial class WriteLaunchAppTag : PhoneApplicationPage
    {
        private ProximityDevice device = null;
        private Button pressed;
        private bool isPublishing = false;
        private long msgId;
        private long writableTagId;
        static Int32 minTagPayloadSize = 128;
        static string prefix = "user=default\tWindowsPhone\t";
        static string appIdNokiaMusic = "f5874252-1f04-4c3f-a335-4fa3b7b85329";
        static string appIdNokiaMaps = "efa4b4a7-7499-46ce-aa95-3e4ab3b39313";
        static string appIdNokiaTv = "8f592862-8bb5-4391-b6ca-c79730d3f34a";
        static string appIdNokiaTransit = "adfdad16-b54a-4ec3-b11e-66bd691be4e6";
        static string appIdNokiaCityLens = "b0a0ac22-cf9e-45ba-8120-815450e2fd71";
        
        public WriteLaunchAppTag()
        {
            InitializeComponent();
            device = ProximityDevice.GetDefault();
            writableTagId = device.SubscribeForMessage("WriteableTag", checkWritableTagSize);
            inputText.Text = appIdNokiaMusic;
        }

        private void checkWritableTagSize(ProximityDevice sender, ProximityMessage message)
        {
            Int32 size= GetInt32FromBuffer(message.Data);
            if (size < minTagPayloadSize) {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("The tag size (" + size + "bytes) is too small for LaunchApp tag, you need a tag with payload size >=" + minTagPayloadSize + "bytes");
                });
                
            }
        }


        private void messageTransmittedHandler(ProximityDevice sender, long messageId)
        {
            sender.StopPublishingMessage(msgId);
            isPublishing = false;
            Dispatcher.BeginInvoke(() =>
            {
                pressed.IsEnabled = true;
                MessageBox.Show("The LaunchApp content has been written to the tag");
            });
        }

        static public IBuffer GetBufferFromString(string str)
        {
            using (var dw = new DataWriter())
            {
                dw.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE;
                dw.WriteString(str);
                return dw.DetachBuffer();
            }
        }

        static public Int32 GetInt32FromBuffer(IBuffer buf)
        {
            Int32 ret = 0;
            using (var dr = DataReader.FromBuffer(buf))
            {
                //dr.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE;
                var bytes = new byte[4];
                dr.ReadBytes(bytes);
                ret = BitConverter.ToInt32(bytes, 0);
            }
            return ret;
        }

        private void WriteToTag(object sender, RoutedEventArgs e)
        {
            pressed = sender as Button;
            pressed.IsEnabled = false;
            msgId = device.PublishBinaryMessage("LaunchApp:WriteTag", GetBufferFromString(prefix+"{"+inputText.Text+"}"), messageTransmittedHandler);
            isPublishing = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            device.StopPublishingMessage(writableTagId);
            if (isPublishing)
            {
                if (device != null)
                {
                    device.StopPublishingMessage(msgId);
                }
                isPublishing = false;
            }
        }

        private void NokiaMusic(object sender, RoutedEventArgs e)
        {
            inputText.Text = appIdNokiaMusic;
        }

        private void NokiaMaps(object sender, RoutedEventArgs e)
        {
            inputText.Text = appIdNokiaMaps;
        }

        private void NokiaTv(object sender, RoutedEventArgs e)
        {
            inputText.Text = appIdNokiaTv;
        }

        private void NokiaCityLens(object sender, RoutedEventArgs e)
        {
            inputText.Text = appIdNokiaCityLens;
        }

        private void NokiaTransit(object sender, RoutedEventArgs e)
        {
            inputText.Text = appIdNokiaTransit;
        }

        private void CurrentApp(object sender, RoutedEventArgs e)
        {
            //var appId = Windows.ApplicationModel.Store.CurrentApp.AppId;
            var appId = GetId();
            inputText.Text = appId.ToString();
        }

        public static Guid GetId()
        {
            Guid applicationId = Guid.Empty;

            var productId = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("ProductID");

            if (productId != null && !string.IsNullOrEmpty(productId.Value))
                Guid.TryParse(productId.Value, out applicationId);

            return applicationId;
        }

    }
}