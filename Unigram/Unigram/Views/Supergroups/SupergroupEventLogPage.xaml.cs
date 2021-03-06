﻿using LinqToVisualTree;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Telegram.Td.Api;
using Unigram.Common;
using Unigram.Controls.Messages;
using Unigram.ViewModels;
using Unigram.ViewModels.Delegates;
using Unigram.ViewModels.Supergroups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Unigram.Views.Supergroups
{
    public sealed partial class SupergroupEventLogPage : HostedPage, IChatDelegate
    {
        public SupergroupEventLogViewModel ViewModel => DataContext as SupergroupEventLogViewModel;

        public SupergroupEventLogPage()
        {
            InitializeComponent();
            DataContext = TLContainer.Current.Resolve<SupergroupEventLogViewModel, IChatDelegate>(this);

            _typeToItemHashSetMapping.Add("UserMessageTemplate", new HashSet<SelectorItem>());
            _typeToItemHashSetMapping.Add("ChatFriendMessageTemplate", new HashSet<SelectorItem>());
            _typeToItemHashSetMapping.Add("FriendMessageTemplate", new HashSet<SelectorItem>());
            _typeToItemHashSetMapping.Add("ServiceMessageTemplate", new HashSet<SelectorItem>());
            _typeToItemHashSetMapping.Add("ServiceMessagePhotoTemplate", new HashSet<SelectorItem>());
            _typeToItemHashSetMapping.Add("EmptyMessageTemplate", new HashSet<SelectorItem>());
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _panel = (ItemsStackPanel)Messages.ItemsPanelRoot;

            var scroll = Messages.Descendants<ScrollViewer>().FirstOrDefault() as ScrollViewer;
            if (scroll != null)
            {
                scroll.ViewChanged += OnViewChanged;
            }
        }

        private void Photo_Click(object sender, RoutedEventArgs e)
        {
            var control = sender as FrameworkElement;
            var message = control.DataContext as Message;
            //if (message != null && message.HasFromId)
            //{
            //    //ViewModel.NavigationService.Navigate(typeof(ProfilePage), new TLPeerUser { UserId = message.FromId.Value });
            //}
        }

        #region Binding

        private string ConvertType(string broadcast, string mega)
        {
            //if (ViewModel.Item is TLChannel channel)
            //{
            //    return Locale.GetString(channel.IsBroadcast ? broadcast : mega);
            //}

            return null;
        }

        private string ConvertSubtitle(ChatEventLogFilters filters, IList<int> userIds)
        {
            if (filters.InfoChanges &&
                filters.MemberInvites &&
                filters.MemberJoins &&
                filters.MemberLeaves &&
                filters.MemberPromotions &&
                filters.MemberRestrictions &&
                filters.MessageDeletions &&
                filters.MessageEdits &&
                filters.MessagePins &&
                filters.SettingChanges &&
                userIds.IsEmpty())
            {
                return Strings.Resources.EventLogAllEvents;
            }

            return Strings.Resources.EventLogSelectedEvents;
        }

        #endregion











        private Dictionary<string, DataTemplate> _typeToTemplateMapping = new Dictionary<string, DataTemplate>();
        private Dictionary<string, HashSet<SelectorItem>> _typeToItemHashSetMapping = new Dictionary<string, HashSet<SelectorItem>>();

        private void OnChoosingItemContainer(ListViewBase sender, ChoosingItemContainerEventArgs args)
        {
            var typeName = SelectTemplateCore(args.Item);

            Debug.Assert(_typeToItemHashSetMapping.ContainsKey(typeName), "The type of the item used with DataTemplateSelectorBehavior must have a DataTemplate mapping");
            var relevantHashSet = _typeToItemHashSetMapping[typeName];

            // args.ItemContainer is used to indicate whether the ListView is proposing an
            // ItemContainer (ListViewItem) to use. If args.Itemcontainer != null, then there was a
            // recycled ItemContainer available to be reused.
            if (args.ItemContainer != null)
            {
                if (args.ItemContainer.Tag.Equals(typeName))
                {
                    // Suggestion matches what we want, so remove it from the recycle queue
                    relevantHashSet.Remove(args.ItemContainer);
#if ENABLE_DEBUG_SPEW
                    Debug.WriteLine($"Removing (suggested) {args.ItemContainer.GetHashCode()} from {typeName}");
#endif // ENABLE_DEBUG_SPEW
                }
                else
                {
                    // The ItemContainer's datatemplate does not match the needed
                    // datatemplate.
                    // Don't remove it from the recycle queue, since XAML will resuggest it later
                    args.ItemContainer = null;
                }
            }

            // If there was no suggested container or XAML's suggestion was a miss, pick one up from the recycle queue
            // or create a new one
            if (args.ItemContainer == null)
            {
                // See if we can fetch from the correct list.
                if (relevantHashSet.Count > 0)
                {
                    // Unfortunately have to resort to LINQ here. There's no efficient way of getting an arbitrary
                    // item from a hashset without knowing the item. Queue isn't usable for this scenario
                    // because you can't remove a specific element (which is needed in the block above).
                    args.ItemContainer = relevantHashSet.First();
                    relevantHashSet.Remove(args.ItemContainer);
#if ENABLE_DEBUG_SPEW
                    Debug.WriteLine($"Removing (reused) {args.ItemContainer.GetHashCode()} from {typeName}");
#endif // ENABLE_DEBUG_SPEW
                }
                else
                {
                    // There aren't any (recycled) ItemContainers available. So a new one
                    // needs to be created.
                    var item = CreateSelectorItem(typeName);
                    item.Style = Messages.ItemContainerStyleSelector.SelectStyle(args.Item, item);
                    args.ItemContainer = item;
#if ENABLE_DEBUG_SPEW
                    Debug.WriteLine($"Creating {args.ItemContainer.GetHashCode()} for {typeName}");
#endif // ENABLE_DEBUG_SPEW
                }
            }

            // Indicate to XAML that we picked a container for it
            args.IsContainerPrepared = true;
        }

        private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.InRecycleQueue == true)
            {
                // XAML has indicated that the item is no longer being shown, so add it to the recycle queue
                var tag = args.ItemContainer.Tag as string;

#if ENABLE_DEBUG_SPEW
                Debug.WriteLine($"Adding {args.ItemContainer.GetHashCode()} to {tag}");
#endif // ENABLE_DEBUG_SPEW

                var added = _typeToItemHashSetMapping[tag].Add(args.ItemContainer);

#if ENABLE_DEBUG_SPEW
                Debug.Assert(added == true, "Recycle queue should never have dupes. If so, we may be incorrectly reusing a container that is already in use!");
#endif // ENABLE_DEBUG_SPEW

                return;
            }

            var content = args.ItemContainer.ContentTemplateRoot as FrameworkElement;
            if (content is Grid grid)
            {
                content = grid.FindName("Bubble") as FrameworkElement;
            }

            if (content is MessageBubble bubble)
            {
                bubble.UpdateMessage(args.Item as MessageViewModel);
                args.Handled = true;
            }
            else if (content is MessageService service)
            {
                service.UpdateMessage(args.Item as MessageViewModel);
                args.Handled = true;
            }
        }

        private SelectorItem CreateSelectorItem(string typeName)
        {
            SelectorItem item = new ListViewItem();
            //item.ContentTemplate = _typeToTemplateMapping[typeName];
            item.ContentTemplate = Resources[typeName] as DataTemplate;
            item.Tag = typeName;
            return item;
        }

        private string SelectTemplateCore(object item)
        {
            //if (item is MessageViewModel message)
            //{

            //}
            var message = item as MessageViewModel;
            if (message == null)
            {
                return "EmptyMessageTemplate";
            }


            if (message.IsService())
            {
                if (message.Content is MessageChatChangePhoto)
                {
                    return "ServiceMessagePhotoTemplate";
                }

                return "ServiceMessageTemplate";
            }

            if (message.IsChannelPost)
            {
                return "FriendMessageTemplate";
            }
            else if (message.IsSaved())
            {
                return "ChatFriendMessageTemplate";
            }
            else if (message.IsOutgoing)
            {
                return "UserMessageTemplate";
            }

            var chat = message.GetChat();
            if (chat?.Type is ChatTypeSupergroup)
            {
                return "ChatFriendMessageTemplate";
            }

            return "FriendMessageTemplate";
        }

        private void Message_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {

        }

        private void ServiceMessage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Message_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }

        #region Delegate

        public void UpdateChat(Chat chat)
        {
            UpdateChatTitle(chat);
            UpdateChatPhoto(chat);
        }

        public void UpdateChatTitle(Chat chat)
        {
            Title.Text = ViewModel.CacheService.GetTitle(chat);
        }

        public void UpdateChatPhoto(Chat chat)
        {
            Photo.Source = PlaceholderHelper.GetChat(ViewModel.ProtoService, chat, 36);
        }

        #endregion
    }
}
