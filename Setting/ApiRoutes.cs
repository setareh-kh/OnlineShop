namespace OnlineShop.Setting;

public class ApiRoutes
{
    private const string Root = "api";
    private const string Version = "v1";
    private const string Base = Root + "/" + Version;
    private const string AdminBase = Base + "/admin";
    private const string ProfileBase = Base + "/profile";
    public static class Admin
    {
        public const string Product = AdminBase + "/products";
        public const string Order = AdminBase + "/orders";
        public const string User = AdminBase + "/users";
        public const string Category = AdminBase + "/categories";
        public const string Cache = AdminBase + "/cache";
        public const string Auth = AdminBase + "/auth";

    }
    public static class Profile
    {
        public const string Product = ProfileBase + "/products";
        public const string Order = ProfileBase + "/orders";
        public const string Auth = ProfileBase + "/auth";
        public const string MyProfile = ProfileBase;


    }
    public static class Website
    {
        public const string Product = Base + "/products";
        public const string Category = Base + "/categories";
        public const string User = Base + "/users";
        public const string Auth = Base + "/auth";


    }



}