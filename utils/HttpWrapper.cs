namespace ResourceMonitorAPI.utils {
    class HttpWrapper {
        public class Headers {        
            public const string CACHE_CONTROL = "Cache-Control";
            public const string ACCESS_CONTROL_ALLOW_ORIGIN = "Access-Control-Allow-Origin";
        }

        public class HeaderValues {
            public class CacheControl {
                public const string NO_CACHE = "no-cache";
            }

            public class AccessControlAllowOrigin {
                public const string ANY_ORIGIN = "*";
            }
        }

        public class ContentTypes {
            public const string JSON = "application/json";
        }
    }
}