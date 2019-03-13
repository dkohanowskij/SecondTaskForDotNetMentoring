using System;
using System.IO;
using Task1.EventArgs;

namespace Task1
{
    public class FileSystemProcessingStrategy : IFileSystemProcessingStrategy
    {
        public ActionType ProcessItemFinded<TItemInfo>(
            TItemInfo itemInfo,
            Func<FileSystemInfo, bool> filter,
            EventHandler<ItemFindedEventArgs<TItemInfo>> itemFinded,
            EventHandler<ItemFindedEventArgs<TItemInfo>> filteredItemFinded,
            Action<EventHandler<ItemFindedEventArgs<TItemInfo>>, ItemFindedEventArgs<TItemInfo>> eventEmitter)
            where TItemInfo : FileSystemInfo
        {
            ItemFindedEventArgs<TItemInfo> args = new ItemFindedEventArgs<TItemInfo>
            {
                FindedItem = itemInfo,
                ActionType = ActionType.ContinueSearch
            };
            eventEmitter(itemFinded, args);

            if (args.ActionType != ActionType.ContinueSearch || filter == null)
            {
                return args.ActionType;
            }

            if (filter(itemInfo))
            {
                args = new ItemFindedEventArgs<TItemInfo>
                {
                    FindedItem = itemInfo,
                    ActionType = ActionType.ContinueSearch
                };
                eventEmitter(filteredItemFinded, args);
                return args.ActionType;
            }

            return ActionType.SkipElement;
        }
    }
}
