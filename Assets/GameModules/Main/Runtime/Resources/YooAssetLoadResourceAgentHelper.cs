using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.FileSystem;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameMain
{
    public sealed class YooAssetLoadResourceAgentHelper : LoadResourceAgentHelperBase
    {
        private string _fileFullPath;
        private string _fileName;
        private RawFileHandle _rawFileHandle;
        
        public override event EventHandler<LoadResourceAgentHelperUpdateEventArgs> LoadResourceAgentHelperUpdate;
        public override event EventHandler<LoadResourceAgentHelperReadFileCompleteEventArgs> LoadResourceAgentHelperReadFileComplete;
        public override event EventHandler<LoadResourceAgentHelperReadBytesCompleteEventArgs> LoadResourceAgentHelperReadBytesComplete;
        public override event EventHandler<LoadResourceAgentHelperParseBytesCompleteEventArgs> LoadResourceAgentHelperParseBytesComplete;
        public override event EventHandler<LoadResourceAgentHelperLoadCompleteEventArgs> LoadResourceAgentHelperLoadComplete;
        public override event EventHandler<LoadResourceAgentHelperErrorEventArgs> LoadResourceAgentHelperError;
        
        public override void Reset()
        {
        }
        
        public override void ReadFile(string fullPath)
        {
            _fileFullPath = fullPath;
            _rawFileHandle = YooAssets.LoadRawFileAsync(fullPath);
        }

        public override void ReadFile(IFileSystem fileSystem, string name)
        {
            FileInfo fileInfo = fileSystem.GetFileInfo(name);
            _fileFullPath = fileSystem.FullPath;
            _fileName = name;
            if (fileInfo.Offset != 0)
            {
                throw new NotImplementedException();
            }
            _rawFileHandle = YooAssets.LoadRawFileAsync(fileSystem.FullPath);
        }

        public override void ReadBytes(string fullPath)
        {
            throw new NotImplementedException();
        }

        public override void ReadBytes(IFileSystem fileSystem, string name)
        {
            throw new NotImplementedException();
        }

        public override void ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public override void LoadAsset(object resource, string assetName, Type assetType, bool isScene)
        {
            throw new NotImplementedException();
        }
    }
}