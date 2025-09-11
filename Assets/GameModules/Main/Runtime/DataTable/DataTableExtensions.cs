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
            Type dataRowType,
            string dataTableName,
            string assetName,
            string customPackageName = "")
        {
            var table = dataTableComponent.CreateDataTable(dataRowType, dataTableName);
            var textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(assetName, customPackageName);
            table.ParseData(textAsset.text);
        }
    }
}
