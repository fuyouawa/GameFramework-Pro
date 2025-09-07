using System;
using GameFramework.DataTable;
using Luban;
using SimpleJSON;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public class JsonDataTableHelper : DataTableHelperBase
    {
        private ResourceComponent _resourceComponent;

        private void Start()
        {
            _resourceComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
            if (_resourceComponent == null)
            {
                Log.Fatal("Resource component is invalid.");
                return;
            }
        }

        public override bool ReadData(DataTableBase dataTable, string dataTableAssetName, object dataTableAsset, object userData)
        {
            var dataTableTextAsset = dataTableAsset as TextAsset;
            if (dataTableTextAsset != null)
            {
                return dataTable.ParseData(dataTableTextAsset.text, userData);
            }
            Log.Warning("Data table asset '{0}' is invalid.", dataTableAssetName);
            return false;
        }

        public override bool ReadData(DataTableBase dataTable, string dataTableAssetName, byte[] dataTableBytes, int startIndex, int length,
            object userData)
        {
            throw new System.NotImplementedException();
        }

        public override bool ParseData(DataTableBase dataTable, string dataTableString, object userData)
        {
            try
            {
                var rootNode = JSON.Parse(dataTableString);
                foreach (var node in rootNode.Children)
                {
                    if (!node.IsObject)
                    {
                        throw new SerializationException();
                    }

                    if (!dataTable.AddDataRow(null, node))
                    {
                        Log.Warning("Can not parse data row string '{0}'.", node.ToString());
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Warning("Can not parse data table string with exception '{0}'.", e);
                return false;
            }
        }

        public override bool ParseData(DataTableBase dataTable, byte[] dataTableBytes, int startIndex, int length, object userData)
        {
            throw new System.NotImplementedException();
        }

        public override void ReleaseDataAsset(DataTableBase dataTable, object dataTableAsset)
        {
            _resourceComponent.UnloadAsset(dataTableAsset);
        }
    }
}
