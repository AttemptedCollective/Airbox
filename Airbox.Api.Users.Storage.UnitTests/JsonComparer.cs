using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbox.Api.Users.Storage.UnitTests
{
    internal class JsonComparer : IEqualityComparer
    {
        public new bool Equals(object? x, object? y)
        {
            return JsonConvert.SerializeObject(x).Equals(JsonConvert.SerializeObject(y));
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
