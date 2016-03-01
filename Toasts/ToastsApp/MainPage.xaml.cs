﻿using System;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using NotificationsExtensions.Toasts;

namespace ToastsApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OnSendNotification(object sender, RoutedEventArgs e)
        {
            string xml = @"<toast launch=""developer-defined-string"">
                          <visual>
                            <binding template=""ToastGeneric"">
                              <image placement=""appLogoOverride"" src=""Assets/MicrosoftLogo.png"" />
                              <text>DotNet Spain Conference</text>
                              <text>How much do you like my session?</text>
                            </binding>
                          </visual>
                          <actions>
                            <input id=""rating"" type=""selection"" defaultInput=""5"" >
                          <selection id=""1"" content=""1 (Not very much)"" />
                          <selection id=""2"" content=""2"" />
                          <selection id=""3"" content=""3"" />
                          <selection id=""4"" content=""4"" />
                          <selection id=""5"" content=""5 (A lot!)"" />
                            </input>
                            <action activationType=""background"" content=""Vote"" arguments=""vote"" />
                          </actions>
                        </toast>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            ToastNotification toast = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(toast);

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (BackgroundTaskRegistration.AllTasks.All(x => x.Value.Name != "ToastTask"))
            {
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                builder.Name = "ToastTask";
                builder.TaskEntryPoint = "ToastsApp.Task.VoteTask";
                builder.SetTrigger(new ToastNotificationActionTrigger());
                var status = await BackgroundExecutionManager.RequestAccessAsync();
                if (status != BackgroundAccessStatus.Denied)
                {
                    builder.Register();
                }
            }
        }

        private void OnSendNotificationWithExtension(object sender, RoutedEventArgs e)
        {
            ToastVisual visual = new ToastVisual
            {
                AppLogoOverride = new ToastAppLogo
                {
                    Crop = ToastImageCrop.None,
                    Source = new ToastImageSource("ms-appx:///Assets/MicrosoftLogo.png")
                },
                TitleText = new ToastText
                {
                    Text = "Dev Days 3"
                },
                BodyTextLine1 = new ToastText
                {
                    Text = "How much do you like my session ?"
                }
            };

            ToastSelectionBox selection = new ToastSelectionBox("rating");
            selection.Items.Add(new ToastSelectionBoxItem("1", "1 (Not very much)"));
            selection.Items.Add(new ToastSelectionBoxItem("2", "2"));
            selection.Items.Add(new ToastSelectionBoxItem("3", "3"));
            selection.Items.Add(new ToastSelectionBoxItem("4", "4"));
            selection.Items.Add(new ToastSelectionBoxItem("5", "5 (A lot!)"));
            selection.DefaultSelectionBoxItemId = "5";

            ToastButton button = new ToastButton("Vote", "vote");
            button.ActivationType = ToastActivationType.Background;

            ToastContent toast = new ToastContent
            {
                Visual = visual,
                ActivationType = ToastActivationType.Background,
                Actions = new ToastActionsCustom
                { 
                    Inputs = {selection},
                    Buttons = { button }
                }
            };

            XmlDocument doc = toast.GetXml();
            ToastNotification notification = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }
    }
}
