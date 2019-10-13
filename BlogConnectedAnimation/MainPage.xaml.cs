using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UIFaces.NET.Models;
using UIFaces.NET.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BlogConnectedAnimation
{
    public sealed partial class MainPage : Page
    {
        Person selectedItem;

        public MainPage()
        {
            this.InitializeComponent();
            GetData();
        }

        private async void GetData()
        {
            UIFacesService Service = new UIFacesService("41ce8f96bade52007646eecac0a0c2");
            List<Person> Persons = await Service.GetFaces();
            PersonsGridView.ItemsSource = Persons;
        }

        private async void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            PersonsGridView.ScrollIntoView(selectedItem, ScrollIntoViewAlignment.Default);
            PersonsGridView.UpdateLayout();

            ConnectedAnimation ConnectedAnimation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("backwardsAnimation", destinationElement);
            ConnectedAnimation.Completed += ConnectedAnimation_Completed;
            ConnectedAnimation.Configuration = new DirectConnectedAnimationConfiguration();
            await PersonsGridView.TryStartConnectedAnimationAsync(ConnectedAnimation, selectedItem, "connectedElement");
        }

        private void ConnectedAnimation_Completed(ConnectedAnimation sender, object args)
        {
            OverlayPopup.Visibility = Visibility.Collapsed;
        }

        private void PersonsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            GridViewItem ClickedItem = PersonsGridView.ContainerFromItem(e.ClickedItem) as GridViewItem;

            if (ClickedItem != null)
            {
                selectedItem = ClickedItem.Content as Person;

                NameTxt.Text = selectedItem.Name;
                PositionTxt.Text = selectedItem.Position;
                EmailTxt.Text = selectedItem.Email;

                PersonThumbnail.ProfilePicture = new BitmapImage(new Uri(selectedItem.Photo));

                ConnectedAnimation ConnectedAnimation = PersonsGridView.PrepareConnectedAnimation("forwardsAnimation", selectedItem, "connectedElement");
                ConnectedAnimation.Configuration = new DirectConnectedAnimationConfiguration();
                ConnectedAnimation.TryStart(destinationElement);

                OverlayPopup.Visibility = Visibility.Visible;
            }
        }
    }
}