﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using mRemoteNG.Connection;
using mRemoteNG.Container;
using mRemoteNG.Tree.Root;


namespace mRemoteNG.Tree
{
    public class ConnectionTreeModel : INotifyCollectionChanged, INotifyPropertyChanged
    {
        public List<ContainerInfo> RootNodes { get; } = new List<ContainerInfo>();

        public void AddRootNode(ContainerInfo rootNode)
        {
            if (RootNodes.Contains(rootNode)) return;
            RootNodes.Add(rootNode);
            rootNode.CollectionChanged += RaiseCollectionChangedEvent;
            rootNode.PropertyChanged += RaisePropertyChangedEvent;
            RaiseCollectionChangedEvent(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rootNode));
        }

        public void RemoveRootNode(ContainerInfo rootNode)
        {
            if (!RootNodes.Contains(rootNode)) return;
            rootNode.CollectionChanged -= RaiseCollectionChangedEvent;
            rootNode.PropertyChanged -= RaisePropertyChangedEvent;
            RootNodes.Remove(rootNode);
            RaiseCollectionChangedEvent(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rootNode));
        }

        public IEnumerable<ConnectionInfo> GetRecursiveChildList()
        {
            var list = new List<ConnectionInfo>();
            foreach (var rootNode in RootNodes)
            {
                list.AddRange(GetRecursiveChildList(rootNode));
            }
            return list;
        }

        public IEnumerable<ConnectionInfo> GetRecursiveChildList(ContainerInfo container)
        {
            return container.GetRecursiveChildList();
        }

        public void RenameNode(ConnectionInfo connectionInfo, string newName)
        {
            if (newName == null || newName.Length <= 0)
                return;

            connectionInfo.Name = newName;
            if (Settings.Default.SetHostnameLikeDisplayName)
                connectionInfo.Hostname = newName;
        }

        public void DeleteNode(ConnectionInfo connectionInfo)
        {
            if (connectionInfo is RootNodeInfo)
                return;
            
            connectionInfo?.Dispose();
        }

        public void CloneNode(ConnectionInfo connectionInfo)
        {
            connectionInfo.Clone();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void RaiseCollectionChangedEvent(object sender, NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(sender, args);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChangedEvent(object sender, PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }
}