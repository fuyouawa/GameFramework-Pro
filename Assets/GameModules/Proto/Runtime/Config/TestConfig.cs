using System;
using Luban;
using SimpleJSON;
using UnityGameFramework.Runtime;

namespace GameProto.Runtime
{
    public class TestConfig : DataRowBase
    {
        private int _id;

        public override int Id => _id;
        public string Name { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            var node = userData as JSONNode;
            if (node == null)
            {
                Log.Error("UserData is invalid.");
                return false;
            }

            if (node["Id"].IsNumber)
                _id = node["Id"];
            else
                throw new SerializationException("Id is not a number.");

            if (node["Name"].IsString)
                _id = node["Name"];
            else
                throw new SerializationException("Name is not a string.");
            return true;
        }
    }
}
