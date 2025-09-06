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
        public override event EventHandler<LoadResourceAgentHelperLoadCompleteEventArgs> LoadComplete;

        public override event EventHandler<LoadResourceAgentHelperErrorEventArgs> Error;

        public override void LoadAsset(string packageName, string assetName, Type assetType, bool isScene)
        {
            throw new NotImplementedException();
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
