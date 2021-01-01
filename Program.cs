using System;
using ResourceMonitorAPI.utils;

namespace ResourceMonitorAPI {
    class Program {
        static void Main(string[] args) {
            API api = new API();
            api.start();
        }
    }
}
