using System;
using System.IO;
using Task1.EventArgs;

namespace Task1
{
    public interface IFileSystemProcessingStrategy
    {
        ActionType ProcessItemFinded<TItemInfo>(
            TItemInfo itemInfo,
            Func<FileSystemInfo, bool> filter,
            EventHandler<ItemFindedEventArgs<TItemInfo>> itemFinded,
            EventHandler<ItemFindedEventArgs<TItemInfo>> filteredItemFinded,
            Action<EventHandler<ItemFindedEventArgs<TItemInfo>>, ItemFindedEventArgs<TItemInfo>> eventEmitter)
            where TItemInfo : FileSystemInfo;
    }
}
