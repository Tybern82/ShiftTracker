using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace com.tybern.ShiftTracker.data {
    public interface EncodeJSON {

        public JObject toJSON();
    }
}
