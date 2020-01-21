namespace PizzaOrder.Business.Helpers
{
    public static class Constants
    {
        public static class Roles
        {
            public static string Customer = "Customer";
            public static string Restaurant = "Restaurant";
            public static string Admin = "Admin";
        }

        public static class AuthPolicy
        {
            public static string CustomerPolicy = "CustomerPolicy";
            public static string RestaurantPolicy = "RestaurantPolicy";
            public static string AdminPolicy = "AdminPolicy";
        }
    }
}
