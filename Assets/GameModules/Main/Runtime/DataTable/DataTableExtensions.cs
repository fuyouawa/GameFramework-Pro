using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public static class DataTableExtensions
    {
        public static async UniTask LoadDataTableAsync(
            this DataTableComponent dataTableComponent,
            Type tableType,
            string assetName,
            string customPackageName = "")
        {
            var table = dataTableComponent.CreateDataTable(tableType, assetName);
            var textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(assetName, customPackageName);
            table.ParseData(textAsset.text);
        }
    }
}
